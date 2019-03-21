import { Component } from '@angular/core';
import {LoginService} from "../../services/login.service";
import {Subscription} from 'rxjs';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent{
    private logged: Subscription;
    email: string| null = null;
    loggedIn = false;
    constructor(private loginService: LoginService) {
    }

    logout() {
        this.loginService.logOut();
    }

    ngOnInit() {
        this.email = this.loginService.email;
        this.loggedIn = this.loginService.token != null;

        this.logged = this.loginService.logged.subscribe((result: boolean) => {
            //Will be value or null
            this.email = this.loginService.email;
            this.loggedIn = result;
        });
    }

    ngOnDestroy() {
        if(this.logged) {
            this.logged.unsubscribe();
        }
    }
}
