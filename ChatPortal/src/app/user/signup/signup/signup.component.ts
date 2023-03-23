import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../../user.service';
declare function EncryptFieldData(data:any): any;
declare function decrypt(data:any): any;
@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {

  signUpForm: UntypedFormGroup = new UntypedFormGroup({});
  constructor(private fb:UntypedFormBuilder, private tostr:ToastrService, private userService:UserService,private router: Router,) { }
  // public barLabel: string = 'Password strength:';
  // public myColors = ['#DD2C00', '#FF6D00', '#FFD600', '#AEEA00', '#00C853'];
  // public thresholds = [90, 75, 45, 25];
  // public strengthLabels = [
  //   '(Bad Choice)',
  //   '(Weak)',
  //   '(Normal)',
  //   '(Strong)',
  //   '(Great!)',
  // ];
  ngOnInit(): void {
    this.initializeForm();
  }
  initializeForm(){
    this.signUpForm = this.fb.group({
      userName: [
        '',
        [
          Validators.required,
          Validators.pattern(/^[A-Za-z0-9][A-Za-z0-9 \.\-_]*[A-Za-z0-9]$/),
        ],
      ],
      email: [
        '',
        [
          Validators.required,
          Validators.pattern(
            /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
          ),
        ],
      ],
      password: [
        '',
        [
          Validators.required,
          Validators.pattern(
            /(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^A-Za-z0-9])(?=.{8,})/
          ),
        ],
      ],
    });
  }
  get UserName() {
    return this.signUpForm.get('userName');
  }
  get Email() {
    return this.signUpForm.get('email');
  }
  get Password() {
    return this.signUpForm.get('password');
  }
  onSubmit(){
    var data = {
      userName: this.signUpForm.value.userName,
      email: this.signUpForm.value.email,
      password:this.signUpForm.value.password
    }
    var encData = EncryptFieldData(JSON.stringify(data))
    const obj = {
      inputString : encData
    }
    this.userService.register(obj).subscribe(
    (response)=>{
        this.tostr.success("Account created successfully.","Success");
        this.signUpForm.reset();
        this.redirect();
    },
    (error)=>{
      this.tostr.error(error.Message,"Error");
      this.signUpForm.reset();
    })
  }
  redirect() {
    this.router.navigate(['/login']);
  }
}
