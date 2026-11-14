/* eslint-disable react-hooks/set-state-in-effect */
/* eslint-disable react-hooks/exhaustive-deps */
import { useEffect, useState } from "react";
import { useAppSelector } from "../state/stateHooks";

interface PropsType {
  currencyFieldValue: string;
  handleOptionButtonClick: (c: string) => void;
  className?: string; // Optional custom width/styling classes
}

function CurrencyDropdown(props: PropsType) {
  const {
    currencyFieldValue,
    handleOptionButtonClick,
    className = "w-full md:w-45",
  } = props;
  const user = useAppSelector((state) => state.authUser.userInformations);
  const userCurrencies = user?.userCurrencies ?? [];

  const [currencyOptions, setCurrencyOptions] = useState(false);
  const [currencies, setCurrencies] = useState<string[]>([]);

  useEffect(() => {
    setCurrencies(userCurrencies.map((c) => c.currency.code));
  }, [user]);

  return (
    <div className={`relative ${className}`}>
      <button
        type="button"
        className={`w-full p-2 bg-bg-surface border-thin border-border-subtle shadow-card cursor-pointer ${currencyOptions ? "rounded-t-sm" : "rounded-sm"}`}
        onClick={() => setCurrencyOptions((prev) => !prev)}
      >
        {currencyFieldValue || "Select Currency"}
      </button>
      {currencyOptions && (
        <div className="absolute w-full z-50">
          {currencies
            .filter((c) => c !== currencyFieldValue)
            .map((c, index, arr) => (
              <button
                type="button"
                key={c}
                onClick={() => {
                  handleOptionButtonClick(c);
                  setCurrencyOptions(false);
                }}
                className={`w-full p-2 bg-bg-surface hover:bg-btn-standard border-x-thin border-b-thin border-border-subtle cursor-pointer ${index === arr.length - 1 ? "rounded-b-sm" : ""}`}
              >
                {c}
              </button>
            ))}
        </div>
      )}
    </div>
  );
}

export default CurrencyDropdown;
