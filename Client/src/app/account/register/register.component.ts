import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, AsyncValidatorFn, AbstractControl } from '@angular/forms';
import { AccountService } from '../account.service';
import { Router } from '@angular/router';
import { timer, of } from 'rxjs';
import { switchMap, map, debounceTime, take, finalize } from 'rxjs/operators';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  errors!: string[];
  constructor(private fb: FormBuilder, private accountService: AccountService, private router: Router) {
  }

  ngOnInit() {
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group({
      displayName: [null, Validators.required],
      email: [null, [Validators.required, Validators.pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$')], [this.validateEmailNotTaken()]],
      password: [null, [Validators.required, Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[$@$!%*?&])[A-Za-z\\d$@$!%*?&]{8,}')]],
    });
  }

  onSubmit() {
    this.accountService.register(this.registerForm.value).subscribe({
      next: () => this.router.navigateByUrl('/shop'),
      error: error => this.errors = error.errors,
    });
  }

  validateEmailNotTaken(): AsyncValidatorFn {
    return (control : AbstractControl) => {
      return control.valueChanges.pipe(
        debounceTime(1000),
        take(1),
        switchMap(() => {
          return this.accountService.checkEmailExists(control.value).pipe(
            map(res => res ? { emailexists: true } : null),
            finalize(() => control.markAsTouched())
          )
        })
      );
    };
  }
}
