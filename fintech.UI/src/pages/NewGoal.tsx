/* eslint-disable react-hooks/exhaustive-deps */

import { useDispatch } from "react-redux";
import { useAppSelector } from "../state/stateHooks";
import { useNavigate } from "react-router-dom";
import type { NewGoalRequest, NewGoalValues } from "../types/goalTypes";
import { useFormik, type FormikHelpers } from "formik";
import goalService from "../API/Services/goalService";
import { toast } from "sonner";
import { useCallback, useEffect } from "react";
import categoryService from "../API/Services/categoryService";
import { setCategories } from "../state/slices/categorySlice";
import { goalSchema } from "../formik/goalSchema";
import { IoChevronBack } from "react-icons/io5";
import CurrencyDropdown from "../components/CurrencyDropdown";
import Button from "../components/Button";
import { GoGoal } from "react-icons/go";

function NewGoal() {
  const user = useAppSelector((state) => state.authUser.userInformations);
  const categories = useAppSelector((state) => state.categories.items);
  const goalCategories = categories.filter(
    (c) =>
      (c.type === "Investment" || c.type === "Savings") &&
      c.parentCategoryId !== null,
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

  const initialValues: NewGoalValues = {
    title: "",
    categoryId: "",
    targetType: undefined,
    targetAmount: { amount: 0.01, currency: { code: "", symbol: "" } },
  };

  const handleSubmit = async (
    values: NewGoalValues,
    props: FormikHelpers<NewGoalValues>,
  ) => {
    try {
      const selectedCurrency = user?.userCurrencies.find(
        (c) => c.currency.code === values.targetAmount.currency.code,
      );

      if (!selectedCurrency) {
        throw new Error("Selected currency not found");
      }

      const request: NewGoalRequest = {
        ...values,
        targetAmount: {
          amount: values.targetAmount.amount,
          currency: {
            code: values.targetAmount.currency.code,
            symbol: selectedCurrency.currency.symbol,
          },
        },
        period,
      };

      await goalService.addAsync(request);

      props.resetForm();
      navigate("/goal");
    } catch (error) {
      toast.error(
        error instanceof Error ? error.message : "An unexpected error occurred",
      );
    } finally {
      props.setSubmitting(false);
    }
  };

  const formik = useFormik<NewGoalValues>({
    initialValues,
    validationSchema: goalSchema,
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
      formik.setFieldValue("targetAmount.currency", {
        code: defaultCurrency.currency.code,
        symbol: defaultCurrency.currency.symbol,
      });
    }
  }, [user]);

  const handleCurrencyChange = (code: string) => {
    const selectedUserCurrency = user?.userCurrencies.find(
      (uc) => uc.currency.code === code,
    );
    formik.setFieldValue("targetAmount.currency", {
      code: code,
      symbol: selectedUserCurrency?.currency.symbol ?? "",
    });
  };
  return (
    <div className="w-dvw flex justify-center">
      <div className="w-full flex flex-col gap-y-5 sm:w-1/2 p-5 bg-bg-muted border-thin border-border-subtle sm:rounded-sm shadow-card">
        <div className="flex mb-5 mr-5 justify-between items-center">
          <button
            type="button"
            onClick={() => navigate(-1)}
            className="flex justify-center items-center h-10 aspect-square md:mx-2 rounded-full bg-bg-primary hover:bg-navbar-bg"
          >
            <IoChevronBack className="stroke-text-surface" />
          </button>
          <h1>New Goal</h1>
          <GoGoal className="size-5 text-grey-500" />
        </div>
        <form
          onSubmit={formik.handleSubmit}
          className="w-full flex flex-col gap-y-5"
        >
          <div className="flex flex-col sm:flex-row sm:justify-between">
            <div>
              <h3>Title</h3>
              <input
                type="text"
                name="title"
                value={formik.values.title}
                onChange={formik.handleChange}
                placeholder="goal title"
                className="w-full p-2 rounded-sm border border-width-thin border-input-border bg-bg-secondary [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none [-moz-appearance:textfield]"
              />
            </div>
          </div>
          <div className="flex flex-col sm:flex-row sm:justify-between">
            <div className="w-full sm:w-1/3">
              <h3>Type</h3>
              <button
                type="button"
                onChange={formik.handleChange}
                className="w-full py-2 bg-bg-surface border-thin border-border-subtle shadow-card rounded-sm cursor-pointer"
              >
                ByCategory
              </button>
            </div>
            <div className="w-full mb-5 sm:w-1/3 sm:mb-0">
              <h3>Category</h3>
              <select
                name="categoryId"
                value={formik.values.categoryId}
                onChange={formik.handleChange}
                className="w-full  appearance-none text-center p-2  bg-bg-surface rounded-sm border-thin border-border-subtle shadow-card cursor-pointer outline-0"
              >
                {goalCategories.map((c, index, arr) => (
                  <option
                    key={c.id}
                    value={c.id}
                    className={`w-full p-2 ${index === arr.length - 1 && "rounded-b-sm"}`}
                  >
                    {c.name}
                  </option>
                ))}
              </select>
            </div>
          </div>
          <div className="flex flex-col sm:flex-row sm:justify-between">
            <div className="w-full mb-5 sm:w-1/3 sm:mb-0">
              <h3>Amount</h3>
              <input
                type="number"
                name="targetAmount.amount"
                min={0.01}
                step="0.01"
                value={formik.values.targetAmount.amount}
                onChange={formik.handleChange}
                className="w-full p-2 bg-bg-surface border-thin border-border-subtle shadow-card rounded-sm outline-0 [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none [-moz-appearance:textfield]"
              />
            </div>
            <div className="w-full sm:w-1/3">
              <h3>Currency</h3>
              <CurrencyDropdown
                currencyFieldValue={formik.values.targetAmount.currency.code}
                handleOptionButtonClick={handleCurrencyChange}
                className="w-full"
              />
            </div>
          </div>
          <div>
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

export default NewGoal;
