/* eslint-disable react-hooks/exhaustive-deps */

import { useFormik, type FormikHelpers } from "formik";
import Button from "../components/Button";
import CurrencyDropdown from "../components/CurrencyDropdown";
import { useAppSelector } from "../state/stateHooks";
import type {
  NewBudgetRequest,
  NewBudgetValuesType,
} from "../types/budgetTypes";
import { budgetSchema } from "../formik/budgetSchema";
import categoryService from "../API/Services/categoryService";
import { useCallback, useEffect } from "react";
import { setCategories } from "../state/slices/categorySlice";
import { useDispatch } from "react-redux";
import { toast } from "sonner";
import budgetService from "../API/Services/budgetService";
import { useNavigate } from "react-router-dom";
import { IoChevronBack } from "react-icons/io5";
import { FaRegMoneyBillAlt } from "react-icons/fa";

function NewBudget() {
  const user = useAppSelector((state) => state.authUser.userInformations);
  const categories = useAppSelector((state) => state.categories.items);
  const expenseCategories = categories.filter(
    (c) => c.type === "Expense" && c.parentCategoryId !== null,
  );

  const today = new Date();
  const startDate = new Date(today.getFullYear(), today.getMonth(), 1);
  const endDate = new Date(today.getFullYear(), today.getMonth() + 1, 0);

  const period = {
    startDate: `${startDate.getFullYear()}-${String(startDate.getMonth() + 1).padStart(2, "0")}-${String(startDate.getDate()).padStart(2, "0")}`,
    endDate: `${endDate.getFullYear()}-${String(endDate.getMonth() + 1).padStart(2, "0")}-${String(endDate.getDate()).padStart(2, "0")}`,
  };

  const dispatch = useDispatch();
  const navigate = useNavigate();

  const initialValues: NewBudgetValuesType = {
    categoryId: "",
    type: "ByCategory",
    amount: { amount: 0.01, currency: { code: "", symbol: "" } },
  };

  const handleSubmit = async (
    values: NewBudgetValuesType,
    props: FormikHelpers<NewBudgetValuesType>,
  ) => {
    try {
      const selectedCurrency = user?.userCurrencies.find(
        (c) => c.currency.code === values.amount.currency.code,
      );

      if (!selectedCurrency) {
        throw new Error("Selected currency not found");
      }

      const request: NewBudgetRequest = {
        ...values,
        amount: {
          amount: values.amount.amount,
          currency: {
            code: values.amount.currency.code,
            symbol: selectedCurrency.currency.symbol,
          },
        },
        period,
      };

      await budgetService.addAsync(request);

      props.resetForm();
      navigate("/budget");
    } catch (error) {
      toast.error(
        error instanceof Error ? error.message : "An unexpected error occurred",
      );
    } finally {
      props.setSubmitting(false);
    }
  };

  const formik = useFormik<NewBudgetValuesType>({
    initialValues,
    validationSchema: budgetSchema,
    onSubmit: handleSubmit,
  });

  const fetchCategories = useCallback(async () => {
    try {
      const categories = await categoryService.getAllAsync();
      dispatch(setCategories(categories));
    } catch (error) {
      toast.error(
        error instanceof Error ? error.message : "Failed to fetch categories",
      );
    }
  }, [dispatch]);

  useEffect(() => {
    fetchCategories();
  }, [fetchCategories]);

  useEffect(() => {
    const userCurrencies = user?.userCurrencies ?? [];
    const defaultCurrency = userCurrencies.find((c) => c.isDefault);
    if (defaultCurrency) {
      formik.setFieldValue("amount.currency", {
        code: defaultCurrency.currency.code,
        symbol: defaultCurrency.currency.symbol,
      });
    }
  }, [user]);

  const handleCurrencyChange = (code: string) => {
    const selectedUserCurrency = user?.userCurrencies.find(
      (uc) => uc.currency.code === code,
    );
    formik.setFieldValue("amount.currency", {
      code: code,
      symbol: selectedUserCurrency?.currency.symbol ?? "",
    });
  };

  return (
    <div className="w-dvw flex justify-center p-4 md:p-6 text-text-primary">
      <div className="w-full flex flex-col gap-y-5 sm:w-2/3 lg:w-1/2 p-5 md:p-6 bg-form-bg border-thin border-border-subtle rounded-md shadow-card">
        <div className="flex pb-4 border-b-thin border-border-subtle justify-between items-center">
          <button
            type="button"
            onClick={() => navigate(-1)}
            className="flex justify-center items-center h-9 aspect-square md:mx-2 rounded-full bg-bg-surface hover:bg-bg-muted border-thin border-border-subtle shadow-subtle cursor-pointer"
          >
            <IoChevronBack className="text-text-primary" />
          </button>
          <h1 className="text-lg md:text-xl font-bold text-text-primary">
            New Budget
          </h1>
          <div className="p-2 rounded-sm bg-bg-muted text-text-secondary">
            <FaRegMoneyBillAlt className="size-4" />
          </div>
        </div>
        <form
          onSubmit={formik.handleSubmit}
          className="w-full flex flex-col gap-y-4"
        >
          <div className="flex flex-col sm:flex-row sm:justify-between gap-4 p-4 rounded-md bg-bg-secondary/70 border-thin border-border-subtle">
            <div className="w-full sm:w-1/2">
              <h3 className="text-xs font-semibold uppercase tracking-wider text-text-secondary mb-1.5">
                Category
              </h3>
              <select
                name="categoryId"
                value={formik.values.categoryId}
                onChange={formik.handleChange}
                className="w-full appearance-none text-center py-2 px-3 text-sm font-medium text-text-primary bg-input-bg hover:bg-bg-muted rounded-sm border-thin border-input-border shadow-subtle cursor-pointer outline-0 focus:border-accent"
              >
                {expenseCategories.map((c, index, arr) => (
                  <option
                    key={c.id}
                    value={c.id}
                    className={`w-full p-2 bg-dropdown-bg text-text-primary ${index === arr.length - 1 && "rounded-b-sm"}`}
                  >
                    {c.name}
                  </option>
                ))}
              </select>
            </div>
            <div className="w-full sm:w-1/2">
              <h3 className="text-xs font-semibold uppercase tracking-wider text-text-secondary mb-1.5">
                Type
              </h3>
              <button
                type="button"
                value={formik.values.type}
                onChange={formik.handleChange}
                className="w-full py-2 px-3 text-sm font-medium text-text-primary bg-bg-surface hover:bg-bg-muted border-thin border-border-subtle shadow-subtle rounded-sm cursor-pointer"
              >
                ByCategory
              </button>
            </div>
          </div>
          <div className="flex flex-col sm:flex-row sm:justify-between gap-4 p-4 rounded-md bg-bg-secondary/70 border-thin border-border-subtle">
            <div className="w-full sm:w-1/2">
              <h3 className="text-xs font-semibold uppercase tracking-wider text-text-secondary mb-1.5">
                Amount
              </h3>
              <input
                type="number"
                name="amount.amount"
                min={0.01}
                step="0.01"
                value={formik.values.amount.amount}
                onChange={formik.handleChange}
                className="w-full py-2 px-3 text-sm font-semibold text-text-primary bg-input-bg border-thin border-input-border shadow-subtle rounded-sm outline-0 focus:border-accent [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none [-moz-appearance:textfield]"
              />
            </div>
            <div className="w-full sm:w-1/2">
              <h3 className="text-xs font-semibold uppercase tracking-wider text-text-secondary mb-1.5">
                Currency
              </h3>
              <CurrencyDropdown
                currencyFieldValue={formik.values.amount.currency.code}
                handleOptionButtonClick={handleCurrencyChange}
                className="w-full"
              />
            </div>
          </div>
          <div className="pt-1">
            <Button
              label="Submit"
              type="submit"
              background="bg-btn-standard"
              hoverBg="hover:bg-btn-standard-hover"
              textColor="text-btn-standard-text"
              disabled={!formik.dirty || !formik.isValid || formik.isSubmitting}
            />
          </div>
        </form>
      </div>
    </div>
  );
}

export default NewBudget;
