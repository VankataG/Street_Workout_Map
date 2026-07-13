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