import { jwtDecode } from "jwt-decode";
import type { DecodedTokenType, JwtPayload } from "../types/authTypes";

export const DecodeToken = (token: string): DecodedTokenType => {
  const decoded = jwtDecode<JwtPayload>(token);
  const tokenClaims: DecodedTokenType = {
    userName:
      decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"],
    emailAddress:
      decoded[
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
      ],
    nameIdentifier:
      decoded[
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
      ],
    role: decoded[
      "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    ],
    baseCurrency:
      decoded[
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata"
      ][0],
    createdAt:
      decoded[
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata"
      ][1],
    expire: decoded.exp ? decoded.exp : undefined,
  };
  return tokenClaims;
};
