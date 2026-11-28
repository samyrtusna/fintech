type PropsType = {
  handleClick: () => void;
  showDate: boolean;
};

function SwitchButton(props: PropsType) {
  const { handleClick, showDate } = props;
  return (
    <button
      type="button"
      onClick={handleClick}
      aria-pressed={showDate}
      className="flex justify-between items-center h-8 w-16 rounded-full bg-bg-muted p-1 border-thin border-border-subtle shadow-subtle cursor-pointer"
    >
      <span
        className={`h-full aspect-square rounded-full transition-opacity ${showDate ? "opacity-0" : "opacity-100 bg-text-muted"}`}
      />
      <span
        className={`h-full aspect-square rounded-full transition-opacity ${showDate ? "opacity-100 bg-btn-standard" : "opacity-0"}`}
      />
    </button>
  );
}

export default SwitchButton;
