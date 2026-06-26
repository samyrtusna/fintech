import { useState } from "react";
import {
  type FinancialTransactionProps,
  type UpdateStateType,
} from "../types/financialTransactionTypes";
import {
  ArrowLeft,
  BadgeDollarSign,
  Banknote,
  BanknoteArrowDown,
  Beer,
  Calendar,
  Coins,
  PiggyBank,
  ShieldCheck,
  ShoppingBasket,
  Sigma,
  Wallet,
  WalletCards,
} from "lucide-react";
import { useAppDispatch } from "../state/stateHooks";
import {
  deleteTransaction,
  updateTransaction,
} from "../state/slices/financialTransactionSlice";

function FinancialTransaction(props: FinancialTransactionProps) {
  const { transaction, baseCurrency, handleBack } = props;

  const today = new Date().getDate();
  const transactionDay = new Date(transaction.transactionDate!).getDate();

  const [updateState, setUpdateState] = useState<UpdateStateType>({
    description: "",
    isEssential: transaction.isEssential!,
  });

  const dispatch = useAppDispatch();

  const handleUpdate = async () => {
    handleBack();
    if (
      updateState.description.length > 0 &&
      updateState.description !== transaction.description
    ) {
      const requestBody = {
        id: transaction.id,
        bodyObject: {
          description: updateState.description,
        },
      };
      return await dispatch(updateTransaction(requestBody));
    }
    if (updateState.isEssential !== transaction.isEssential) {
      const requestBody = {
        id: transaction.id,
        bodyObject: {
          isEssential: updateState.isEssential,
        },
      };
      return await dispatch(updateTransaction(requestBody));
    }
    if (
      updateState.description.length > 0 &&
      updateState.description !== transaction.description &&
      updateState.isEssential !== transaction.isEssential
    ) {
      const requestBody = {
        id: transaction.id,
        bodyObject: {
          description: updateState.description,
          isEssential: updateState.isEssential,
        },
      };
      return await dispatch(updateTransaction(requestBody));
    }
  };

  const handleDelete = async () => {
    handleBack();
    dispatch(deleteTransaction(transaction.id));
  };

  return (
    <div className="absolute top-15 md:top-30 bottom-0 md:bottom-25 lg:left-1/2 lg:-translate-x-1/2 z-20 w-full lg:w-10/12 px-10 bg-bg-sec ">
      <div className="flex justify-between items-center my-2">
        <button
          onClick={handleBack}
          className="cursor-pointer"
        >
          <ArrowLeft className="stroke-gray-500" />
        </button>
        <div className="hidden md:flex">
          {transactionDay === today && (
            <button
              onClick={handleDelete}
              className="hidden md:block mx-2 py-0.5 px-4 border-2 border-red-600
           font-semibold hover:font-bold text-red-600 rounded-full cursor-pointer"
            >
              Delete
            </button>
          )}
          <button
            onClick={handleUpdate}
            className="hidden md:block py-0.5 px-3 border-2 border-bt-g-h text-bt-g-h font-semibold hover:font-bold rounded-full cursor-pointer"
          >
            Update
          </button>
        </div>
      </div>
      <div className="md:flex w-full rounded-xl shadow-xl">
        <div className="flex flex-col justify-center items-center py-3 md:w-5/12 bg-bg">
          <div className="flex justify-center items-center w-15 h-15 m-3 p-3 rounded-xl bg-bg-sec">
            {transaction.type === "Income" ? (
              <BadgeDollarSign className="size-8 stroke-1 stroke-blue-500" />
            ) : transaction.type === "Expense" ? (
              <ShoppingBasket className="size-8 stroke-1 stroke-blue-500" />
            ) : transaction.type === "Investment" ? (
              <Coins className="size-8 stroke-1 stroke-blue-500" />
            ) : transaction.type === "ContractedLoan" ? (
              <Banknote className="size-8 stroke-1 stroke-blue-500" />
            ) : transaction.type === "Debt" ? (
              <WalletCards className="size-8 stroke-1 stroke-blue-500" />
            ) : transaction.type === "InterestPayment" ||
              transaction.type === "PrincipalRepayment" ? (
              <BanknoteArrowDown className="size-8 stroke-1 stroke-blue-500" />
            ) : transaction.type === "Savings" ? (
              <PiggyBank className="size-8 stroke-1 stroke-blue-500" />
            ) : null}
          </div>
          <div className="text-sm font-bold py-1 px-2 bg-bg-sec rounded-xl">
            {transaction.type}
          </div>
          <div className="flex py-2 items-end">
            <h2 className="text-4xl font-bold">{transaction.baseAmount}</h2>
            <h2 className="pl-2">{baseCurrency}</h2>
          </div>
          <div className="flex flex-col items-center text-gray-500">
            <input
              type="text"
              placeholder={transaction.description}
              onChange={(e) =>
                setUpdateState({ ...updateState, description: e.target.value })
              }
              className="text-center"
            />
          </div>
          <div className="opacity-0 md:opacity-100 w-1/6 md:mt-10 border-t"></div>
        </div>
        <div className="md:w-7/12 p-10">
          <div className="md:flex md:justify-between md:my-5 w-full">
            <div className="flex items-center w-full md:w-40 md:shrink-0 my-2 md:my-0">
              <div className="flex h-full aspect-square rounded-xl bg-bg justify-center items-center">
                <Beer />
              </div>
              <div className="ml-3">
                <h2 className="text-sm text-gray-400 font-semibold">
                  Category
                </h2>
                <h2 className="font-bold">{transaction.categoryName}</h2>
              </div>
            </div>
            <div className="flex items-center w-full md:w-40 md:shrink-0 my-2 md:my-0">
              <div className="flex h-full aspect-square rounded-xl bg-bg justify-center items-center">
                <Calendar />
              </div>
              <div className="ml-3">
                <h2 className="text-sm text-gray-400 font-semibold">Date</h2>
                <h2 className="font-bold">
                  {transaction.transactionDate &&
                    new Date(transaction.transactionDate).toLocaleDateString()}
                </h2>
              </div>
            </div>
          </div>
          <div className="md:flex md:justify-between w-full md:my-5">
            <div className="flex items-center w-full md:w-40 md:shrink-0 my-2 md:my-0">
              <div className="flex h-full aspect-square rounded-xl bg-bg justify-center items-center">
                <Wallet />
              </div>
              <div className="ml-3">
                <h2 className="text-sm text-gray-400 font-semibold">Amount</h2>
                <div className="flex items-baseline">
                  <h2 className="font-bold text-lg mr-1 ">
                    {transaction.amount}
                  </h2>
                  <span className="text-xs font-light text-bt-g-h">
                    {transaction.currency}
                  </span>{" "}
                </div>
              </div>
            </div>
            <div className="flex items-center w-full md:w-40 md:shrink-0 my-2 md:my-0">
              <div className="flex h-full aspect-square rounded-xl bg-bg justify-center items-center">
                <Sigma />
              </div>
              <div className="ml-3">
                <h2 className="text-sm text-gray-400 font-semibold">
                  Exchange_Rate
                </h2>
                <h2 className="font-bold">{transaction.exchangeRate}</h2>
              </div>
            </div>
          </div>
          <div className="md:flex md:justify-between w-full md:my-5">
            <div className="flex items-center w-full md:w-40 md:shrink-0 my-2 md:my-0">
              <div className="flex h-full aspect-square rounded-xl bg-bg justify-center items-center">
                <ShieldCheck />
              </div>
              <div className="ml-3">
                <h2 className="text-sm text-gray-400 font-semibold">
                  Importance
                </h2>
                <select
                  name="isEssential"
                  id="isEssential"
                  value={updateState.isEssential?.toString()}
                  onChange={(e) =>
                    setUpdateState({
                      ...updateState,
                      isEssential: e.target.value === "true" ? true : false,
                    })
                  }
                >
                  <option value="true">Essential</option>
                  <option value="false">Not Essential</option>
                </select>
              </div>
            </div>
          </div>
          <div className="w-full mt-10 border-t border-t-gray-300"></div>
          <button
            onClick={handleUpdate}
            className="w-full mt-2 py-1 px-3 bg-bt-g-h text-bt-g-text font-semibold rounded-full cursor-pointer md:hidden"
          >
            Update
          </button>
          {transactionDay === today && (
            <button
              onClick={handleDelete}
              className="w-full my-2 py-1 px-3 bg-bt-d-h text-bt-g-text font-semibold rounded-full cursor-pointer md:hidden"
            >
              Delete
            </button>
          )}
        </div>
      </div>
    </div>
  );
}

export default FinancialTransaction;
