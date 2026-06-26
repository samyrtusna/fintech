import { createContext } from "react";
import type { ThemeContextType } from "../types/themeTypes";

export const ThemeContext = createContext<ThemeContextType | null>(null);
