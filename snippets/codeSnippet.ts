// type MethodType = "get" | "post" | "put" | "delete";

// const request = async <TResponse,TBody ,TParams,>(method:MethodType, route:string, body?:TBody, params?:TParams) =>
// {
//   const response = await AxiosService
//         .request<TResponse>({method, url:route, data:body, params});

//   return response.data;
// };

// RuleFor((x) => x.BaseCurrency)
//   .NotEmpty()
//   .WithMessage("Base currency is required.")
//   .Length(3)
//   .WithMessage("Base currency must be a 3-letter code.")
//   .Must((c) => c.Equals(c, StringComparison.CurrentCultureIgnoreCase))
//   .WithMessage("Base currency must be uppercase.")
//   .Matches("^[A-Z]{3}$")
//   .WithMessage("Base currency must be a valid ISO code (e.g., USD, EUR)");
