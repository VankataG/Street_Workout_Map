document.addEventListener("DOMContentLoaded", () => {
    initializeDetailsGallery();
    initializeDetailsMap();
});

function initializeDetailsGallery() {
    const galleryMain =
        document.getElementById("spot-gallery-main");

    const mainImage =
        document.getElementById("spot-main-image");

    const imageCounter =
        document.getElementById("spot-image-counter");

    const thumbnailButtons = Array.from(
        document.querySelectorAll(".spot-thumbnail-button")
    );

    const lightbox =
        document.getElementById("spot-lightbox");

    const lightboxImage =
        document.getElementById("spot-lightbox-image");

    const lightboxCounter =
        document.getElementById("spot-lightbox-counter");

    const closeButton =
        document.getElementById("spot-lightbox-close");

    const previousButton =
        document.getElementById("spot-lightbox-previous");

    const nextButton =
        document.getElementById("spot-lightbox-next");


    if (!galleryMain || !mainImage || !lightbox || !lightboxImage) {
        return;
    }


    const images = thumbnailButtons.length > 0
        ? thumbnailButtons.map(button => button.dataset.imageUrl)
        : [mainImage.src];

    let currentIndex = 0;


    function updateMainImage(index) {
        const imageUrl = images[index];

        if (!imageUrl) {
            return;
        }

        currentIndex = index;

        mainImage.classList.add("is-changing");

        const preloadedImage = new Image();

        preloadedImage.onload = () => {
            mainImage.src = imageUrl;

            thumbnailButtons.forEach((thumbnail, thumbnailIndex) => {
                thumbnail.classList.toggle(
                    "is-active",
                    thumbnailIndex === index
                );
            });

            if (imageCounter) {
                imageCounter.textContent =
                    `${index + 1} / ${images.length}`;
            }

            window.setTimeout(() => {
                mainImage.classList.remove("is-changing");
            }, 70);
        };

        preloadedImage.onerror = () => {
            mainImage.classList.remove("is-changing");
        };

        preloadedImage.src = imageUrl;
    }


    function updateLightboxImage() {
        lightboxImage.src = images[currentIndex];

        if (lightboxCounter) {
            lightboxCounter.textContent =
                `${currentIndex + 1} / ${images.length}`;
        }
    }


    function openLightbox() {
        updateLightboxImage();

        lightbox.hidden = false;

        document.body.classList.add(
            "spot-lightbox-open"
        );

        closeButton?.focus();
    }


    function closeLightbox() {
        lightbox.hidden = true;

        document.body.classList.remove(
            "spot-lightbox-open"
        );

        galleryMain.focus();
    }


    function showPreviousImage() {
        currentIndex =
            (currentIndex - 1 + images.length) %
            images.length;

        updateLightboxImage();
    }


    function showNextImage() {
        currentIndex =
            (currentIndex + 1) %
            images.length;

        updateLightboxImage();
    }


    thumbnailButtons.forEach((button, index) => {
        button.addEventListener("click", () => {
            updateMainImage(index);
        });
    });


    galleryMain.addEventListener(
        "click",
        openLightbox
    );


    galleryMain.addEventListener(
        "keydown",
        event => {
            if (
                event.key === "Enter" ||
                event.key === " "
            ) {
                event.preventDefault();
                openLightbox();
            }
        }
    );


    closeButton?.addEventListener(
        "click",
        closeLightbox
    );


    previousButton?.addEventListener(
        "click",
        event => {
            event.stopPropagation();
            showPreviousImage();
        }
    );


    nextButton?.addEventListener(
        "click",
        event => {
            event.stopPropagation();
            showNextImage();
        }
    );


    lightbox.addEventListener(
        "click",
        event => {
            if (event.target === lightbox) {
                closeLightbox();
            }
        }
    );


    document.addEventListener(
        "keydown",
        event => {

            if (lightbox.hidden) {
                return;
            }

            if (event.key === "Escape") {
                closeLightbox();
            }

            if (
                event.key === "ArrowLeft" &&
                images.length > 1
            ) {
                showPreviousImage();
            }

            if (
                event.key === "ArrowRight" &&
                images.length > 1
            ) {
                showNextImage();
            }
        }
    );


    if (images.length <= 1) {
        if (previousButton) {
            previousButton.hidden = true;
        }

        if (nextButton) {
            nextButton.hidden = true;
        }
    }


    let touchStartX = 0;

    lightbox.addEventListener(
        "touchstart",
        event => {
            touchStartX =
                event.changedTouches[0].screenX;
        },
        { passive: true }
    );


    lightbox.addEventListener(
        "touchend",
        event => {

            if (images.length <= 1) {
                return;
            }

            const touchEndX =
                event.changedTouches[0].screenX;

            const difference =
                touchEndX - touchStartX;

            if (Math.abs(difference) < 50) {
                return;
            }

            if (difference > 0) {
                showPreviousImage();
            }
            else {
                showNextImage();
            }
        },
        { passive: true }
    );
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