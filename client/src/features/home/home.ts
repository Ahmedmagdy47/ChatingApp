import { Component, Input, signal } from '@angular/core';
import { Register } from '../account/register/register';
import { required } from '@angular/forms/signals';
import { User } from '../../types/user';

@Component({
  imports: [Register],
  selector: 'app-home',
  styleUrl: './home.css',
  templateUrl: './home.html',
})
export class Home {
  protected registerMode = signal(false);

  ShowRegister(value: boolean) {
    this.registerMode.set(value);
  }
}
