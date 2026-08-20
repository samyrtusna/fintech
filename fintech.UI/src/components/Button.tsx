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
      className={`w-full p-1 items-center ${background} ${hoverBg} ${textColor} disabled:bg-btn-disabled disabled:hover:bg-btn-disabled-hover disabled:text-btn-disabled-text disabled:cursor-not-allowed rounded-full lg:rounded-md cursor-pointer`}
    >
      {label}
    </button>
  );
}

export default Button;
