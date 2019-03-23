import { Injectable } from '@angular/core';
import 'rxjs/add/operator/map';
import {Make} from "../models/make";
import {HttpClient} from '@angular/common/http';

@Injectable()
export class MakeService {

    constructor(private http: HttpClient) { }

    getMake(id: number) {
        return this.http.get('api/makes/'+id)
    }

    createMake(make: Make) {
        return this.http.post('api/makes', make)
    }

    updateMake(make: Make, id: number) {
        return this.http.put('api/makes/'+id, make)
    }
}
