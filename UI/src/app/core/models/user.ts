export class User {
  id: number;
  creatorName?: string;
  email?: string;
  active: boolean;
  roleId: number;
  collectionId: number;

  constructor(
    id: number,
    active: boolean,
    roleId: number,
    collectionId: number,
    creatorName?: string,
    email?: string
  ) {
    this.id = id;
    this.active = active;
    this.roleId = roleId;
    this.collectionId = collectionId;
    this.creatorName = creatorName;
    this.email = email;
  }
}
