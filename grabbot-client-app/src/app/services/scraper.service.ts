import { Inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { APP_SETTINGS, IAppSettings } from '../app-settings';
import { LocalStorageService } from './local-storage.service';

@Injectable({
  providedIn: 'root'
})
export class ScraperService {
   headerDict = {
    Authorization: `Bearer ${this.localstorageService.getToken()}`,
  }
   requestOptions = {                                                                                                                                                                                 
    headers: new HttpHeaders(this.headerDict), 
  };

  constructor(@Inject(APP_SETTINGS) protected config: IAppSettings,  private httpClient: HttpClient, private localstorageService: LocalStorageService) { }

  

  getCompleteDownloadableBooksWithPrices(): Observable<any> {
    const url = this.config.apiEndpoint + "BookData/GetCompleteDownloadableBooksWithPricesList"
    return this.httpClient.get<any>(url,this.requestOptions);
  }


  GetDownloadableBooksDataFromLastHour(): Observable<any> {
    const url = this.config.apiEndpoint + "BookData/GetDownloadableBooksDataFromLastHour"
    console.log(url);
    return this.httpClient.get<any>(url,this.requestOptions);
  }

  getBooksWithPrices(pageNumber:number, pageSize:number, searchText:string|null, searchTextChanged:boolean): Observable<any> {
    let url = this.config.apiEndpoint + "BookData/GetBooksPricesFromWebsites?pageNumber=" + pageNumber + "&pageSize=" + pageSize;
    if (searchText) {
        url += "&searchText=" + searchText; }
        
    if (searchTextChanged == true) {
          url += "&searchTextChanged=" + searchTextChanged; 
    }
    console.log(url)
    
    console.log(this.headerDict)
    return this.httpClient.get<any>(url, this.requestOptions);
  }

  getCrawlStatus(): Observable<any> {
    const url = this.config.apiEndpoint + "BookData/GetCrawlStatus"
    console.log(url);
    return this.httpClient.get<any>(url,this.requestOptions);
  }

}
