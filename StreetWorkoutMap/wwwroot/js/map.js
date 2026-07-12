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

function createPopup(spot) {
    const imageContent = spot.ImageUrl
        ? `
            <img class="spot-popup-image"
                 src="${spot.ImageUrl}"
                 alt="${spot.Name}" />
        `
        : `
            <div class="spot-popup-image-placeholder">
                <span>SW-MAP</span>
                <small>Очаквайте снимка</small>
            </div>
        `;

    return `
        <article class="spot-popup">
            <div class="spot-popup-media">
                ${imageContent}
            </div>

            <div class="spot-popup-body">
                <div class="spot-popup-heading">
                    <h3>${spot.Name}</h3>

                    <span class="spot-rating">
                        ★ ${spot.Rating}
                    </span>
                </div>

                <p class="spot-location">
                    ${spot.District}, ${spot.City}
                </p>

                <p class="spot-description">
                    ${spot.Description}
                </p>

                <div class="spot-equipment">
                    <button class="equipment-toggle"
                            type="button"
                            aria-expanded="false">
                        <span>Оборудване и условия</span>
                        <span class="equipment-toggle-icon">⌄</span>
                    </button>

                    <div class="equipment-content" hidden>
                        <ul>
                            ${createEquipmentItem(
        "Лостове",
        spot.HasPullUpBars
    )}

                            ${createEquipmentItem(
        "Успоредка",
        spot.HasParallelBars
    )}

                            ${createEquipmentItem(
        "Халки",
        spot.HasRings
    )}

                            ${createEquipmentItem(
        "Осветление",
        spot.HasLighting
    )}

                            ${createEquipmentItem(
        "Закрита площадка",
        spot.IsIndoor
    )}
                        </ul>
                    </div>
                </div>
            </div>
        </article>
    `;
}

function createEquipmentItem(label, isAvailable) {
    const itemClass = isAvailable
        ? "equipment-available"
        : "equipment-unavailable";

    const icon = isAvailable ? "✓" : "✕";

    return `
        <li class="${itemClass}">
            <span class="equipment-status">${icon}</span>
            <span>${label}</span>
        </li>
    `;
}





function initializeSearch(map, spots, spotMarkers) {
    const searchInput = document.getElementById("spot-search-input");
    const searchResults = document.getElementById("spot-search-results");

    if (!searchInput || !searchResults) {
        return;
    }

    searchInput.addEventListener("input", () => {
        const searchTerm = searchInput.value
            .trim()
            .toLocaleLowerCase("bg-BG");

        if (searchTerm.length === 0) {
            hideSearchResults(searchResults);
            return;
        }

        const matchingSpots = spots.filter(spot =>
            containsSearchTerm(spot.Name, searchTerm) ||
            containsSearchTerm(spot.City, searchTerm) ||
            containsSearchTerm(spot.District, searchTerm)
        );

        renderSearchResults(
            matchingSpots,
            searchResults,
            map,
            spotMarkers,
            searchInput
        );
    });
}

function containsSearchTerm(value, searchTerm) {
    if (!value) {
        return false;
    }

    return value
        .toLocaleLowerCase("bg-BG")
        .includes(searchTerm);
}


function renderSearchResults(
    matchingSpots,
    searchResults,
    map,
    spotMarkers,
    searchInput
) {
    searchResults.innerHTML = "";

    if (matchingSpots.length === 0) {
        searchResults.innerHTML = `
            <div class="spot-search-empty">
                Няма намерени площадки.
            </div>
        `;

        searchResults.hidden = false;
        return;
    }

    matchingSpots.forEach(spot => {
        const resultButton = document.createElement("button");

        resultButton.type = "button";
        resultButton.className = "spot-search-result";

        resultButton.innerHTML = `
            <span class="spot-search-result-name">
                ${spot.Name}
            </span>

            <span class="spot-search-result-location">
                ${spot.District}, ${spot.City}
            </span>
        `;

        resultButton.addEventListener("click", () => {
            focusSpot(map, spotMarkers, spot);

            searchInput.value = spot.Name;
            hideSearchResults(searchResults);
        });

        searchResults.appendChild(resultButton);
    });

    searchResults.hidden = false;
}


function focusSpot(map, spotMarkers, selectedSpot) {
    const selectedEntry = spotMarkers.find(entry =>
        entry.spot.Id === selectedSpot.Id
    );

    if (!selectedEntry) {
        return;
    }

    map.setView(
        [selectedSpot.Latitude, selectedSpot.Longitude],
        17,
        {
            animate: true
        }
    );

    selectedEntry.marker.openPopup();
}



function hideSearchResults(searchResults) {
    searchResults.hidden = true;
    searchResults.innerHTML = "";
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


function locateUser(map, button) {
    if (!navigator.geolocation) {
        alert("Браузърът не поддържа геолокация.");
        return;
    }

    button.classList.add("is-loading");

    navigator.geolocation.getCurrentPosition(
        position => {
            const latitude = position.coords.latitude;
            const longitude = position.coords.longitude;
            const accuracy = position.coords.accuracy;

            showUserLocation(
                map,
                latitude,
                longitude,
                accuracy
            );

            button.classList.remove("is-loading");
        },
        error => {
            button.classList.remove("is-loading");
            handleLocationError(error);
        },
        {
            enableHighAccuracy: true,
            timeout: 10000,
            maximumAge: 30000
        }
    );
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


