import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContactusService, ContactU } from '../services/contactus';

@Component({
  selector: 'app-contactus',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './contactus.html',
  styleUrls: ['./contactus.css']
})
export class Contactus implements OnInit {
  contacts: ContactU[] = [];

  constructor(private contactService: ContactusService) {}

  ngOnInit(): void {
    this.contactService.getAll().subscribe(res => this.contacts = res);
  }
}
