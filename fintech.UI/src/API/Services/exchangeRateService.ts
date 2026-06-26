import axios from "axios";

const exchangeRateKey: string = import.meta.env.VITE_EXCHANGE_RATE_KEY;
const exchangeRateUrl: string = import.meta.env.VITE_EXCHANGE_RATE_URL;

const getCurrencies = async () => {
  const response = await axios.get(`${exchangeRateUrl}/symbols`, {
    params: { access_key: exchangeRateKey },
  });
  return response.data;
};

export default { getCurrencies };
