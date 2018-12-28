import {Component, Input, OnInit} from '@angular/core';
import {AnyService} from "../../../services/any.service";
import {ColumnItem} from "../../../models/common/column-item";

@Component({
  selector: 'app-editable-table',
  templateUrl: './editable-table.component.html',
  styleUrls: ['./editable-table.component.css']
})
export class EditableTableComponent implements OnInit {

  @Input() fetchUrl: string
  @Input() editUrlPattern: string
  @Input() newUrl: string
  @Input() deleteUrlPattern: string
  @Input() columns: ColumnItem[]
  constructor(private service: AnyService) {
  }

  private data : any[] = [];

  private displayData: any[] = [];

  ngOnInit() {
    this.loadData()
  }
  private loadData() {
    this.service.getAny(this.fetchUrl).subscribe(result => {
      this.data = result;

      for(let i = 0;i<this.data.length; i++) {
        let dataItem = this.data[i];
        let displayItem: any = {};

        //fill original data item properties matching columns
        for(let c = 0;c < this.columns.length;c++) {
          let column = this.columns[c];
          if(dataItem.hasOwnProperty(column.name)) {
            displayItem[column.name] = dataItem[column.name];
          }
        }

        displayItem['editLink'] = this.editUrlPattern + dataItem['id'];
        displayItem['deleteLink'] = this.deleteUrlPattern + dataItem['id'];
        displayItem['id'] = dataItem['id'];

        this.displayData.push(displayItem);
      }
    });
  }

  private deleteClick(id: number) {
    this.service.deleteItem(this.deleteUrlPattern + id).subscribe(
        result => {
          this.loadData();
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

}
