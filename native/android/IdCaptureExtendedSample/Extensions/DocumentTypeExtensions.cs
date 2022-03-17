/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Scandit.DataCapture.ID.Data;

namespace IdCaptureExtendedSample.Extensions
{
    public static class DocumentTypeExtensions
    {
        public static string GetName(this DocumentType documentType)
        {
            return documentType switch
            {
                DocumentType.ConsularId => "Consular Id",
                DocumentType.DrivingLicense => "Driving License",
                DocumentType.DrivingLicensePublicServicesCard => "Driving License Public Services Card",
                DocumentType.EmploymentPass => "Employment Pass",
                DocumentType.FinCard => "Fin Card",
                DocumentType.Id => "Id",
                DocumentType.MultipurposeId => "Multipurpose Id",
                DocumentType.MyKad => "My KAD",
                DocumentType.MyKid => "My KID",
                DocumentType.MyPR => "My PR",
                DocumentType.MyTentera => "My Tentera",
                DocumentType.PanCard => "Pan Card",
                DocumentType.ProfessionalId => "Professional Id",
                DocumentType.PublicServicesCard => "Public Services Card",
                DocumentType.ResidencePermit => "Residence Permit",
                DocumentType.ResidentId => "Resident Id",
                DocumentType.TemporaryResidencePermit => "Temporary Residence Permit",
                DocumentType.VoterId => "Voter Id",
                DocumentType.WorkPermit => "Work Permit",
                DocumentType.IKad => "iKAD",
                DocumentType.MilitaryId => "Military Id",
                DocumentType.MyKas => "My KAS",
                DocumentType.SocialSecurityCard => "Social Security Card",
                DocumentType.HealthInsuranceCard => "Health Insurance Card",
                DocumentType.Passport => "Passport",
                DocumentType.DiplomaticPassport => "Diplomatic Passport",
                DocumentType.ServicePassport => "Service Passport",
                DocumentType.TemporaryPassport => "Temporary Passport",
                DocumentType.Visa => "Visa",
                DocumentType.SPass => "sPass",
                DocumentType.AddressCard => "Address Card",
                DocumentType.AlienId => "Alien ID",
                DocumentType.AlienPassport => "Alien Passport",
                DocumentType.GreenCard => "Green Card",
                DocumentType.MinorsId => "Minors ID",
                DocumentType.PostalId => "Postal ID",
                DocumentType.ProfessionalDl => "Professional DL",
                DocumentType.TaxId => "Tax ID",
                DocumentType.WeaponPermit => "Weapon Permit",
                DocumentType.BorderCrossingCard => "Border Crossing Card",
                DocumentType.DriverCard => "Driver Card",
                DocumentType.GlobalEntryCard => "Global Entry Card",
                DocumentType.MyPolis => "MyPolis",
                DocumentType.NexusCard => "Nesux Card",
                DocumentType.PassportCard => "Passport Card",
                DocumentType.ProofOfAgeCard => "Proof of age Card",
                DocumentType.RefugeeId => "Refugee ID",
                DocumentType.TribalId => "Tribal ID",
                DocumentType.VeteranId => "Veteran ID",
                _ => "None",
            };
        }
    }
}
