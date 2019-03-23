import { Injectable, Inject,PLATFORM_ID } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';
import {Login} from "../models/login";
import {Subject} from 'rxjs';
import {isPlatformBrowser} from '@angular/common';

@Injectable()
export class LoginService {
    static instance:LoginService;

    private _token: string| null = null;
    private _email: string| null = null;

    logged = new Subject<boolean>();

    constructor(private http: Http, @Inject(PLATFORM_ID) private platformId: Object) {
    }

    login(login: Login) : Promise<boolean> {
        return this.http.post('api/account/login', login).toPromise().
            then((response) => {
                console.log(response);
                this.setTokenAndEmail((<any>response)._body,login.email);
                return true;
            })
            .catch(err => {
                console.log(err);
                this.setTokenAndEmail(null, null);
                return false;
            });
    }

    private isBrowser() : boolean {
        return isPlatformBrowser(this.platformId);
    }

    get token() : string|null {
        if(!this._token) {
            if(this.isBrowser()) {
                let obj = localStorage.getItem('currentUser');
                if (obj) {
                    let parsed = JSON.parse(obj);
                    this._token = parsed.token;
                }
            }
        }
        return this._token;
    }

    get email() : string|null {
        if(!this._email) {
            if(this.isBrowser()) {
                let obj = localStorage.getItem('currentUser');
                if (obj) {
                    let parsed = JSON.parse(obj);
                    this._email = parsed.email;
                }
            }
        }
        return this._email;
    }

    private setTokenAndEmail(tokenParam: string | null, emailParam: string | null) {
        if(!tokenParam || !emailParam) {
            if(this.isBrowser()) {
                localStorage.removeItem('currentUser');
            }
            this._email = null;
            this._token = null;
            this.logged.next(false);
        } else {
            if(this.isBrowser()) {
                localStorage.setItem('currentUser', JSON.stringify({token: tokenParam, email: emailParam}));
            }
            this._email = emailParam;
            this._token = tokenParam;
            this.logged.next(true);
        }

    }

    logOut() {
        this.setTokenAndEmail(null, null);
    }

    isAuthenticated() {
        return this.token != null && this.token != '';
    }
}
