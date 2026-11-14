/* eslint-disable react-hooks/exhaustive-deps */
import { useCallback, useEffect, useState } from "react";
import Dropdown from "../components/Dropdown";
import generateMonths from "../helpers/generateMonths";
import { useAppSelector } from "../state/stateHooks";
import type { MonthItem } from "../types/financialTransactionTypes";
import budgetService from "../API/Services/budgetService";
import { setBudget } from "../state/slices/budgetSlice";
import { toast } from "sonner";
import { useDispatch } from "react-redux";
import type { BudgetParamType, UpdateBudgetType } from "../types/budgetTypes";
import Button from "../components/Button";
import { CiEdit } from "react-icons/ci";
import { RiDeleteBin6Line } from "react-icons/ri";
import ScrollReveal from "../components/ScrollReveal";
import CurrencyDropdown from "../components/CurrencyDropdown";
import { useFormik, type FormikHelpers } from "formik";
import { updateBudgetSchema } from "../formik/updateBudgetSchema";
import { IoMdClose } from "react-icons/io";
import { useNavigate } from "react-router-dom";

function Budgets() {
  const userInformations = useAppSelector(
    (state) => state.authUser.userInformations,
  );
  const budgets = useAppSelector((state) => state.budgets.budgets);
  const categoryBudgets = budgets?.filter((b) => b.type !== "Global");
  const globalBudget = budgets?.find((b) => b.type === "Global");

  const dispatch = useDispatch();
  const navigate = useNavigate();

  const months = generateMonths(userInformations!.createdAt);

  const [selectedMonth, setSelectedMonth] = useState<MonthItem>(months[0]);
  const [updateBuget, setUpdateBudget] = useState("");
  const [updateGlobalBudget, setUpdateGlobalBudget] = useState(false);

  const initialValues: UpdateBudgetType = {
    amount: 0.01,
    currency: { code: "", symbol: "" },
  };

  const handleSubmit = async (
    values: UpdateBudgetType,
    props: FormikHelpers<UpdateBudgetType>,
  ) => {
    console.log("Formik submitted:", values);
    console.log("Budget ID:", updateBuget);
    try {
      const selectedCurrency = userInformations?.userCurrencies.find(
        (c) => c.currency.code === values.currency.code,
      );
      const currency = {
        code: values.currency.code,
        symbol: selectedCurrency!.currency.symbol,
      };
      const requestBody: UpdateBudgetType = {
        amount: values.amount,
        currency: currency,
      };
      await budgetService.updateAsync(updateBuget, requestBody);

      await fetchMonthBudgets({ month: selectedMonth.value.toISOString() });

      props.resetForm();
      setUpdateBudget("");
      toast.success("Budget updated successfully");
    } catch (error) {
      toast.error(
        error instanceof Error ? error.message : "An unexpected error occurred",
      );
    } finally {
      props.setSubmitting(false);
    }
  };

  const formik = useFormik<UpdateBudgetType>({
    initialValues,
    validationSchema: updateBudgetSchema,
    onSubmit: handleSubmit,
  });

  useEffect(() => {
    const userCurrencies = userInformations?.userCurrencies ?? [];
    const defaultCurrency = userCurrencies.find((c) => c.isDefault);
    if (defaultCurrency) {
      formik.setFieldValue("currency", {
        code: defaultCurrency.currency.code,
        symbol: defaultCurrency.currency.symbol,
      });
    }
  }, [userInformations]);

  const fetchMonthBudgets = useCallback(
    async (date: BudgetParamType) => {
      try {
        const budgets = await budgetService.getByMonthAsync(date);
        dispatch(setBudget(budgets));
      } catch (error) {
        toast.error(
          error instanceof Error
            ? error.message
            : "Failed to fetch month budgets",
        );
      }
    },
    [dispatch],
  );

  useEffect(() => {
    const monthDate = selectedMonth.value.toISOString();
    fetchMonthBudgets({ month: monthDate });
  }, [fetchMonthBudgets, selectedMonth]);

  const handleMonthClick = (month: MonthItem) => {
    setSelectedMonth(month);
  };

  const handleCurrencyChange = (code: string) => {
    const selectedUserCurrency = userInformations?.userCurrencies.find(
      (uc) => uc.currency.code === code,
    );
    formik.setFieldValue("currency", {
      code: code,
      symbol: selectedUserCurrency?.currency.symbol ?? "",
    });
  };

  const handleDelete = async (id: string) => {
    await budgetService.deleteAsync(id);
    fetchMonthBudgets({ month: new Date().toISOString() });
  };

  return (
    <div className="w-full mx-auto p-4 md:p-6 space-y-6 bg-bg-primary text-text-primary">
      <div className="flex flex-col md:flex-row  md:justify-between items-center gap-4 bg-form-bg p-6 border border-border-subtle rounded-md shadow-card">
        <h1 className="text-xl md:text-2xl font-bold">Budget</h1>
        <div className="md:w-1/2 md:flex md:justify-end">
          <p className="text-sm text-text-surface mt-2 md:mr-1">
            Budget summary for{" "}
          </p>
          <div className="md:w-[30%] pt-1">
            <Dropdown<MonthItem>
              selectedValue={selectedMonth}
              options={months}
              onSelect={(selected) => handleMonthClick(selected)}
            />
          </div>
        </div>
      </div>

      <ScrollReveal>
        {" "}
        <div className="w-full flex flex-col gap-y-5 p-4 border-thin border-border-subtle rounded-md bg-bg-contrast">
          {categoryBudgets && categoryBudgets.length == 0 && (
            <div className="w-full flex justify-end mb-5">
              {updateGlobalBudget ? (
                <button
                  onClick={() => setUpdateGlobalBudget(false)}
                  className={`h-8 aspect-square justify-center items-center rounded-full p-1 hover:bg-bg-secondary cursor-pointer`}
                >
                  <IoMdClose className="size-6" />
                </button>
              ) : (
                <>
                  <button
                    onClick={() => handleDelete(globalBudget!.id)}
                    className="h-8 aspect-square flex justify-center items-center rounded-full p-1 mr-2 hover:bg-bg-secondary text-xl text-red-500 cursor-pointer"
                  >
                    <RiDeleteBin6Line />
                  </button>
                  <button
                    onClick={() => {
                      setUpdateGlobalBudget(true);
                      formik.setFieldValue(
                        "amount",
                        globalBudget!.amount.amount,
                      );
                      formik.setFieldValue("currency", {
                        code: globalBudget!.amount.currency.code,
                        symbol: globalBudget!.amount.currency.symbol,
                      });
                    }}
                    className={
                      "h-8 aspect-square flex justify-center items-center rounded-full p-1 hover:bg-bg-secondary cursor-pointer"
                    }
                  >
                    <CiEdit className="size-6" />
                  </button>
                </>
              )}
            </div>
          )}
          <div className="flex justify-between">
            <h3 className="font-bold">Total Budgets</h3>
            <h3>{globalBudget?.budgetConsumptionPercentage.toFixed()}%</h3>
          </div>
          <div className="flex justify-between">
            {!updateGlobalBudget ? (
              <>
                <h3>
                  <span className="font-bold text-green-500 ">
                    {globalBudget!.amount.currency.symbol}
                  </span>
                  {globalBudget!.amount.amount.toFixed(2)}
                </h3>
                <h3>
                  <span className="font-bold text-green-500 ">
                    {globalBudget!.amount.currency.symbol}
                  </span>
                  {globalBudget!.budgetConsumption}
                </h3>
              </>
            ) : (
              <form
                onSubmit={formik.handleSubmit}
                className="w-full"
              >
                <div className="w-full flex justify-between mb-5 items-center">
                  <div className="w-28">
                    <input
                      type="number"
                      name="amount"
                      min={0.01}
                      step="0.01"
                      value={formik.values.amount}
                      onChange={formik.handleChange}
                      placeholder={globalBudget!.amount.amount.toString()}
                      className="w-full p-2 rounded-sm border border-width-thin border-input-border bg-bg-secondary [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none [-moz-appearance:textfield]"
                    />
                  </div>

                  <div className="w-28">
                    <CurrencyDropdown
                      currencyFieldValue={formik.values.currency.code}
                      handleOptionButtonClick={handleCurrencyChange}
                      className="w-full"
                    />
                  </div>
                </div>

                <Button
                  type="submit"
                  label={formik.isSubmitting ? "Updating..." : "Edit"}
                  background="bg-btn-standard"
                  hoverBg="hover:bg-btn-standard-hover"
                  textColor="text-white"
                  disabled={
                    !formik.dirty || !formik.isValid || formik.isSubmitting
                  }
                />
              </form>
            )}
          </div>
        </div>
      </ScrollReveal>

      <div>
        {categoryBudgets?.map((b) => (
          <ScrollReveal key={b.id}>
            <div className="p-4 my-4 border-thin border-border-subtle rounded-md bg-bg-muted">
              <div className="w-full flex justify-end mb-5">
                {updateBuget === b.id ? (
                  <button
                    onClick={() => setUpdateBudget("")}
                    className={`h-8 aspect-square justify-center items-center rounded-full p-1 hover:bg-bg-secondary cursor-pointer`}
                  >
                    <IoMdClose className="size-6" />
                  </button>
                ) : (
                  <>
                    <button
                      onClick={() => handleDelete(b.id)}
                      className="h-8 aspect-square flex justify-center items-center rounded-full p-1 mr-2 hover:bg-bg-secondary text-xl text-red-500 cursor-pointer"
                    >
                      <RiDeleteBin6Line />
                    </button>
                    <button
                      onClick={() => {
                        setUpdateBudget(b.id);
                        formik.setFieldValue("amount", b.amount.amount);
                        formik.setFieldValue("currency", {
                          code: b.amount.currency.code,
                          symbol: b.amount.currency.symbol,
                        });
                      }}
                      className={
                        "h-8 aspect-square flex justify-center items-center rounded-full p-1 hover:bg-bg-secondary cursor-pointer"
                      }
                    >
                      <CiEdit className="size-6" />
                    </button>
                  </>
                )}
              </div>

              <div className="w-full flex flex-col gap-y-5">
                <div className="flex justify-between ">
                  <h3 className="font-bold">{b.categoryName}</h3>
                  <h3>{b.budgetConsumptionPercentage.toFixed()}%</h3>
                </div>
                <div className="flex justify-between">
                  {updateBuget !== b.id ? (
                    <>
                      <h3>
                        <span className="font-bold text-green-500 ">
                          {b.amount.currency.symbol}
                        </span>
                        {b.amount.amount.toFixed(2)}
                      </h3>
                      <h3>
                        <span className="font-bold text-green-500 ">
                          {b.amount.currency.symbol}
                        </span>
                        {b.budgetConsumption}
                      </h3>
                    </>
                  ) : (
                    <form
                      onSubmit={formik.handleSubmit}
                      className="w-full"
                    >
                      <div className="w-full flex justify-between mb-5 items-center">
                        <div className="w-28">
                          <input
                            type="number"
                            name="amount"
                            min={0.01}
                            step="0.01"
                            value={formik.values.amount}
                            onChange={formik.handleChange}
                            placeholder={b.amount.amount.toString()}
                            className="w-full p-2 rounded-sm border border-width-thin border-input-border bg-bg-secondary [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none [-moz-appearance:textfield]"
                          />
                        </div>

                        <div className="w-28">
                          <CurrencyDropdown
                            currencyFieldValue={formik.values.currency.code}
                            handleOptionButtonClick={handleCurrencyChange}
                            className="w-full"
                          />
                        </div>
                      </div>

                      <Button
                        type="submit"
                        label={formik.isSubmitting ? "Updating..." : "Edit"}
                        background="bg-btn-standard"
                        hoverBg="hover:bg-btn-standard-hover"
                        textColor="text-white"
                        disabled={
                          !formik.dirty ||
                          !formik.isValid ||
                          formik.isSubmitting
                        }
                      />
                    </form>
                  )}
                </div>
              </div>
            </div>
          </ScrollReveal>
        ))}
        <div className="my-5">
          <Button
            type="button"
            label="add category budget"
            background="bg-btn-standard"
            hoverBg="hover:bg-btn-standard-hover"
            textColor="text-white"
            handleClick={() => navigate("/budget/new")}
          />
        </div>
      </div>
    </div>
  );
}

export default Budgets;
