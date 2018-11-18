import { Http, BaseRequestOptions, Response, ResponseOptions, RequestMethod } from '@angular/http';
import { MockBackend, MockConnection } from '@angular/http/testing';

export let fakeBackendProvider = {
    // use fake backend in place of Http service for backend-less development
    provide: Http,
    useFactory: (backend: MockBackend, options: BaseRequestOptions) => {
        console.log('linking fakeBackendProvider');
        // array in local storage for registered users
        let users: any[] = JSON.parse(localStorage.getItem('users')) || [
            { id: 1, username: 'abdul', password: 'abdul' },
            { id: 1, username: 'crossover', password: 'crossover' }
        ];

        // configure fake backend
        backend.connections.subscribe((connection: MockConnection) => {

            // wrap in timeout to simulate server api call
            setTimeout(() => {

                // authenticate
                if (connection.request.url.endsWith('/api/authentication/login') && connection.request.method === RequestMethod.Post) {

                    // get parameters from post request
                    let params = JSON.parse(connection.request.getBody());

                    // find if any user matches login credentials
                    let filteredUsers = users.filter(user => {
                        return user.username === params.Username && user.password === params.Password;
                    });

                    if (filteredUsers.length) {
                        console.log('login succesful');
                        // if login details are valid return 200 OK with user details and fake jwt token
                        let user = filteredUsers[0];
                        connection.mockRespond(new Response(new ResponseOptions({
                            status: 200,
                            body: {
                                name: user.username,
                                access_token: 'fake-jwt-token'
                            }
                        })));
                    } else {
                        // else return 400 bad request
                        connection.mockError(new Error('Username or password is incorrect'));
                    }
                }
            }, 500);

        });

        return new Http(backend, options);
    },
    deps: [MockBackend, BaseRequestOptions]
};
