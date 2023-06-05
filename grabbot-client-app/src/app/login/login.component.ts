import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { take } from 'rxjs';
import { LocalStorageService } from '../services/local-storage.service';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  
  public loginValid = true;
  public email = '';
  public password = '';
  public isLoading: boolean = false;
  constructor( private userService: UserService, private localstorageService: LocalStorageService,private route: Router) { }
// public notificationService: NzNotificationService,
  ngOnInit(): void {
    console.log("error in ligin page")
  }
 

  public onSubmit(): void {
    if (this.isLoading) {
        return;
    }

    this.isLoading = true;
       console.log("somethinggg")

        this.userService.login(
            this.email,
            this.password
        ).pipe(take(1))
            .subscribe(
                (resp): void => {
                    if (resp.isLoginSuccessful) {
                        this.localstorageService.saveToken(resp.token);
                        this.route.navigateByUrl('/booksprice');
                    } else {
                      //this.notificationService.error('Oops!', resp.Message);
                      alert(resp.message);
                        this.isLoading = false;
                    }
                 }
                //,
                // (error: ServerException) => {
                  
                //     this.notificationService.error('Oops!', 'Server Error');
                //     this.isLoading = false;
                // }
            );
    }
}
