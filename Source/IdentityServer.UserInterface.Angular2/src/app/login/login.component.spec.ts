import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { LoginComponent } from './login.component';
import { fakeBackendProvider } from '../_helpers/index';
import { MockBackend, MockConnection } from '@angular/http/testing';
import { BaseRequestOptions } from '@angular/http';

import { AuthGuard } from '../_guards/index';
import { AlertService, AuthenticationService } from '../_services/index';
import { AppSettings } from '../appSettings';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { routing } from '../app.routing';
import { HomeComponent } from '../home/index';
import {APP_BASE_HREF} from '@angular/common';

describe('LoginComponent', () => {

    let comp: LoginComponent;
    let fixture: ComponentFixture<LoginComponent>;
    let de: DebugElement;
    let el: HTMLElement;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [
                BrowserModule,
                FormsModule,
                HttpModule,
                routing
            ],
            declarations: [LoginComponent, HomeComponent], // declare the test component
            providers: [
                AppSettings,
                AuthGuard,
                AlertService,
                AuthenticationService,
                fakeBackendProvider,
                MockBackend,
                BaseRequestOptions,
                {provide: APP_BASE_HREF, useValue : '/' }
            ],
        });

        fixture = TestBed.createComponent(LoginComponent);

        comp = fixture.componentInstance; 

        // de = fixture.debugElement.query(By.css('button'));
        // el = de.nativeElement;        
    });

    it('should not be any error during login ', () => {
        fixture.detectChanges();
        console.log('testing login component ');
        comp.model.username = 'abdul';
        comp.model.password = 'abdul';
        comp.login();
        expect(true).toBe(true);
    });


});
