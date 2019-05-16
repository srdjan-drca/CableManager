using System;
using System.IO;
using System.Xml.Linq;
using CableManager.Common.Helpers;
using CableManager.Localization;

namespace CableManager.Repository
{
   public abstract class RepositoryBase
   {
      protected RepositoryBase(LabelProvider labelProvider)
      {
         LabelProvider = labelProvider;
      }

      protected LabelProvider LabelProvider { get; }

      protected abstract string RootNodeName { get; }

      protected XDocument GetXDocument(string fullFileName)
      {
         XDocument xDocument = null;

         try
         {
            var fileInfo = new FileInfo(fullFileName);

            xDocument = File.Exists(fileInfo.FullName)
               ? XDocument.Load(fileInfo.FullName)
               : CreateEmptyRepository(fileInfo.FullName);
         }
         catch (Exception exception)
         {
            //Logger.Error(exception.Message);
         }

         return xDocument;
      }

      private XDocument CreateEmptyRepository(string fullFileName)
      {
         string directoryName = Path.GetDirectoryName(fullFileName);

         DirectoryHelper.CreateDirectory(directoryName);
         XDocument xDocument = CreateRepositoryFile(fullFileName);

         return xDocument;
      }

      private XDocument CreateRepositoryFile(string fullFileName)
      {
         var xDeclaration = new XDeclaration("1.0", null, null);
         var xDocument = new XDocument(xDeclaration);

         xDocument.Add(new XElement(RootNodeName));
         xDocument.Save(fullFileName);

         return xDocument;
      }
   }
}