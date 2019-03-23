import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import 'rxjs/add/operator/map';
import {Feature} from "../models/feature";

@Injectable()
export class FeatureService {

    constructor(private http: HttpClient) { }

    getFeature(id: number) {
        return this.http.get('api/features/'+id);
    }

    createFeature(feature: Feature) {
        return this.http.post('api/features', feature);
    }

    updateFeature(feature: Feature, id: number) {
        return this.http.put('api/features/'+id, feature);
    }
}
