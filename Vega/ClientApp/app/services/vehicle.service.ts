import { Injectable } from '@angular/core';
import 'rxjs/add/operator/map';
import {HttpClient} from '@angular/common/http';
import {Feature} from "../models/feature";
import {Make} from '../models/make';

@Injectable()
export class VehicleService {

  constructor(private http: HttpClient) {
  }

  getFeatures(){
    return this.http.get<Array<Feature>>('/api/features');
  }

  getMakes() {
    return this.http.get<Array<Make>>('/api/makes');
  }

  create(vehicle: any) {
    return this.http.post<any>('/api/vehicles', vehicle);
  }
}
