import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BooksPricesListComponent } from './books-prices-list/books-prices-list.component';
import { AntiAuthGuard } from './guards/anti-auth.guard';
import { AuthGuard } from './guards/auth.guard';
import { LoginComponent } from './login/login.component';

const routes: Routes = [
  { path: '', component: LoginComponent,  canActivate: [AntiAuthGuard] },
  { path: 'booksprice', component: BooksPricesListComponent ,  canActivate: [AuthGuard]}
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
