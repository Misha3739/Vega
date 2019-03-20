import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Params, Router} from "@angular/router";
import {Subscription} from "rxjs";
import {Make} from "../../../models/make";
import {FormArray, FormControl, FormGroup, Validators} from "@angular/forms";
import {AnyService} from "../../../services/any.service";
import {MakeService} from "../../../services/make.service";
import {stringify} from "@angular/core/src/util";

@Component({
  selector: 'app-edit-make',
  templateUrl: './edit-make.component.html',
  styleUrls: ['./edit-make.component.css']
})

export class EditMakeComponent implements OnInit, OnDestroy {
  private  id: number;
  private editMode = false;

  constructor(private route: ActivatedRoute,
              private router: Router,
              private anyService: AnyService,
              private makeService: MakeService) { }

  subscription: Subscription;
  fetchedSubscription: Subscription;
  saveSubscription: Subscription;

  make: Make;
  makeForm: FormGroup;

  ngOnInit() {
    this.subscription =  this.route.params.subscribe(
        (params: Params) => {
          this.editMode = params['id'] != null && params['id'] != 'new' ? true : false;
          this.id = this.editMode ? params['id'] : 0;
          this.loadData();
        });

        this.fetchedSubscription = this.anyService.dataFetched.subscribe((fetched: string) => {
            if(fetched == 'makes') {
                this.loadData();
            }});
  }

  private loadData() {
      let models = new FormArray([]);
      if(this.editMode) {
        this.make = this.anyService.getFetchedItem('makes', this.id);
          console.log(this.make);
          if(this.make) {
              if (this.make.models) {
                  for (let model of this.make.models) {
                      models.push(new FormGroup({
                          'name': new FormControl(model.name, [Validators.required]),
                          'description': new FormControl(model.description)
                      }));
                  }
              }
          }
      }

      if(!this.make) {
        this.make = new Make(0,'',[]);
      }
      this.makeForm = new FormGroup({
          'name': new FormControl(this.make.name, [Validators.required]),
          'description': new FormControl(this.make.description),
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
      this.router.navigate(['/makes/edit']);
  }

  getFormControls(controlGroup: string) {
        return   (<FormArray>this.makeForm.get(controlGroup)).controls;
  }

  onSubmit() {
      let makeToSave = this.makeForm.value;
      if(this.editMode) {
          this.saveSubscription = this.makeService.updateMake(makeToSave, this.id).
              subscribe(result => {
              this.redirectToParentComponent(result);
          });
      }
     else {
          this.saveSubscription = this.makeService.createMake(makeToSave).
            subscribe(result => {
              this.redirectToParentComponent(result);
          });
      }
  }

  redirectToParentComponent(result: any){
      console.log(result);
      this.anyService.reloadData('makes');
      this.router.navigate(['/makes/edit']);
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
    if(this.fetchedSubscription) {
        this.fetchedSubscription.unsubscribe();
    }
    if(this.saveSubscription) {
        this.saveSubscription.unsubscribe()
    }
  }
}
