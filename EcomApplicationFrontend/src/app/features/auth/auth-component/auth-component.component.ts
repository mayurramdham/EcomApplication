import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoaderComponent } from '../utility/loader/loader.component';

@Component({
  selector: 'app-auth-component',
  standalone: true,
  imports: [RouterOutlet, LoaderComponent],
  templateUrl: './auth-component.component.html',
  styleUrl: './auth-component.component.css',
})
export class AuthComponentComponent {}
