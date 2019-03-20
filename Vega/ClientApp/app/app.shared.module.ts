import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { CounterComponent } from './components/counter/counter.component';
import { VehicleFormComponent } from './components/vehicle-form/vehicle-form.component'
import { VehicleService } from './services/vehicle.service';
import { EditMakesComponent } from './components/makes/edit-makes/edit-makes.component'
import {EditableTableComponent} from "./components/common/editable-table/editable-table.component";
import {AnyService} from "./services/any.service";
import { EditMakeComponent } from "./components/makes/edit-make/edit-make.component";
import {MakeService} from "./services/make.service";
import {EditFeaturesComponent} from "./components/features/features-edit/edit-features.component";
import {FeatureEditComponent} from "./components/features/feature-edit/feature-edit.component";
import {FeatureService} from "./services/feture.service";

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        CounterComponent,
        FetchDataComponent,
        HomeComponent,
		VehicleFormComponent,
		EditMakesComponent,
        EditMakeComponent,
        EditFeaturesComponent,
        FeatureEditComponent,
        EditableTableComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'vehicles/new', component: VehicleFormComponent },
            { path: 'home', component: HomeComponent },
            { path: 'counter', component: CounterComponent },
			{ path: 'fetch-data', component: FetchDataComponent },
			{ path: 'makes/edit', component: EditMakesComponent, children: [
			    { path: ':id', component: EditMakeComponent },
                { path: 'new', component: EditMakeComponent }
                ] },
            { path: 'features/edit', component: EditFeaturesComponent, children: [
                { path: ':id', component: FeatureEditComponent },
                { path: 'new', component: FeatureEditComponent }
                ] },
			{ path: '**', redirectTo: 'home' },
        ])
    ],
    providers : [
        VehicleService,
        AnyService,
        MakeService,
        FeatureService
    ]
})
export class AppModuleShared {
}
