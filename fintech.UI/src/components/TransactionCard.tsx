import Button from "./Button";
import type { GetTransactionResponse } from "../types/financialTransactionTypes";
import { IoMdClose } from "react-icons/io";
import { CiEdit } from "react-icons/ci";
import FormatAmount from "../helpers/amountFormatter";
import ScrollReveal from "./ScrollReveal";

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

  return (
    <ScrollReveal>
      <div
        className={`my-2 border-thin border-border-subtle rounded-md bg-bg-muted w-full transition-all duration-200 ${
          updateTransaction === transaction.id
            ? "shadow-lg p-2"
            : "cursor-pointer"
        }`}
      >
        <div
          className={`min-w-full ${transactionDetails === transaction.id ? "flex" : "hidden"} justify-end items-baseline px-5 py-3`}
        >
          <button
            onClick={() => {
              setUpdateTransaction("");
            }}
            className={`h-8 aspect-square justify-center items-center rounded-full p-1 hover:bg-bg-secondary cursor-pointer ${updateTransaction === transaction.id ? "flex" : "hidden"}`}
          >
            <IoMdClose className="size-6" />
          </button>
          <button
            onClick={startUpdateTransaction}
            className={`h-8 aspect-square justify-center items-center rounded-full p-1 hover:bg-bg-secondary cursor-pointer ${updateTransaction === transaction.id ? "hidden" : "flex"}`}
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
              className={`my-1 ${updateTransaction === transaction.id ? "block" : "hidden"}`}
            >
              Type
            </h3>
            <h3
              className={`${updateTransaction === transaction.id ? "w-full px-2 py-1 border-thin border-border-subtle rounded-sm bg-bg-contrast" : "mx-0"}`}
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
              className={`my-1 ${updateTransaction === transaction.id ? "block" : "hidden"}`}
            >
              Category
            </h3>
            <h3
              className={`${updateTransaction === transaction.id ? "w-full px-2 py-1 border-thin border-border-subtle rounded-sm bg-bg-contrast" : "mx-0"}`}
            >
              {transaction.categoryName}
            </h3>
          </div>
          <div className="w-full md:w-1/3">
            <h3
              className={`my-1 ${updateTransaction === transaction.id ? "block" : "hidden"}`}
            >
              Amount
            </h3>
            <div
              className={`flex ${updateTransaction === transaction.id ? "w-full px-2 py-1 border-thin border-border-subtle rounded-sm bg-bg-contrast" : "float-end"}`}
            >
              <h3 className="font-bold text-green-500 ">
                {transaction.currencySymbol}
              </h3>{" "}
              <h3 className="font-bold ml-1">
                {FormatAmount(transaction.amount)}
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
                className={`${updateTransaction === transaction.id ? "block" : "hidden"}`}
              >
                Description
              </h3>
              <input
                type="text"
                placeholder={transaction!.description}
                onChange={handleDescription}
                className={`placeholder:text-text-primary ${updateTransaction === transaction.id ? "w-full px-2 py-1 border-thin border-border-subtle rounded-sm bg-bg-contrast outline-0 " : "mx-0"}`}
              />
            </div>
            <div className="w-full md:w-1/3">
              <h3
                className={`my-1 ${updateTransaction === transaction.id ? "block" : "hidden"}`}
              >
                Base Amount
              </h3>
              <div
                className={`flex ${updateTransaction === transaction.id ? "w-full px-2 py-1 border-thin border-border-subtle rounded-sm bg-bg-contrast font-bold" : "float-end"}`}
              >
                {transaction.baseAmount !== transaction.amount ? (
                  <>
                    {" "}
                    <h3 className="font-bold text-green-500">
                      {transaction.baseCurrencySymbol}
                    </h3>{" "}
                    <h3 className="ml-1">
                      {FormatAmount(transaction.baseAmount)}
                    </h3>
                  </>
                ) : updateTransaction === transaction.id ? (
                  FormatAmount(transaction.amount)
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
                className={`${updateTransaction === transaction.id ? "block" : "hidden"}`}
              >
                Transaction Date
              </h3>
              <h3
                className={`${updateTransaction === transaction.id ? "w-full px-2 py-1 border-thin border-border-subtle rounded-sm bg-bg-contrast" : "mx-0"}`}
              >
                {transaction.transactionDate.toDateString()}
              </h3>
            </div>

            <div className="w-full md:w-1/3">
              <h3
                className={`my-1 ${updateTransaction === transaction.id ? "block" : "hidden"}`}
              >
                Exchange Rate
              </h3>
              <h3
                className={`${updateTransaction === transaction.id ? "w-full px-2 py-1 border-thin border-border-subtle rounded-sm bg-bg-contrast" : "float-end"}`}
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
            className={`min-w-full  px-5 py-3 ${updateTransaction === transaction.id ? "flex-col md:flex md:flex-row md:justify-between md:items-end" : "block"}`}
          >
            <div className="w-full md:w-1/3">
              <h3
                className={`my-1 ${updateTransaction === transaction.id ? "block" : "hidden"}`}
              >
                Importance
              </h3>
              <h3
                onClick={handleIsEssential}
                className={`${updateTransaction === transaction.id ? "w-full px-2 py-1 border-thin border-border-subtle rounded-sm bg-bg-contrast cursor-pointer" : "mx-0"}`}
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
                  handleClick={handleDelete}
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
              className={`w-50 h-1 mx-auto rounded-full my-2 ${transaction.isEssential ? "bg-lime-500" : "bg-red-500"} ${updateTransaction === transaction.id ? "hidden" : "block"}`}
            ></div>
          </div>
        </div>
      </div>
    </ScrollReveal>
  );
}

export default TransactionCard;
