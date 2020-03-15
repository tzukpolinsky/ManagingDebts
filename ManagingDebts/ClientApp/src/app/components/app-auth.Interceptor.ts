import {
    HttpInterceptor,
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpErrorResponse,
  } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { SendMailService } from '../services/send-mail.service';
import { Injectable } from '@angular/core';
@Injectable({
  providedIn: 'root'
})
export class AuthInterceptor implements HttpInterceptor  {
    constructor(private mailService: SendMailService) {}
    intercept(
        req: HttpRequest<any>,
        next: HttpHandler,
      ): Observable<HttpEvent<any>> {
        return next.handle(this.createRequest(req))
        .pipe(
          catchError((error: HttpErrorResponse) => {
              if (error.status === 500 && !error.url.includes('/mail/')) {
                this.mailService.reportError(JSON.parse(localStorage.getItem('user')))
                .subscribe(result => {
                  if (result) {
                    Swal.fire({
                      icon: 'error',
                      title: 'קרתה שגיאה!',
                      text: 'נשלח מייל לצוות הטכני לגבי השגיאה המדוברת',
                    });
                  }
                });
              } else if (error.status === 401 || error.status === 403 || error.status === 405) {
                Swal.fire({
                  icon: 'error',
                  title: ':( אופס',
                  text: 'אינך מורשה לבצע את הפעולה',
                });
              }
              return throwError('something went wrong');
          }),
        );
      }
      createRequest(request: HttpRequest<any>) {
          return request.clone();
      }
  }
