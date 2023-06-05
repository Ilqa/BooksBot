import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { APP_SETTINGS, IAppSettings } from '../app-settings';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClient: HttpClient,@Inject(APP_SETTINGS) protected config: IAppSettings ) {}

 
login(email: string, password: string): Observable<any> {
  console.log("reached service")
  const url = this.config.apiEndpoint + "identity/token";
  return this.httpClient.post<any>(url, {email, password});
}


}
