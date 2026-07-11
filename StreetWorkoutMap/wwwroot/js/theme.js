const themeToggleButton = document.getElementById("theme-toggle");

const savedTheme = localStorage.getItem("theme");

if (savedTheme) {
    applyTheme(savedTheme);
} else {
    const preferedDarkTheme = window.matchMedia(
        "(prefers-color-scheme: dark)"
    ).matches;

    applyTheme(preferedDarkTheme ? "dark" : "light");
}

themeToggleButton?.addEventListener("click", () => {
    const currentTheme = document.documentElement.dataset.theme ?? "light";

    const newTheme = currentTheme === "dark" ? "light" : "dark";

    applyTheme(newTheme);
    localStorage.setItem("theme", newTheme)
});



function applyTheme(theme) {
    document.documentElement.dataset.theme = theme;

    if (themeToggleButton) {
        themeToggleButton.textContent = theme === "dark" ? "☀️" : "🌙";

    }

    updateLogo(theme);
}


function updateLogo(theme) {
    const logo = document.getElementById("site-logo");

    if (!logo) {
        return;
    }

    logo.src = theme === "dark"
        ? "/images/logo/icon.png"
        : "/images/logo/icon_light.png";
}