import { of, throwError } from 'rxjs';
import { IdentityVerifyResult } from '../models/identity-verify-result.model';
import { IdNumberService } from './id-number.service';
import { MessageType } from '../enums/message-type.enum';
import { HttpErrorResponse } from '@angular/common/http';

describe('IdNumberService', () => {

    let httpResult: IdentityVerifyResult;
    let httpSpy: { get: jasmine.Spy };
    let messageServiceSpy: { showMessage: jasmine.Spy };

    const baseUrl = 'http://test.co.za/';

    let idService: IdNumberService;

    beforeEach(() => {
        httpResult = {
            isValid: true,
            idDetails: {
                idNumber: '9006105999088',
                dateOfBirth: new Date(1990, 6, 10),
                gender: 'male',
                citizenship: 'SA Citizen'
            }
        };
        httpSpy = jasmine.createSpyObj('HttpClient', ['get']);
        messageServiceSpy = jasmine.createSpyObj('ToastService', ['showMessage']);
    });

    it('should create', () => {
        idService = new IdNumberService(<any> httpSpy, <any> messageServiceSpy, baseUrl);

        expect(idService).toBeDefined();
    });

    describe('verify(idNumber)', () => {
        beforeEach(() => {
            idService = new IdNumberService(<any> httpSpy, <any> messageServiceSpy, baseUrl);
            httpSpy.get.and.returnValue(of(httpResult));
        });

        it('should make http get call to correct url', () => {
            idService.verify(httpResult.idDetails.idNumber);
            expect(httpSpy.get).toHaveBeenCalledWith(`${baseUrl}api/identitynumber/verify/${httpResult.idDetails.idNumber}`);
        });

        it('should show success message when valid id number', () => {
            idService.verify(httpResult.idDetails.idNumber);

            expect(messageServiceSpy.showMessage).toHaveBeenCalledWith('Identity number is valid', MessageType.Success);
        });

        it('should emit id details when valid id number', () => {
            idService
                .verify$
                .subscribe(details => {
                    expect(details).toEqual(httpResult.idDetails);
                });

            idService.verify(httpResult.idDetails.idNumber);
        });

        it('should show error message when invalid id number', () => {
            httpResult.isValid = false;

            idService.verify(httpResult.idDetails.idNumber);

            expect(messageServiceSpy.showMessage).toHaveBeenCalledWith('Identity number is invalid', MessageType.Error);
        });

        it('should return server error message when server returns 500', () => {
            const error = new HttpErrorResponse({
                error: 'test 500 error',
                status: 500,
                statusText: 'Internal Server Error'
            });

            httpSpy.get.and.returnValue(throwError(error));

            idService.verify(httpResult.idDetails.idNumber);

            expect(messageServiceSpy.showMessage).toHaveBeenCalledWith('Server Error', MessageType.Error);
        });
    });
});
