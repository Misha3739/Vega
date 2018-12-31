import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Params, Router} from "@angular/router";
import {Subscription} from "rxjs";
import {Make} from "../../../models/make";
import {FormArray, FormControl, FormGroup, Validators} from "@angular/forms";
import {AnyService} from "../../../services/any.service";

@Component({
  selector: 'app-edit-make',
  templateUrl: './edit-make.component.html',
  styleUrls: ['./edit-make.component.css']
})
export class EditMakeComponent implements OnInit, OnDestroy {
  private  id: number;
  private editMode = false;

  constructor(private route: ActivatedRoute, private anyService: AnyService) { }

  subscription: Subscription;

  make: Make;
  makeForm: FormGroup;

  ngOnInit() {
    this.subscription =  this.route.params.subscribe(
        (params: Params) => {
          this.editMode = params['id'] != null && params['id'] != 'new' ? true : false;
          this.id = this.editMode ? params['id'] : 0;
            console.log(this.anyService.fetchedData);
          this.initForm();
        });
  }

  private initForm() {
      let models = new FormArray([]);
      if(this.editMode) {
        this.make = this.anyService.getFetchedItem('makes', this.id);
          console.log(this.make);
          if(this.make) {
              if (this.make.models) {
                  for (let model of this.make.models) {
                      models.push(new FormGroup({
                          'name': new FormControl(model.name, [Validators.required])
                      }));
                  }
              }
          }
        } else {
          this.make = new Make(0,'',[]);
      }
      this.makeForm = new FormGroup({
          'name': new FormControl(this.make.name, [Validators.required]),
          'models': models
      });
  }


  onAddModel() {
      (<FormArray>this.makeForm.get('models')).push(
          new FormGroup({
              'name': new FormControl(null, [Validators.required]),
          }));
  }

  onDeleteModel(id: number) {
        (<FormArray>this.makeForm.get('models')).removeAt(id);
  }

  onCancel() {

  }

  getFormControls(controlGroup: string) {
        return   (<FormArray>this.makeForm.get(controlGroup)).controls;
  }

  submit() {

  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
