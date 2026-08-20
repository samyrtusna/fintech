export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest extends LoginRequest {
  username: string;
}

export interface AuthInitialState {
  accessToken: string | null;
}

export interface JwtPayload {
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress": string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata": [
    string,
    string,
  ];
  exp?: number;
}

export interface DecodedTokenType {
  userName: string;
  emailAddress: string;
  nameIdentifier: string;
  role: string;
  baseCurrency: string;
  createdAt: string;
  expire?: number;
}
