import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CartService } from '../../../core/services/cart.service';
import { ToaterService } from '../../../core/services/toater.service';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../../auth/utility/navbar/navbar.component';

@Component({
  selector: 'app-invoice',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl: './invoice.component.html',
  styleUrl: './invoice.component.css',
})
export class InvoiceComponent implements OnInit {
  id?: number;
  invoice: any;
  constructor(
    private route: ActivatedRoute,
    private cartService: CartService,
    private toasterService: ToaterService
  ) {}
  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id') || '');
    this.GenerateInvoicemesage();
  }
  GenerateInvoicemesage() {
    this.cartService.GenerateInvoice(Number(this.id)).subscribe(
      (response) => {
        if (response.status == 200) {
          this.invoice = response.invoice;
          console.log('invoice data', this.invoice);
        } else {
          this.toasterService.showError('unable to gnerate invoice');
        }
      },
      (errror) => {
        this.toasterService.showError('unable to get response');
      }
    );
  }
}
