import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';
import {Make} from "../models/make";

@Injectable()
export class MakeService {

    constructor(private http: Http) { }

    getMake(id: number) {
        return this.http.get('api/makes/'+id)
            .map(res => res.json());
    }

    createMake(make: Make) {
        return this.http.put('api/makes', make)
            .map(res => res.json());
    }

    updateMake(make: Make, id: number) {
        return this.http.post('api/makes/'+id, make)
            .map(res => res.json());
    }
}
