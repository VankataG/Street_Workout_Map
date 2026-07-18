document.addEventListener("DOMContentLoaded", () => {
    initializeDetailsGallery();
    initializeDetailsMap();
});

function initializeDetailsGallery() {
    const mainImage = document.getElementById("spot-main-image");
    const imageCounter = document.getElementById("spot-image-counter");
    const thumbnailButtons = document.querySelectorAll(
        ".spot-thumbnail-button"
    );

    if (!mainImage || thumbnailButtons.length === 0) {
        return;
    }

    thumbnailButtons.forEach((button) => {
        button.addEventListener("click", () => {
            const imageUrl = button.dataset.imageUrl;
            const imageIndex = Number(button.dataset.imageIndex);

            if (!imageUrl || mainImage.src === imageUrl) {
                return;
            }

            mainImage.classList.add("is-changing");

            const preloadedImage = new Image();

            preloadedImage.onload = () => {
                mainImage.src = imageUrl;

                thumbnailButtons.forEach((thumbnail) => {
                    thumbnail.classList.remove("is-active");
                });

                button.classList.add("is-active");

                if (imageCounter) {
                    imageCounter.textContent =
                        `${imageIndex + 1} / ${thumbnailButtons.length}`;
                }

                window.setTimeout(() => {
                    mainImage.classList.remove("is-changing");
                }, 70);
            };

            preloadedImage.onerror = () => {
                mainImage.classList.remove("is-changing");
            };

            preloadedImage.src = imageUrl;
        });
    });
}

function initializeDetailsMap() {
    const mapElement = document.getElementById("details-spot-map");
    const directionsLink = document.getElementById(
        "spot-directions-link"
    );

    if (!mapElement || typeof L === "undefined") {
        return;
    }

    const latitude = Number(mapElement.dataset.latitude);
    const longitude = Number(mapElement.dataset.longitude);
    const spotName = mapElement.dataset.spotName || "Площадка";

    if (!Number.isFinite(latitude) || !Number.isFinite(longitude)) {
        mapElement.innerHTML =
            "<p class=\"details-map-error\">" +
            "Локацията не може да бъде показана." +
            "</p>";

        if (directionsLink) {
            directionsLink.hidden = true;
        }

        return;
    }

    const map = L.map(mapElement, {
        scrollWheelZoom: false
    }).setView([latitude, longitude], 16);

    L.tileLayer(
        "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
        {
            maxZoom: 19,
            attribution:
                "&copy; OpenStreetMap contributors"
        }
    ).addTo(map);

    const markerIcon = L.divIcon({
        className: "sw-map-marker-wrapper",
        html: `
            <div class="sw-map-marker">
                <svg viewBox="0 0 24 24" aria-hidden="true">
                    <path d="M5 8v8"></path>
                    <path d="M19 8v8"></path>
                    <path d="M8 6v12"></path>
                    <path d="M16 6v12"></path>
                    <path d="M8 12h8"></path>
                    <path d="M3 10v4"></path>
                    <path d="M21 10v4"></path>
                </svg>
            </div>
        `,
        iconSize: [46, 46],
        iconAnchor: [23, 46]
    });

    L.marker([latitude, longitude], {
        icon: markerIcon
    })
        .addTo(map)
        .bindPopup(`<strong>${escapeHtml(spotName)}</strong>`)
        .openPopup();

    if (directionsLink) {
        const destination =
            `${latitude.toString()},${longitude.toString()}`;

        directionsLink.href =
            "https://www.google.com/maps/search/?api=1" +
            `&query=${encodeURIComponent(destination)}`;
    }

    window.setTimeout(() => {
        map.invalidateSize();
    }, 100);
}

function escapeHtml(value) {
    const element = document.createElement("div");
    element.textContent = value;

    return element.innerHTML;
}