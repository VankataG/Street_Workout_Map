let userLocationMarker = null;
let userAccuracyCircle = null;

function addLocationControl(map) {
    const LocationControl = L.Control.extend({
        options: {
            position: "topleft"
        },

        onAdd: function () {
            const button = L.DomUtil.create(
                "button",
                "location-control"
            );

            button.type = "button";
            button.title = "Покажи моята локация";
            button.setAttribute(
                "aria-label",
                "Покажи моята локация"
            );

            button.innerHTML = "⌖";

            L.DomEvent.disableClickPropagation(button);

            L.DomEvent.on(button, "click", () => {
                locateUser(map, button);
            });

            return button;
        }
    });

    map.addControl(new LocationControl());
}


async function getCurrentLocation() {
    return new Promise((resolve, reject) => {
        if (!navigator.geolocation) {
            reject(new Error("Браузърът не поддържа геолокация."));
            return;
        }

        navigator.geolocation.getCurrentPosition(
            position => {
                resolve({
                    latitude: position.coords.latitude,
                    longitude: position.coords.longitude,
                    accuracy: position.coords.accuracy
                });
            },
            error => {
                reject(error);
            },
            {
                enableHighAccuracy: true,
                timeout: 10000,
                maximumAge: 30000
            }
        );
    });
}



async function locateUser(map, button) {
    button.classList.add("is-loading");

    try {
        const location = await getCurrentLocation();

        showUserLocation(
            map,
            location.latitude,
            location.longitude,
            location.accuracy
        );
    } catch (error) {
        handleLocationError(error);
    } finally {
        button.classList.remove("is-loading");
    }
}

function showUserLocation(
    map,
    latitude,
    longitude,
    accuracy
) {
    const coordinates = [latitude, longitude];

    if (userLocationMarker) {
        userLocationMarker.setLatLng(coordinates);
        userAccuracyCircle.setLatLng(coordinates);
        userAccuracyCircle.setRadius(accuracy);
    } else {
        userLocationMarker = L.circleMarker(coordinates, {
            radius: 9,
            color: "#ffffff",
            weight: 3,
            fillColor: "#258cfb",
            fillOpacity: 1
        })
            .addTo(map)
            .bindPopup("Вие сте тук");

        userAccuracyCircle = L.circle(coordinates, {
            radius: accuracy,
            color: "#258cfb",
            weight: 1,
            fillColor: "#258cfb",
            fillOpacity: 0.12
        }).addTo(map);
    }

    map.setView(coordinates, 16, {
        animate: true
    });

    userLocationMarker.openPopup();
}


function handleLocationError(error) {
    let message = "Не успяхме да определим локацията ви.";

    if (error.code === error.PERMISSION_DENIED) {
        message =
            "Достъпът до локацията е отказан. Разрешете го от настройките на браузъра.";
    } else if (error.code === error.POSITION_UNAVAILABLE) {
        message =
            "Информацията за текущата локация не е налична.";
    } else if (error.code === error.TIMEOUT) {
        message =
            "Определянето на локацията отне прекалено много време.";
    }

    alert(message);
}