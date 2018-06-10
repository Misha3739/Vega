import { Feature } from './../../models/feature';
import { Make } from './../../models/make';
import { VehicleService } from './../../services/vehicle.service';
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
  features: Feature[] = [];

  constructor(private vehicleService: VehicleService) { 
  }

  ngOnInit() {
    this.vehicleService.getMakes().subscribe(makes => { 
            this.makes = makes;
      });

      this.vehicleService.getFeatures().subscribe(features => {
        this.features = features;
      });
  }

  onMakeChange() {
    var selectedMake = this.makes.find(m => m.id == this.vehicle.makeId);
    this.models = selectedMake ? selectedMake.models : [];
  }

}
