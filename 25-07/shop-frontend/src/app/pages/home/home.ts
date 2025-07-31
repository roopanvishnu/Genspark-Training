import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContactU, ContactUsService } from '../../services/contact-us';

@Component({
  standalone: true,
  imports: [CommonModule],
  selector: 'app-home',
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class Home implements OnInit {
  contacts: ContactU[] = [];

  constructor(private contactService: ContactUsService) {}

  ngOnInit() {
    this.contactService.getAll().subscribe(res => this.contacts = res);
  }
}
