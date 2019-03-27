import { Component, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { Subscription, Subject } from 'rxjs';
import { IdNumberService } from 'src/app/services/id-number.service';
import { IdentityDetails } from 'src/app/models/identity-details.model';

@Component({
    selector: 'app-id-verify',
    templateUrl: 'id-verify.component.html'
})

export class IdVerifyComponent implements OnDestroy {

    idGroup: FormGroup;
    idList: IdentityDetails[] = [];
    private _valueSubscription: Subscription;
    private _unsubscribe$ = new Subject<void>();

    constructor(private _idNumberService: IdNumberService) {
        this.idGroup = this.createInputGroup();
        this.subscribeToVerify();
    }

    private subscribeToVerify() {
        this._idNumberService
            .verify$
            .pipe(takeUntil(this._unsubscribe$))
            .subscribe(details => this.idList.push(details));
    }

    ngOnDestroy(): void {
        this._unsubscribe$.next();
        this._unsubscribe$.complete();
    }

    submit(): void {
        const idNumber = this.idGroup.get('idnumber').value;
        this.idGroup.reset();
        this._idNumberService.verify(idNumber);
    }

    controlErrorRequired(controlName: string): boolean {
        const control = this.idGroup.get(controlName);

        return (
            control.touched
            && control.invalid
            && !!control.errors['required']
        );
    }

    controlErrorLength(controlName: string): boolean {
        const control = this.idGroup.get(controlName);

        return (
            control.touched
            && control.invalid
            && (!!control.errors['maxlength'] || !!control.errors['minlength'])
        );
    }

    private createInputGroup(): FormGroup {
        const group = new FormGroup({
            idnumber: new FormControl('', [Validators.required, Validators.minLength(13), Validators.maxLength(13)])
        });

        if (this._valueSubscription) {
            this._valueSubscription.unsubscribe();
        }

        this._valueSubscription =
            group
                .get('idnumber')
                .valueChanges
                .pipe(
                    takeUntil(this._unsubscribe$),
                    distinctUntilChanged()
                )
                .subscribe((val: string) => {
                    if (!val) {
                        return;
                    }

                    const newVal = this.removeNonNumbers(val);

                    group.get('idnumber').setValue(newVal);
                });

        return group;
    }

    private removeNonNumbers(val: string) {
        const numbers = val.match(/[0-9]/g);
        const newVal = numbers
            ? numbers.reduce((prev, curr) => prev + curr)
            : '';
        return newVal;
    }
}
