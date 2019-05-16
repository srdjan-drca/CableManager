using System.Linq;
using System.Xml.Linq;

namespace CableManager.Common.Extensions
{
   public static class XDocumentHelper
   {
      public static XElement Find(this XDocument xDocument, string nodeId)
      {
         return xDocument.Descendants().FirstOrDefault(x => x.Element("Id")?.Value == nodeId);
      }
   }
}