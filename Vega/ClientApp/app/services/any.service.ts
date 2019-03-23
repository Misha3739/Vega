import { Injectable } from '@angular/core';
import 'rxjs/add/operator/map';
import {FetchedData} from "../models/common/fetched-data";
import {Subject} from "rxjs";
import {HttpClient} from '@angular/common/http';

@Injectable()
export class AnyService {
    static instance:AnyService;

    constructor(private http: HttpClient) {
        return AnyService.instance = AnyService.instance || this;
    }

    private fetchedData:FetchedData[] = [];

    dataFetched = new Subject();

  getAny(url: string, key: string) {
    return this.http.get(url)
        .subscribe((result: any) => {
            let fetched = this.fetchedData.find(f => f.key == key) as FetchedData;
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
              .subscribe((result: any) => {
                  fetched!.data = result;
                  this.dataFetched.next(key);
              });
      }
  }
}
