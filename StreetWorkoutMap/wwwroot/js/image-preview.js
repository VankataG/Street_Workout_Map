document.addEventListener("DOMContentLoaded", () => {
    const input = document.getElementById("image-upload-input");
    const label = document.getElementById("image-upload-label");
    const title = document.getElementById("image-upload-title");
    const description = document.getElementById(
        "image-upload-description"
    );
    const preview = document.getElementById("image-preview");
    const message = document.getElementById(
        "image-upload-message"
    );

    if (
        !input ||
        !label ||
        !title ||
        !description ||
        !preview ||
        !message
    ) {
        return;
    }

    const maxFiles = 3;
    const maxFileSize = 5 * 1024 * 1024;

    const allowedTypes = [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    let selectedFiles = [];

    input.addEventListener("change", () => {
        const newFiles = Array.from(input.files);

        addFiles(newFiles);
    });

    function addFiles(files) {
        clearMessage();

        for (const file of files) {
            if (selectedFiles.length >= maxFiles) {
                showError(
                    `Можеш да добавиш най-много ${maxFiles} снимки.`
                );

                break;
            }

            if (!allowedTypes.includes(file.type)) {
                showError(
                    `"${file.name}" не е JPG, PNG или WEBP файл.`
                );

                continue;
            }

            if (file.size > maxFileSize) {
                showError(
                    `"${file.name}" е по-голям от 5 MB.`
                );

                continue;
            }

            const alreadySelected = selectedFiles.some(
                selectedFile =>
                    selectedFile.name === file.name &&
                    selectedFile.size === file.size &&
                    selectedFile.lastModified === file.lastModified
            );

            if (alreadySelected) {
                continue;
            }

            selectedFiles.push(file);
        }

        syncInputFiles();
        renderPreviews();
    }

    function removeFile(index) {
        selectedFiles.splice(index, 1);

        syncInputFiles();
        renderPreviews();
        clearMessage();
    }

    function syncInputFiles() {
        const dataTransfer = new DataTransfer();

        for (const file of selectedFiles) {
            dataTransfer.items.add(file);
        }

        input.files = dataTransfer.files;
    }

    function renderPreviews() {
        preview.innerHTML = "";

        selectedFiles.forEach((file, index) => {
            const previewItem = document.createElement("div");
            previewItem.className = "image-preview-item";

            const image = document.createElement("img");
            image.alt = `Преглед на ${file.name}`;

            const objectUrl = URL.createObjectURL(file);
            image.src = objectUrl;

            image.addEventListener(
                "load",
                () => URL.revokeObjectURL(objectUrl),
                { once: true }
            );

            const removeButton =
                document.createElement("button");

            removeButton.type = "button";
            removeButton.className = "image-preview-remove";
            removeButton.textContent = "×";
            removeButton.setAttribute(
                "aria-label",
                `Премахни ${file.name}`
            );

            removeButton.addEventListener("click", event => {
                event.preventDefault();
                event.stopPropagation();

                removeFile(index);
            });

            const fileName = document.createElement("span");
            fileName.className = "image-preview-file-name";
            fileName.textContent = file.name;
            fileName.title = file.name;

            previewItem.append(
                image,
                removeButton,
                fileName
            );

            preview.appendChild(previewItem);
        });

        updateUploadLabel();
    }

    function updateUploadLabel() {
        const count = selectedFiles.length;

        if (count === 0) {
            label.classList.remove("has-images");
            title.textContent = "Избери снимки";
            description.textContent =
                "JPG, PNG или WEBP — до 5 MB на снимка";

            return;
        }

        label.classList.add("has-images");

        title.textContent =
            count === 1
                ? "Избрана е 1 снимка"
                : `Избрани са ${count} снимки`;

        description.textContent =
            count < maxFiles
                ? `Можеш да добавиш още ${maxFiles - count}`
                : "Достигнат е максималният брой снимки";
    }

    function showError(text) {
        message.textContent = text;
        message.classList.add("image-upload-message-error");
    }

    function clearMessage() {
        message.textContent = "";
        message.classList.remove(
            "image-upload-message-error"
        );
    }
});