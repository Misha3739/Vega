import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {AnyService} from "../../../services/any.service";
import {ColumnItem} from "../../../models/common/column-item";
import {Subscription} from "rxjs";

@Component({
  selector: 'app-editable-table',
  templateUrl: './editable-table.component.html',
  styleUrls: ['./editable-table.component.css']
})
export class EditableTableComponent implements OnInit, OnDestroy {

  @Input() sourceName: string;
  @Input() fetchUrl: string
  @Input() editUrlPattern: string
  @Input() newUrl: string
  @Input() newLinkLabel:string
  @Input() deleteUrlPattern: string
  @Input() columns: ColumnItem[]
  constructor(private service: AnyService) {
  }

  private data : any[] = [];

  private displayData: any[] = [];

  loadDataSubscription: Subscription;
  deleteSubscription: Subscription;

  ngOnInit() {
    this.loadData()
  }
  private loadData() {
    this.loadDataSubscription = this.service.dataFetched.subscribe((fetched: string) => {
      if(fetched === this.sourceName) {
        this.data = (this.service.fetchedData.filter(f => f.key == this.sourceName))[0].data;
        for(let i = 0;i<this.data.length; i++) {
          let dataItem = this.data[i];
          let displayItem: any = {};

          //fill original data item properties matching columns
          for (let c = 0; c < this.columns.length; c++) {
            let column = this.columns[c];
            if (dataItem.hasOwnProperty(column.name)) {
              displayItem[column.name] = dataItem[column.name];
            }
          }

          displayItem['editLink'] = this.editUrlPattern + dataItem['id'];
          displayItem['deleteLink'] = this.deleteUrlPattern + dataItem['id'];
          displayItem['id'] = dataItem['id'];

          this.displayData.push(displayItem);
        }
      }
    });
    this.loadDataSubscription = this.service.getAny(this.fetchUrl,this.sourceName);
  }

  private deleteClick(id: number) {
    this.deleteSubscription = this.service.deleteItem(this.deleteUrlPattern + id).subscribe(
        result => {
          console.log(result);
          this.data = [];
          this.displayData = [];
          this.loadData();
        },
        (err) => {
          alert("Error on deleting item with id" + id +" : "+err);
        }
    );
  }

  private getProperties(obj: any){
    let keys = [];
    for(let key in obj){
      if(key != 'id')
        keys.push(key);
    }
    return keys;
  }

  ngOnDestroy() {
    this.loadDataSubscription.unsubscribe();
    if(this.deleteSubscription) {
      this.deleteSubscription.unsubscribe();
    }
  }

}
