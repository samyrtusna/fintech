import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";
import { Provider } from "react-redux";
import store from "./state/store.ts";
import { ThemeProvider } from "./providers/themeProvider.tsx";
import { setupAxiosInterceptors } from "./API/Axios/setupInterceptors.ts";
import { AuthProvider } from "./providers/authProvider.tsx";

setupAxiosInterceptors();

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <Provider store={store}>
      <ThemeProvider>
        <AuthProvider>
          <App />
        </AuthProvider>
      </ThemeProvider>
    </Provider>
  </StrictMode>,
);
