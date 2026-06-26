import { BrowserRouter, Routes, Route } from "react-router-dom";
import { Toaster } from "sonner";
import Login from "./pages/Login";
import Register from "./pages/Register";
import AppLayout from "./layout/AppLayout";
import FinancialTransactions from "./pages/FinancialTransactions";
import ProtectedRoute from "./providers/protectedRoute";
import NewFinancialTransaction from "./components/NewFinancialTransaction";

function App() {
  return (
    <BrowserRouter>
      <Toaster position="top-right" />
      <Routes>
        <Route
          path="/"
          element={<AppLayout />}
        >
          <Route
            path="financialTransactions"
            element={
              <ProtectedRoute>
                <FinancialTransactions />
              </ProtectedRoute>
            }
          />
          <Route
            path="financialTransactions/newFinancialTransaction"
            element={<NewFinancialTransaction />}
          />
        </Route>
        <Route
          path="login"
          element={<Login />}
        />
        <Route
          path="register"
          element={<Register />}
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
