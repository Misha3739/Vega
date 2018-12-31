import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';

@Injectable()
export class MakeService {

    constructor(private http: Http) { }

    getMake(id: number) {
        return this.http.get('api/makes/'+id)
            .map(res => res.json());
    }
}
