
const map = L.map('map').setView([43.0757, 25.6172], 14);

L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '&copy; OpenStreetMap contributors'
}).addTo(map);


const mapElement = document.getElementById('map');

console.log(mapElement.dataset.spots)
spots = JSON.parse(mapElement.dataset.spots);

spots.forEach(spot => {
    const popup = `
                     <div class="spot-popup">
                        <h3>${spot.Name}</h3>

                        <p>${spot.Description}</p>

                        <p><strong>Рейтинг:</strong>⭐ ${spot.Rating}</strong>

                        <p>
                            ${spot.HasParallelBars ? "✅ Има успоредка" : "❌ Няма успоредка"}
                        </p>

                        <p>
                            ${spot.HasRings ? "✅ Има халки" : "❌ Няма халки"}
                        </p>
                    </div>
            `;

    L.marker([spot.Latitude, spot.Longitude])
        .addTo(map)
        .bindPopup(popup);
});
