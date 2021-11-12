import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';
import { User } from 'src/app/models/user';
import { AdminService } from 'src/app/services/admin.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {

  users: Partial<User[]>;
  bsModalRef: BsModalRef;

  constructor(private adminService: AdminService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe(data => this.users = data);
  }

  openRolesModal(user: User) {

    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        user,
        roles: this.getRolesArray(user)
      }
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    this.bsModalRef.content.UpdateSelectedRoles.subscribe(data => {
      var roles = [...data.filter(rl => rl.checked === true).map(rl => rl.name)];

      this.adminService.updateUserRoles(user.username, roles).subscribe(() => {
        user.roles = roles;
      });

    });
  }

  getRolesArray(user: User) {
    const roles = [];
    const userRoles = user.roles;
    const availableRoles: any[] = [
      { name: 'Admin', value: 'Admin', checked: userRoles.includes('Admin') },
      { name: 'Moderator', value: 'Moderator', checked: userRoles.includes('Moderator') },
      { name: 'Member', value: 'Member', checked: userRoles.includes('Member') },
    ];

    return availableRoles;
  }

}
