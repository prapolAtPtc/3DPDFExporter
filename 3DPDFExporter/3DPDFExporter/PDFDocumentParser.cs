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
using iTextSharp;

namespace _3DPDFExporter
{
    public class PDFDocumentParser
    {
        public PDFDocumentParser()
        {
            // Default constructor
        }

        public PdfDocument Parse(String PdfPath)
        {
            using (FileStream fileStream = new FileStream(PdfPath, FileMode.Open, FileAccess.ReadWrite))
            {
                int len = (int)fileStream.Length;
                Byte[] fileArray = new Byte[len];
                fileStream.Read(fileArray, 0, len);
                fileStream.Close();

                return this.Open(fileArray);
            }
        }

        private PdfDocument Open(byte[] fileArray)
        {
            return this.Open(new MemoryStream(fileArray));
        }

        private PdfDocument Open(MemoryStream sourceStream)
        {
            PdfDocument outDoc = null;
            sourceStream.Position = 0;

            try
            {
                outDoc = PdfReader.Open(sourceStream, PdfDocumentOpenMode.Modify);
            }
            catch (PdfSharp.Pdf.IO.PdfReaderException)
            {
                //workaround if pdfsharp doesn't support this pdf
                //sourceStream.Position = 0;
                //MemoryStream outputStream = new MemoryStream();
                //iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(sourceStream);
                //iTextSharp.text.pdf.PdfStamper pdfStamper = new iTextSharp.text.pdf.PdfStamper(reader, outputStream);
                //pdfStamper.FormFlattening = true;
                //pdfStamper.Writer.SetPdfVersion(iTextSharp.text.pdf.PdfWriter.PDF_VERSION_1_4);
                //pdfStamper.Writer.CloseStream = false;
                //pdfStamper.Close();

                //outDoc = PdfReader.Open(outputStream, PdfDocumentOpenMode.Modify);
                return null;
            }
            return outDoc;
        }
    }
}


/*
private PdfDocument Open(String PdfPath)
{
    using (FileStream fileStream = new FileStream(PdfPath, FileMode.Open, FileAccess.ReadWrite))
    {
        int len = (int)fileStream.Length;
        Byte[] fileArray = new Byte[len];
        fileStream.Read(fileArray, 0, len);
        fileStream.Close();

        return Open(fileArray);
    }
}

private PdfDocument Open(byte[] fileArray)
{
    return Open(new MemoryStream(fileArray));
}

private PdfDocument Open(MemoryStream sourceStream)
{
    PdfDocument outDoc = null;
    sourceStream.Position = 0;

    try
    {
        outDoc = PdfReader.Open(sourceStream, PdfDocumentOpenMode.Modify);
    }
    catch (PdfSharp.Pdf.IO.PdfReaderException)
    {
        //workaround if pdfsharp doesn't support this pdf
        sourceStream.Position = 0;
        MemoryStream outputStream = new MemoryStream();
        iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(sourceStream);
        iTextSharp.text.pdf.PdfStamper pdfStamper = new iTextSharp.text.pdf.PdfStamper(reader, outputStream);
        pdfStamper.FormFlattening = true;
        pdfStamper.Writer.SetPdfVersion(iTextSharp.text.pdf.PdfWriter.PDF_VERSION_1_4);
        pdfStamper.Writer.CloseStream = false;
        pdfStamper.Close();

        outDoc = PdfReader.Open(outputStream, PdfDocumentOpenMode.Modify);
    }

    return outDoc;
}
*/