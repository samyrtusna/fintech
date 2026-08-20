import { useState } from "react";
import financialTrascationService from "../API/Services/financialTrascationService";
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
import FormatAmount from "../helpers/amountFormatter";
import Button from "./Button";
import FormatDate from "../helpers/formatDate";

function FinancialTransaction(props: FinancialTransactionProps) {
  const { transaction, baseCurrency, handleBack } = props;

  const today = new Date().getDate();
  const transactionDay = new Date(transaction.transactionDate!).getDate();

  const [updateState, setUpdateState] = useState<UpdateStateType>({
    description: "",
    isEssential: transaction.isEssential!,
  });

  const handleUpdate = async () => {
    handleBack();
    if (
      updateState.description.length > 0 &&
      updateState.description !== transaction.description
    ) {
      const requestBody = {
        description: updateState.description,
      };
      return await financialTrascationService.updateAsync(
        transaction.id,
        requestBody,
      );
    }
    if (updateState.isEssential !== transaction.isEssential) {
      const requestBody = {
        isEssential: updateState.isEssential,
      };
      return await financialTrascationService.updateAsync(
        transaction.id,
        requestBody,
      );
    }
    if (
      updateState.description.length > 0 &&
      updateState.description !== transaction.description &&
      updateState.isEssential !== transaction.isEssential
    ) {
      const requestBody = {
        description: updateState.description,
        isEssential: updateState.isEssential,
      };

      return await financialTrascationService.updateAsync(
        transaction.id,
        requestBody,
      );
    }
  };

  const handleDelete = async () => {
    handleBack();
    await financialTrascationService.deleteAsync(transaction.id);
  };
  //TODO Add Button
  return (
    <div className="absolute top-5 lg:left-1/2 lg:-translate-x-1/2 z-20 w-full lg:w-10/12 p-10 bg-bg-secondary rounded-sm ">
      <div className="flex justify-between items-center w-full my-2">
        <button
          onClick={handleBack}
          className=" flex justify-center items-center h-10 aspect-square mx-2 rounded-full hover:bg-bg-muted"
        >
          <ArrowLeft className="stroke-gray-500 cursor-pointer" />
        </button>
        <div className="hidden w-1/4 md:flex">
          {transactionDay === today && (
            <div className="w-1/2 p-1">
              <Button
                label="Delete"
                type="button"
                background="bg-btn-danger"
                hoverBg="hover:bg-btn-danger-hover"
                textColor="text-btn-danger-text"
                handleClick={handleDelete}
              />
            </div>
          )}
          <div className="w-1/2 p-1">
            <Button
              label="Update"
              type="button"
              background="bg-btn-primary"
              hoverBg="hover:bg-btn-primary-hover"
              textColor="text-btn-primary-text"
              handleClick={handleUpdate}
            />
          </div>
        </div>
      </div>
      <div className="md:flex w-full bg-bg-surface rounded-md shadow-card">
        <div className="flex flex-col justify-center items-center py-3 md:w-5/12 rounded-t-md md:rounded-t-none md:rounded-l-md bg-bg-muted">
          <div className="flex justify-center w-15 m-3">
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
          <div className="font-bold px-2">{transaction.type}</div>
          <div className="flex py-5 items-end">
            <h2 className="text-4xl font-bold">
              {FormatAmount(transaction.baseAmount)}
            </h2>
            <h2 className="pl-2">{baseCurrency}</h2>
          </div>
          <div className="flex flex-col items-center">
            <input
              type="text"
              placeholder={transaction.description}
              onChange={(e) =>
                setUpdateState({ ...updateState, description: e.target.value })
              }
              className="text-center placeholder:text-text-primary"
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
                <h2 className="text-sm text-text-secondary font-semibold">
                  Category
                </h2>
                <h2 className="font-bold">{transaction.categoryName}</h2>
              </div>
            </div>
            <div className="flex items-center w-full md:w-40 md:shrink-0 my-2 md:my-0">
              <div className="flex h-full aspect-square justify-center items-center">
                <Calendar />
              </div>
              <div className="ml-3">
                <h2 className="text-sm text-text-secondary font-semibold">
                  Date
                </h2>
                <h2 className="font-bold">
                  {transaction.transactionDate &&
                    FormatDate(transaction.transactionDate)}
                </h2>
              </div>
            </div>
          </div>
          <div className="md:flex md:justify-between w-full md:my-5">
            <div className="flex items-center w-full md:w-40 md:shrink-0 my-2 md:my-0">
              <div className="flex h-full aspect-square justify-center items-center">
                <Wallet />
              </div>
              <div className="ml-3">
                <h2 className="text-sm text-text-secondary font-semibold">
                  Amount
                </h2>
                <div className="flex items-baseline">
                  <h2 className="font-bold text-lg mr-1 ">
                    {FormatAmount(transaction.amount)}
                  </h2>
                  <span className="text-xs font-light text-text-secondary">
                    {transaction.currency}
                  </span>
                </div>
              </div>
            </div>
            <div className="flex items-center w-full md:w-40 md:shrink-0 my-2 md:my-0">
              <div className="flex h-full aspect-square justify-center items-center">
                <Sigma />
              </div>
              <div className="ml-3">
                <h2 className="text-sm text-text-secondary font-semibold">
                  Exchange_Rate
                </h2>
                <h2 className="font-bold">{transaction.exchangeRate}</h2>
              </div>
            </div>
          </div>
          <div className="md:flex md:justify-between w-full md:my-5">
            <div className="flex items-center w-full md:w-40 md:shrink-0 my-2 md:my-0">
              <div className="flex h-full aspect-square justify-center items-center">
                <ShieldCheck />
              </div>
              <div className="ml-3">
                <h2 className="text-sm text-text-secondary font-semibold">
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
          <div className="w-full mt-10 border-t border-t-border-subtle"></div>
          <div className="md:hidden">
            <Button
              label="Update"
              type="submit"
              background="bg-btn-primary"
              hoverBg="hover:bg-btn-primary-hover"
              textColor="text-btn-primary-text"
              disabled={false}
            />
          </div>
          <div className="py-1 md:hidden">
            {transactionDay === today && (
              <Button
                label="Delete"
                type="button"
                background="bg-btn-danger"
                hoverBg="hover:bg-btn-danger-hover"
                textColor="text-btn-danger-text"
                disabled={false}
              />
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

export default FinancialTransaction;
