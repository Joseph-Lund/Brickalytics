export class User {
  id: number;
  creatorName: string;
  email: string;
  accessToken: string;
  refreshToken: string;
  refreshTokenExpiration: string;

  constructor(id: number, creatorName: string, email: string, accessToken: string, refreshToken: string, refreshTokenExpiration: string) {
    this.id = id;
    this.creatorName = creatorName;
    this.email = email;
    this.accessToken = accessToken;
    this.refreshToken = refreshToken;
    this.refreshTokenExpiration = refreshTokenExpiration;
  }
}
