export class User {
  id: number | null;
  creatorName: string;
  email: string | null;
  active: boolean;
  roleId: number;
  collectionId: number | null;

  constructor(
    id: number | null,
    active: boolean,
    roleId: number,
    collectionId: number | null,
    creatorName: string,
    email: string | null
  ) {
    this.id = id;
    this.active = active;
    this.roleId = roleId;
    this.collectionId = collectionId;
    this.creatorName = creatorName;
    this.email = email;
  }
}
