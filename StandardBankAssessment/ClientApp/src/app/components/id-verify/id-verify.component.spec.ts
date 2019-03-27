import { IdNumberService } from 'src/app/services/id-number.service';
import { TestBed, ComponentFixture } from '@angular/core/testing';
import { IdVerifyComponent } from './id-verify.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { IdentityDetails } from 'src/app/models/identity-details.model';

describe('IdVerifyComponent', () => {
    let service: { verify: jasmine.Spy, verify$: jasmine.SpyObj<Subject<IdentityDetails>> };
    let fixture: ComponentFixture<IdVerifyComponent>;
    let component: IdVerifyComponent;
    let idDetails: IdentityDetails;
    let verifySubject$: Subject<IdentityDetails>;

    beforeEach(async() => {
        idDetails = {
            idNumber: '9006105999088',
            dateOfBirth: new Date(1990, 6, 10),
            gender: 'male',
            citizenship: 'SA Citizen'
        };

        service = jasmine.createSpyObj('IdNumberService', ['verify', 'verify$']);
        verifySubject$ = new Subject<IdentityDetails>();
        service.verify$ = jasmine.createSpyObj('Subject<IdentityDetails>', ['pipe', 'subscribe']);
        service.verify$.pipe.and.returnValue(verifySubject$);

        TestBed.configureTestingModule({
            imports: [
                FormsModule,
                ReactiveFormsModule
            ],
            declarations: [
                IdVerifyComponent
            ],
            providers: [
                { provide: IdNumberService, useValue: service }
            ]
        })
        .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(IdVerifyComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeDefined();
    });

    it('should create id input group', () => {
        expect(component.idGroup).toBeDefined();
        expect(component.idGroup.get('idnumber')).toBeDefined();
        expect(component.idGroup.get('idnumber').value).toBe('');
    });

    it('should have min length of 13 on id number input', () => {
        const idnumberControl = component.idGroup.get('idnumber');

        idnumberControl.setValue('123456789012');
        idnumberControl.markAsTouched();

        const isInvalid = component.controlErrorLength('idnumber');

        expect(isInvalid).toBe(true);
    });

    it('should have max length 13 on id number input', () => {
        const idnumberControl = component.idGroup.get('idnumber');

        idnumberControl.setValue('12345678901234');
        idnumberControl.markAsTouched();

        const isInvalid = component.controlErrorLength('idnumber');

        expect(isInvalid).toBe(true);
    });

    it('should have a required contraint for id number input', () => {
        const idnumberControl = component.idGroup.get('idnumber');

        idnumberControl.setValue('');
        idnumberControl.markAsTouched();

        const isInvalid = component.controlErrorRequired('idnumber');

        expect(isInvalid).toBe(true);
    });

    it('should add id details to list', () => {
        verifySubject$.next(idDetails);

        expect(component.idList).toContain(idDetails);
    });
});
