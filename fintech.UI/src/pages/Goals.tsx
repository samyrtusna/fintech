/* eslint-disable react-hooks/exhaustive-deps */
import { GoGoal } from "react-icons/go";
import Dropdown from "../components/Dropdown";
import type { MonthItem } from "../types/financialTransactionTypes";
import generateMonths from "../helpers/generateMonths";
import { useAppSelector } from "../state/stateHooks";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import ScrollReveal from "../components/ScrollReveal";
import { IoMdClose } from "react-icons/io";
import { RiDeleteBin6Line } from "react-icons/ri";
import { CiEdit } from "react-icons/ci";
import CurrencyDropdown from "../components/CurrencyDropdown";
import Button from "../components/Button";
import goalService from "../API/Services/goalService";
import type { MonthParamType } from "../types/budgetTypes";
import { setGoal } from "../state/slices/goalSlice";
import { toast } from "sonner";
import type { UpdateGoalType } from "../types/goalTypes";
import { useFormik, type FormikHelpers } from "formik";
import { updateGoalSchema } from "../formik/updateGoalSchema";
import userService from "../API/Services/userService";
import { setUserInformations } from "../state/slices/authSlice";
import Spinner from "../components/Spinner";

function Goals() {
  const userInformations = useAppSelector(
    (state) => state.authUser.userInformations,
  );
  const goals = useAppSelector((state) => state.goals.goals);

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
  const [updateGoal, setUpdateGoal] = useState("");
  const [goalToDelete, setGoalToDelete] = useState<string | null>(null);

  useEffect(() => {
    if (months.length > 0 && !selectedMonth) {
      setSelectedMonth(months[0]);
    }
  }, [months, selectedMonth]);

  const isCurrentMonth =
    selectedMonth?.value.getFullYear() === new Date().getFullYear() &&
    selectedMonth.value.getMonth() === new Date().getMonth() + 1;

  const initialValues: UpdateGoalType = {
    title: "",
    targetAmount: {
      amount: 0.01,
      currency: { code: "", symbol: "" },
    },
  };

  const handleSubmit = async (
    values: UpdateGoalType,
    props: FormikHelpers<UpdateGoalType>,
  ) => {
    if (!selectedMonth) {
      return;
    }
    try {
      const selectedCurrency = userInformations?.userCurrencies.find(
        (c) => c.currency.code === values.targetAmount?.currency.code,
      );
      const currency = {
        code: values.targetAmount!.currency.code,
        symbol: selectedCurrency!.currency.symbol,
      };
      const requestBody: UpdateGoalType = {
        title: values.title,
        targetAmount: {
          amount: values.targetAmount!.amount,
          currency: currency,
        },
      };
      await goalService.updateAsync(updateGoal, requestBody);

      await fetchMonthGoals({ month: selectedMonth.value.toISOString() });

      props.resetForm();
      setUpdateGoal("");
      toast.success("Goal updated successfully");
    } catch (error) {
      toast.error(
        error instanceof Error ? error.message : "An unexpected error occurred",
      );
    } finally {
      props.setSubmitting(false);
    }
  };

  const formik = useFormik<UpdateGoalType>({
    initialValues,
    validationSchema: updateGoalSchema,
    onSubmit: handleSubmit,
  });

  useEffect(() => {
    const userCurrencies = userInformations?.userCurrencies ?? [];
    const defaultCurrency = userCurrencies.find((c) => c.isDefault);
    if (defaultCurrency) {
      formik.setFieldValue("targetAmount.currency", {
        code: defaultCurrency.currency.code,
        symbol: defaultCurrency.currency.symbol,
      });
    }
  }, [userInformations]);

  const fetchMonthGoals = useCallback(
    async (date: MonthParamType) => {
      try {
        const goals = await goalService.getByMonthAsync(date);
        dispatch(setGoal(goals));
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
      fetchMonthGoals({ month: monthDate });
    }
  }, [selectedMonth]);

  const handleMonthClick = (month: MonthItem) => {
    setSelectedMonth(month);
  };

  const handleCurrencyChange = (code: string) => {
    const selectedUserCurrency = userInformations?.userCurrencies.find(
      (uc) => uc.currency.code === code,
    );
    formik.setFieldValue("targetAmount.currency", {
      code: code,
      symbol: selectedUserCurrency?.currency.symbol ?? "",
    });
  };

  const handleDelete = async (id: string) => {
    await goalService.deleteAsync(id);
    fetchMonthGoals({ month: new Date().toISOString() });
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
          Goals
        </h1>
        <div className="w-full md:w-1/2 flex items-center justify-center md:justify-end gap-2">
          <p className="text-sm text-text-secondary whitespace-nowrap">
            Goal summary for
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

      {!goals || goals.length === 0 ? (
        <div className="flex flex-col justify-center items-center w-full h-fit mx-auto mt-10 md:w-1/2 lg:w-5/12 p-6 border-thin border-border-subtle rounded-md bg-bg-surface shadow-card">
          <div className="flex justify-center m-4 p-3 rounded-full bg-bg-muted">
            <GoGoal className="size-9 text-text-secondary" />
          </div>
          {isCurrentMonth && (
            <h3 className="text-xl font-bold my-2 mx-auto text-center text-text-primary">
              A new month, a clearer financial picture.
            </h3>
          )}

          <h3 className="py-1.5 text-sm font-medium text-center text-text-secondary">
            No goals recorded for this month{" "}
            <span className={`${isCurrentMonth ? "inline" : "hidden"}`}>
              yet
            </span>
            .
          </h3>
          <h3 className="text-sm pt-3 text-center text-text-primary">
            Keep your goals on track by setting up financial objectives.
          </h3>
          <h3 className="text-sm pb-4 text-center text-text-primary">
            <span className="text-text-primary font-bold font-serif">
              Finance<span className="text-text-secondary">_</span>It
            </span>{" "}
            will help you monitor your financial targets as the month unfolds.
          </h3>
          {isCurrentMonth && (
            <div className="my-3 px-4 py-2.5 bg-btn-standard hover:bg-btn-standard-hover text-btn-standard-text font-bold rounded-sm shadow-card cursor-pointer">
              <button
                onClick={() => navigate("/goal/new")}
                className="flex items-center cursor-pointer"
              >
                <span className="text-lg mr-1">+</span>
                Add a goal
              </button>
            </div>
          )}

          <p className="text-xs text-text-muted mt-2">
            Small steps today. Better financial insights tomorrow.
          </p>
        </div>
      ) : (
        <div className="p-4 rounded-md bg-bg-secondary/70 border-thin border-border-subtle">
          {goals?.map((g) => (
            <div
              key={g.id}
              className={`relative ${updateGoal === g.id ? "z-50" : "z-0"}`}
            >
              <ScrollReveal>
                <div className="p-4 my-3 border-thin border-border-subtle rounded-md bg-bg-surface hover:bg-bg-contrast shadow-subtle">
                  <div className="w-full flex justify-end mb-3">
                    {updateGoal === g.id ? (
                      <button
                        onClick={() => setUpdateGoal("")}
                        className="h-8 aspect-square flex justify-center items-center rounded-full p-1 text-text-primary hover:bg-bg-muted cursor-pointer"
                      >
                        <IoMdClose className="size-6" />
                      </button>
                    ) : (
                      <>
                        {isCurrentMonth && (
                          <button
                            onClick={() => setGoalToDelete(g.id)}
                            className="h-8 aspect-square flex justify-center items-center rounded-full p-1 mr-2 hover:bg-bg-muted text-xl text-red-500 cursor-pointer"
                          >
                            <RiDeleteBin6Line />
                          </button>
                        )}

                        <button
                          onClick={() => {
                            setUpdateGoal(g.id);
                            formik.setValues({
                              title: g.title,
                              targetAmount: {
                                amount: g.targetAmount.amount,
                                currency: {
                                  code: g.targetAmount.currency.code,
                                  symbol: g.targetAmount.currency.symbol,
                                },
                              },
                            });
                          }}
                          className="h-8 aspect-square flex justify-center items-center rounded-full p-1 text-text-secondary hover:bg-bg-muted cursor-pointer"
                        >
                          <CiEdit className="size-6" />
                        </button>
                      </>
                    )}
                  </div>

                  {updateGoal !== g.id ? (
                    <div className="w-full flex flex-col gap-y-4">
                      <div className="flex justify-between items-center">
                        <h3 className="font-bold text-text-primary">
                          {g.title}
                        </h3>
                        <h3 className="font-semibold text-text-secondary">
                          {(
                            (g.goalProgress / g.targetAmount.amount) *
                            100
                          ).toFixed()}
                          %
                        </h3>
                      </div>
                      <div className="flex justify-between items-center">
                        <h3 className="text-sm font-medium text-text-secondary">
                          {g.category?.name ?? g.targetType}
                        </h3>
                        <h3 className="font-medium text-text-primary">
                          <span className="font-bold text-emerald-600 dark:text-emerald-400 mr-0.5">
                            {g.targetAmount.currency.symbol}
                          </span>
                          {g.targetAmount.amount.toFixed(2)}
                        </h3>
                      </div>
                    </div>
                  ) : (
                    <form
                      onSubmit={formik.handleSubmit}
                      className="w-full"
                    >
                      <div className="w-full flex flex-col gap-y-4 mb-5">
                        <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-3">
                          <div className="w-full sm:w-56">
                            <input
                              type="text"
                              name="title"
                              value={formik.values.title}
                              onChange={formik.handleChange}
                              placeholder={g.title}
                              className="w-full p-2 text-sm text-text-primary rounded-sm border-thin border-input-border bg-input-bg outline-0 focus:border-accent"
                            />
                          </div>
                          <h3 className="text-sm font-semibold text-text-secondary">
                            {g.category?.name ?? g.targetType}
                          </h3>
                        </div>
                        <div className="flex justify-between items-center gap-4">
                          <div className="w-32">
                            <input
                              type="number"
                              name="targetAmount.amount"
                              min={0.01}
                              step="0.01"
                              value={formik.values.targetAmount!.amount}
                              onChange={formik.handleChange}
                              placeholder={g.targetAmount.amount.toString()}
                              className="w-full p-2 text-sm text-text-primary rounded-sm border-thin border-input-border bg-input-bg outline-0 focus:border-accent [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none [-moz-appearance:textfield]"
                            />
                          </div>

                          <div className="w-32">
                            <CurrencyDropdown
                              currencyFieldValue={
                                formik.values.targetAmount!.currency.code
                              }
                              handleOptionButtonClick={handleCurrencyChange}
                              className="w-full"
                            />
                          </div>
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
              </ScrollReveal>
            </div>
          ))}
          {isCurrentMonth && (
            <div className="mt-5">
              <Button
                type="button"
                label="add a goal"
                background="bg-btn-standard"
                hoverBg="hover:bg-btn-standard-hover"
                textColor="text-btn-standard-text"
                handleClick={() => navigate("/goal/new")}
              />
            </div>
          )}
        </div>
      )}

      {goalToDelete && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-xs p-4">
          <div className="w-full max-w-md rounded-md border-thin border-border-subtle bg-bg-surface p-6 shadow-card">
            <h2 className="text-lg font-bold text-text-primary mb-3">
              Delete goal?
            </h2>

            <p className="text-sm text-text-secondary mb-6">
              Are you sure you want to delete the{" "}
              <span className="font-bold text-text-primary">
                {goals?.find((g) => g.id === goalToDelete)?.title ??
                  goals?.find((g) => g.id === goalToDelete)?.category?.name ??
                  "Goal"}
              </span>{" "}
              goal? This action cannot be undone.
            </p>

            <div className="flex justify-end gap-3">
              <button
                onClick={() => setGoalToDelete(null)}
                className="px-4 py-2 text-sm font-medium rounded-sm bg-bg-muted text-text-primary hover:bg-bg-secondary border-thin border-border-subtle cursor-pointer"
              >
                Cancel
              </button>

              <button
                onClick={async () => {
                  await handleDelete(goalToDelete);
                  setGoalToDelete(null);
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

export default Goals;
