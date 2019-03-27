import { IdentityDetails } from './identity-details.model';

export interface IdentityVerifyResult {
    isValid: boolean;
    idDetails?: IdentityDetails;
}
