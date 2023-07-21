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

namespace _3DPDFExporter
{
    class General
    {

        // Function to get image rect from template pdf
        public static PdfRectangle getImageRectFromPDFDocument(PdfDocument document, int pageNumber)
        {
            //if (this.document == null)
            //{
            //    float width = (float)page.Width.Value;
            //    float height = (float)page.Height.Value;
            //    return new PdfRectangle(new System.Drawing.PointF(0.0F, 0.0F), new System.Drawing.SizeF(width, height));
            //}

            // Get resources dictionary
            try
            {
                PdfPage page = document.Pages[pageNumber];                 
                PdfDictionary contents = page.Elements.GetDictionary("/Contents");
                if (contents != null)
                {
                    string stream = contents.Stream.ToString();

                    int imindex = stream.IndexOf("/Im");
                    if (imindex != -1)
                    {
                        var substr = stream.Substring(0, imindex);
                        var substr2 = stream.Substring(imindex);
                        int qIndex = substr.LastIndexOf("q");
                        int Qindex = substr2.IndexOf("Q");
                        string imSting = stream.Substring(qIndex + 2, ((imindex + Qindex) - qIndex) - 2);
                        string matString = imSting.Substring(0, imSting.IndexOf("cm"));
                        var array = matString.Split(' ');
                        if (array.Length >= 6)
                        {
                            float sizeX = float.Parse(array[0], System.Globalization.CultureInfo.InvariantCulture) + 2.0f;
                            float sizeY = float.Parse(array[3], System.Globalization.CultureInfo.InvariantCulture) + 2.0f;
                            float x = float.Parse(array[4], System.Globalization.CultureInfo.InvariantCulture) - 1.0f;
                            float y = float.Parse(array[5], System.Globalization.CultureInfo.InvariantCulture) - 1.0f;
                            return new PdfRectangle(new System.Drawing.PointF(x, y), new System.Drawing.SizeF(sizeX, sizeY));
                        }
                    }
                }
                return new PdfRectangle();
            }
            catch { return new PdfRectangle(); }

        }
    }
}
