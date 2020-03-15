import { Component, OnInit, Input } from '@angular/core';
import { Msg } from '../../../entites/msg';

@Component({
  selector: 'app-msg',
  templateUrl: './msg.component.html',
})
export class MsgComponent implements OnInit {
  @Input() msg: Msg;
  closed = false;
  constructor() { }

  ngOnInit() {
  }
  close() {
    this.msg = null;
  }
}
