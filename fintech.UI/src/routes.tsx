import { Routes as Switch, Route } from "react-router-dom";
import NewFinancialTransaction from "./pages/NewFinancialTransaction";
import AppLayout from "./layout/AppLayout";
import FinancialTransactions from "./pages/FinancialTransactions";
import FinancialTransaction from "./pages/FinancialTransaction";
import Login from "./pages/Login";
import Register from "./pages/Register";
import ProtectedRoute from "./providers/protectedRoute";

const Routes = () => {
  return (
    <Switch>
      <Route
        path="/"
        element={<AppLayout />}
      >
        <Route
          path="/financialTransactions"
          element={
            <ProtectedRoute>
              <FinancialTransactions />
            </ProtectedRoute>
          }
        />
        <Route
          path="/financialTransactions/newFinancialTransaction"
          element={<NewFinancialTransaction />}
        />
        <Route
          path="/financialTransactions/financialTransaction/:id"
          element={<FinancialTransaction />}
        />
      </Route>
      <Route
        path="/login"
        element={<Login />}
      />
      <Route
        path="/register"
        element={<Register />}
      />
    </Switch>
  );
};

export default Routes;
