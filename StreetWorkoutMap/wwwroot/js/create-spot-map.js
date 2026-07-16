document.addEventListener("DOMContentLoaded", initializeCreateSpotMap);

function initializeCreateSpotMap() {
    const mapElement = document.getElementById("create-spot-map");
    const latitudeInput = document.getElementById("Input_Latitude");
    const longitudeInput = document.getElementById("Input_Longitude");
    const locationButton = document.getElementById("use-current-location");
    const locationMessage = document.getElementById("location-message");

    if (!mapElement || !latitudeInput || !longitudeInput) {
        return;
    }

    const defaultLocation = {
        latitude: 42.7339,
        longitude: 25.4858,
        zoom: 7
    };

    const initialLatitude = Number(latitudeInput.value);
    const initialLongitude = Number(longitudeInput.value);

    const hasInitialCoordinates =
        isValidLatitude(initialLatitude) &&
        isValidLongitude(initialLongitude) &&
        !(initialLatitude === 0 && initialLongitude === 0);

    const map = L.map(mapElement).setView(
        hasInitialCoordinates
            ? [initialLatitude, initialLongitude]
            : [defaultLocation.latitude, defaultLocation.longitude],
        hasInitialCoordinates ? 16 : defaultLocation.zoom
    );

    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        maxZoom: 19,
        attribution: "&copy; OpenStreetMap contributors"
    }).addTo(map);

    let marker = null;

    if (hasInitialCoordinates) {
        marker = createDraggableMarker(
            map,
            initialLatitude,
            initialLongitude,
            latitudeInput,
            longitudeInput
        );
    }

    map.on("click", event => {
        const { lat, lng } = event.latlng;

        if (marker === null) {
            marker = createDraggableMarker(
                map,
                lat,
                lng,
                latitudeInput,
                longitudeInput
            );
        } else {
            marker.setLatLng([lat, lng]);
        }

        updateCoordinateInputs(latitudeInput, longitudeInput, lat, lng);
        setLocationMessage(locationMessage, "Локацията е избрана от картата.");
    });

    latitudeInput.addEventListener("change", updateMapFromInputs);
    longitudeInput.addEventListener("change", updateMapFromInputs);

    function updateMapFromInputs() {
        const latitude = Number(latitudeInput.value);
        const longitude = Number(longitudeInput.value);

        if (!isValidLatitude(latitude) || !isValidLongitude(longitude)) {
            setLocationMessage(
                locationMessage,
                "Въведи валидни координати.",
                true
            );

            return;
        }

        if (marker === null) {
            marker = createDraggableMarker(
                map,
                latitude,
                longitude,
                latitudeInput,
                longitudeInput
            );
        } else {
            marker.setLatLng([latitude, longitude]);
        }

        map.setView([latitude, longitude], 16);
        setLocationMessage(locationMessage, "Картата е преместена до координатите.");
    }

    locationButton?.addEventListener("click", () => {
        locateCurrentUser(
            map,
            locationButton,
            locationMessage,
            latitudeInput,
            longitudeInput,
            location => {
                if (marker === null) {
                    marker = createDraggableMarker(
                        map,
                        location.latitude,
                        location.longitude,
                        latitudeInput,
                        longitudeInput
                    );
                } else {
                    marker.setLatLng([
                        location.latitude,
                        location.longitude
                    ]);
                }
            }
        );
    });

    setTimeout(() => map.invalidateSize(), 100);
}

function createDraggableMarker(
    map,
    latitude,
    longitude,
    latitudeInput,
    longitudeInput
) {
    const marker = L.marker([latitude, longitude], {
        draggable: true
    }).addTo(map);

    marker.on("dragend", event => {
        const position = event.target.getLatLng();

        updateCoordinateInputs(
            latitudeInput,
            longitudeInput,
            position.lat,
            position.lng
        );
    });

    return marker;
}

function updateCoordinateInputs(
    latitudeInput,
    longitudeInput,
    latitude,
    longitude
) {
    latitudeInput.value = latitude.toFixed(6);
    longitudeInput.value = longitude.toFixed(6);
}

function locateCurrentUser(
    map,
    button,
    messageElement,
    latitudeInput,
    longitudeInput,
    updateMarker
) {
    if (!navigator.geolocation) {
        setLocationMessage(
            messageElement,
            "Браузърът не поддържа геолокация.",
            true
        );

        return;
    }

    button.disabled = true;
    setLocationMessage(messageElement, "Определяме местоположението ти...");

    navigator.geolocation.getCurrentPosition(
        position => {
            const location = {
                latitude: position.coords.latitude,
                longitude: position.coords.longitude
            };

            updateCoordinateInputs(
                latitudeInput,
                longitudeInput,
                location.latitude,
                location.longitude
            );

            updateMarker(location);

            map.setView(
                [location.latitude, location.longitude],
                17
            );

            setLocationMessage(
                messageElement,
                "Текущото ти местоположение е избрано."
            );

            button.disabled = false;
        },
        error => {
            const message = getGeolocationErrorMessage(error);

            setLocationMessage(messageElement, message, true);
            button.disabled = false;
        },
        {
            enableHighAccuracy: true,
            timeout: 10000,
            maximumAge: 30000
        }
    );
}

function getGeolocationErrorMessage(error) {
    switch (error.code) {
        case error.PERMISSION_DENIED:
            return "Достъпът до местоположението е отказан.";

        case error.POSITION_UNAVAILABLE:
            return "Местоположението не може да бъде определено.";

        case error.TIMEOUT:
            return "Определянето на местоположението отне твърде дълго.";

        default:
            return "Възникна грешка при определяне на местоположението.";
    }
}

function setLocationMessage(element, message, isError = false) {
    if (!element) {
        return;
    }

    element.textContent = message;
    element.classList.toggle("location-message-error", isError);
}

function isValidLatitude(value) {
    return Number.isFinite(value) && value >= -90 && value <= 90;
}

function isValidLongitude(value) {
    return Number.isFinite(value) && value >= -180 && value <= 180;
}