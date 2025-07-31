import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ContactUsService, ContactU, ContactUsAddDTO } from '../../services/contact-us';

@Component({
  standalone: true,
  selector: 'app-contactus',
  imports: [CommonModule, FormsModule],
  templateUrl: './contactus.html',
  styleUrls: ['./contactus.css']
})
export class Contactus implements OnInit {
  contacts: ContactU[] = [];
  form: ContactUsAddDTO = { name: '', email: '', phone: '', content: '' };
  editingId: number | null = null;

  constructor(private contactService: ContactUsService) {}

  ngOnInit() {
    this.loadContacts();
  }

  loadContacts() {
    this.contactService.getAll().subscribe(res => this.contacts = res);
  }

  save() {
    if (this.editingId) {
      this.contactService.update(this.editingId, this.form).subscribe(() => {
        this.cancel();
        this.loadContacts();
      });
    } else {
      this.contactService.create(this.form).subscribe(() => {
        this.form = { name: '', email: '', phone: '', content: '' };
        this.loadContacts();
      });
    }
  }

  edit(contact: ContactU) {
    this.editingId = contact.id;
    this.form = { ...contact };
  }

  delete(id: number) {
    this.contactService.delete(id).subscribe(() => this.loadContacts());
  }

  cancel() {
    this.editingId = null;
    this.form = { name: '', email: '', phone: '', content: '' };
  }
}
