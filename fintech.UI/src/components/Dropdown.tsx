import { useState } from "react";

export interface DropdownProps<T> {
  selectedValue: T;
  options: T[];
  onSelect: (value: T) => void;
}

function Dropdown<
  T extends { key?: string | number; value?: string | number | Date },
>(props: DropdownProps<T>) {
  const { selectedValue, options, onSelect } = props;

  const [isOpen, setIsOpen] = useState(false);

  return (
    <div className="relative w-full ">
      <button
        type="button"
        onClick={() => setIsOpen((prev) => !prev)}
        className={`w-full py-1 px-2 border-thin border-border-subtle bg-bg-muted hover:bg-bg-secondary cursor-pointer ${
          isOpen ? "rounded-t-sm" : "rounded-sm"
        }`}
      >
        {selectedValue.key}
      </button>

      {isOpen && (
        <div className="absolute top-8 w-full flex flex-col z-50">
          {options
            .filter((item) => item.key !== selectedValue.key)
            .map((item, index, arr) => (
              <button
                key={item.key}
                type="button"
                onClick={() => {
                  onSelect(item);
                  setIsOpen(false);
                }}
                className={`py-1 px-2 bg-bg-muted hover:bg-bg-secondary border-x-thin border-x-border-subtle cursor-pointer ${
                  index === arr.length - 1
                    ? "rounded-b-sm border-b-thin border-b-border-subtle"
                    : ""
                }`}
              >
                {item.key}
              </button>
            ))}
        </div>
      )}
    </div>
  );
}

export default Dropdown;
