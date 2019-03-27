import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { MessageType } from '../enums/message-type.enum';

@Injectable()
export class ToastService {

    constructor(private _toastr: ToastrService) { }

    showMessage(message: string, type: MessageType): void {
        switch(type) {
            case MessageType.Success: {
                this._toastr.success(message, 'Success');
                break;
            }
            case MessageType.Info: {
                this._toastr.info(message, 'Information');
                break;
            }
            case MessageType.Error: {
                this._toastr.error(message, 'Error');
                break;
            }
        }
    }
}
