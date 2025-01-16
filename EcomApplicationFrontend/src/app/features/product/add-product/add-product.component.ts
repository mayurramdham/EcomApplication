import { Component, inject, Inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { ProductService } from '../../../core/services/product.service';
import { ToaterService } from '../../../core/services/toater.service';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../../auth/utility/navbar/navbar.component';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-add-product',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NavbarComponent],
  templateUrl: './add-product.component.html',
  styleUrl: './add-product.component.css',
})
export class AddProductComponent implements OnInit {
  productForm: FormGroup;
  selectedFile: File | null = null;
  imagePreview: string | null = null;
  products: any[] = [];
  productId: number = 0;
  isupdate: boolean = false;
  productService = inject(ProductService);
  toasterService = inject(ToaterService);
  selectedProduct: any = null;
  currentEditedElement?: number;
  todayDate = new Date().toISOString().split('T')[0];

  constructor(private fb: FormBuilder) {
    // Initialize the form
    this.productForm = this.fb.group(
      {
        prName: ['', Validators.required],
        prCategory: ['', Validators.required],
        prImageFile: [''],
        prBrand: ['', Validators.required],
        sellingPrice: ['', Validators.required],
        purchasePrice: ['', Validators.required],
        purchaseDate: [''],
        stock: ['', Validators.required],
      },
      {
        validators: this.sellingPriceGreaterThanPurchasePrice, // Custom validator applied at FormGroup level
      }
    );
  }

  onClickReset() {
    this.productForm = new FormGroup({
      prName: new FormControl(''),
      prCategory: new FormControl(''),
      prBrand: new FormControl(''),
      sellingPrice: new FormControl(''),
      purchasePrice: new FormControl(''),
      purchaseDate: new FormControl(''),
      stock: new FormControl(''),
    });
  }
  ngOnInit(): void {
    this.getAllProducts();

    // this.productId = this.products[0].prId;
  }

  sellingPriceGreaterThanPurchasePrice(
    group: FormGroup
  ): ValidationErrors | null {
    const sellingPrice = group.get('sellingPrice')?.value;
    const purchasePrice = group.get('purchasePrice')?.value;

    if (sellingPrice && purchasePrice && sellingPrice <= purchasePrice) {
      return { priceNotValid: true }; // Return error if sellingPrice <= purchasePrice
    }

    return null; // Valid if sellingPrice > purchasePrice
  }

  getAllProducts() {
    this.productService.getProducts().subscribe(
      (response: any) => {
        console.log('productResponse', response);
        if (response.status === 200) {
          this.products = response.product;
          console.log('productResponse2', this.products);
        } else {
          console.error('Error fetching products:', response.message);
        }
      },
      (error) => {
        console.error('Error:', error);
      }
    );
  }
  openModal() {
    const modal = document.getElementById('addProductModal');
    if (modal) {
      modal.style.display = 'block';
      modal.classList.add('show');
      modal.setAttribute('aria-hidden', 'false');
    }
  }

  updateModal(product: any) {
    this.currentEditedElement = product.prId;
    this.isupdate = true;
    console.log('checkPatchImageBeforePatching', this.productForm);
    this.productForm.patchValue(product);
    this.productForm
      .get('purchaseDate')
      ?.setValue(
        product.purchaseDate
          ? new Date(product.purchaseDate).toISOString().split('T')[0]
          : ''
      );

    const modal = document.getElementById('addProductModal');
    if (modal) {
      modal.style.display = 'block';
      modal.classList.add('show');
      modal.setAttribute('aria-hidden', 'false');
    }
  }

  closeModal() {
    const modal = document.getElementById('addProductModal');
    if (modal) {
      modal.style.display = 'none';
      modal.classList.remove('show');
      modal.setAttribute('aria-hidden', 'true');
    }
    this.onClickReset();
  }

  onFileSelect(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    console.log('event--', file);
    if (file) {
      this.selectedFile = file; // Store the file for uploading
    }
  }
  submitProduct() {
    if (this.productForm.invalid || !this.selectedFile) {
      this.toasterService.showError(
        'Please fill all required fields and upload an image.'
      );
      return;
    }
    console.log('prodction from', this.productForm.value);
    const formData = new FormData();
    Object.keys(this.productForm.value).forEach((key) => {
      formData.append(key, this.productForm.value[key]);
    });
    // Append the file (image)
    formData.append('prImageFile', this.selectedFile, this.selectedFile.name);
    console.log('formData:', formData);
    this.productService.addProduct(formData).subscribe(
      (response: any) => {
        console.log('Product response:', response);
        if (response.status === 200) {
          this.toasterService.showSuccess('Product Added Successfully');
          this.getAllProducts();
          this.closeModal();
          this.onClickReset();

          this.selectedFile = null;
        } else {
          this.toasterService.showError('Unable to add Product');
        }
      },
      (error) => {
        console.error('Error adding product:', error);
        this.toasterService.showError('Unable to get response');
      }
    );
  }



  deleteProduct(id: number): void {
    Swal.fire({
      title: 'Are you sure?',
      text: 'Are you sure you want to delete this product?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'No, keep it',
    }).then((result) => {
      if (result.isConfirmed) {
        // Proceed with the deletion if confirmed
        this.productService.deleteProducts(id).subscribe(
          (response) => {
            console.log('login response', response);
            if (response.status == 200) {
              this.toasterService.showSuccess('Product Deleted Successfully');
              this.getAllProducts();
            } else {
              this.toasterService.showError('Unable to delete');
            }
          },
          (error) => {
            this.toasterService.showError('Unable to get response');
          }
        );
      } else {
        // If the user cancels, do nothing
        console.log('Product deletion canceled');
      }
    });
  }

  editProduct() {
    const formData = new FormData();
    Object.keys(this.productForm.value).forEach((key) => {
      formData.append(key, this.productForm.value[key]);
    });
    console.log('fromdata', formData);
    if (this.selectedFile) {
      formData.append('prImageFile', this.selectedFile, this.selectedFile.name);
    }
    if (this.currentEditedElement) {
      formData.append('prId', this.currentEditedElement.toString());
    }
    this.productService.updateProducts(formData).subscribe(
      (response: any) => {
        console.log('update respnse', response);
        if ((response.status = 200)) {
          console.log('update respnse', response);
          this.toasterService.showSuccess('Product updateded successfully');

          this.selectedFile = null;

          this.getAllProducts();
          this.closeModal();
          this.onClickReset();
        } else {
          this.toasterService.showError('Errror in updated the product');
        }
      },
      (error) => {
        this.toasterService.showError('unable to get the response');
      }
    );
  }

  //*****view product section******//
  viewProduct(product: any): void {
    this.selectedProduct = product;
    console.log('selectedProduct', this.selectedProduct);
    const modal = document.getElementById('viewProductModal');
    if (modal) {
      modal.style.display = 'block';
      modal.classList.add('show');
      modal.setAttribute('aria-hidden', 'false');
    }
  }

  closeViewModal(): void {
    const modal = document.getElementById('viewProductModal');
    if (modal) {
      modal.style.display = 'none';
      modal.classList.remove('show');
      modal.setAttribute('aria-hidden', 'true');
    }
    this.onClickReset();
    this.selectedProduct = null;
  }
}
