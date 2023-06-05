import { Injectable } from '@angular/core';

@Injectable({
	providedIn: 'root'
})
export class LocalStorageService {

	constructor() {
	}

	saveToken(token: string): void {
		localStorage.setItem('X-AUTH-Token', token);
	}

	getToken(): string | null {
		const token = localStorage.getItem('X-AUTH-Token');
		return token;
	}

	removeToken(): void {
		localStorage.removeItem('X-AUTH-Token');
	}

    saveLocationId(locationId: string): void {
		localStorage.setItem('locationId', locationId);
	}

	getLocationId(): string | null {
		const locationId = localStorage.getItem('locationId');
		return locationId;
	}




}