using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp3dAnnot;
using PdfSharpAcroFields;
using System.Web;


namespace _3DPDFExporter
{
    public class Rectangle
    {
        public int pageNumber { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float width { get; set; }
        public float height { get; set; }

        public Rectangle()
        {
            this.pageNumber = 0;
            this.x = 0.0f;
            this.y = 0.0f;
            this.width = 0.0f;
            this.height = 0.0f;
        }

    }

    public class xmlParse
    {
        private string xmlfilepath;
        public xmlParse() 
        { 
            this.xmlfilepath = null; 
        }
        public xmlParse(string filePath) 
        {
            this.xmlfilepath = filePath; 
        }

        public void parse() { }

    }

    class MainDriverClass
    {
        static Rectangle _3dannotRect;
        static Rectangle custNameRect;
        static Rectangle custAddRect;
        static Rectangle buttonFieldRect;
        static Rectangle sigFieldRect;

        static void Main(string[] args)
        {

            _3dannotRect = new Rectangle();
            custNameRect = new Rectangle();
            custAddRect = new Rectangle();
            buttonFieldRect = new Rectangle();
            sigFieldRect = new Rectangle();

            string inputFile = args[0];

            string jsonString = System.IO.File.ReadAllText(inputFile);

            var jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            Dictionary<string, object> dict = (Dictionary<string, object>)jsSerializer.DeserializeObject(jsonString);

            object[] objArray = (object[])dict["data"];

            //string s = (string)((Dictionary<string, object>)(str[0]))["templatepath"];

            String quotationPath = (string)((Dictionary<string, object>)(objArray[0]))["templatepath"];
            String _3dFilePath = (string)((Dictionary<string, object>)(objArray[1]))["3dpath"];
            String outPutFilePath = (string)((Dictionary<string, object>)(objArray[6]))["outfilepath"];



            //string[] lines = System.IO.File.ReadAllLines(inputFile);

            //Dictionary<string, object>[] array = new Dictionary<string, object>[lines.Length];

            //for (Int16 i = 0; i < lines.Length; ++i)
            //{
            //    //string jsonString = lines[i].Substring(lines[i].IndexOf(':') + 1);
            //    string jsonString = lines[i];

            //    var jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //    //Dictionary<string, object> temp = (Dictionary<string, object>)jsSerializer.DeserializeObject(jsonString);
            //    array[i] = (Dictionary<string, object>)jsSerializer.DeserializeObject(jsonString);
            //}

            //String quotationPath = (string)(array[0])["templatepath"];
            //String _3dFilePath = (string)(array[1])["3dpath"];
            //String outPutFilePath = (string)(array[6])["outfilepath"];

            bool success = false;
            try
            {
                //success = Create3DPDF(_3dFilePath, outPutFilePath);
                success = Create_Quotation_PDF(quotationPath, _3dFilePath, outPutFilePath);
                Console.WriteLine((success == true ? "Successful" : "falied"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static bool Create3DPDF(string u3dPath, string outfilename)
        {
            try
            {
                PdfDocument document = null;
                Pdf3dAnnotation _3dannot = new Pdf3dAnnotation();
                document = _3dannot.create3DPdf(u3dPath);
                document.Save(outfilename);
                return true;
            }
            catch { return false; }
        }

        static bool Create_Quotation_PDF (string templatePath, string u3dPath, string outfilename)
        {
            try
            {
                int pageNumber = 0; // first page
                // Function call to embed 3D into PDF which contain image
                PdfDocument document = null;
                PDFDocumentParser parser = new PDFDocumentParser();
                document = parser.Parse(templatePath);

                Pdf3dAnnotation pdf3dAnnot = new Pdf3dAnnotation(document);
                pdf3dAnnot.Rectangle = General.getImageRectFromPDFDocument(document, pageNumber);
                document = pdf3dAnnot.replaceImageWith3DAnnotation(u3dPath, pageNumber);

                PDFTextField tf = new PDFTextField(140.0f, 705.01f, 120.0f, 30.5f);
                document = tf.addTextFieldInDocument(document, pageNumber);

                PDFTextField tf1 = new PDFTextField(140.0f, 610.1f, 120.0f, 91.0f);
                tf1.multilineFlag = true;
                document = tf1.addTextFieldInDocument(document, pageNumber);

                PDFButtonField btn = new PDFButtonField(306.986f, 610.502f, 528.056f - 306.986f, 669.899f - 613.502f);
                document = btn.addButtonFieldInDocument(document, pageNumber);

                PDFSignatureField sig = new PDFSignatureField(375.7858f, 41.3858f, 150.0f, 40.0f);
                document = sig.addSignatureFieldInDocument(document, pageNumber);

                PDFAcroFields af = new PDFAcroFields();
                PdfArray annotArray = new PdfArray();
                annotArray.Elements.Add(tf.Reference);
                annotArray.Elements.Add(tf1.Reference);
                annotArray.Elements.Add(sig.Reference);
                annotArray.Elements.Add(btn.Reference);
                document = af.addAcrofieldObject(document, annotArray);
                if (File.Exists(@"temp.pdf"))
                {
                    File.Delete(@"temp.pdf");
                }
                document.Save("temp.pdf");

                savePDF(outfilename);
                //document.Save(outfilename);
                return true;
            }
            catch(Exception e) 
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                return false; 
            }
        }

        static void savePDF(string outfilename) 
        {
            if (File.Exists(outfilename))
            {
                File.Delete(outfilename);
            }
            FileStream fs = new FileStream(outfilename, FileMode.Create, FileAccess.Write, FileShare.None);

            MemoryStream outputStream = new MemoryStream();
            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader("temp.pdf");
            iTextSharp.text.pdf.PdfStamper pdfStamper = new iTextSharp.text.pdf.PdfStamper(reader, outputStream);
            pdfStamper.Writer.SetPdfVersion(iTextSharp.text.pdf.PdfWriter.PDF_VERSION_1_5);
            pdfStamper.Writer.CloseStream = false;
            pdfStamper.Close();
            outputStream.WriteTo(fs);

            reader.Close();
            fs.Close();
            File.Delete(@"temp.pdf");
        }
    }
}