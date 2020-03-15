import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'dateString'
})
export class DateStringPipe implements PipeTransform {

  transform(value: string): string {
    const date = new Date(value);
    return date.toLocaleDateString('he').replace(/\./gi, '/');
  }

}
