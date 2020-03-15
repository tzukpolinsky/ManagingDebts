import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'date'
})
export class DatePipe implements PipeTransform {

  transform(Date: Date): string {

    return Date.toLocaleDateString('he').replace(/\./gi, '/');
  }

}
