interface SpinnerProps {
  size?: "sm" | "md" | "lg";
  color?: string;
}

function Spinner({ size = "md", color = "text-blue-600" }: SpinnerProps) {
  const sizeClasses = {
    sm: "h-5 w-5 stroke-[3]",
    md: "h-8 w-8 stroke-[2]",
    lg: "h-12 w-12 stroke-[1.5]",
  };
  return (
    <svg
      className={`animate-spin ${sizeClasses[size]} ${color}`}
      fill="none"
      viewBox="0 0 24 24"
    >
      {/* Background track circle */}
      <circle
        className="opacity-25"
        cx="12"
        cy="12"
        r="10"
        stroke="currentColor"
      />
      {/* Dynamic spinning arc */}
      <path
        className="opacity-75"
        fill="currentColor"
        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
      />
    </svg>
  );
}

export default Spinner;
