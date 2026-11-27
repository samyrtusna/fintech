import { Link, NavLink, useNavigate } from "react-router-dom";
import { MdCurrencyExchange } from "react-icons/md";
import IconComponent from "./IconComponent";
import useTheme from "../hooks/useTheme";
import { useAppSelector } from "../state/stateHooks";
import { DecodeToken } from "../helpers/tokenDecoder";
import { IoMdHome } from "react-icons/io";
import { FaChartPie, FaUser, FaWallet } from "react-icons/fa";
import { AiOutlineTransaction } from "react-icons/ai";
import { GoGoal } from "react-icons/go";
import { IoChevronForward, IoSunnyOutline } from "react-icons/io5";
import { BsCardChecklist } from "react-icons/bs";
import { LuMoonStar } from "react-icons/lu";
import { useState } from "react";

function Navbar() {
  const { theme, toggleTheme } = useTheme();

  const accessToken = useAppSelector((state) => state.authUser.accessToken);

  const [showUserDropdown, setShowUserDropdown] = useState(false);

  const navigate = useNavigate();

  const iconLinks = [
    { icon: IoMdHome, to: "/" },
    { icon: FaChartPie, to: "/dashboard" },
    {
      icon: AiOutlineTransaction,
      to: "/financialTransactions",
    },
    { icon: FaWallet, to: "/budget" },
    { icon: GoGoal, to: "/goal" },
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
      <div className="flex justify-between items-center min-w-dvw h-12 px-3 bg-navbar-bg backdrop-blur-md border-b-thin border-border-subtle md:hidden">
        {!accessToken ? (
          <NavLink
            to="/login"
            className={({ isActive }) =>
              `h-8 w-8 flex justify-center items-center border-thin border-border-subtle ${isActive ? "bg-bg-muted text-text-secondary scale-105 rounded-sm" : "bg-bg-surface text-text-primary rounded-xl"}`
            }
          >
            <IconComponent icon={FaUser} />
          </NavLink>
        ) : (
          <div className="relative">
            <NavLink
              to="#"
              onClick={() => setShowUserDropdown(!showUserDropdown)}
              className={({ isActive }) =>
                `h-8 w-8 flex justify-center items-center-safe text-xs font-bold border-thin border-border-subtle ${isActive ? "bg-bg-muted text-text-secondary scale-105 rounded-sm" : "bg-bg-surface text-text-primary rounded-xl"}`
              }
            >
              {initials}
            </NavLink>
            <div
              className={`z-20 w-40 rounded-md shadow-card overflow-hidden ${showUserDropdown ? "absolute" : "hidden"}`}
            >
              <button
                onClick={() => {
                  navigate("#");
                  setShowUserDropdown(false);
                }}
                className="w-full p-2 text-sm text-text-primary bg-dropdown-bg hover:bg-bg-muted border-thin border-border-subtle rounded-t-sm"
              >
                Category
              </button>
              <button
                onClick={() => {
                  navigate("#");
                  setShowUserDropdown(false);
                }}
                className="w-full p-2 text-sm text-text-primary bg-dropdown-bg hover:bg-bg-muted border-thin border-border-subtle rounded-b-sm"
              >
                Currency
              </button>
            </div>
          </div>
        )}
        {iconLinks.map((link) => (
          <NavLink
            key={link.to}
            to={link.to}
            className={({ isActive }) =>
              `h-8 w-8 flex justify-center items-center border-thin border-border-subtle ${isActive ? "bg-bg-muted text-text-secondary scale-105 rounded-sm" : "bg-bg-surface text-text-primary rounded-xl"}`
            }
          >
            <IconComponent icon={link.icon} />
          </NavLink>
        ))}
        <button
          onClick={toggleTheme}
          className="h-8 w-8 flex justify-center items-center rounded-xl bg-bg-surface text-text-secondary border-thin border-border-subtle"
        >
          <IconComponent
            icon={theme === "dark" ? IoSunnyOutline : LuMoonStar}
          />
        </button>
      </div>

      <div className="hidden relative md:flex w-dvw my-8 justify-center items-center">
        <div className="flex h-12 w-fit px-5 gap-1 items-center bg-navbar-bg backdrop-blur-md border-thin border-border-subtle rounded-full shadow-card">
          {textLinks.map((link) => (
            <NavLink
              key={link.label}
              to={link.to}
              className={({ isActive }) =>
                `flex h-9 items-center px-3 rounded-full text-sm transition-all ${isActive ? "bg-bg-muted text-text-secondary font-bold shadow-subtle" : "text-text-primary hover:bg-bg-secondary"}`
              }
            >
              {link.label}
            </NavLink>
          ))}
        </div>
        <div className="absolute flex items-center h-12 px-4 rounded-full shadow-card bg-navbar-bg backdrop-blur-md border-thin border-border-subtle top-0 right-10">
          <button
            className="cursor-pointer text-text-secondary hover:scale-110"
            onClick={toggleTheme}
          >
            <IconComponent
              icon={theme === "dark" ? IoSunnyOutline : LuMoonStar}
            />
          </button>
        </div>
        <div
          className={`${accessToken ? "absolute" : "hidden"} top-0 left-10 h-12 w-12`}
        >
          <Link
            to="#"
            onClick={() => setShowUserDropdown(!showUserDropdown)}
          >
            <div className="relative w-full h-full bg-bg-surface hover:bg-bg-muted text-text-secondary border-thin border-border-subtle rounded-full shadow-card font-bold flex justify-center items-center">
              {initials}
            </div>
          </Link>
          <div
            className={`z-20 w-44 mt-2 rounded-md shadow-card overflow-hidden ${showUserDropdown ? "absolute" : "hidden"}`}
          >
            <button
              onClick={() => {
                navigate("#");
                setShowUserDropdown(false);
              }}
              className="inline-flex items-center justify-between w-full px-3 py-2 text-text-primary bg-dropdown-bg hover:bg-bg-muted border-thin border-border-subtle rounded-t-sm cursor-pointer"
            >
              <BsCardChecklist className="text-orange-500" />
              <span className="text-sm font-medium">Category</span>
              <IoChevronForward className="text-text-muted" />
            </button>
            <button
              onClick={() => {
                navigate("#");
                setShowUserDropdown(false);
              }}
              className="inline-flex items-center justify-between w-full px-3 py-2 text-text-primary bg-dropdown-bg hover:bg-bg-muted border-thin border-border-subtle rounded-b-sm cursor-pointer"
            >
              <MdCurrencyExchange className="text-emerald-500" />
              <span className="text-sm font-medium">Currency</span>
              <IoChevronForward className="text-text-muted" />
            </button>
          </div>
        </div>
      </div>
    </>
  );
}

export default Navbar;
