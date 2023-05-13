export class Token {
  accessToken: string;
  refreshToken: string;
  refreshTokenExpiration: string;

  constructor(accessToken: string, refreshToken: string, refreshTokenExpiration: string) {
    this.accessToken = accessToken;
    this.refreshToken = refreshToken;
    this.refreshTokenExpiration = refreshTokenExpiration;
  }
}
