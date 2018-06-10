import { Make } from './../../models/make';
import { MakeService } from './../../services/make.service';
import { Component, OnInit } from '@angular/core';
import { Model } from '../../models/model';

@Component({
  selector: 'app-vehicle-form',
  templateUrl: './vehicle-form.component.html',
  styleUrls: ['./vehicle-form.component.css']
})
export class VehicleFormComponent implements OnInit {
  makes : Make[] = [];
  models: Model[] = [];
  vehicle: any = {};

  constructor(private makeService: MakeService) { 
  }

  ngOnInit() {
    this.makeService.getMakes().subscribe(makes => { 
            this.makes = makes;
      });
  }

  onMakeChange() {
    var selectedMake = this.makes.find(m => m.id == this.vehicle.makeId);
    this.models = selectedMake ? selectedMake.models : [];
  }

}
