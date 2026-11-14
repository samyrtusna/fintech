import * as Yup from "yup";

export const budgetSchema = Yup.object({
  categoryId: Yup.string().required("Category is required"),
  type: Yup.string().required("Budget type is required"),
  amount: Yup.object({
    amount: Yup.number()
      .positive("Amount must be greater than zero")
      .required("Amount is required"),
    currency: Yup.object({
      code: Yup.string().required("Currency code is required"),
      symbol: Yup.string().required(),
    }),
  }).required(),
});
