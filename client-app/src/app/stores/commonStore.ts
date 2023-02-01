import { makeAutoObservable, reaction } from "mobx";
import { ServerError } from "../models/serverError";

export default class CommonStore {
  error: ServerError | null = null;
  token: string | null = localStorage.getItem('jwt');
  appLoaded = false;

  constructor() {
    makeAutoObservable(this);

    // there are two types of reactions in MobX. 
    // This one is only going to run what an observable changes. It doesn't run when it's initially being set. So in class CommonStore, when token is initially set, there is nothing happening to the methods in this reaction. When this abservable(token) changes, so a user logs in and we set the token, this reaction is going to set this inside local storage; a user logs out, the token is updated and this reaction is going to run and remove the token from local storage.
    // A different reaction is call AutoRun that would actually as soon as our store is initialized, it would immediately execute what's inside the reaction.
    reaction(
      () => this.token,
      token => {
        if (token) {
          localStorage.setItem('jwt', token)
        } else {
          localStorage.removeItem('jwt')
        }
      }
    )
  }

  setServerError(error: ServerError) {
    this.error = error
  }

  setToken = (token: string | string | null) => {
      this.token = token;
 }

  setAppLoaded = () => {
      this.appLoaded = true;
  }
}