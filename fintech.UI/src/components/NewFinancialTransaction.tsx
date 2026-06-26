/* eslint-disable react-hooks/set-state-in-effect */
/* eslint-disable react-hooks/exhaustive-deps */
import { Banknote } from "lucide-react";
import { useEffect, useState } from "react";
import {
  FinancialTypes,
  type NewTransactionRequest,
} from "../types/financialTransactionTypes";
import useClickOutside from "../hooks/useClickOutside";
import { useAppDispatch, useAppSelector } from "../state/stateHooks";
import { fetchCategories } from "../state/slices/categorySlice";
import type { GetCategoryResponse } from "../types/categoryTypes";
import exchangeRateService from "../API/Services/exchangeRateService";
import Calendar from "./Calendar";
import { financialTransactionSchema } from "../formik/financialTransactionSchema";
import { addTransaction } from "../state/slices/financialTransactionSlice";
import { useFormik } from "formik";

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

  const dispatch = useAppDispatch();

  useEffect(() => {
    dispatch(fetchCategories());
  }, []);

  const categories = useAppSelector((state) => state.categories.categories);

  const [selectedType, setSelectedType] = useState<FinancialTypes>(types[0]);
  const [showTypes, setShowTypes] = useState<boolean>(false);
  const [filteredCategories, setFilteredCategories] = useState<
    GetCategoryResponse[]
  >([]);
  const [selectedCategory, setSelectedCategory] =
    useState<GetCategoryResponse>();
  const [showCategories, setShowCategories] = useState<boolean>(false);

  const [isEssential, setIsEssential] = useState<boolean>(true);

  const [currencies, setCurrencies] = useState<string[]>([]);
  const [showCurrencies, setShowCurrencies] = useState<boolean>(false);
  const [selectedCurrency, setSelectedCurrency] = useState<string>();
  const [date, setDate] = useState<Date>(new Date());

  const formik = useFormik<NewTransactionRequest>({
    enableReinitialize: true,
    initialValues: {
      categoryId: "",
      amount: 0,
      currency: currencies[0] ?? "",
      transactionDate: new Date(),
      description: "",
      isEssential: true,
    },
    validationSchema: financialTransactionSchema,
    onSubmit: async (values) => {
      console.log(values);

      await dispatch(
        addTransaction({
          categoryId: values.categoryId,
          amount: values.amount,
          currency: values.currency,
          transactionDate: values.transactionDate,
          description: values.description,
          isEssential: values.isEssential,
        }),
      );
    },
  });

  const loadCurrencies = async () => {
    try {
      const data = await exchangeRateService.getCurrencies();
      const symbols = data.symbols;
      const symbolsKeys: string[] = [];
      Object.keys(symbols).forEach((key) => symbolsKeys.push(key));
      setCurrencies(symbolsKeys);
      setSelectedCurrency(symbolsKeys[0]);
      console.log("currencies : ", symbolsKeys);
    } catch (error) {
      console.error(error);
    }
  };

  useEffect(() => {
    loadCurrencies();
  }, []);

  const [showCalendar, setShowCalendar] = useState<boolean>(false);

  const typeRef = useClickOutside<HTMLDivElement>(() => setShowTypes(false));
  const categoryRef = useClickOutside<HTMLDivElement>(() =>
    setShowCategories(false),
  );

  useEffect(() => {
    const filtered = categories.filter(
      (category) =>
        category.type === selectedType && category.parentCategoryId !== null,
    );

    setFilteredCategories(filtered);

    if (
      filtered.length > 0 &&
      !filtered.some((c) => c.id === formik.values.categoryId)
    ) {
      formik.setFieldValue("categoryId", filtered[0].id);
      setSelectedCategory(filtered[0]);
    }
  }, [categories, selectedType]);

  const toggleShowTypes = () => setShowTypes(!showTypes);
  const selectType = (t: FinancialTypes) => {
    setSelectedType(t);
    toggleShowTypes();
    const subCategories = categories.filter(
      (category) => category.type === t && category.parentCategoryId !== null,
    );
    setFilteredCategories(subCategories);
    formik.setFieldValue("categoryId", subCategories[0]?.id ?? "");
  };

  const toggleShowCategories = () => setShowCategories(!showCategories);
  const selectCategory = (category: GetCategoryResponse) => {
    setSelectedCategory(category);
    formik.setFieldValue("categoryId", category.id);

    toggleShowCategories();
  };

  const toggleEssential = () => {
    setIsEssential(!isEssential);
    formik.setFieldValue("isEssential", isEssential);
  };

  const toggleShowCurrencies = () => setShowCurrencies(!showCurrencies);
  const selectCurrency = (currency: string) => {
    setSelectedCurrency(currency);
    formik.setFieldValue("currency", currency);
    toggleShowCurrencies();
  };

  const toggleShowCalendar = () => setShowCalendar(!showCalendar);
  const handleDate = (date: Date) => {
    setDate(date);
    formik.setFieldValue("transactionDate", date);

    toggleShowCalendar();
  };

  return (
    <div className="w-dvw flex p-5">
      <div className="w-7/12 p-5 border border-gray-200 rounded-box-r shadow-lg ">
        <div className="flex p-3 justify-between ">
          <h1>Add a New Financial Transaction</h1>
          <Banknote className="stroke-blue-500" />
        </div>
        <form onSubmit={formik.handleSubmit}>
          <div className="flex w-full p-5 justify-between">
            <div
              ref={typeRef}
              className="relative"
            >
              <h2>Type</h2>
              <button
                onClick={toggleShowTypes}
                className="w-45 p-2  border border-gray-200 rounded-lg cursor-pointer"
              >
                {selectedType}
              </button>
              <div
                className={`absolute top-16 w-45 z-20 bg-gray-50 cursor-pointer ${showTypes ? "flex flex-col" : "hidden"}`}
              >
                {types
                  .filter((t) => t !== selectedType)
                  .map((t, index, arr) => (
                    <button
                      key={t}
                      onClick={() => {
                        selectType(t);
                      }}
                      className={`w-full p-2  border-x border-t border-gray-200 ${index === arr.length - 1 && "border-b rounded-b-lg"}`}
                    >
                      {t}
                    </button>
                  ))}
              </div>
            </div>
            <div
              ref={categoryRef}
              className="relative"
            >
              <h2>Category</h2>
              <button
                onClick={toggleShowCategories}
                className="w-45 p-2  border border-gray-200 rounded-lg cursor-pointer"
              >
                {selectedCategory
                  ? selectedCategory.name
                  : filteredCategories[0]?.name}
              </button>
              <div
                className={`absolute top-16 w-45 z-20 bg-gray-50 cursor-pointer ${showCategories ? "flex flex-col" : "hidden"}`}
              >
                {filteredCategories
                  .filter((c) => c.id !== selectedCategory?.id)
                  .map((c, index, arr) => (
                    <button
                      key={c.id}
                      onClick={() => selectCategory(c)}
                      className={`w-full p-2  border-x border-t border-gray-200 ${index === arr.length - 1 && "border-b rounded-b-lg"}`}
                    >
                      {c.name}
                    </button>
                  ))}
              </div>
            </div>
            <div className="relative">
              <h2>Is Essential</h2>
              <button
                onClick={toggleEssential}
                className="w-45 p-2  border border-gray-200 rounded-lg cursor-pointer"
              >
                {isEssential ? "Essential" : "Not Essential"}
              </button>
            </div>
          </div>
          <div className="flex w-full p-5 justify-between">
            <div>
              <h2>Amount</h2>
              <input
                type="number"
                name="amount"
                value={formik.values.amount}
                onChange={formik.handleChange}
                onBlur={formik.handleBlur}
                className="w-45 p-2 border border-gray-200 rounded-lg"
              />
              {formik.touched.amount && formik.errors.amount && (
                <p className="text-red-500 text-sm">{formik.errors.amount}</p>
              )}
            </div>
            <div className="relative">
              <h2>Currency</h2>
              <button
                onClick={toggleShowCurrencies}
                className="w-45 p-2  border border-gray-200 rounded-lg cursor-pointer"
              >
                {selectedCurrency}
              </button>
              <div
                className={`absolute top-16 h-100 overflow-y-scroll w-45 z-20 bg-gray-50 cursor-pointer ${showCurrencies ? "flex flex-col" : "hidden"}`}
              >
                {currencies
                  .filter((c) => c !== formik.values.currency)
                  .map((c, index, arr) => (
                    <button
                      key={c}
                      onClick={() => selectCurrency(c)}
                      className={`w-full p-2  border-x border-t border-gray-200 ${index === arr.length - 1 && "border-b rounded-b-lg"}`}
                    >
                      {c}
                    </button>
                  ))}
              </div>
            </div>
            <div className="relative">
              <h2>Transaction date</h2>
              <button
                onClick={toggleShowCalendar}
                className="w-45 p-2  border border-gray-200 rounded-lg cursor-pointer"
              >
                {date?.toLocaleDateString("en-GB")}
              </button>
              <div
                className={`absolute -top-25 right-80 z-20 w-full ${showCalendar ? "block" : "hidden"}`}
              >
                <Calendar
                  handledate={handleDate}
                  selectedDate={formik.values.transactionDate!}
                />
              </div>
            </div>
          </div>
          <div className="flex w-full p-5 justify-between">
            <div className="w-full">
              <h2>Description</h2>
              <textarea
                name="description"
                value={formik.values.description}
                onChange={formik.handleChange}
                onBlur={formik.handleBlur}
                className="w-full p-2 border border-gray-200 rounded-lg"
              />
              {formik.touched.description && formik.errors.description && (
                <p className="text-red-500 text-sm">
                  {formik.errors.description}
                </p>
              )}
            </div>
          </div>
          <div className="flex justify-center">
            <button
              type="submit"
              className="w-1/2 p-1 items-center bg-bt-g hover:bg-bt-g-h text-gray-50 rounded-full cursor-pointer "
            >
              Submit
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default NewFinancialTransaction;
