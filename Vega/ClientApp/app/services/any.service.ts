import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';

@Injectable()
export class AnyService {

  constructor(private http: Http) { }

  getAny(url: string) {
    return this.http.get(url)
        .map(res => res.json());
  }

  deleteItem(url: string) {
    return this.http.delete(url);
  }
}
