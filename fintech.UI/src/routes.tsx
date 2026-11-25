import { Routes as Switch, Route } from "react-router-dom";
import NewFinancialTransaction from "./pages/NewFinancialTransaction";
import AppLayout from "./layout/AppLayout";
import FinancialTransactions from "./pages/FinancialTransactions";
import Login from "./pages/Login";
import Register from "./pages/Register";
import ProtectedRoute from "./providers/protectedRoute";
import LandingPage from "./pages/LandingPage";
import Dashboard from "./pages/Dashboard";
import Budgets from "./pages/Budgets";
import NewBudget from "./pages/NewBudget";
import Goals from "./pages/Goals";
import NewGoal from "./pages/NewGoal";

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
          path="/financialTransactions/new"
          element={
            <ProtectedRoute>
              <NewFinancialTransaction />
            </ProtectedRoute>
          }
        />
        <Route
          index
          element={<LandingPage />}
        />
        <Route
          path="/dashboard"
          element={
            <ProtectedRoute>
              <Dashboard />
            </ProtectedRoute>
          }
        />
        <Route
          path="/budget"
          element={
            <ProtectedRoute>
              <Budgets />
            </ProtectedRoute>
          }
        />
        <Route
          path="/budget/new"
          element={
            <ProtectedRoute>
              <NewBudget />
            </ProtectedRoute>
          }
        />
        <Route
          path="/goal"
          element={
            <ProtectedRoute>
              <Goals />
            </ProtectedRoute>
          }
        />
        <Route
          path="/goal/new"
          element={
            <ProtectedRoute>
              <NewGoal />
            </ProtectedRoute>
          }
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
