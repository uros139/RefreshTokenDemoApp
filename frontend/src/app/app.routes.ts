import { Routes } from '@angular/router';
import { StudentsListComponent } from './features/students-list/students-list.component';
import { LoginComponent } from './features/login/login.component';

export const routes: Routes = [
    { path: 'students-list', component: StudentsListComponent },
    { path: 'login', component: LoginComponent }
];
