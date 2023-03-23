import { GoogleLoginProvider, SocialAuthService, SocialUser } from "@abacritt/angularx-social-login";
import { Component, OnInit } from "@angular/core";
import { UntypedFormBuilder, UntypedFormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { AuthenticationService } from "src/app/service/authentication.service";
import { UserService } from "../../user.service";
declare function EncryptFieldData(data: any): any;
declare function decrypt(data: any): any;
@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.css"],
})
export class LoginComponent implements OnInit {
  loginForm: UntypedFormGroup = new UntypedFormGroup({});
  public IsLogged:boolean = false;
  constructor(
    private fb: UntypedFormBuilder,
    private toastr: ToastrService,
    private userService: UserService,
    private router: Router,
    private autService: AuthenticationService,
    private oAuthService: SocialAuthService
  ) {
    this.signInWithGoogle();
  }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.loginForm = this.fb.group({
      email: [
        "",
        [
          Validators.required,
          Validators.pattern(
            /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
          ),
        ],
      ],
      password: [
        "",
        [
          Validators.required,
          Validators.pattern(
            /(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^A-Za-z0-9])(?=.{8,})/
          ),
        ],
      ],
    });
  }
  onSubmit() {
    var data = {
      email: this.loginForm.value.email,
      password: this.loginForm.value.password,
    };
    var encData = {
      inputString: EncryptFieldData(JSON.stringify(data)),
    };
    this.userService.login(encData).subscribe(
      (response: any) => {
        if (response) {
          localStorage.setItem("token", response.token);
          localStorage.setItem("userId", response.userId);
          localStorage.setItem("userName", response.userName);
          this.router.navigate(["/chat"]);
        } else {
          this.toastr.error(
            "Incorrect username or password.",
            "Authentication failed."
          );
        }
      },
      (error) => {
        console.log(error);
      }
    );
  }
  get Email() {
    return this.loginForm.get("email");
  }
  get Password() {
    return this.loginForm.get("password");
  }

  signInWithGoogle():void{
    // var googleLoginOptions = {
    //   scope: 'profile email'
    // };
    // this.oAuthService.signIn(GoogleLoginProvider.PROVIDER_ID, googleLoginOptions ).then((googleData:any)=>{
    //   console.log(googleData)
    // })
    this.oAuthService.authState.subscribe((user:SocialUser)=>{
      if(user){
        console.log(user)
        var encData = {
          inputString: EncryptFieldData(JSON.stringify(user)),
        }
        this.userService.oAuthLogin(encData).subscribe((response:any)=>{
          if (response) {
            this.IsLogged = true;
            localStorage.setItem("token", response.token);
            localStorage.setItem("userId", response.userId);
            localStorage.setItem("userName", response.userName);
            this.router.navigate(["/chat"]);
          }
        },
        (err)=>{
          console.log(err)
        })
      }
    })
  }

}
