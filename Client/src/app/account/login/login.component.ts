import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AccountService } from '../account.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent  {
  // implements OnInit
  // loginForm: FormGroup;
  // returnUrl: string;
  constructor(private accountService: AccountService, private router: Router, private activatedRoute: ActivatedRoute){
    this.returnUrl = this.activatedRoute.snapshot.queryParams['returnUrl'] || '/shop';
  }
  // constructor(private accountService: AccountService, private router: Router, private activatedRoute: ActivatedRoute) { }
  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators
      .pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$'), Validators.email]),
    password: new FormControl('', Validators.required)
  });
  returnUrl: string = '';
  // ngOnInit() {
  //   this.returnUrl = this.activatedRoute.snapshot.queryParams.returnUrl || '/shop';
  //   this.createLoginForm();
  // }

  // createLoginForm() {
    
  // }

  onSubmit() {
    this.accountService.login(this.loginForm.value).subscribe({
      next: () => this.router.navigateByUrl(this.returnUrl)
    });
  }

}
