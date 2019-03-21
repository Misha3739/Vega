
import {ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot} from '@angular/router';
import {Injectable} from "@angular/core";
import {LoginService} from "./login.service";
import {Observable} from 'rxjs';

@Injectable()
export class AuthGuardService implements  CanActivate{

    constructor(private  loginService: LoginService){}

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
        return this.loginService.isAuthenticated();
    }


}