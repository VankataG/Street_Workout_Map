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

    const ratingContent = spot.Rating !== null
        ? `
            <span class="spot-rating">
                ★ ${spot.Rating}
            </span>
        `
        : "";

    return `
        <article class="spot-popup">
            <div class="spot-popup-media">
                ${imageContent}
            </div>

            <div class="spot-popup-body">
                <div class="spot-popup-heading">
                    <h3>${spot.Name}</h3>
                    ${ratingContent}
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
                        </ul>
                    </div>
                </div>

                <div class="spot-popup-actions">
                    <a
                        class="spot-directions-button"
                        href="https://www.google.com/maps/dir/?api=1&destination=${spot.Latitude},${spot.Longitude}"
                        target="_blank"
                        rel="noopener noreferrer">
                        Навигация
                    </a>
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