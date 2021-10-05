import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../Services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Output() RegisterCancelEvent = new EventEmitter();
  model: any = {};
  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
  }

  register() {
    this.accountService.Register(this.model).subscribe(
      response => {
        console.log(response);
        this.cancel();
      },
      error => {
        console.log(error);
      }
    )
  }

  cancel() {
    this.RegisterCancelEvent.emit(false);
  }

}
