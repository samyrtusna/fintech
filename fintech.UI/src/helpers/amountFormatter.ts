const FormatAmount = (amount: number) => {
  const amountFormatter = new Intl.NumberFormat("en-US", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });
  return amountFormatter.format(amount);
};

export default FormatAmount;
