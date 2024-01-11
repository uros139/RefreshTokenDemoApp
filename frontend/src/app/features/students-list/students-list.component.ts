import { Component } from '@angular/core';

@Component({
  selector: 'app-students-list',
  standalone: true,
  imports: [],
  templateUrl: './students-list.component.html',
  styleUrl: './students-list.component.scss'
})

export class StudentsListComponent {
  private students: string[];

  constructor() {
    this.students = ["sd"];
  }
}
