import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Params, Router} from "@angular/router";
import {Subscription} from "rxjs";
import {Make} from "../../../models/make";
import {FormArray, FormControl, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'app-edit-make',
  templateUrl: './edit-make.component.html',
  styleUrls: ['./edit-make.component.css']
})
export class EditMakeComponent implements OnInit, OnDestroy {
  private  id?: number;
  private editMode = false;

  constructor(private route: ActivatedRoute) { }

  subscription: Subscription;

  make: Make = new Make(0,'',[]);
  makeForm: FormGroup;

  ngOnInit() {
    this.subscription =  this.route.params.subscribe(
        (params: Params) => {
          this.editMode = params['id'] != null ? true : false;
          this.id = params['id'];

          this.initForm();
        });
  }

  private initForm() {
      let models = new FormArray([]);
      this.makeForm = new FormGroup({
          'name': new FormControl('name', [Validators.required]),
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
