import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ContactusService, ContactU, ContactUsAddDTO } from '../services/contactus';

@Component({
  selector: 'app-contact-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './contact-management.html',
  styleUrls: ['./contact-management.css']
})
export class ContactManagement implements OnInit {
  contacts: ContactU[] = [];
  form: ContactUsAddDTO = { name: '', email: '', phone: '', content: '' };
  editingId: number | null = null;

  constructor(private contactService: ContactusService) {}

  ngOnInit(): void {
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
        this.cancel();
        this.loadContacts();
      });
    }
  }

  edit(c: ContactU) {
    this.editingId = c.id;
    this.form = {
      name: c.name,
      email: c.email,
      phone: c.phone,
      content: c.content
    };
  }

  delete(id: number) {
    this.contactService.delete(id).subscribe(() => this.loadContacts());
  }

  cancel() {
    this.editingId = null;
    this.form = { name: '', email: '', phone: '', content: '' };
  }
}
