type PropsType = {
  label: string;
  type: "submit" | "reset" | "button";
  background: string;
  hoverBg: string;
  textColor: string;
  handleClick?: () => void;
  disabled?: boolean;
};
function Button(props: PropsType) {
  const { label, type, background, hoverBg, textColor, handleClick, disabled } =
    props;
  return (
    <button
      type={type}
      onClick={handleClick}
      disabled={disabled}
      className={`w-full py-2 px-4 text-sm font-semibold items-center shadow-subtle ${background} ${hoverBg} ${textColor} disabled:bg-btn-disabled disabled:hover:bg-btn-disabled-hover disabled:text-btn-disabled-text disabled:cursor-not-allowed rounded-full md:rounded-sm cursor-pointer`}
    >
      {label}
    </button>
  );
}

export default Button;
