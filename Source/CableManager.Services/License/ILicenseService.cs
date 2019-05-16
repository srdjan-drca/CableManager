using CableManager.Common.Result;

namespace CableManager.Services.License
{
   public interface ILicenseService
   {
      ReturnResult ValidateLicenseKey(string licenseKey);

      ReturnResult CheckLicense();
   }
}
