using System.IO;
using QLicense;
using CableManager.Common.Result;
using CableManager.Common.Helpers;
using CableManager.License;
using CableManager.Localization;

namespace CableManager.Services.License
{
   public class LicenseService : ILicenseService
   {
      private readonly LabelProvider _labelProvider;

      public LicenseService(LabelProvider labelProvider)
      {
         _labelProvider = labelProvider;
      }

      public ReturnResult ValidateLicenseKey(string licenseKey)
      {
         byte[] certificatePublicKey = GetCertificatePublicKey();
         string licenseFullFileName = new FileInfo(DirectoryHelper.GetApplicationStoragePath() + @"\license.lic").FullName;

         if (string.IsNullOrWhiteSpace(licenseKey))
         {
            return new FailResult(_labelProvider["UI_EnterLicenseKey"]);
         }

         if (certificatePublicKey == null)
         {
            return new FailResult("Certificate public key not found");
         }

         ReturnResult result = new FailResult(_labelProvider["UI_LicenseIsInvalid"]);
         LicenseStatus licenseStatus;
         string message;

         LicenseHandler.ParseLicenseFromBASE64String(
            typeof(MyLicense), licenseKey.Trim(), certificatePublicKey, out licenseStatus, out message);

         switch (licenseStatus)
         {
            case LicenseStatus.VALID:
               result = new SuccessResult(_labelProvider["UI_LicenseIsValid"]);

               File.WriteAllText(licenseFullFileName, licenseKey);
               break;

            case LicenseStatus.CRACKED:
            case LicenseStatus.INVALID:
            case LicenseStatus.UNDEFINED:
               result = new FailResult(_labelProvider["UI_LicenseIsInvalid"]);
               break;
         }

         return result;
      }

      public ReturnResult CheckLicense()
      {
         string licenseFullFileName = new FileInfo(DirectoryHelper.GetApplicationStoragePath() + @"\license.lic").FullName;
         byte[] certificatePublicKey = GetCertificatePublicKey();

         if (!File.Exists(licenseFullFileName))
         {
            return new FailResult("Enter license to activate product");
         }

         if (certificatePublicKey == null)
         {
            return new FailResult("Certificate public key not found");
         }

         string licenseKey = File.ReadAllText(licenseFullFileName);
         LicenseStatus status;
         string message;

         LicenseHandler.ParseLicenseFromBASE64String(
            typeof(MyLicense), licenseKey, certificatePublicKey, out status, out message);

         ReturnResult result = status == LicenseStatus.VALID
            ? new SuccessResult(_labelProvider["UI_LicenseIsValid"])
            : (ReturnResult)new FailResult(_labelProvider["UI_LicenseIsInvalid"]);

         return result;
      }

      private byte[] GetCertificatePublicKey()
      {
         string licenseFile = new FileInfo(DirectoryHelper.GetApplicationPath() + @"\LicenseVerify.cer").FullName;
         byte[] publicKey = null;

         if (File.Exists(licenseFile))
         {
            using (var memoryStream = new MemoryStream())
            {
               Stream licenseVerifyStream = File.OpenRead(licenseFile);
               licenseVerifyStream?.CopyTo(memoryStream);

               publicKey = memoryStream.ToArray();
            }
         }

         return publicKey;
      }
   }
}
