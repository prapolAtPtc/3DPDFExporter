using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.Annotations;
using System.IO;

namespace PdfSharp3dAnnot
{
    public sealed class Pdf3dAnnotation : PdfAnnotation
    {
        private string _u3dPath;
        // Color variable for backgound color
        private float _red;
        private float _green;
        private float _blue;
        // Lighting scheme variable
        private string _lightScheme;
        private bool _toolbar;
        private PdfDocument document;

        public Pdf3dAnnotation()
        {
            this.document = null;
            //this._templatePath = null;
            this._u3dPath = null;
            this._red = 0.97000f;
            this._green = 0.97000f;
            this._blue = 0.9700f;
            this._lightScheme = "CAD";
            this._toolbar = false;
        }

        public Pdf3dAnnotation(PdfDocument document)
        {
            this.document = document;
            this._u3dPath = null;
            this._red = 0.97000f;
            this._green = 0.97000f;
            this._blue = 0.9700f;
            this._lightScheme = "CAD";
            this._toolbar = false;
        }

        // function to replace image with 3D annotation
        // Paratmeters: u3d file path, and output file path
        // return: PDF Document with 3D content
        public PdfDocument replaceImageWith3DAnnotation(String u3dFilePath)
        {
            bool success = false;
            if (this.document != null)
            {
                this._u3dPath = u3dFilePath;
                PdfPage page = this.document.Pages[0];
                //this.Rectangle = General.getImageRect(page);
                success = this.embed3DAnnotation(page);
                if (success)
                {
                    return this.document;
                }
            }
            else
            {
                Console.Write("Cannot find template pdf file to replace image with 3D annotation");
            }
            return null;
        }

        // function to create new 3D pdf
        public PdfDocument create3DPdf(String u3dFilePath)
        {
            bool success = false;
            if (this.document == null)
            {
                this._u3dPath = u3dFilePath;
                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                XUnit width = page.Width;
                page.Width = page.Height;
                page.Height = width;
                this.Rectangle = new PdfRectangle(new System.Drawing.PointF(0.0F, 0.0F), new System.Drawing.SizeF((float)page.Width.Value, (float)page.Height.Value));
                success = this.embed3DAnnotation(page);
                if (success)
                {
                    return document;
                }
            }
            else
            {
                return this.replaceImageWith3DAnnotation(u3dFilePath);
            }
            return null;
        }

        private bool embed3DAnnotation(PdfPage page)
        {
            try
            {
                page.Annotations.Add(this);
                // embed annot object into pdf
                bool success = this.create3DAnnot(page);
                return success;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }

        private bool create3DAnnot(PdfPage page)
        {
            try
            {
                bool success = this.add3DDictionaryObject(page);
                return success;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        // Function to create and add /3DD dictionary object into 3D annot object
        private bool add3DDictionaryObject(PdfPage page)
        {
            bool success = false;
            try
            {
                this.elements.SetName(Key.Subtype, "/3D");

                // Set Rectangle of 3D annotation
                this.elements.SetRectangle(Key.Rect, this.Rectangle);

                // Adds /3DA dictionary
                success = this.add3DADictionaryObject();

                // Adds /AP dictionary
                success = this.addAPDictionaryObject();

                // Adds /N dictionary
                success = this.addNDictionaryObject();

                // Adds /3DD dictionary
                success = this.add3DDDictionaryObject();

                return success;
            }
            catch { return false; }
        }

        // function to create and add /3DA dictionary into 3d annotation object
        private bool add3DADictionaryObject()
        {
            try
            {
                PdfDictionary da3 = new PdfDictionary(this.Owner);
                da3.Elements.SetName(Key.A, "/XA");
                da3.Elements.SetBoolean(Key.TB, this._toolbar);
                da3.Elements.SetBoolean(Key.NP, true);
                da3.Elements.SetName(Key.A, "/PV");
                this.elements.SetObject(Key.D3A, da3);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        // function to create and add /AP dictionary into 3d annotation object
        private bool addAPDictionaryObject()
        {
            try
            {
                PdfDictionary ap = new PdfDictionary(this.Owner);
                this.elements.SetObject(Key.AP, ap);
                return true;
            }
            catch { return false; }
        }

        // function to create and add /N dictionary into 3d annotation object
        private bool addNDictionaryObject()
        {
            try
            {
                PdfDictionary n = new PdfDictionary(this.Owner);
                this.elements.SetObject(Key.N, n);
                return true;
            }
            catch { return false; }
        }

        // function to create and add /3DD dictionary into 3d annotation object
        private bool add3DDDictionaryObject()
        {
            bool success = false;
            try
            {
                PdfDictionary dd3 = new PdfDictionary(this.Owner);
                dd3.Elements.SetName(Key.Type, "/3D");
                dd3.Elements.SetName(Key.Subtype, "/U3D");
                PdfDictionary va = new PdfDictionary(this.Owner);
                va.Elements.SetName(Key.Type, "/3DView");
                va.Elements.SetString(Key.IN, "DefaultView");

                this.add3DBGObject(va);
                this.add3DLightingSchemeObject(va);

                PdfArray vaarry = new PdfArray();
                vaarry.Elements.Add(va);

                dd3.Elements.SetObject(Key.VA, vaarry);
                dd3.Elements.SetString(Key.DV, "DefaultView");

                success = this.addu3dStream(dd3);

                success = this.addOnInstantiateFitVisibleObject(dd3);

                this.Owner.Internals.AddObject(dd3);
                this.elements.SetReference(Key.DD3, dd3);


                return success;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        // function to create and add u3d stream into 3dd dictionary
        private bool addu3dStream(PdfDictionary dd3)
        {
            try
            {
                byte[] data = null;
                int length = 0;
                if (!File.Exists(this._u3dPath))
                    return false;
                using (FileStream fs = new FileStream(this._u3dPath, FileMode.Open, FileAccess.Read))
                {
                    data = new byte[fs.Length];
                    length = Convert.ToInt32(fs.Length);
                    fs.Read(data, 0, length);
                    fs.Close();
                }
                // encode the u3d file data
                PdfSharp.Pdf.Filters.FlateDecode decode = new PdfSharp.Pdf.Filters.FlateDecode();
                data = decode.Encode(data);
                length = data.Length;
                dd3.Elements.SetName(Key.Filter, "/FlateDecode");
                dd3.CreateStream(data);
                dd3.Elements.SetInteger(Key.Length, length);
                return true;
            }
            catch { return false; }
        }

        // function to create and add fitvisible stream into 3dd dictionary
        private bool addOnInstantiateFitVisibleObject(PdfDictionary dd3)
        {
            try
            {
                PdfDictionary onInstantiate = new PdfDictionary(dd3.Owner);
                onInstantiate.Elements.SetName(Key.Filter, "/FlateDecode");
                byte[] jsdata = this.getFitVisibleBytes();
                if (jsdata.Length != 0)
                {
                    onInstantiate.CreateStream(jsdata);
                    int jslength = jsdata.Length;
                    onInstantiate.Elements.SetInteger(Key.Length, jslength);
                    dd3.Owner.Internals.AddObject(onInstantiate);
                    dd3.Elements.SetReference(Key.OnInstantiate, onInstantiate);
                    return true;
                }
                return false;
            }
            catch { return false; }
        }

        // function to add /3DBG backgound color dictionary into 3dd dictionary
        private void add3DBGObject(PdfDictionary va)
        {
            try
            {
                PdfDictionary _3dbg = new PdfDictionary(this.Owner);
                _3dbg.Elements.SetName(Key.Type, "/3DBG");

                PdfArray colorA = new PdfArray();
                PdfReal r = new PdfReal(this._red);
                PdfReal g = new PdfReal(this._green);
                PdfReal b = new PdfReal(this._blue);

                colorA.Elements.Add(r);
                colorA.Elements.Add(g);
                colorA.Elements.Add(b);

                _3dbg.Elements.SetObject(Key.C, colorA);

                va.Elements.SetObject(Key.BG, _3dbg);
            }
            catch { }
        }

        // function to add /LS lighting scheme dictionary into 3dd dictionary
        private void add3DLightingSchemeObject(PdfDictionary va)
        {
            try
            {
                PdfDictionary ls = new PdfDictionary(this.Owner);
                ls.Elements.SetName(Key.Type, "/3DLightingScheme");
                ls.Elements.SetName(Key.Subtype, "/" + this._lightScheme);
                va.Elements.SetObject(Key.LS, ls);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // function to get Fitvisble javascript stream
        private byte[] getFitVisibleBytes()
        {
            try
            {
                //FitVisible fs = new FitVisible();
                String jsString = FitVisible.FitVisibleJs;
                int length = jsString.Length;
                byte[] jsData = new byte[length];
                jsData = UTF8Encoding.UTF8.GetBytes(jsString);

                PdfSharp.Pdf.Filters.FlateDecode decode = new PdfSharp.Pdf.Filters.FlateDecode();
                jsData = decode.Encode(jsData);
                return jsData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public void set3DBackgroundColor(float r, float g, float b)
        {
            this._red = r;
            this._green = g;
            this._blue = b;
        }

        // Dictionary keys for 3D Annotation
        public sealed class Key : Keys
        {
            public const string TB = "/TB";
            public const string NP = "/NP";
            public const string A = "/A";
            public const string D3A = "/3DA";
            public const string DD3 = "/3DD";
            public const string LS = "/LS";
            public const string BG = "/BG";
            public const string C = "/C";
            public const string OnInstantiate = "/OnInstantiate";
            public const string Length = "/Length";
            public const string Filter = "/Filter";
            public const string DV = "/DV";
            public const string VA = "/VA";
            public const string IN = "/IN";
            public const string N = "/N";
        }
    }
}
