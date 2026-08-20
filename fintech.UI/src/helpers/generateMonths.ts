import store from "../state/store";
import type { MonthItem } from "../types/financialTransactionTypes";
import { DecodeToken } from "./tokenDecoder";

export default function generateMonths(): MonthItem[] {
  const result: MonthItem[] = [];

  const state = store.getState();
  const token = state.authUser?.accessToken;
  const tokenClaims = DecodeToken(token!);
  const createdAt = new Date(tokenClaims.createdAt);

  const startYear = createdAt.getFullYear();
  const startMonth = createdAt.getMonth() + 1;
  const now = new Date();

  let year = startYear;
  let month = startMonth;

  while (
    year < now.getFullYear() ||
    (year === now.getFullYear() && month <= now.getMonth() + 1)
  ) {
    const mm = String(month).padStart(2, "0");
    const numericDate = new Date(year, month - 1, 1);
    const stringDate = numericDate.toLocaleDateString("en-US", {
      month: "long",
      year: "numeric",
    });

    result.push({
      key: `${year}-${mm}`,
      label: year === now.getFullYear() ? stringDate : `${mm}-${year}`,
      year,
      month,
    });
    month++;

    if (month > 12) {
      month = 1;
      year++;
    }
  }
  return result.reverse();
}
