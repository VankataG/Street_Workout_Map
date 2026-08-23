
const mapElement = document.getElementById('map');

InitializeMap();




async function InitializeMap() {
    const map = createMap();
    const spots = await getSpotsAsync();

    const { spotMarkers, markerCluster } = addMarkers(map, spots);

    initializeSearch(map, spots, spotMarkers);
    addLocationControl(map);
    initializeNearestSpotButton(map, spots, spotMarkers, markerCluster);
}

async function getSpotsAsync() {
    const response = await fetch("/api/workoutspots");

    return await response.json();
}

function createMap() {
    const map = L.map('map').setView([42.75, 25.50], 8);

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

    const markerCluster = L.markerClusterGroup({
        removeOutsideVisibleBounds: false,

        iconCreateFunction: function (cluster) {
            const count = cluster.getChildCount();

            return L.divIcon({
                html: `<span>${count}</span>`,
                className: "sw-map-cluster",
                iconSize: [48, 48]
            });
        }
    });

    spots.forEach(spot => {
        const popup = createPopup(spot);

        const marker = L.marker(
            [spot.Latitude, spot.Longitude],
            {
                icon: workoutMarkerIcon,
                title: spot.Name
            }
        )
            .bindPopup(popup, {
                minWidth: 280,
                maxWidth: 320
            });

        markerCluster.addLayer(marker);

        spotMarkers.push({
            spot: spot,
            marker: marker
        });
    });

    map.addLayer(markerCluster);

    return {spotMarkers, markerCluster};
}

