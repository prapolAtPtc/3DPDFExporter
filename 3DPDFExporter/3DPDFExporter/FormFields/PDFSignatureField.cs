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
    public sealed class PDFSignatureField : PDFAcroFormFields
    {

        public PDFSignatureField()
        {
            this.initialize();
            this.BGColorString = "0.74902 0.74902 0.74902";
        }

        public PDFSignatureField(float xPos, float yPos, float sizeX, float sizeY)
        {
            this.initialize();
            this.BGColorString = "0.74902 0.74902 0.74902";
            this.setSignatureFieldRectangle(xPos, yPos, sizeX, sizeY);
        }

        public void setSignatureFieldRectangle(float xPos, float yPos, float sizeX, float sizeY)
        {
            this.setRectangle(xPos, yPos, sizeX, sizeY);
        }

        public PdfDocument addSignatureFieldInDocument(PdfDocument document, int pageNumber)
        {
            try
            {
                PdfPage page = document.Pages[pageNumber];
                this.addSignatureFieldAnnotation(page);
                return document;
            }
            catch { return null; }
        }

        private void addSignatureFieldAnnotation(PdfPage page)
        {
            this.Init(page);
            Elements.SetObject(Key.MK, this.getMKDictionary());
            Elements.SetName(Key.FT, "/Sig");
            Elements.SetString(Key.T, "Signature" + page.Annotations.Count);
            Elements.SetString("/TU", "Click to attach signature");
            Elements.SetObject(Key.AP, this.getAPDictionary());
            Elements.SetObject("/Lock", this.getLockDictionary());
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
            String nString = this.BGColorString + " rg 0 0 " + this.Rectangle.Width + " " + this.Rectangle.Height + " re f /Tx BMC EMC";
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

        private PdfDictionary getLockDictionary()
        {
            PdfDictionary lockD = new PdfDictionary();
            lockD.Elements.SetName(Key.Type, "/SigFieldLock");
            lockD.Elements.SetName("/Action","All");
            return lockD;
        }

    }
}

