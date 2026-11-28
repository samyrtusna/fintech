import { useState } from "react";
import {
  format,
  addMonths,
  subMonths,
  startOfWeek,
  endOfWeek,
  startOfMonth,
  endOfMonth,
  eachDayOfInterval,
  isSameMonth,
  isSameDay,
  isToday,
} from "date-fns";
import { IoChevronBack, IoChevronForward } from "react-icons/io5";

type PropsType = {
  handledate: (d: Date) => void;
  selectedDate: Date | null;
};
export default function Calendar(props: PropsType) {
  const { handledate, selectedDate } = props;

  const weekDays = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
  const [currentMonth, setCurrentMonth] = useState(new Date());

  // Calculate dates for the grid
  const monthStart = startOfMonth(currentMonth);
  const monthEnd = endOfMonth(monthStart);
  const startDate = startOfWeek(monthStart);
  const endDate = endOfWeek(monthEnd);

  const days = eachDayOfInterval({ start: startDate, end: endDate });

  const nextMonth = () => setCurrentMonth(addMonths(currentMonth, 1));
  const prevMonth = () => setCurrentMonth(subMonths(currentMonth, 1));

  return (
    <div className="w-xs mx-auto p-5 bg-bg-surface text-text-primary rounded-md shadow-card border-thin border-border-subtle">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-base font-bold text-text-primary">
          {format(currentMonth, "MMMM yyyy")}
        </h2>
        <div className="flex space-x-1">
          <button
            type="button"
            onClick={prevMonth}
            className="p-1.5 text-text-secondary hover:bg-bg-muted rounded-sm transition cursor-pointer"
          >
            <IoChevronBack className="w-4 h-4" />
          </button>
          <button
            type="button"
            onClick={nextMonth}
            className="p-1.5 text-text-secondary hover:bg-bg-muted rounded-sm transition cursor-pointer"
          >
            <IoChevronForward className="w-4 h-4" />
          </button>
        </div>
      </div>

      <div className="grid grid-cols-7 gap-0.5 text-center font-semibold text-xs text-text-muted mb-2">
        {weekDays.map((d) => (
          <div
            key={d}
            className="py-1.5"
          >
            {d}
          </div>
        ))}
      </div>

      <div className="grid grid-cols-7 gap-0.5">
        {days.map((day, idx) => {
          const isCurrentMonth = isSameMonth(day, currentMonth);
          const isSelected = isSameDay(day, selectedDate!);
          const isCurrentDay = isToday(day);

          return (
            <button
              type="button"
              key={idx}
              onClick={() => handledate(day)}
              className={`
                h-8 w-8 mx-auto flex items-center justify-center text-sm rounded-full transition-all duration-200 cursor-pointer
                ${!isCurrentMonth ? "text-text-muted/40" : "text-text-primary"}
                ${isCurrentDay && !isSelected ? "border border-accent font-bold text-text-secondary" : ""}
                ${isSelected ? "bg-btn-standard text-btn-standard-text font-semibold shadow-subtle" : "hover:bg-bg-muted"}
              `}
            >
              <time dateTime={format(day, "yyyy-MM-dd")}>
                {format(day, "d")}
              </time>
            </button>
          );
        })}
      </div>
    </div>
  );
}
