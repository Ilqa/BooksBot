import { InjectionToken } from '@angular/core';
import { environment } from 'src/environments/environment';


export let APP_SETTINGS = new InjectionToken<IAppSettings>('app.settings');

export interface IAppSettings {
   
    apiBaseUrl: string;
    apiEndpoint: string;
    baseUrl: string;
    // notificationPlacement: 'bottomLeft' | 'topLeft' | 'bottomRight' | 'topRight';
    // requestTimeoutValue: number;
}

export const AppSettings: IAppSettings = {
   
    apiBaseUrl: environment.apiBaseUrl,
    apiEndpoint: environment.apiEndpoint,
    baseUrl: environment.baseUrl
    // notificationPlacement: 'bottomLeft',
    // requestTimeoutValue: 2 * 60 * 1000
};
