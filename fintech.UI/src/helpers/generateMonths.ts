import type { MonthItem } from "../types/financialTransactionTypes";

export default function generateMonths(creationDate: Date): MonthItem[] {
  const result: MonthItem[] = [];

  const createdAt = new Date(creationDate);
  const now = new Date();

  // Start from the signup month
  const currentDate = new Date(
    createdAt.getFullYear(),
    createdAt.getMonth(),
    1,
  );

  // Generate one entry for every month until the current month
  while (
    currentDate.getFullYear() < now.getFullYear() ||
    (currentDate.getFullYear() === now.getFullYear() &&
      currentDate.getMonth() <= now.getMonth())
  ) {
    const shortLabel = currentDate
      .toLocaleDateString("en-US", {
        month: "short",
        year: "numeric",
      })
      .toUpperCase();

    const longLabel = currentDate.toLocaleDateString("en-US", {
      month: "long",
      year: "numeric",
    });

    const value = new Date(
      currentDate.getFullYear(),
      currentDate.getMonth() + 1,
      1,
    );

    result.push({
      shortLabel,
      longLabel,
      value,
    });

    // Move to the next month
    currentDate.setMonth(currentDate.getMonth() + 1);
  }

  // Most recent month first
  return result.reverse();
}
