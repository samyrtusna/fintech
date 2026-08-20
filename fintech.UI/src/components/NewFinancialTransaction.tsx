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
import exchangeRateService from "../API/Services/exchangeRateService";
import Calendar from "./Calendar";
import { useFormik, type FormikHelpers } from "formik";
import { Link, useNavigate } from "react-router-dom";
import { toast } from "sonner";
import Button from "./Button";
import categoryService from "../API/Services/categoryService";
import { setCategories } from "../state/slices/categorySlice";
import Spinner from "./Spinner";

function NewFinancialTransaction() {
  //
  const types: FinancialTypes[] = [
    FinancialTypes.Income,
    FinancialTypes.Expense,
    FinancialTypes.Investment,
    FinancialTypes.Savings,
    FinancialTypes.Debt,
    FinancialTypes.ContractedLoan,
    FinancialTypes.InterestPayment,
    FinancialTypes.PrincipalRepayment,
  ];

  const [isLoading, setIsLoading] = useState(false);
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const initialValues: NewTransactionRequest = {
    categoryId: "",
    amount: 0.01,
    currency: "",
    transactionDate: new Date(),
    description: "",
    isEssential: true,
  };

  const handleSubmit = async (
    values: NewTransactionRequest,
    props: FormikHelpers<NewTransactionRequest>,
  ) => {
    try {
      setIsLoading(true);
      await financialTrascationService.addAsync(values);
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

  useEffect(() => {
    const loadCategories = async () => {
      try {
        const categories = await fetchCategories();
        dispatch(setCategories(categories));
      } catch (error) {
        console.error("Failed to fetch categories:", error);
      }
    };
    loadCategories();
  }, [dispatch]);

  const categories = useAppSelector((state) => state.categories.categories);
  const [selectedType, setSelectedType] = useState<FinancialTypes>(types[0]);
  const [filteredCategories, setFilteredCategories] = useState<
    GetCategoryResponse[]
  >([]);
  const [currencies, setCurrencies] = useState<string[]>([]);
  const [showCalendar, setShowCalendar] = useState<boolean>(false);

  const loadCurrencies = async () => {
    try {
      const data = await exchangeRateService.getCurrencies();
      const symbolsKeys = Object.keys(data);

      setCurrencies(symbolsKeys);
      formik.setFieldValue("currency", "USD");
    } catch (error) {
      console.error(error);
    }
  };

  useEffect(() => {
    loadCurrencies();
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
    <div className="w-dvw flex justify-center p-5">
      <div className="w-full lg:w-10/12 xl:w-7/12 p-5 bg-bg-secondary rounded-sm shadow-card ">
        <div className="flex mb-5 justify-between">
          <Link
            to="/financialTransactions"
            className="flex justify-center items-center h-10 aspect-square mx-2 rounded-full hover:bg-bg-muted"
          >
            <ArrowLeft className="stroke-gray-500" />
          </Link>
          <h1>Add a New Financial Transaction</h1>
          <Banknote className="stroke-blue-500" />
        </div>

        <form onSubmit={formik.handleSubmit}>
          <div className="flex w-full p-5 justify-between">
            <div className="relative">
              <h2>Type</h2>
              <select
                name="type"
                value={selectedType}
                onChange={(e) =>
                  setSelectedType(e.target.value as FinancialTypes)
                }
                className="w-45 p-2 bg-bg-surface rounded-sm shadow-card cursor-pointer"
              >
                {types.map((t, index, arr) => (
                  <option
                    key={t}
                    value={t}
                    className={`w-full p-2 hover:bg-bg-secondary  ${index === arr.length - 1 && "rounded-b-sm"}`}
                  >
                    {t}
                  </option>
                ))}
              </select>
            </div>
            <div className="relative">
              <h2>Category</h2>
              <select
                name="categoryId"
                value={formik.values.categoryId}
                onChange={formik.handleChange}
                className="w-45 p-2  bg-bg-surface rounded-sm shadow-card cursor-pointer"
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
            <div className="relative">
              <h2>Is Essential</h2>
              <button
                type="button"
                onClick={() =>
                  formik.setFieldValue(
                    "isEssential",
                    !formik.values.isEssential,
                  )
                }
                className="w-45 p-2 bg-bg-surface rounded-sm shadow-card"
              >
                {formik.values.isEssential ? "Essential" : "Not Essential"}
              </button>
            </div>
          </div>
          <div className="flex w-full p-5 justify-between">
            <div>
              <h2>Amount</h2>
              <input
                type="number"
                name="amount"
                min={0.01}
                step="0.01"
                value={formik.values.amount}
                onChange={formik.handleChange}
                className="w-45 p-2 bg-bg-surface shadow-card rounded-sm"
              />
            </div>
            <div className="relative">
              <h2>Currency</h2>
              <select
                name="currency"
                value={formik.values.currency}
                onChange={formik.handleChange}
                className="w-45 p-2 bg-bg-surface rounded-sm shadow-card cursor-pointer"
              >
                {currencies.map((c, index, arr) => (
                  <option
                    key={c}
                    value={c}
                    className={`w-full p-2  ${index === arr.length - 1 && "rounded-b-lg"}`}
                  >
                    {c}
                  </option>
                ))}
              </select>
            </div>
            <div className="relative">
              <h2>Transaction date</h2>
              <button
                type="button"
                onClick={toggleShowCalendar}
                className="w-45 p-2  bg-bg-surface rounded-sm shadow-card cursor-pointer"
              >
                {formik.values.transactionDate?.toLocaleDateString("en-GB")}
              </button>

              {showCalendar && (
                <div className="absolute -top-25 right-80 z-20 w-full">
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
          <div className="flex w-full p-5 justify-between">
            <div className="w-full">
              <h2>Description</h2>
              <textarea
                name="description"
                value={formik.values.description}
                onChange={formik.handleChange}
                className="w-full p-2 bg-bg-surface shadow-card rounded-sm"
              />
            </div>
          </div>
          <div className="px-5">
            <Button
              label={formik.isSubmitting ? "Saving..." : "Submit"}
              type="submit"
              background="bg-btn-primary"
              hoverBg="hover:bg-btn-primary-hover"
              textColor="text-btn-primary-text"
              disabled={formik.isSubmitting}
            />
          </div>
        </form>
      </div>
    </div>
  );
}

export default NewFinancialTransaction;
