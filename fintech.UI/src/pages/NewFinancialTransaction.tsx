/* eslint-disable react-hooks/set-state-in-effect */
/* eslint-disable react-hooks/exhaustive-deps */
import { ArrowLeft, Banknote } from "lucide-react";
import { useEffect, useState } from "react";
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

function NewFinancialTransaction() {
  //
  const types: FinancialTypes[] = [
    FinancialTypes.Income,
    FinancialTypes.Expense,
    FinancialTypes.Investment,
    FinancialTypes.Savings,
    FinancialTypes.ContractedLoan,
    FinancialTypes.InterestPayment,
    FinancialTypes.PrincipalRepayment,
  ];

  const [isLoading, setIsLoading] = useState(false);
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const user = useAppSelector((state) => state.authUser.userInformations);

  const [currencyOptions, setCurrencyOptions] = useState(false);

  const initialValues: NewTransactionRequest = {
    categoryId: "",
    amount: 0.01,
    currency: "",
    currencySymbol: "",
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
        (c) => c.currencyCode === values.currency,
      );

      const request: NewTransactionRequest = {
        ...values,
        currencySymbol: selectedCurrency?.currencySymbol ?? "",
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
    onSubmit: handleSubmit,
  });

  const fetchCategories = async () => {
    return await categoryService.getAllAsync();
  };

  const categories = useAppSelector((state) => state.categories.items);
  const [selectedType, setSelectedType] = useState<FinancialTypes>(types[0]);
  const [filteredCategories, setFilteredCategories] = useState<
    GetCategoryResponse[]
  >([]);
  const [currencies, setCurrencies] = useState<string[]>([]);
  const [showCalendar, setShowCalendar] = useState<boolean>(false);

  useEffect(() => {
    const loadCategories = async () => {
      if (categories.length == 0) {
        try {
          const categories = await fetchCategories();
          dispatch(setCategories(categories));
        } catch (error) {
          console.error("Failed to fetch categories:", error);
        }
      }
    };
    loadCategories();
  }, [dispatch]);

  useEffect(() => {
    const currencies = user?.userCurrencies ?? [];

    setCurrencies(currencies.map((c) => c.currencyCode));
    const defaultCurrency = currencies.find((c) => c.isDefault);
    if (defaultCurrency) {
      formik.setFieldValue("currency", defaultCurrency.currencyCode);
    }
  }, []);

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

  const toggleShowCalendar = () => setShowCalendar((prev) => !prev);
  if (isLoading) {
    return <Spinner />;
  }
  return (
    <div className="w-dvw flex justify-center md:p-5">
      <div className="w-full lg:w-10/12 xl:w-7/12 p-5 bg-bg-muted border-thin border-border-subtle rounded-sm shadow-card ">
        <div className="flex mb-5 justify-between items-center">
          <button
            type="button"
            onClick={() => navigate(-1)}
            className="flex justify-center items-center h-10 aspect-square md:mx-2 rounded-full bg-bg-primary hover:bg-navbar-bg"
          >
            <ArrowLeft className="stroke-text-surface" />
          </button>
          <h1>New Financial Transaction</h1>
          <Banknote className="stroke-blue-500" />
        </div>

        <form onSubmit={formik.handleSubmit}>
          <div className="w-full  md:p-5 md:flex md:justify-between">
            <div className="relative mb-6 md:mb-0">
              <h2>Type</h2>
              <select
                name="type"
                value={selectedType}
                onChange={(e) =>
                  setSelectedType(e.target.value as FinancialTypes)
                }
                className="w-full md:w-45 appearance-none text-center p-2 bg-bg-surface border-thin border-border-subtle rounded-sm shadow-card cursor-pointer outline-0"
              >
                {types.map((t, index, arr) => (
                  <option
                    key={t}
                    value={t}
                    className={`w-full p-2 hover:bg-bg-secondary  ${index === arr.length - 1 ? "rounded-b-sm" : "rounded-none"}`}
                  >
                    {t}
                  </option>
                ))}
              </select>
            </div>
            <div className="relative mb-6 md:mb-0">
              <h2>Category</h2>
              <select
                name="categoryId"
                value={formik.values.categoryId}
                onChange={formik.handleChange}
                className="w-full md:w-45 appearance-none text-center p-2  bg-bg-surface rounded-sm border-thin border-border-subtle shadow-card cursor-pointer outline-0"
              >
                {filteredCategories.map((c, index, arr) => (
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
            <div className="relative mb-6 md:mb-0">
              <h2>Is Essential</h2>
              <button
                type="button"
                onClick={() =>
                  formik.setFieldValue(
                    "isEssential",
                    !formik.values.isEssential,
                  )
                }
                className="w-full md:w-45 p-2 bg-bg-surface rounded-sm border-thin border-border-subtle shadow-card"
              >
                {formik.values.isEssential ? "Essential" : "Not Essential"}
              </button>
            </div>
          </div>
          <div className="w-full md:p-5 md:flex md:justify-between">
            <div className="mb-6 md:mb-0">
              <h2>Amount</h2>
              <input
                type="number"
                name="amount"
                min={0.01}
                step="0.01"
                value={formik.values.amount}
                onChange={formik.handleChange}
                className="w-full md:w-45  p-2 bg-bg-surface border-thin border-border-subtle shadow-card rounded-sm outline-0"
              />
            </div>
            <div className="relative mb-6 md:mb-0">
              <h2>Currency</h2>
              <button
                type="button"
                className={`w-full md:w-45 p-2 bg-bg-surface  border-thin border-border-subtle shadow-card cursor-pointer ${currencyOptions ? "rounded-t-sm" : "rounded-sm"}`}
                onClick={() => setCurrencyOptions((prev) => !prev)}
              >
                {formik.values.currency}
              </button>
              {currencyOptions && (
                <div className="absolute w-full md:w-45 z-50">
                  {currencies
                    .filter((c) => c !== formik.values.currency)
                    .map((c, index, arr) => (
                      <button
                        type="button"
                        key={c}
                        onClick={() => {
                          formik.setFieldValue("currency", c);
                          setCurrencyOptions(false);
                        }}
                        className={`w-full p-2 bg-bg-surface hover:bg-btn-standard border-x-thin border-b-thin border-border-subtle cursor-pointer ${index === arr.length - 1 ? "rounded-b-sm" : ""}`}
                      >
                        {c}
                      </button>
                    ))}
                </div>
              )}
            </div>
            <div className="relative">
              <h2>Transaction date</h2>
              <button
                type="button"
                onClick={toggleShowCalendar}
                className="w-full md:w-45 p-2  bg-bg-surface rounded-sm border-thin border-border-subtle shadow-card cursor-pointer"
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
          <div className="flex w-full  py-5 md:px-5 justify-between">
            <div className="w-full">
              <h2>Description</h2>
              <textarea
                name="description"
                value={formik.values.description}
                onChange={formik.handleChange}
                className="w-full p-2 bg-bg-surface border-thin border-border-subtle shadow-card rounded-sm outline-0"
              />
            </div>
          </div>
          <div className="md:px-5">
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
