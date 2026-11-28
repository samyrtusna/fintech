/* eslint-disable react-hooks/exhaustive-deps */
import { useCallback, useEffect, useMemo, useState } from "react";
import Dropdown from "../components/Dropdown";
import generateMonths from "../helpers/generateMonths";
import { useAppSelector } from "../state/stateHooks";
import type { MonthItem } from "../types/financialTransactionTypes";
import budgetService from "../API/Services/budgetService";
import { setBudget } from "../state/slices/budgetSlice";
import { toast } from "sonner";
import { useDispatch } from "react-redux";
import type { MonthParamType, UpdateBudgetType } from "../types/budgetTypes";
import Button from "../components/Button";
import { CiEdit } from "react-icons/ci";
import { RiDeleteBin6Line } from "react-icons/ri";
import ScrollReveal from "../components/ScrollReveal";
import CurrencyDropdown from "../components/CurrencyDropdown";
import { useFormik, type FormikHelpers } from "formik";
import { updateBudgetSchema } from "../formik/updateBudgetSchema";
import { IoMdClose } from "react-icons/io";
import { useNavigate } from "react-router-dom";
import { MdAccountBalanceWallet } from "react-icons/md";
import Spinner from "../components/Spinner";
import userService from "../API/Services/userService";
import { setUserInformations } from "../state/slices/authSlice";

function Budgets() {
  const userInformations = useAppSelector(
    (state) => state.authUser.userInformations,
  );
  const budgets = useAppSelector((state) => state.budgets.budgets);
  const categoryBudgets = budgets?.filter((b) => b.type !== "Global");
  const globalBudget = budgets?.find((b) => b.type === "Global");

  const dispatch = useDispatch();
  const navigate = useNavigate();

  const fetchUserInformations = useCallback(async () => {
    if (userInformations !== null) {
      return;
    }
    try {
      const fetchedUserInformations = await userService.getUserAsync();
      dispatch(setUserInformations(fetchedUserInformations));
    } catch (error) {
      toast.error(
        error instanceof Error
          ? error.message
          : "Failed to fetch user informations",
      );
    }
  }, [dispatch, userInformations]);

  useEffect(() => {
    fetchUserInformations();
  }, [fetchUserInformations]);

  const createdAt = userInformations?.createdAt;

  const months: MonthItem[] = useMemo(() => {
    return createdAt ? generateMonths(createdAt) : [];
  }, [createdAt]);

  const [selectedMonth, setSelectedMonth] = useState<MonthItem | undefined>(
    undefined,
  );
  const [updateBuget, setUpdateBudget] = useState("");
  const [updateGlobalBudget, setUpdateGlobalBudget] = useState(false);
  const [budgetToDelete, setBudgetToDelete] = useState<string | null>(null);

  if (months.length > 0 && !selectedMonth) {
    setSelectedMonth(months[0]);
  }

  const isCurrentMonth =
    selectedMonth?.value.getFullYear() === new Date().getFullYear() &&
    selectedMonth.value.getMonth() === new Date().getMonth() + 1;

  const initialValues: UpdateBudgetType = {
    amount: 0.01,
    currency: { code: "", symbol: "" },
  };

  const handleSubmit = async (
    values: UpdateBudgetType,
    props: FormikHelpers<UpdateBudgetType>,
  ) => {
    if (!selectedMonth) {
      return;
    }
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
    async (date: MonthParamType) => {
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
    const monthDate = selectedMonth?.value.toISOString();
    if (monthDate) {
      fetchMonthBudgets({ month: monthDate });
    }
  }, [selectedMonth]);

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
  if (!userInformations || !selectedMonth) {
    return (
      <div className="flex min-h-dvh w-full justify-center items-center">
        <Spinner />
      </div>
    );
  }

  return (
    <div className="w-full mx-auto p-4 md:p-6 space-y-6 text-text-primary">
      <div className="flex flex-col md:flex-row md:justify-between items-center gap-4 bg-form-bg p-6 border-thin border-border-subtle rounded-md shadow-card">
        <h1 className="text-xl md:text-2xl font-bold text-text-primary">
          Budget
        </h1>
        <div className="w-full md:w-1/2 flex items-center justify-center md:justify-end gap-2">
          <p className="text-sm text-text-secondary whitespace-nowrap">
            Budget summary for
          </p>
          <div className="w-36 md:w-[35%]">
            <Dropdown<MonthItem>
              selectedValue={selectedMonth}
              options={months}
              onSelect={(selected) => handleMonthClick(selected)}
            />
          </div>
        </div>
      </div>

      {!budgets || budgets.length === 0 ? (
        <div className="flex flex-col justify-center items-center w-full h-fit mx-auto mt-10 md:w-1/2 lg:w-5/12 p-6 border-thin border-border-subtle rounded-md bg-bg-surface shadow-card">
          <div className="flex justify-center m-4 p-3 rounded-full bg-bg-muted">
            <MdAccountBalanceWallet className="size-9 text-text-secondary" />
          </div>
          {isCurrentMonth && (
            <h3 className="text-xl font-bold my-2 mx-auto text-center text-text-primary">
              A new month, a clearer financial picture.
            </h3>
          )}

          <h3 className="py-1.5 text-sm font-medium text-center text-text-secondary">
            No budgets recorded for this month{" "}
            <span className={`${isCurrentMonth ? "inline" : "hidden"}`}>
              yet
            </span>
            .
          </h3>
          <h3 className="text-sm pt-3 text-center text-text-primary">
            Keep your spending on track by setting up a global budget or target
            category limits.
          </h3>
          <h3 className="text-sm pb-4 text-center text-text-primary">
            <span className="font-bold font-serif">
              Finance<span className="text-text-secondary">_</span>It
            </span>{" "}
            will help you monitor your budget consumption as the month unfolds.
          </h3>
          {isCurrentMonth && (
            <div className="my-3 px-4 py-2.5 bg-btn-standard hover:bg-btn-standard-hover text-btn-standard-text font-bold rounded-sm shadow-card cursor-pointer">
              <button
                onClick={() => navigate("/budget/new")}
                className="flex items-center cursor-pointer"
              >
                <span className="text-lg mr-1">+</span>
                Add a budget
              </button>
            </div>
          )}

          <p className="text-xs text-text-muted mt-2">
            Small steps today. Better financial insights tomorrow.
          </p>
        </div>
      ) : (
        <>
          <ScrollReveal>
            {globalBudget && (
              <div className="w-full flex flex-col gap-y-4 p-5 border-thin border-border-subtle rounded-md bg-bg-contrast shadow-card">
                {categoryBudgets && categoryBudgets.length == 0 && (
                  <div className="w-full flex justify-end mb-2">
                    {updateGlobalBudget ? (
                      <button
                        onClick={() => setUpdateGlobalBudget(false)}
                        className="h-8 aspect-square flex justify-center items-center rounded-full p-1 text-text-primary hover:bg-bg-muted cursor-pointer"
                      >
                        <IoMdClose className="size-6" />
                      </button>
                    ) : (
                      <>
                        {isCurrentMonth && (
                          <button
                            onClick={() => setBudgetToDelete(globalBudget.id)}
                            className="h-8 aspect-square flex justify-center items-center rounded-full p-1 mr-2 hover:bg-bg-muted text-xl text-red-500 cursor-pointer"
                          >
                            <RiDeleteBin6Line />
                          </button>
                        )}

                        <button
                          onClick={() => {
                            setUpdateGlobalBudget(true);
                            formik.setFieldValue(
                              "amount",
                              globalBudget.amount.amount,
                            );
                            formik.setFieldValue("currency", {
                              code: globalBudget.amount.currency.code,
                              symbol: globalBudget.amount.currency.symbol,
                            });
                          }}
                          className="h-8 aspect-square flex justify-center items-center rounded-full p-1 text-text-secondary hover:bg-bg-muted cursor-pointer"
                        >
                          <CiEdit className="size-6" />
                        </button>
                      </>
                    )}
                  </div>
                )}
                <div className="flex justify-between items-center">
                  <h3 className="font-bold text-text-primary">Total Budgets</h3>
                  <h3 className="font-semibold text-text-secondary">
                    {globalBudget.budgetConsumptionPercentage.toFixed()}%
                  </h3>
                </div>
                <div className="flex justify-between items-center">
                  {!updateGlobalBudget ? (
                    <>
                      <h3 className="font-medium text-text-primary">
                        <span className="font-bold text-emerald-600 dark:text-emerald-400 mr-0.5">
                          {globalBudget.amount.currency.symbol}
                        </span>
                        {globalBudget.amount.amount.toFixed(2)}
                      </h3>
                      <h3 className="font-medium text-text-secondary">
                        <span className="font-bold text-emerald-600 dark:text-emerald-400 mr-0.5">
                          {globalBudget.amount.currency.symbol}
                        </span>
                        {globalBudget.budgetConsumption}
                      </h3>
                    </>
                  ) : (
                    <form
                      onSubmit={formik.handleSubmit}
                      className="w-full"
                    >
                      <div className="w-full flex justify-between mb-5 items-center gap-4">
                        <div className="w-32">
                          <input
                            type="number"
                            name="amount"
                            min={0.01}
                            step="0.01"
                            value={formik.values.amount}
                            onChange={formik.handleChange}
                            placeholder={globalBudget.amount.amount.toString()}
                            className="w-full p-2 text-sm text-text-primary rounded-sm border-thin border-input-border bg-input-bg outline-0 focus:border-accent [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none [-moz-appearance:textfield]"
                          />
                        </div>

                        <div className="w-32">
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
                        textColor="text-btn-standard-text"
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
            )}
          </ScrollReveal>

          <div className="p-4 rounded-md bg-bg-secondary/70 border-thin border-border-subtle">
            {categoryBudgets?.map((b) => (
              <div
                key={b.id}
                className={`relative ${updateBuget === b.id ? "z-50" : "z-0"}`}
              >
                <ScrollReveal>
                  <div className="p-4 my-3 border-thin border-border-subtle rounded-md bg-bg-surface hover:bg-bg-contrast shadow-subtle">
                    <div className="w-full flex justify-end mb-3">
                      {updateBuget === b.id ? (
                        <button
                          onClick={() => setUpdateBudget("")}
                          className="h-8 aspect-square flex justify-center items-center rounded-full p-1 text-text-primary hover:bg-bg-muted cursor-pointer"
                        >
                          <IoMdClose className="size-6" />
                        </button>
                      ) : (
                        <>
                          {isCurrentMonth && (
                            <button
                              onClick={() => setBudgetToDelete(b.id)}
                              className="h-8 aspect-square flex justify-center items-center rounded-full p-1 mr-2 hover:bg-bg-muted text-xl text-red-500 cursor-pointer"
                            >
                              <RiDeleteBin6Line />
                            </button>
                          )}

                          <button
                            onClick={() => {
                              setUpdateBudget(b.id);
                              formik.setFieldValue("amount", b.amount.amount);
                              formik.setFieldValue("currency", {
                                code: b.amount.currency.code,
                                symbol: b.amount.currency.symbol,
                              });
                            }}
                            className="h-8 aspect-square flex justify-center items-center rounded-full p-1 text-text-secondary hover:bg-bg-muted cursor-pointer"
                          >
                            <CiEdit className="size-6" />
                          </button>
                        </>
                      )}
                    </div>

                    <div className="w-full flex flex-col gap-y-4">
                      <div className="flex justify-between items-center">
                        <h3 className="font-bold text-text-primary">
                          {b.categoryName}
                        </h3>
                        <h3 className="font-semibold text-text-secondary">
                          {b.budgetConsumptionPercentage.toFixed()}%
                        </h3>
                      </div>
                      <div className="flex justify-between items-center">
                        {updateBuget !== b.id ? (
                          <>
                            <h3 className="font-medium text-text-primary">
                              <span className="font-bold text-emerald-600 dark:text-emerald-400 mr-0.5">
                                {b.amount.currency.symbol}
                              </span>
                              {b.amount.amount.toFixed(2)}
                            </h3>
                            <h3 className="font-medium text-text-secondary">
                              <span className="font-bold text-emerald-600 dark:text-emerald-400 mr-0.5">
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
                            <div className="w-full flex justify-between mb-5 items-center gap-4">
                              <div className="w-32">
                                <input
                                  type="number"
                                  name="amount"
                                  min={0.01}
                                  step="0.01"
                                  value={formik.values.amount}
                                  onChange={formik.handleChange}
                                  placeholder={b.amount.amount.toString()}
                                  className="w-full p-2 text-sm text-text-primary rounded-sm border-thin border-input-border bg-input-bg outline-0 focus:border-accent [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none [-moz-appearance:textfield]"
                                />
                              </div>

                              <div className="w-32">
                                <CurrencyDropdown
                                  currencyFieldValue={
                                    formik.values.currency.code
                                  }
                                  handleOptionButtonClick={handleCurrencyChange}
                                  className="w-full"
                                />
                              </div>
                            </div>

                            <Button
                              type="submit"
                              label={
                                formik.isSubmitting ? "Updating..." : "Edit"
                              }
                              background="bg-btn-standard"
                              hoverBg="hover:bg-btn-standard-hover"
                              textColor="text-btn-standard-text"
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
              </div>
            ))}
            {isCurrentMonth && (
              <div className="mt-5">
                <Button
                  type="button"
                  label="add category budget"
                  background="bg-btn-standard"
                  hoverBg="hover:bg-btn-standard-hover"
                  textColor="text-btn-standard-text"
                  handleClick={() => navigate("/budget/new")}
                />
              </div>
            )}
          </div>
        </>
      )}

      {budgetToDelete && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-xs p-4">
          <div className="w-full max-w-md rounded-md border-thin border-border-subtle bg-bg-surface p-6 shadow-card">
            <h2 className="text-lg font-bold text-text-primary mb-3">
              Delete budget?
            </h2>

            <p className="text-sm text-text-secondary mb-6">
              Are you sure you want to delete the{" "}
              <span className="font-bold text-text-primary">
                {budgets?.find((b) => b.id === budgetToDelete)?.categoryName ??
                  "Global"}
              </span>{" "}
              budget? This action cannot be undone.
            </p>

            <div className="flex justify-end gap-3">
              <button
                onClick={() => setBudgetToDelete(null)}
                className="px-4 py-2 text-sm font-medium rounded-sm bg-bg-muted text-text-primary hover:bg-bg-secondary border-thin border-border-subtle cursor-pointer"
              >
                Cancel
              </button>

              <button
                onClick={async () => {
                  await handleDelete(budgetToDelete);
                  setBudgetToDelete(null);
                }}
                className="px-4 py-2 text-sm font-semibold rounded-sm bg-btn-danger text-btn-danger-text hover:bg-btn-danger-hover cursor-pointer"
              >
                Confirm
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default Budgets;
