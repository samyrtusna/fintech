import { useEffect, useState, type ReactNode } from "react";
import type { Theme } from "../types/themeTypes";
import { ThemeContext } from "../context/themeContext";

export function ThemeProvider({ children }: { children: ReactNode }) {
  const THEME_STORAGE_KEY = "theme";
  const [theme, setTheme] = useState<Theme>(() => {
    return (localStorage.getItem(THEME_STORAGE_KEY) as Theme) || "light";
  });

  useEffect(() => {
    document.documentElement.classList.remove("light", "dark");

    document.documentElement.classList.add(theme);

    localStorage.setItem(THEME_STORAGE_KEY, theme);
  }, [theme]);

  const toggleTheme = () => {
    setTheme((prev) => (prev === "light" ? "dark" : "light"));
  };

  return (
    <ThemeContext.Provider value={{ theme, toggleTheme }}>
      {children}
    </ThemeContext.Provider>
  );
}
