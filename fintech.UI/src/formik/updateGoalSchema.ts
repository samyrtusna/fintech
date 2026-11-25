import * as Yup from "yup";

export const updateGoalSchema = Yup.object({
  title: Yup.string().required("Title is required"),
  targetAmount: Yup.object({
    amount: Yup.number()
      .positive("Amount must be greater than zero")
      .required("Amount is required"),
    currency: Yup.object({
      code: Yup.string().required("Currency code is required"),
      symbol: Yup.string().required(),
    }),
  }).required(),
});
