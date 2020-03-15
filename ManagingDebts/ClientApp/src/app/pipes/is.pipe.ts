import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'is'
})
export class IsPipe implements PipeTransform {

  transform(status: boolean): string {
    return status ? 'כן' : 'לא';
  }

}
