import { Component, OnInit } from '@angular/core';

import { User } from '../_models/index';

@Component({
    //moduleId: module.id,
    templateUrl: 'home.component.html'
})

export class HomeComponent implements OnInit {
    access_token: any;
    
    constructor() {
        this.access_token = localStorage.getItem('access_token');
    }

    ngOnInit() {
        
    }
   
}