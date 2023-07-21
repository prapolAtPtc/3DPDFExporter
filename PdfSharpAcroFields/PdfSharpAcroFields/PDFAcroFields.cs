using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.Annotations;
using System.IO;

namespace PdfSharpAcroFields
{
    public sealed class PDFAcroFields
    {
        public PdfDocument addAcrofieldObject(PdfDocument document, PdfArray annotArray)
        {
            try
            {
                return this.addAcroField(document, annotArray);
            }
            catch { return null; }
        }

        private PdfDocument addAcroField(PdfDocument doc, PdfArray annotArray)
        {
            PdfDictionary catalog = doc.Internals.Catalog;

            PdfDictionary acroForm = new PdfDictionary(catalog.Owner);
            
            acroForm.Elements.SetString(Key.DA, "/Helv 0 Tf 0 g");

            acroForm.Elements.SetObject(Key.Fields, annotArray);
            PdfDictionary dr = new PdfDictionary(acroForm.Owner);
            PdfDictionary font = new PdfDictionary(dr.Owner);
            acroForm.Elements.SetObject(Key.Font, font);
            acroForm.Elements.SetObject(Key.DR, dr);

            catalog.Elements.SetObject(Key.AcroForm, acroForm);
            return doc;
        }

        private sealed class Key
        {
            public const string DA = "/DA";
            public const string Fields = "/Fields";
            public const string Font = "/Font";
            public const string DR = "/DR";
            public const string AcroForm = "/AcroForm";
        }

    }
}
