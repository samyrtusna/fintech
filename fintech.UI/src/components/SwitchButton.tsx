type PropsType = {
  handleClick: () => void;
  showDate: boolean;
};

function SwitchButton(props: PropsType) {
  const { handleClick, showDate } = props;
  return (
    <div className="flex justify-between h-8 w-16 rounded-full bg-bg-contrast p-1 border-thin border-border-subtle">
      <div
        onClick={handleClick}
        className={`h-full aspect-square rounded-full bg-btn-disabled cursor-pointer ${showDate ? "opacity-0" : "opacity-100"}`}
      ></div>
      <div
        onClick={handleClick}
        className={`h-full aspect-square rounded-full bg-bg-surface cursor-pointer ${showDate ? "opacity-100" : "opacity-0"}`}
      ></div>
    </div>
  );
}

export default SwitchButton;
