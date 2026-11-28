import Button from "./Button";
import type { GetTransactionResponse } from "../types/financialTransactionTypes";
import { IoMdClose } from "react-icons/io";
import { CiEdit } from "react-icons/ci";
import FormatAmount from "../helpers/amountFormatter";
import ScrollReveal from "./ScrollReveal";
import { useState } from "react";

interface PropsType<GetTransactionResponse> {
  transaction: GetTransactionResponse;
  startUpdateTransaction: () => void;
  handleDescription: (e: React.ChangeEvent<HTMLInputElement>) => void;
  handleIsEssential: () => void;
  isEssential: boolean;
  handleDelete: () => void;
  handleUpdate: () => void;
  description: string;
  updateTransaction: string;
  setUpdateTransaction: React.Dispatch<React.SetStateAction<string>>;
  transactionDetails: string;
  setTransactionDetails: React.Dispatch<React.SetStateAction<string>>;
}

function TransactionCard<T extends GetTransactionResponse>(
  props: PropsType<T>,
) {
  const {
    transaction,
    startUpdateTransaction,
    handleDescription,
    handleIsEssential,
    isEssential,
    handleDelete,
    handleUpdate,
    description,
    updateTransaction,
    setUpdateTransaction,
    transactionDetails,
    setTransactionDetails,
  } = props;

  const [transactionToDelete, setTransactionToDelete] = useState<string | null>(
    null,
  );
  return (
    <ScrollReveal>
      <div
        className={`my-2.5 border-thin border-border-subtle rounded-md bg-bg-surface hover:bg-bg-contrast w-full transition-all duration-200 ${
          updateTransaction === transaction.id
            ? "shadow-card p-2 bg-bg-contrast"
            : "shadow-subtle cursor-pointer"
        }`}
      >
        <div
          className={`min-w-full ${transactionDetails === transaction.id ? "flex" : "hidden"} justify-end items-baseline px-5 py-3`}
        >
          <button
            onClick={() => {
              setUpdateTransaction("");
            }}
            className={`h-8 aspect-square justify-center items-center rounded-full p-1 text-text-primary hover:bg-bg-muted cursor-pointer ${updateTransaction === transaction.id ? "flex" : "hidden"}`}
          >
            <IoMdClose className="size-6" />
          </button>
          <button
            onClick={startUpdateTransaction}
            className={`h-8 aspect-square justify-center items-center rounded-full p-1 text-text-secondary hover:bg-bg-muted cursor-pointer ${updateTransaction === transaction.id ? "hidden" : "flex"}`}
          >
            <CiEdit className="size-6" />
          </button>
        </div>
        <div
          onClick={() =>
            updateTransaction !== transaction.id && setTransactionDetails("")
          }
          className={`min-w-full ${transactionDetails === transaction.id ? "flex" : "hidden"} justify-between items-baseline px-5 py-3`}
        >
          <div className="w-full md:w-1/3">
            <h3
              className={`my-1 text-xs font-semibold uppercase tracking-wider text-text-muted ${updateTransaction === transaction.id ? "block" : "hidden"}`}
            >
              Type
            </h3>
            <h3
              className={`${updateTransaction === transaction.id ? "w-full px-2.5 py-1.5 border-thin border-input-border rounded-sm bg-input-bg text-text-primary" : "mx-0 font-medium text-text-secondary"}`}
            >
              {transaction.type}
            </h3>
          </div>
        </div>
        <div
          onClick={() =>
            transactionDetails !== transaction.id
              ? setTransactionDetails(transaction.id)
              : updateTransaction !== transaction.id &&
                setTransactionDetails("")
          }
          className={`min-w-full justify-between items-baseline px-5 py-3 ${updateTransaction === transaction.id ? "flex-col md:flex md:flex-row" : "flex"}`}
        >
          <div
            className={`w-full pb-6 md:pb-0 ${updateTransaction === transaction.id ? "md:w-1/3" : "md:w-fit"}`}
          >
            <h3
              className={`my-1 text-xs font-semibold uppercase tracking-wider text-text-muted ${updateTransaction === transaction.id ? "block" : "hidden"}`}
            >
              Category
            </h3>
            <h3
              className={`${updateTransaction === transaction.id ? "w-full px-2.5 py-1.5 border-thin border-input-border rounded-sm bg-input-bg text-text-primary" : "mx-0 font-semibold text-text-primary"}`}
            >
              {transaction.categoryName}
            </h3>
          </div>
          <div className="w-full md:w-1/3">
            <h3
              className={`my-1 text-xs font-semibold uppercase tracking-wider text-text-muted ${updateTransaction === transaction.id ? "block" : "hidden"}`}
            >
              Amount
            </h3>
            <div
              className={`flex ${updateTransaction === transaction.id ? "w-full px-2.5 py-1.5 border-thin border-input-border rounded-sm bg-input-bg" : "float-end"}`}
            >
              <h3 className="font-bold text-emerald-600 dark:text-emerald-400">
                {transaction.amount.currency.symbol}
              </h3>{" "}
              <h3 className="font-bold ml-1 text-text-primary">
                {FormatAmount(transaction.amount.amount)}
              </h3>
            </div>
          </div>
        </div>
        <div
          onClick={() =>
            updateTransaction !== transaction.id && setTransactionDetails("")
          }
          className={`min-w-full ${transactionDetails === transaction.id ? "block" : "hidden"}`}
        >
          <div
            className={`min-w-full justify-between items-baseline px-5 py-3 ${updateTransaction === transaction.id ? "flex-col md:flex md:flex-row" : "flex"}`}
          >
            <div
              className={`w-full pb-6 md:pb-0 ${updateTransaction === transaction.id ? "md:w-1/3" : "md:w-fit"}`}
            >
              <h3
                className={`text-xs font-semibold uppercase tracking-wider text-text-muted ${updateTransaction === transaction.id ? "block mb-1" : "hidden"}`}
              >
                Description
              </h3>
              <input
                type="text"
                placeholder={transaction!.description}
                onChange={handleDescription}
                className={`placeholder:text-text-muted text-text-primary ${updateTransaction === transaction.id ? "w-full px-2.5 py-1.5 border-thin border-input-border rounded-sm bg-input-bg outline-0 focus:border-accent" : "mx-0 bg-transparent"}`}
              />
            </div>
            <div className="w-full md:w-1/3">
              <h3
                className={`my-1 text-xs font-semibold uppercase tracking-wider text-text-muted ${updateTransaction === transaction.id ? "block" : "hidden"}`}
              >
                Base Amount
              </h3>
              <div
                className={`flex ${updateTransaction === transaction.id ? "w-full px-2.5 py-1.5 border-thin border-input-border rounded-sm bg-input-bg font-bold" : "float-end"}`}
              >
                {transaction.baseAmount !== transaction.amount ? (
                  <>
                    {" "}
                    <h3 className="font-bold text-emerald-600 dark:text-emerald-400">
                      {transaction.baseAmount.currency.symbol}
                    </h3>{" "}
                    <h3 className="ml-1 text-text-primary">
                      {FormatAmount(transaction.baseAmount.amount)}
                    </h3>
                  </>
                ) : updateTransaction === transaction.id ? (
                  FormatAmount(transaction.amount.amount)
                ) : (
                  ""
                )}
              </div>
            </div>
          </div>
          <div
            className={`min-w-full justify-between items-baseline px-5 py-3 ${updateTransaction === transaction.id ? "flex-col md:flex md:flex-row" : "flex"}`}
          >
            <div className="w-full pb-6 md:w-1/3 md:pb-0">
              <h3
                className={`text-xs font-semibold uppercase tracking-wider text-text-muted ${updateTransaction === transaction.id ? "block mb-1" : "hidden"}`}
              >
                Transaction Date
              </h3>
              <h3
                className={`${updateTransaction === transaction.id ? "w-full px-2.5 py-1.5 border-thin border-input-border rounded-sm bg-input-bg text-text-primary" : "mx-0 text-text-secondary"}`}
              >
                {transaction.transactionDate}
              </h3>
            </div>

            <div className="w-full md:w-1/3">
              <h3
                className={`my-1 text-xs font-semibold uppercase tracking-wider text-text-muted ${updateTransaction === transaction.id ? "block" : "hidden"}`}
              >
                Exchange Rate
              </h3>
              <h3
                className={`${updateTransaction === transaction.id ? "w-full px-2.5 py-1.5 border-thin border-input-border rounded-sm bg-input-bg text-text-primary" : "float-end text-text-secondary"}`}
              >
                {transaction.exchangeRate !== 1
                  ? transaction.exchangeRate
                  : updateTransaction === transaction.id
                    ? 1
                    : ""}
              </h3>
            </div>
          </div>
          <div
            className={`min-w-full px-5 py-3 ${updateTransaction === transaction.id ? "flex-col md:flex md:flex-row md:justify-between md:items-end" : "block"}`}
          >
            <div className="w-full md:w-1/3">
              <h3
                className={`my-1 text-xs font-semibold uppercase tracking-wider text-text-muted ${updateTransaction === transaction.id ? "block" : "hidden"}`}
              >
                Importance
              </h3>
              <h3
                onClick={handleIsEssential}
                className={`${updateTransaction === transaction.id ? "w-full px-2.5 py-1.5 border-thin border-input-border rounded-sm bg-input-bg hover:bg-bg-muted text-text-primary cursor-pointer" : "mx-0"}`}
              >
                {updateTransaction === transaction.id
                  ? isEssential
                    ? "Important"
                    : "Not Important"
                  : ""}
              </h3>
            </div>
            <div
              className={`w-full md:w-1/3 ${updateTransaction === transaction.id ? "flex-col md:flex md:flex-row justify-between" : "hidden"} `}
            >
              <div className="w-full mt-10 md:mt-2 md:w-[45%]">
                <Button
                  label="Delete"
                  type="button"
                  background="bg-btn-danger"
                  hoverBg="hover:bg-btn-danger-hover"
                  textColor="btn-danger-text"
                  handleClick={() => setTransactionToDelete(transaction.id)}
                  disabled={
                    new Date(transaction.transactionDate).getFullYear <
                    new Date().getFullYear
                  }
                />
              </div>
              <div className="w-full mt-2 md:w-[45%]">
                <Button
                  label="Update"
                  type="button"
                  background="bg-btn-standard"
                  hoverBg="hover:bg-btn-standard-hover"
                  textColor="btn-standard-text"
                  handleClick={handleUpdate}
                  disabled={
                    description === transaction.description &&
                    isEssential === transaction.isEssential
                  }
                />
              </div>
            </div>
            <div
              className={`w-50 h-1 mx-auto rounded-full my-2 ${transaction.isEssential ? "bg-emerald-500" : "bg-red-500"} ${updateTransaction === transaction.id ? "hidden" : "block"}`}
            ></div>
            {transactionToDelete && (
              <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-xs p-4">
                <div className="w-full max-w-md rounded-md border-thin border-border-subtle bg-bg-surface p-6 shadow-card">
                  <h2 className="text-lg font-bold text-text-primary mb-3">
                    Delete Financial Transaction?
                  </h2>

                  <p className="text-sm text-text-secondary mb-6">
                    Are you sure you want to delete the{" "}
                    <span className="font-bold text-text-primary">
                      {transaction.categoryName}
                    </span>{" "}
                    transaction? This action cannot be undone.
                  </p>

                  <div className="flex justify-end gap-3">
                    <button
                      onClick={() => setTransactionToDelete(null)}
                      className="px-4 py-2 text-sm font-medium rounded-sm bg-bg-muted text-text-primary hover:bg-bg-secondary border-thin border-border-subtle cursor-pointer"
                    >
                      Cancel
                    </button>

                    <button
                      onClick={() => {
                        handleDelete();
                        setTransactionToDelete(null);
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
        </div>
      </div>
    </ScrollReveal>
  );
}

export default TransactionCard;
