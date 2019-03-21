import {Component, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Params, Router} from "@angular/router";
import {Login} from "../../models/login";
import {LoginService} from "../../services/login.service";
import {Subscription} from 'rxjs';
import {NgForm} from "@angular/forms";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {
  @ViewChild('f') form: NgForm;
  login: Login = new Login('','');
  wrongCredentials = false;
  submitEnabled: Boolean|null = null;

  constructor(private loginService: LoginService, private router: Router) { }

  ngOnInit() {
  }

  submit() {
    this.wrongCredentials = false;
    this.submitEnabled = false;
    this.loginService.login(this.login).then(
        (data: boolean) => {
            if(data) {
                this.router.navigate(['/home']);
            } else  {
                this.wrongCredentials = true;
                this.submitEnabled = true;
            }
        });
  }
}
