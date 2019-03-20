import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';
import {Feature} from "../models/feature";

@Injectable()
export class FeatureService {

    constructor(private http: Http) { }

    getMake(id: number) {
        return this.http.get('api/features/'+id)
            .map(res => res.json());
    }

    createMake(feature: Feature) {
        return this.http.post('api/features', feature)
            .map(res => res.json());
    }

    updateMake(feature: Feature, id: number) {
        return this.http.put('api/features/'+id, feature)
            .map(res => res.json());
    }
}
