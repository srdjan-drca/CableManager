using QLicense;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CableManager.License
{
   public class MyLicense : LicenseEntity
   {
      [DisplayName("Enable Feature 01")]
      [Category("License Options")]
      [XmlElement("EnableFeature01")]
      [ShowInLicenseInfo(true, "Enable Feature 01", ShowInLicenseInfoAttribute.FormatType.String)]
      public bool EnableFeature01 { get; set; }

      [DisplayName("Enable Feature 02")]
      [Category("License Options")]
      [XmlElement("EnableFeature02")]
      [ShowInLicenseInfo(true, "Enable Feature 02", ShowInLicenseInfoAttribute.FormatType.String)]
      public bool EnableFeature02 { get; set; }

      [DisplayName("Enable Feature 03")]
      [Category("License Options")]
      [XmlElement("EnableFeature03")]
      [ShowInLicenseInfo(true, "Enable Feature 03", ShowInLicenseInfoAttribute.FormatType.String)]
      public bool EnableFeature03 { get; set; }

      public MyLicense()
      {
         AppName = "CableManager";
      }

      public override LicenseStatus DoExtraValidation(out string validationMsg)
      {
         LicenseStatus licenseStatus = LicenseStatus.UNDEFINED;
         validationMsg = string.Empty;

         switch (this.Type)
         {
            case LicenseTypes.Single:
               //For Single License, check whether UID is matched
               if (this.UID == LicenseHandler.GenerateUID(this.AppName))
               {
                  licenseStatus = LicenseStatus.VALID;
               }
               else
               {
                  validationMsg = "The license is NOT for this copy!";
                  licenseStatus = LicenseStatus.INVALID;
               }
               break;

            case LicenseTypes.Volume:
               //No UID checking for Volume License
               licenseStatus = LicenseStatus.VALID;
               break;

            default:
               validationMsg = "Invalid license";
               licenseStatus = LicenseStatus.INVALID;
               break;
         }

         return licenseStatus;
      }
   }
}
