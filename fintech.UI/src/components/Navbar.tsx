import { Link, NavLink } from "react-router-dom";
import {
  CircleUser,
  ChartNoAxesCombined,
  MoonStar,
  Sun,
  Handshake,
  Wallet,
  Gem,
  Home,
} from "lucide-react";
import IconComponent from "./IconComponent";
import useTheme from "../hooks/useTheme";
import { useAppSelector } from "../state/stateHooks";
import { DecodeToken } from "../helpers/tokenDecoder";

function Navbar() {
  const { theme, toggleTheme } = useTheme();
  const accessToken = useAppSelector((state) => state.authUser.accessToken);
  const iconLinks = [
    { icon: Home, to: "/" },
    { icon: ChartNoAxesCombined, to: "/dashboard" },
    { icon: Handshake, to: "/financialTransactions" },
    { icon: Wallet, to: "/budget" },
    { icon: Gem, to: "/goal" },
  ];

  const textLinks = [
    { label: "Home", to: "/" },
    { label: "Dashboard", to: "/dashboard" },
    { label: "Transactions", to: "/financialTransactions" },
    { label: "Budget", to: "/budget" },
    { label: "Goals", to: "/goal" },
  ];

  let initials: string = "";

  if (accessToken) {
    const decoded = DecodeToken(accessToken);
    const userName = decoded.userName;

    initials = userName.slice(0, 2).toUpperCase();
  }
  return (
    <>
      <div className="flex justify-between items-center min-w-dvw h-12 px-3 bg-navbar-bg md:hidden">
        {!accessToken ? (
          <Link
            to="/login"
            className="h-8 w-8 rounded-xl "
          >
            <IconComponent icon={CircleUser} />
          </Link>
        ) : (
          <Link
            to="#"
            className="h-8 w-8 py-0.5 bg-bg-muted rounded-md text-center font-bold"
          >
            {initials}
          </Link>
        )}{" "}
        {iconLinks.map((link) => (
          <NavLink
            key={link.to}
            to={link.to}
            className={({ isActive }) =>
              `h-8 w-8 rounded-sm ${isActive ? "bg-bg-muted" : "bg-navbar-bg "}`
            }
          >
            <IconComponent icon={link.icon} />
          </NavLink>
        ))}
        <button
          onClick={toggleTheme}
          className="h-8 w-8 rounded-xl bg-navbar-bg "
        >
          <IconComponent icon={theme === "dark" ? Sun : MoonStar} />
        </button>
      </div>
      {/* the div background is hardcoded */}

      <div className="hidden relative md:flex w-dvw my-8 justify-center items-center">
        {/* Navlink border color is hardcoded */}

        <div className="flex h-12 w-fit px-5 items-center bg-navbar-bg  rounded-full shadow-card">
          {textLinks.map((link) => (
            <NavLink
              key={link.label}
              to={link.to}
              className={({ isActive }) =>
                `flex h-full items-center px-2 text-sm ${isActive ? "scale-110 font-bold" : "scale-100 "}`
              }
            >
              {link.label}
            </NavLink>
          ))}
        </div>
        <div className="absolute flex items-center h-12 px-4 rounded-full shadow-card bg-navbar-bg top-0 right-10">
          <button
            className="cursor-pointer hover:scale-110 hover:font-bold"
            onClick={toggleTheme}
          >
            <IconComponent icon={theme === "dark" ? Sun : MoonStar} />
          </button>
        </div>
        <div
          className={`${accessToken ? "absolute" : "hidden"} top-0 left-10 h-12 w-12`}
        >
          <Link to="#">
            <div className="w-full h-full bg-bg-muted rounded-full shadow-card font-bold flex justify-center items-center">
              {initials}
            </div>
          </Link>
        </div>
      </div>
    </>
  );
}

export default Navbar;
