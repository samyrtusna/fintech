import { useState } from "react";

export interface DropdownProps<T> {
  selectedValue: T;
  options: T[];
  onSelect: (value: T) => void;
}

function Dropdown<
  T extends {
    shortLabel?: string | number;
    longLabel: string | number;
    value?: string | number | Date;
  },
>(props: DropdownProps<T>) {
  const { selectedValue, options, onSelect } = props;

  const [isOpen, setIsOpen] = useState(false);

  return (
    <div className="relative w-full">
      <button
        type="button"
        onClick={() => setIsOpen((prev) => !prev)}
        className={`w-full py-1.5 px-3 text-sm font-medium text-text-primary border-thin border-border-subtle bg-bg-surface hover:bg-bg-muted shadow-subtle cursor-pointer ${
          isOpen ? "rounded-t-sm" : "rounded-sm"
        }`}
      >
        {selectedValue.shortLabel}
      </button>

      {isOpen && (
        <div className="absolute top-full w-full flex flex-col z-50 bg-dropdown-bg shadow-card rounded-b-sm overflow-hidden">
          {options
            .filter((item) => item.shortLabel !== selectedValue.shortLabel)
            .map((item, index, arr) => (
              <button
                key={item.shortLabel}
                type="button"
                onClick={() => {
                  onSelect(item);
                  setIsOpen(false);
                }}
                className={`py-1.5 px-3 text-sm text-text-primary bg-dropdown-bg hover:bg-bg-muted border-x-thin border-x-border-subtle cursor-pointer ${
                  index === arr.length - 1
                    ? "rounded-b-sm border-b-thin border-b-border-subtle"
                    : ""
                }`}
              >
                {item.shortLabel}
              </button>
            ))}
        </div>
      )}
    </div>
  );
}

export default Dropdown;
