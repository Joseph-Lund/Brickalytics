export class Result<T = null> {
  code: number;
  message: string;
  data: T | null = null;

  constructor(accessToken: number, message: string, data: T | null = null) {
    this.code = accessToken;
    this.message = message;
    this.data = data;
  }
}
