import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { LocalStorageService } from '../services/local-storage.service';
import { UserService } from '../services/user.service';

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate {
    constructor(public localStorageService: LocalStorageService, public router: Router) { }

    canActivate(): boolean {
        if (this.localStorageService.getToken() == null) {
            this.router.navigateByUrl('/');
            return false;
        }
        return true;
    }
}