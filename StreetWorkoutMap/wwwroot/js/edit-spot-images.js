document.addEventListener("DOMContentLoaded", () => {
    const manager = document.getElementById("edit-images-manager");

    if (!manager) {
        return;
    }

    const maxImages = Number.parseInt(
        manager.dataset.maxImages ?? "3",
        10
    );

    const maxFileSize = 5 * 1024 * 1024;
    const allowedTypes = new Set([
        "image/jpeg",
        "image/png",
        "image/webp"
    ]);

    const existingCountElement =
        document.getElementById("existing-images-count");

    const newImagesInput =
        document.getElementById("new-images-input");

    const newImagesGrid =
        document.getElementById("new-images-grid");

    const newImagesCountElement =
        document.getElementById("new-images-count");

    const uploadTitle =
        document.getElementById("new-images-upload-title");

    const uploadDescription =
        document.getElementById("new-images-upload-description");

    const messageElement =
        document.getElementById("images-limit-message");

    if (!newImagesInput || !newImagesGrid) {
        return;
    }

    let selectedFiles = [];

    function getExistingCards() {
        return Array.from(
            manager.querySelectorAll(".existing-image-card")
        );
    }

    function getActiveExistingCount() {
        return getExistingCards().filter(
            card => !card.classList.contains("marked-for-delete")
        ).length;
    }

    function getResultingImageCount() {
        return getActiveExistingCount() + selectedFiles.length;
    }

    function showMessage(message) {
        if (!messageElement) {
            return;
        }

        messageElement.textContent = message;
        messageElement.hidden = false;
    }

    function clearMessage() {
        if (!messageElement) {
            return;
        }

        messageElement.textContent = "";
        messageElement.hidden = true;
    }

    function updateCountersAndLabel() {
        const existingCount = getActiveExistingCount();
        const newCount = selectedFiles.length;
        const totalCount = existingCount + newCount;

        if (existingCountElement) {
            existingCountElement.textContent = existingCount.toString();
        }

        if (newImagesCountElement) {
            newImagesCountElement.textContent = newCount.toString();
        }

        if (uploadTitle && uploadDescription) {
            if (newCount === 0) {
                uploadTitle.textContent = "Добави снимки";
                uploadDescription.textContent =
                    `Използвани ${totalCount} от ${maxImages} места`;
            } else {
                uploadTitle.textContent =
                    newCount === 1
                        ? "Избрана е 1 нова снимка"
                        : `Избрани са ${newCount} нови снимки`;

                uploadDescription.textContent =
                    totalCount < maxImages
                        ? `Можеш да добавиш още ${maxImages - totalCount}`
                        : "Достигнат е максималният брой снимки";
            }
        }
    }

    function createDeleteInput(imageId, card) {
        const existingInput = manager.querySelector(
            `input[data-delete-input="${imageId}"]`
        );

        if (existingInput) {
            return;
        }

        const input = document.createElement("input");
        input.type = "hidden";
        input.name = "Input.ImagesToDelete";
        input.value = imageId;
        input.dataset.deleteInput = imageId;

        card.appendChild(input);
    }

    function removeDeleteInput(imageId) {
        manager.querySelector(
            `input[data-delete-input="${imageId}"]`
        )?.remove();
    }

    function updateExistingCardButton(card, markedForDelete) {
        const button = card.querySelector(".existing-image-toggle");
        const icon = card.querySelector(".edit-image-action-icon");
        const text = card.querySelector(".edit-image-action-text");

        if (button) {
            button.setAttribute(
                "aria-pressed",
                markedForDelete ? "true" : "false"
            );
        }

        if (icon) {
            icon.textContent = markedForDelete ? "↶" : "✕";
        }

        if (text) {
            text.textContent = markedForDelete
                ? "Отмени"
                : "Премахни";
        }
    }

    function markExistingImageForDeletion(card) {
        const imageId = card.dataset.imageId;

        if (!imageId) {
            return;
        }

        card.classList.add("marked-for-delete");
        createDeleteInput(imageId, card);
        updateExistingCardButton(card, true);
        clearMessage();
        updateCountersAndLabel();
    }

    function restoreExistingImage(card) {
        const imageId = card.dataset.imageId;

        if (!imageId) {
            return;
        }

        if (getResultingImageCount() + 1 > maxImages) {
            showMessage(
                `Можеш да имаш най-много ${maxImages} снимки общо. ` +
                "Премахни нова снимка, преди да възстановиш тази."
            );
            return;
        }

        card.classList.remove("marked-for-delete");
        removeDeleteInput(imageId);
        updateExistingCardButton(card, false);
        clearMessage();
        updateCountersAndLabel();
    }

    function toggleExistingImage(card) {
        if (card.classList.contains("marked-for-delete")) {
            restoreExistingImage(card);
        } else {
            markExistingImageForDeletion(card);
        }
    }

    function isDuplicate(file) {
        return selectedFiles.some(item =>
            item.file.name === file.name &&
            item.file.size === file.size &&
            item.file.lastModified === file.lastModified
        );
    }

    function createFileId(file) {
        const randomPart =
            typeof crypto.randomUUID === "function"
                ? crypto.randomUUID()
                : `${Date.now()}-${Math.random()}`;

        return [
            file.name,
            file.size,
            file.lastModified,
            randomPart
        ].join("-");
    }

    function synchronizeFileInput() {
        const dataTransfer = new DataTransfer();

        selectedFiles.forEach(item => {
            dataTransfer.items.add(item.file);
        });

        newImagesInput.files = dataTransfer.files;
    }

    function formatFileSize(bytes) {
        if (bytes < 1024) {
            return `${bytes} B`;
        }

        if (bytes < 1024 * 1024) {
            return `${(bytes / 1024).toFixed(1)} KB`;
        }

        return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
    }

    function renderNewImages() {
        newImagesGrid.innerHTML = "";

        selectedFiles.forEach(item => {
            const objectUrl = URL.createObjectURL(item.file);

            const card = document.createElement("article");
            card.className = "edit-image-card new-image-card";

            const preview = document.createElement("div");
            preview.className = "edit-image-preview";

            const image = document.createElement("img");
            image.src = objectUrl;
            image.alt = `Преглед на ${item.file.name}`;
            image.addEventListener(
                "load",
                () => URL.revokeObjectURL(objectUrl),
                { once: true }
            );

            const info = document.createElement("div");
            info.className = "new-image-info";

            const fileName = document.createElement("strong");
            fileName.textContent = item.file.name;
            fileName.title = item.file.name;

            const fileSize = document.createElement("small");
            fileSize.textContent = formatFileSize(item.file.size);

            const removeButton = document.createElement("button");
            removeButton.type = "button";
            removeButton.className =
                "edit-image-action new-image-remove";
            removeButton.dataset.fileId = item.id;
            removeButton.innerHTML = `
                <span class="edit-image-action-icon">✕</span>
                <span class="edit-image-action-text">Премахни</span>
            `;

            preview.appendChild(image);
            info.append(fileName, fileSize);
            card.append(preview, info, removeButton);
            newImagesGrid.appendChild(card);
        });

        synchronizeFileInput();
        updateCountersAndLabel();
    }

    function removeNewImage(fileId) {
        selectedFiles = selectedFiles.filter(
            item => item.id !== fileId
        );

        clearMessage();
        renderNewImages();
    }

    function addSelectedFiles(files) {
        clearMessage();

        for (const file of files) {
            if (!allowedTypes.has(file.type)) {
                showMessage(
                    `„${file.name}“ не е JPG, PNG или WEBP файл.`
                );
                continue;
            }

            if (file.size === 0) {
                showMessage(`„${file.name}“ е празен файл.`);
                continue;
            }

            if (file.size > maxFileSize) {
                showMessage(
                    `„${file.name}“ е по-голям от 5 MB.`
                );
                continue;
            }

            if (isDuplicate(file)) {
                continue;
            }

            if (getResultingImageCount() >= maxImages) {
                showMessage(
                    `Можеш да имаш най-много ${maxImages} снимки общо.`
                );
                break;
            }

            selectedFiles.push({
                id: createFileId(file),
                file
            });
        }

        renderNewImages();
    }

    manager.addEventListener("click", event => {
        const target = event.target;

        if (!(target instanceof Element)) {
            return;
        }

        const existingButton = target.closest(
            ".existing-image-toggle"
        );

        if (existingButton) {
            const card = existingButton.closest(
                ".existing-image-card"
            );

            if (card) {
                toggleExistingImage(card);
            }

            return;
        }

        const newImageRemoveButton = target.closest(
            ".new-image-remove"
        );

        if (newImageRemoveButton) {
            const fileId = newImageRemoveButton.dataset.fileId;

            if (fileId) {
                removeNewImage(fileId);
            }
        }
    });

    newImagesInput.addEventListener("change", () => {
        const files = Array.from(newImagesInput.files ?? []);
        addSelectedFiles(files);
    });

    document
        .getElementById("edit-spot-form")
        ?.addEventListener("submit", event => {
            if (getResultingImageCount() < 1) {
                event.preventDefault();
                showMessage(
                    "Площадката трябва да има поне една снимка."
                );
            }
        });

    updateCountersAndLabel();
});
