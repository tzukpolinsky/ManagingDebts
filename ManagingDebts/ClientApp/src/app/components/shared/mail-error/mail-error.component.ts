import { Component, OnInit, Input } from '@angular/core';
import { SendMailService } from '../../../services/send-mail.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-mail-error',
  templateUrl: './mail-error.component.html',
})
export class MailErrorComponent implements OnInit {
  destroyed = new Subject();
  constructor(private mailService: SendMailService) { }
  @Input() reportErrorMail: boolean;
  @Input() reportAccessDeniedMail: boolean;
  ngOnInit() {
    debugger;
  }
  reportError() {
    this.mailService.reportError(JSON.parse(localStorage.getItem('user')))
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      if (!result) {
        Swal.fire({
          title: 'קרתה תקלה בעת שליחת המייל, נא להתקשר לשירות לקוחות'
        });
      }
    });
  }
  reportAccessDenied() {
    this.mailService.reportAccessDenied(JSON.parse(localStorage.getItem('user')))
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      if (!result) {
        Swal.fire({
          title: 'קרתה תקלה בעת שליחת המייל, נא להתקשר לשירות לקוחות'
        });
      }
    });
  }
}
