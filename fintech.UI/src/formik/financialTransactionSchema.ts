import * as Yup from "yup";

export const financialTransactionSchema = Yup.object({
  categoryId: Yup.string().required("Category is required"),

  amount: Yup.number()
    .positive("Amount must be greater than zero")
    .required("Amount is required"),

  currency: Yup.string().required("Currency is required"),

  transactionDate: Yup.date().required("Transaction date is required"),

  description: Yup.string().max(
    500,
    "Description cannot exceed 500 characters",
  ),

  isEssential: Yup.boolean().required(),
});
