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
    public sealed class PDFTextField : PDFAcroFormFields
    {

        public bool multilineFlag { get; set; }

        public PDFTextField()
        {
            this.initialize();
            this.multilineFlag = false;
            //this.BGColorString = "0.9 0.9 0.9";

        }

        public PDFTextField(float xPos, float yPos, float sizeX, float sizeY)
        {
            this.initialize();
            this.multilineFlag = false;
            //this.BGColorString = "0.9 0.9 0.9";
            this.setTextFieldRectangle(xPos, yPos, sizeX, sizeY);
        }

        public void setTextFieldRectangle(float xPos, float yPos, float sizeX, float sizeY)
        {
            this.setRectangle(xPos, yPos, sizeX, sizeY);
        }

        public PdfDocument addTextFieldInDocument(PdfDocument document, int pageNumber)
        {
            try
            {
                PdfPage page = document.Pages[pageNumber];
                this.addTextFieldAnnotation(page);
                return document;
            }
            catch { return null; }
        }

        private void addTextFieldAnnotation(PdfPage page)
        {
            this.Init(page);
            this.setMKDictionary();
            this.setFTKey();
            this.setTextKey(page.Annotations.Count);
            this.setFfKey();
            this.setDAKey();
            this.setVKey();
            this.setAPDictionary();
        }

        private void setMKDictionary()
        {
            Elements.SetObject(Key.MK, this.getMKDictionary());
        }

        private void setFTKey()
        {
            Elements.SetName(Key.FT, "/Tx");
        }

        private void setTextKey(int count)
        {
            Elements.SetString(Key.T, "Text" + count);
        }

        private void setFfKey()
        {
            if (!this.multilineFlag)
                Elements.SetValue(Key.Ff, new PdfReal(0));
            else
                Elements.SetValue(Key.Ff, new PdfReal(8392704));
        }

        private void setDAKey()
        {
            Elements.SetString(Key.DA, "/HeBo 11 Tf 0 0 0 rg");
        }

        private void setVKey()
        {
            Elements.SetString(Key.V, "");
        }

        private void setAPDictionary()
        {
            Elements.SetObject(Key.AP, this.getAPDictionary());
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
            catch (Exception e) { Console.WriteLine(e.Message); return null; }
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
            String nString = this.BGColorString + " rg 0 0 " + this.Rectangle.Width + " " + this.Rectangle.Height + " re f 0 0 0 RG 1 w 0.5 0.5 " + (this.Rectangle.Width - 0.6) + " " + (this.Rectangle.Height - 0.6) + " re S /Tx BMC EMC";
            return UTF8Encoding.UTF8.GetBytes(nString);
        }

        private PdfDictionary getMKDictionary()
        {
            PdfDictionary mk = new PdfDictionary();
            PdfLiteral bg = new PdfLiteral("[" + this.BGColorString + "]");
            PdfLiteral bc = new PdfLiteral("[" + this.BCColorString + "]");

            mk.Elements[Key.BG] = bg;
            //mk.Elements[Key.BC] = bc;
            mk.Elements.SetValue(Key.R, new PdfReal(0));
            return mk;
        }

    }
}
