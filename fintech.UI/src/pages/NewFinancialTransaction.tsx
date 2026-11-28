/* eslint-disable react-hooks/set-state-in-effect */
/* eslint-disable react-hooks/exhaustive-deps */
import { useCallback, useEffect, useState } from "react";
import {
  FinancialTypes,
  type NewTransactionRequest,
} from "../types/financialTransactionTypes";
import { useAppDispatch, useAppSelector } from "../state/stateHooks";
import type { GetCategoryResponse } from "../types/categoryTypes";
import financialTrascationService from "../API/Services/financialTrascationService";
import Calendar from "../components/Calendar";
import { useFormik, type FormikHelpers } from "formik";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import Button from "../components/Button";
import categoryService from "../API/Services/categoryService";
import { setCategories } from "../state/slices/categorySlice";
import Spinner from "../components/Spinner";
import { format } from "date-fns";
import financialAggregatesService from "../API/Services/financialAggregatesService";
import { setGlobalAggregates } from "../state/slices/financialAggregatesSlice";
import { IoChevronBack } from "react-icons/io5";
import { FaRegMoneyBillAlt } from "react-icons/fa";
import { financialTransactionSchema } from "../formik/financialTransactionSchema";
import CurrencyDropdown from "../components/CurrencyDropdown";

function NewFinancialTransaction() {
  const types: FinancialTypes[] = [
    FinancialTypes.Income,
    FinancialTypes.Expense,
    FinancialTypes.Investment,
    FinancialTypes.Savings,
    FinancialTypes.ContractedLoan,
    FinancialTypes.InterestPayment,
    FinancialTypes.PrincipalRepayment,
  ];

  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const user = useAppSelector((state) => state.authUser.userInformations);

  const [isLoading, setIsLoading] = useState(false);
  const amount = 0.01;
  const currency = {
    code: "",
    symbol: "",
  };

  const initialValues: NewTransactionRequest = {
    categoryId: "",
    amount: { amount, currency },
    transactionDate: new Date(),
    description: "",
    isEssential: true,
  };

  const fetchGlobalAggregates = async () => {
    try {
      const fetchedAggregates =
        await financialAggregatesService.getGlobalAggregates();
      dispatch(setGlobalAggregates(fetchedAggregates));
    } catch (error) {
      toast.error(
        error instanceof Error
          ? error.message
          : "Failed to fetch globalAggregates",
      );
    }
  };

  const handleSubmit = async (
    values: NewTransactionRequest,
    props: FormikHelpers<NewTransactionRequest>,
  ) => {
    try {
      setIsLoading(true);
      const selectedCurrency = user?.userCurrencies.find(
        (c) => c.currency.code === values.amount.currency.code,
      );
      const currency = {
        code: values.amount.currency.code,
        symbol: selectedCurrency!.currency.symbol,
      };
      const request: NewTransactionRequest = {
        ...values,
        amount: { amount: values.amount.amount, currency: currency },
      };
      await financialTrascationService.addAsync(request);
      fetchGlobalAggregates();
      props.resetForm();
      navigate("/financialTransactions");
    } catch (error) {
      toast.error(
        error instanceof Error ? error.message : "An unexpected error occurred",
      );
    } finally {
      props.setSubmitting(false);
      setIsLoading(false);
    }
  };

  const formik = useFormik<NewTransactionRequest>({
    initialValues,
    validationSchema: financialTransactionSchema,
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

  const categories = useAppSelector((state) => state.categories.items);
  const [selectedType, setSelectedType] = useState<FinancialTypes>(types[0]);
  const [filteredCategories, setFilteredCategories] = useState<
    GetCategoryResponse[]
  >([]);
  const [showCalendar, setShowCalendar] = useState<boolean>(false);

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

  useEffect(() => {
    const filtered = categories.filter(
      (category) =>
        category.type === selectedType && category.parentCategoryId !== null,
    );
    setFilteredCategories(filtered);
    if (filtered.length > 0) {
      formik.setFieldValue("categoryId", filtered[0].id);
    }
  }, [selectedType]);

  const handleCurrencyChange = (code: string) => {
    const selectedUserCurrency = user?.userCurrencies.find(
      (uc) => uc.currency.code === code,
    );
    formik.setFieldValue("amount.currency", {
      code: code,
      symbol: selectedUserCurrency?.currency.symbol ?? "",
    });
  };

  const toggleShowCalendar = () => setShowCalendar((prev) => !prev);
  if (isLoading) {
    return <Spinner />;
  }

  return (
    <div className="w-dvw flex justify-center p-4 md:p-6 text-text-primary">
      <div className="w-full lg:w-10/12 xl:w-7/12 p-5 md:p-6 bg-form-bg border-thin border-border-subtle rounded-md shadow-card">
        <div className="flex mb-6 pb-4 border-b-thin border-border-subtle justify-between items-center">
          <button
            type="button"
            onClick={() => navigate(-1)}
            className="flex justify-center items-center h-9 aspect-square md:mx-2 rounded-full bg-bg-surface hover:bg-bg-muted border-thin border-border-subtle shadow-subtle cursor-pointer"
          >
            <IoChevronBack className="text-text-primary" />
          </button>
          <h1 className="text-lg md:text-xl font-bold text-text-primary">
            New Financial Transaction
          </h1>
          <div className="p-2 rounded-sm bg-bg-muted text-text-secondary">
            <FaRegMoneyBillAlt className="size-4" />
          </div>
        </div>

        <form
          onSubmit={formik.handleSubmit}
          className="space-y-4"
        >
          <div className="w-full p-4 md:px-5 rounded-md bg-bg-secondary/70 border-thin border-border-subtle md:flex md:justify-between md:gap-4">
            <div className="relative mb-5 md:mb-0 flex-1">
              <h2 className="text-xs font-semibold uppercase tracking-wider text-text-secondary mb-1.5">
                Type
              </h2>
              <select
                name="type"
                value={selectedType}
                onChange={(e) =>
                  setSelectedType(e.target.value as FinancialTypes)
                }
                className="w-full appearance-none text-center py-2 px-3 text-sm font-medium text-text-primary bg-input-bg hover:bg-bg-muted border-thin border-input-border rounded-sm shadow-subtle cursor-pointer outline-0 focus:border-accent"
              >
                {types.map((t, index, arr) => (
                  <option
                    key={t}
                    value={t}
                    className={`w-full p-2 bg-dropdown-bg text-text-primary ${index === arr.length - 1 ? "rounded-b-sm" : "rounded-none"}`}
                  >
                    {t}
                  </option>
                ))}
              </select>
            </div>
            <div className="relative mb-5 md:mb-0 flex-1">
              <h2 className="text-xs font-semibold uppercase tracking-wider text-text-secondary mb-1.5">
                Category
              </h2>
              <select
                name="categoryId"
                value={formik.values.categoryId}
                onChange={formik.handleChange}
                className="w-full appearance-none text-center py-2 px-3 text-sm font-medium text-text-primary bg-input-bg hover:bg-bg-muted rounded-sm border-thin border-input-border shadow-subtle cursor-pointer outline-0 focus:border-accent"
              >
                {filteredCategories.map((c, index, arr) => (
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
            <div className="relative flex-1">
              <h2 className="text-xs font-semibold uppercase tracking-wider text-text-secondary mb-1.5">
                Is Essential
              </h2>
              <button
                type="button"
                onClick={() =>
                  formik.setFieldValue(
                    "isEssential",
                    !formik.values.isEssential,
                  )
                }
                className="w-full py-2 px-3 text-sm font-medium text-text-primary bg-bg-surface hover:bg-bg-muted rounded-sm border-thin border-border-subtle shadow-subtle cursor-pointer"
              >
                {formik.values.isEssential ? "Essential" : "Not Essential"}
              </button>
            </div>
          </div>

          <div className="w-full p-4 md:px-5 rounded-md bg-bg-secondary/70 border-thin border-border-subtle md:flex md:justify-between md:gap-4">
            <div className="mb-5 md:mb-0 flex-1">
              <h2 className="text-xs font-semibold uppercase tracking-wider text-text-secondary mb-1.5">
                Amount
              </h2>
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
            <div className="relative mb-5 md:mb-0 flex-1">
              <h2 className="text-xs font-semibold uppercase tracking-wider text-text-secondary mb-1.5">
                Currency
              </h2>
              <CurrencyDropdown
                currencyFieldValue={formik.values.amount.currency.code}
                handleOptionButtonClick={handleCurrencyChange}
                className="w-full"
              />
            </div>
            <div className="relative flex-1">
              <h2 className="text-xs font-semibold uppercase tracking-wider text-text-secondary mb-1.5">
                Transaction date
              </h2>
              <button
                type="button"
                onClick={toggleShowCalendar}
                className="w-full py-2 px-3 text-sm font-medium text-text-primary bg-bg-surface hover:bg-bg-muted rounded-sm border-thin border-border-subtle shadow-subtle cursor-pointer"
              >
                {format(formik.values.transactionDate!, "dd-MM-yyyy")}
              </button>

              {showCalendar && (
                <div className="absolute -top-73 z-20 w-full">
                  <Calendar
                    selectedDate={formik.values.transactionDate!}
                    handledate={(date) => {
                      formik.setFieldValue("transactionDate", date);
                      setShowCalendar(false);
                    }}
                  />
                </div>
              )}
            </div>
          </div>

          <div className="flex w-full p-4 md:px-5 rounded-md bg-bg-secondary/70 border-thin border-border-subtle justify-between">
            <div className="w-full">
              <h2 className="text-xs font-semibold uppercase tracking-wider text-text-secondary mb-1.5">
                Description
              </h2>
              <textarea
                name="description"
                value={formik.values.description}
                onChange={formik.handleChange}
                className="w-full p-2.5 text-sm text-text-primary placeholder:text-text-muted bg-input-bg border-thin border-input-border shadow-subtle rounded-sm outline-0 focus:border-accent"
              />
            </div>
          </div>

          <div className="pt-2 md:px-1">
            <Button
              label={formik.isSubmitting ? "Saving..." : "Submit"}
              type="submit"
              background="bg-btn-standard"
              hoverBg="hover:bg-btn-standard-hover"
              textColor="text-btn-standard-text"
              disabled={formik.isSubmitting}
            />
          </div>
        </form>
      </div>
    </div>
  );
}

export default NewFinancialTransaction;
