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

    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));

    return earthRadius * c;
}

function toRadians(degrees) {
    return degrees * Math.PI / 180;
}