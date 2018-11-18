import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import { AppSettings } from '../appSettings';

@Injectable()
export class AuthenticationService {
    constructor(private http: Http) { }

    login(username: string, password: string) {
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');
        let loginUrl = AppSettings.API_ENDPOINT;
        let loginRequest = JSON.stringify({ Username: username, Password: password });

        return this.http.post(loginUrl, loginRequest, { headers: headers })
            .map((response: Response) => {
                // login successful if there's a jwt token in the response
                let rsp = response.json();
                if (rsp && rsp.access_token) {
                    localStorage.setItem('access_token', rsp.access_token);
                }
                return rsp;
            });
    }

    logout() {
        // remove user from local storage to log user out
        localStorage.removeItem('access_token');
    }
}