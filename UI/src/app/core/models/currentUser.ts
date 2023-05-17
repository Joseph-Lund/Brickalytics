export class CurrentUser {
  id: number;
  creatorName: string;
  email: string;
  isAdmin: boolean;
  accessToken: string;
  refreshToken: string;
  refreshTokenExpiration: string;

  constructor(id: number, creatorName: string, email: string, isAdmin: boolean, accessToken: string, refreshToken: string, refreshTokenExpiration: string) {
    this.id = id;
    this.creatorName = creatorName;
    this.email = email;
    this.isAdmin = isAdmin;
    this.accessToken = accessToken;
    this.refreshToken = refreshToken;
    this.refreshTokenExpiration = refreshTokenExpiration;
  }
}
