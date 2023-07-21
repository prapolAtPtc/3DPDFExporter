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
    public sealed class PDFButtonField : PDFAcroFormFields
    {

        public PdfReal multilineFlag;
        public string buttonString {get; set;}

        public PDFButtonField()
        {
            this.initialize();
            this.multilineFlag = new PdfReal(0);
            //this.BGColorString = "0.74902 0.74902 0.74902";
        }

        public PDFButtonField(float xPos, float yPos, float sizeX, float sizeY)
        {
            this.initialize();
            //this.BGColorString = "0.74902 0.74902 0.74902";
            this.multilineFlag = new PdfReal(0);
            this.setButtonFieldRectangle(xPos, yPos, sizeX, sizeY);
        }

        public void setButtonFieldRectangle(float xPos, float yPos, float sizeX, float sizeY)
        {
            this.setRectangle(xPos, yPos, sizeX, sizeY);
        }

        public PdfDocument addButtonFieldInDocument(PdfDocument document, int pageNumber)
        {
            try
            {
                PdfPage page = document.Pages[pageNumber];
                this.addButtonFieldAnnotation(page);
                return document;
            }
            catch { return null; }
        }

        private void addButtonFieldAnnotation(PdfPage page)
        { 
            this.Init(page);
            Elements.SetObject(Key.MK, this.getMKDictionary());
            Elements.SetName(Key.FT, "/Btn");
            Elements.SetValue(Key.Ff, new PdfReal(65536));
            Elements.SetString(Key.T, "Button" + page.Annotations.Count);            
            Elements.SetObject(Key.AP, this.getAPDictionary());
            Elements.SetObject(Key.A, this.getActionDictionary());
        }

        private PdfDictionary getActionDictionary()
        {
            PdfDictionary action = new PdfDictionary(this.Owner);

            action.Elements.SetString(Key.JS, "event.target.buttonImportIcon();");
            action.Elements.SetName(Key.S, "/JavaScript");
            return action;
        }

        private PdfDictionary getAPDictionary()
        {
            PdfDictionary ap = new PdfDictionary(this.Owner);
            ap.Elements.SetReference(Key.N, this.getNDictionary(ap));
            return ap;
        }

        private PdfDictionary getNDictionary(PdfDictionary ap)
        {
            try
            {
                PdfDictionary n = new PdfDictionary(ap.Owner);
                n.Elements.SetName(Key.Type, "/XObject");
                n.Elements.SetName(Key.Subtype, "/Form");
                n.Elements.SetValue(Key.FormType, new PdfReal(1));
                n.Elements.SetMatrix(Key.Matrix, new XMatrix(1, 0, 0, 1, 0, 0));
                n.Elements.SetRectangle(Key.BBox, new PdfRectangle(new System.Drawing.PointF(0, 0), new System.Drawing.SizeF((float)this.Rectangle.Width, (float)this.Rectangle.Height)));
                n.Elements.SetObject(Key.Resources, this.getResourcesDictionary());
                byte[] value = this.getStream();
                n.Elements.SetValue(Key.Length, new PdfReal(value.Length));
                n.CreateStream(value);

                ap.Owner.Internals.AddObject(n);
                return n;
            }
            catch (Exception e) { Console.WriteLine(e.Message);  return null; }
        }

        private PdfDictionary getResourcesDictionary()
        {
            PdfDictionary res = new PdfDictionary();
            res.Elements[Key.ProcSet] = new PdfLiteral("[/PDF/Text/ImageB/ImageC/ImageI]"); ;
            res.Elements.SetObject(Key.Font, new PdfDictionary());
            return res;
        }

        private byte[] getStream()
        {
            String n0 = this.BGColorString + " rg 0 0 " + this.Rectangle.Width + " " + this.Rectangle.Height + " re f /Tx BMC"; 
            String n1 = "q 1 1 " + this.Rectangle.Width + " " + this.Rectangle.Height + " re W n 0 0 0 rg BT /HeBo 12 Tf " + "40.5407" + " 24.5407 Td (Click Here To Attach Logo)Tj ET Q EMC";
            String nString = n0 + " " +n1;
            return UTF8Encoding.UTF8.GetBytes(nString);
        }

        private PdfDictionary getMKDictionary()
        {
            PdfDictionary mk = new PdfDictionary();
            PdfLiteral bg = new PdfLiteral("[" + this.BGColorString + "]");
            PdfLiteral bc = new PdfLiteral("[" + this.BCColorString + "]");

            mk.Elements[Key.BG] = bg;
            //mk.Elements[Key.BC] = bc;
            mk.Elements.SetValue(Key.TP, new PdfReal(1));
            mk.Elements.SetValue(Key.R, new PdfReal(0));
            mk.Elements.SetString(Key.CA, "Click Here To Attach Logo");
            return mk;
        }
    }
}
