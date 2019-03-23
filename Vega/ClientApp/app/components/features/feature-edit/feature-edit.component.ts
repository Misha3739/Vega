import {Component, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Params, Router} from "@angular/router";
import {AnyService} from "../../../services/any.service";
import {FeatureService} from "../../../services/feture.service";
import {Feature} from "../../../models/feature";
import {Subscription} from "rxjs";
import {NgForm} from "@angular/forms";

@Component({
  selector: 'app-feature-edit',
  templateUrl: './feature-edit.component.html',
  styleUrls: ['./feature-edit.component.css']
})
export class FeatureEditComponent implements OnInit {
  @ViewChild('f') form: NgForm;

  feature: Feature;
  subscription: Subscription;
  fetchedSubscription: Subscription;
  saveSubscription: Subscription;
  editMode: boolean;
  id: number;

  constructor(private route: ActivatedRoute,
              private router: Router,
              private anyService: AnyService,
              private featuresService: FeatureService) {

  }

  ngOnInit() {
    this.subscription =  this.route.params.subscribe(
        (params: Params) => {
          this.editMode = params['id'] != null && params['id'] != 'new' ? true : false;
          this.id = this.editMode ? params['id'] : 0;
          this.loadData();
        });

    this.fetchedSubscription = this.anyService.dataFetched.subscribe((fetched: string) => {
      if(fetched == 'features') {
        this.loadData();
      }});
  }

  loadData() {
    if(this.editMode) {
      this.feature = this.anyService.getFetchedItem('features', this.id);
    }
    if(!this.feature) {
      this.feature = new Feature(0,'');
    }
  }

  submit() {
    if(this.editMode) {
      this.saveSubscription = this.featuresService.updateFeature(this.feature, this.id).
      subscribe(result => {
        this.redirectToParentComponent(result);
      });
    }
    else {
      this.saveSubscription = this.featuresService.createFeature(this.feature).
      subscribe(result => {
        this.redirectToParentComponent(result);
      });
    }
  }

  onCancel() {
    this.router.navigate(['/makes/edit']);
  }

  redirectToParentComponent(result: any){
    console.log(result);
    this.anyService.reloadData('features');
    this.router.navigate(['/features/edit']);
  }

  onDestroy() {
    if(this.subscription) {
      this.subscription.unsubscribe();
    }
    if(this.fetchedSubscription) {
      this.fetchedSubscription.unsubscribe();
    }
    if(this.saveSubscription) {
      this.saveSubscription.unsubscribe();
    }
  }

}
