import { Routes } from '@angular/router';
import { StudentsListComponent } from './features/students-list/students-list.component';
import { LoginComponent } from './features/login/login.component';
import { RegisterComponent } from './features/register/register.component';

export const routes: Routes = [
    { path: 'students-list', component: StudentsListComponent },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent }
];
