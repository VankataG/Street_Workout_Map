
const mapElement = document.getElementById('map');

InitializeMap();




async function InitializeMap() {
    const map = createMap();
    const spots = await getSpotsAsync();

    const spotMarkers = addMarkers(map, spots);

    initializeSearch(map, spots, spotMarkers);
    addLocationControl(map);
    initializeNearestSpotButton(map, spots, spotMarkers);
}

async function getSpotsAsync() {
    const response = await fetch("/data/spots.json");

    return await response.json();
}

function createMap() {
    const map = L.map('map').setView([43.0757, 25.6172], 14);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    return map;
}


const workoutMarkerIcon = L.divIcon({
    className: "sw-map-marker-wrapper",
    html: `
        <div class="sw-map-marker">
            <svg viewBox="0 0 24 24" aria-hidden="true">
                <path d="M4 7v10M7 9v6M17 9v6M20 7v10M7 12h10" />
            </svg>
        </div>
    `,
    iconSize: [46, 54],
    iconAnchor: [23, 54],
    popupAnchor: [0, -48]
});

function addMarkers(map, spots) {
    const spotMarkers = [];

    spots.forEach(spot => {
        const popup = createPopup(spot);

        const marker = L.marker(
            [spot.Latitude, spot.Longitude],
            {
                icon: workoutMarkerIcon,
                title: spot.Name
            }
        )
            .addTo(map)
            .bindPopup(popup, {
                minWidth: 280,
                maxWidth: 320
            });

        spotMarkers.push({
            spot: spot,
            marker: marker
        });
    });

    return spotMarkers;
}











document.addEventListener("click", event => {
    const toggleButton = event.target.closest(".equipment-toggle");

    if (!toggleButton) {
        return;
    }

    const equipmentSection = toggleButton.closest(".spot-equipment");
    const equipmentContent =
        equipmentSection.querySelector(".equipment-content");

    const isOpen =
        toggleButton.getAttribute("aria-expanded") === "true";

    toggleButton.setAttribute(
        "aria-expanded",
        String(!isOpen)
    );

    equipmentContent.hidden = isOpen;
});


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


function initializeNearestSpotButton(map, spots, spotMarkers) {
    const button =
        document.getElementById("find-nearest-button");

    if (!button) {
        return;
    }

    button.addEventListener("click", async () => {
        const location = await getCurrentLocation();

        const result = findNearestSpot(location.latitude, location.longitude, spots);

        const nearestMarker = spotMarkers.find(x => x.spot.Id === result.spot.Id);

        if (nearestMarker) {
            map.setView(
                [result.spot.Latitude, result.spot.Longitude],
                17,
                {
                    animate: true
                }
            );

            nearestMarker.marker.openPopup();
        }
    });
}

function findNearestSpot(latitude, longitude, spots) {
    let nearestSpot = null;
    let shortestDistance = Infinity;

    for (const spot of spots) {
        const distance = calculateDistance(latitude, longitude, spot.Latitude, spot.Longitude);

        if (distance < shortestDistance) {
            shortestDistance = distance;
            nearestSpot = spot;
        }

    }

    return {
        spot: nearestSpot,
        distance: shortestDistance
    }
}


function calculateDistance(latitude, longitude, spotLatitude, spotLongitude) {
    const earthRadius = 6371000;

    const latitudeDifference = toRadians(spotLatitude - latitude);
    const longitudeDifference = toRadians(spotLongitude - longitude);

    const latitude1 = toRadians(latitude);
    const latitude2 = toRadians(spotLatitude);

    const a = Math.sin(latitudeDifference / 2) ** 2 + Math.cos(latitude1) * Math.cos(latitude2) * Math.sin(longitudeDifference / 2) ** 2;

    const c =2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));

    return earthRadius * c;
}

function toRadians(degrees) {
    return degrees * Math.PI / 180;
}