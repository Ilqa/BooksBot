import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LocalStorageService } from './services/local-storage.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'grab-bots';
  constructor( public localstorageService: LocalStorageService,private route: Router){}
  showLogoutButton = this.localstorageService.getToken() !== null;
  
  Logout(): void
  {
    this.localstorageService.removeToken();
    this.route.navigateByUrl('/');
  }

}
