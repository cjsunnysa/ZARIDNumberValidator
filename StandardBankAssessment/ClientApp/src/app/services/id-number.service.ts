import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IdentityVerifyResult } from '../models/identity-verify-result.model';
import { Observable, Subject } from 'rxjs';
import { ToastService } from './toast.service';
import { MessageType } from '../enums/message-type.enum';
import { IdentityDetails } from '../models/identity-details.model';

@Injectable()
export class IdNumberService {
    constructor(
        private _http: HttpClient,
        private _messageService: ToastService,
        @Inject('BASE_URL') private _baseUrl: string
    ) { }

    verify$ = new Subject<IdentityDetails>();

    verify(idNumber: string): void {
        this._http
            .get<IdentityVerifyResult>(`${this._baseUrl}api/identitynumber/verify/${idNumber}`)
            .subscribe(
                result => {
                    if (!result.isValid) {
                        this._messageService.showMessage('Identity number is invalid', MessageType.Error);
                        return;
                    }

                    this._messageService.showMessage('Identity number is valid', MessageType.Success);
                    this.verify$.next(result.idDetails);
                },
                error => this._messageService.showMessage('Server Error', MessageType.Error)
            );
    }
}
