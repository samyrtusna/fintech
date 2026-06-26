import { useEffect } from "react";

function App() {
  const toggleTheme = () => {
    const newTheme = document.documentElement.classList.contains("dark")
      ? "light"
      : "dark";

    document.documentElement.classList.remove("light", "dark");
    document.documentElement.classList.add(newTheme);

    localStorage.setItem("theme", newTheme);
  };

  useEffect(() => {
    // If the user has selected a theme, use that
    const selectedTheme = localStorage.getItem("theme");

    document.documentElement.classList.remove("light", "dark");

    if (selectedTheme) {
      document.documentElement.classList.add(selectedTheme);

      // else if the users OS preferences prefers dark mode
    } else if (window.matchMedia("(prefers-color-scheme: dark)").matches) {
      document.documentElement.classList.add("dark");

      // else use light mode
    } else {
      document.documentElement.classList.add("light");
    }
  }, []);
  return (
    <div className=" bg-bg flex">
      <div className="h-dvh w-1/2 grid  place-content-center">
        <h1 className="text-5xl font-extrabold my-4 text-t-active">
          Hello Azul Bonjour
        </h1>
        <button
          className="mt-3 py-1 px-3  rounded-bt-r bg-bt-g hover:bg-bt-g-h text-bt-g-text shadow-bt-sh"
          onClick={toggleTheme}
        >
          Toggle Mode
        </button>
        <button className="mt-3 py-1 px-3 rounded-bt-r bg-bt-d hover:bg-bt-d-h text-bt-g-text shadow-bt-sh">
          Delete Mode
        </button>
        <button className="mt-3 py-1 px-3 rounded-bt-r bg-bt-ds hover:bg-bt-ds-h text-bt-g-text shadow-bt-sh">
          Disable Mode
        </button>
      </div>
      <div className="h-dvh w-1/2 grid  place-content-center bg-bg-sec border border-l-box-br-c border-l-box-br-w">
        <h1 className="text-5xl font-extrabold my-4 text-t-active">
          Tailwind Themes
        </h1>
      </div>
    </div>
  );
}
