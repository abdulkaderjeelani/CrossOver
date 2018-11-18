import {
    TestBed,
    getTestBed,
    async,
    inject
} from '@angular/core/testing';
import {
    Headers, BaseRequestOptions,
    Response, HttpModule, Http, XHRBackend, RequestMethod
} from '@angular/http';

import { ResponseOptions } from '@angular/http';
import { MockBackend, MockConnection } from '@angular/http/testing';
import { AuthenticationService } from './authentication.service';
import { fakeBackendProvider } from '../_helpers/index';

describe('AuthenticationService', () => {

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            providers: [
                AuthenticationService,
                fakeBackendProvider,
                MockBackend,
                BaseRequestOptions
            ],
            imports: [
                HttpModule
            ]
        });

    }));

    it('should issue token on proper credentitals',
        async(inject([AuthenticationService], (authenticationService: AuthenticationService) => {
            console.log('testing authentication service - login');
            authenticationService.login('abdul', 'abdul').subscribe(
                rsp => {
                    console.log(rsp);
                    expect(rsp.access_token).toBe('fake-jwt-token');
                },
                error => {
                   throw (error);
                });

        })));

});
