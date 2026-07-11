const mapElement = document.getElementById('map');

InitializeMap();




async function InitializeMap() {
    const map = createMap();
    const spots = await getSpotsAsync();

    addMarkers(map, spots);
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

function addMarkers(map, spots) {
    spots.forEach(spot => {
        const popup = createPopup(spot);

        L.marker([spot.Latitude, spot.Longitude])
            .addTo(map)
            .bindPopup(popup);
    });
}

function createPopup(spot) {
    return `
                     <div class="spot-popup">
                        <h3>${spot.Name}</h3>

                        <p>${spot.Description}</p>

                        <p><strong>Рейтинг:</strong>⭐ ${spot.Rating}</p>

                        <p>
                            ${spot.HasParallelBars ? "✅ Има успоредка" : "❌ Няма успоредка"}
                        </p>

                        <p>
                            ${spot.HasRings ? "✅ Има халки" : "❌ Няма халки"}
                        </p>
                    </div>
            `;
}