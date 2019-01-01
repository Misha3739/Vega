import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';
import {FetchedData} from "../models/common/fetched-data";
import {Subject} from "rxjs";

@Injectable()
export class AnyService {
    static instance:AnyService;

    constructor(private http: Http) {
        return AnyService.instance = AnyService.instance || this;
    }

    private fetchedData:FetchedData[] = [];

    dataFetched = new Subject();

  getAny(url: string, key: string) {
    return this.http.get(url)
        .map(res => res.json())
        .subscribe(result => {
            let fetched = (this.fetchedData.find(f => f.key == key));
            if(fetched) {
                fetched.data = result;
            } else {
                this.fetchedData.push(new FetchedData(key,url, result))
            }
            this.dataFetched.next(key);
        });
  }

  getFetchedItem(key: string, id: number): any {
      let fetched = (this.fetchedData.filter(f => f.key == key))[0];
      if(fetched) {
          return fetched.data.find(d => d.id == id);
      }
      return null;
  }

  hasData(key: string) {
      return this.fetchedData.find(f => f.key == key);
  }

  getData(key: string) {
    let found = this.fetchedData.find(f => f.key == key);
    return found? found.data : null;
}

  deleteItem(url: string) {
    return this.http.delete(url);
  }

  reloadData(key: string) {
      let fetched = (this.fetchedData.find(f => f.key == key));
      if(fetched) {
          this.http.get(fetched.url)
              .map(res => res.json())
              .subscribe(result => {
                  fetched!.data = result;
                  this.dataFetched.next(key);
              });
      }
  }
}
