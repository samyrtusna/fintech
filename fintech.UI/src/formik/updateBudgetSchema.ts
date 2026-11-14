import * as Yup from "yup";

export const updateBudgetSchema = Yup.object({
  amount: Yup.number()
    .positive("Amount must be greater than zero")
    .required("Amount is required"),

  currency: Yup.object({
    code: Yup.string().required("Currency code is required"),
    symbol: Yup.string().required("Currency symbol is required"),
  }).required("Currency is required"),
});
