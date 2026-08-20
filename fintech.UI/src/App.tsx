import { BrowserRouter } from "react-router-dom";
import { Toaster } from "sonner";
import Routes from "./routes";

function App() {
  return (
    <BrowserRouter>
      <Toaster position="top-right" />
      <Routes />
    </BrowserRouter>
  );
}

export default App;
