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
    public abstract class PDFAcroFormFields: PdfAnnotation
    {
        protected string BGColorString { get; set; }
        protected string BCColorString { get; set; }
        protected string FontColorString { get; set; }

        protected void initialize()
        {
            // Setting annotation subtype
            Elements.SetName(Key.Subtype, "/Widget");

            this.BGColorString = "1 1 1";
            this.BCColorString = "0 0 0";
            this.FontColorString = "0 0 0";
        }

        protected void Init(PdfPage page)
        {
            // Adding annotatioon object into pages
            page.Annotations.Add(this);

            // Setting annotation rectangle
            Elements.SetRectangle(Key.Rect, this.Rectangle);

            // Setting BS dictionary
            Elements.SetObject(Key.BS, this.getBSDictionary());

            // Setting /F dictionary
            Elements.SetValue(Key.F, new PdfReal(4));

            Elements.SetString(Key.DA, "/HeBo 12 Tf 0 0 0 rg");
        }

        private PdfDictionary getBSDictionary()
        {
            PdfDictionary bs = new PdfDictionary();
            bs.Elements.SetValue(Key.W, new PdfReal(1));
            bs.Elements.SetName(Key.S, "/S");
            return bs;
        }

        protected void setRectangle(float xPos, float yPos, float sizeX, float sizeY)
        {
            this.Rectangle = new PdfRectangle(new System.Drawing.PointF(xPos, yPos), new System.Drawing.SizeF(sizeX, sizeY));
        }

        protected sealed class Key : Keys
        {
            public const string BBox = "/BBox";
            public const string BC = "/BC";
            public const string BG = "/BG";
            public const string DA = "/DA";
            public const string Font = "/Font";
            public const string Ff = "/Ff";
            public const string FT = "/FT";
            public const string JS = "/JS";
            public const string Length = "/Length";
            public const string Matrix = "/Matrix";
            public const string MK = "/MK";
            public const string N = "/N";
            public const string ProcSet = "/ProcSet";            
            public const string Resources = "/Resources";
            public const string R = "/R";
            public const string S = "/S";
            public const string TP = "/TP";
            public const string W = "/W";
            public const string V = "/V";
            public const string FormType = "/FormType";
        }
    }
}
