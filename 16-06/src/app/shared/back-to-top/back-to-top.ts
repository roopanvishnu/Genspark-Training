import { Component, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-back-to-top',
  template: `<button (click)="scroll.emit()">↑ Back to Top</button>`,
  styleUrls: ['./back-to-top.scss']
})
export class BackToTop {
  @Output() scroll = new EventEmitter<void>();
}
