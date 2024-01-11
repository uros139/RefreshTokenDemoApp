import { Routes } from '@angular/router';
import { StudentsListComponent } from './features/students-list/students-list.component';
import { AppComponent } from './app.component';

export const routes: Routes = [
    { path: 'students-list', component: StudentsListComponent }
];
