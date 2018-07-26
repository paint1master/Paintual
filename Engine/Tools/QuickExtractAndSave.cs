/**********************************************************

MIT License

Copyright (c) 2018 Michel Belisle

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

**********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Tools
{
    public class QuickExtractAndSave : Engine.Tools.Tool
    {
        private int t_sequence = 0;

        private const string t_folderPath_PropertyName = "FolderPath";
        private const string t_prefix_PropertyName = "Prefix";
        private const string t_format_PropertyName = "Format";

        private const string t_defaultPrefix = "fragment_";
        private const string t_defaultFormat = "000";

        public QuickExtractAndSave()
        {
            t_visualProperties = new Engine.Effects.VisualProperties("Quick Extract and Save", typeof(QuickExtractAndSave));
        }

        public override void Initialize(Viome w)
        {
            base.Initialize(w);
            t_attributeCollection.Add(t_folderPath_PropertyName, new Engine.Attributes.StringAttribute());
            t_attributeCollection.Add(t_prefix_PropertyName, new Engine.Attributes.StringAttribute());
            t_attributeCollection.Add(t_format_PropertyName, new Engine.Attributes.StringAttribute());

            // moved to HasErrors prop. because the property is set before the tool is used
            // so if errros, no selectionGlass is created for nothing
            //t_VIOM.SelectionGlassRequest(SelectionGlassRequestType.Create);
        }

        /// <summary>
        /// Extracts and saves a copy of the selection portion of the ImageSource
        /// </summary>
        /// <remarks>This method should not be called continuously like other tools. It does not draw but
        /// saves an image.</remarks>
        public override void Process()
        {

        }

        public override int AfterDraw(Point p)
        {
            // required by t_workflow.ThreadingQueue
            return 0;
        }

        public override void BeforeDraw(int x, int y)
        {
            throw new NotImplementedException();
        }

        internal override void Draw(MousePoint p)
        {

        }

        public override IGraphicActivity Duplicate(Viome w)
        {
            QuickExtractAndSave qeas = new QuickExtractAndSave();
            qeas.Initialize(w);

            return qeas;
        }

        public void HandleDoubleClick(object sender, Engine.Utilities.Selection.SelectionEventArgs e)
        {
            if (t_hasErrors)
            {
                return;
            }

            if (String.IsNullOrEmpty(Folder))
            {
                // TODO : validation code and message back to UI
                return;
            }

            Engine.Rectangle r = new Rectangle(e.Rectangle.Location.X, e.Rectangle.Location.Y, e.Rectangle.Width, e.Rectangle.Height);
            Engine.Surface.Canvas c = Engine.Surface.Ops.CopyFromImage(this.t_imageSource, r);

            string fileName = BuildFileName();

            // check file exists, if so, repeat and increase sequence until file does not exist
            while (Engine.Utilities.SFO.FileExists(fileName))
            {
                fileName = BuildFileName();
            }

            Engine.Surface.Ops.Save(c, fileName, Surface.ImageFileFormats.PNG);
        }

        private string BuildFileName()
        {
            string result = Folder + "\\" + Prefix + NextFileSeq() + ".png";
            return result;
        }

        private string NextFileSeq()
        {
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-us");
            string s = t_sequence.ToString("D5", ci);
            t_sequence++;

            return s;
        }

        public override bool HasErrors
        {
            get { return base.HasErrors; }
            set
            {
                if (value)
                {
                    // if there are errors then remove the selection glass so the user is not mislead into using
                    // the tool while it doesn't actually work
                    t_VIOM.SelectionGlassRequest(SelectionGlassRequestType.Delete);
                }
                else
                {
                    t_VIOM.SelectionGlassRequest(SelectionGlassRequestType.Create);
                }
            }
        }

        [Engine.Attributes.Meta.DisplayName("Folder")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.FolderSelector)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Text)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.StringNotEmpty, "")]
        public string Folder
        {
            get { return ((Engine.Attributes.StringAttribute)t_attributeCollection.Get(t_folderPath_PropertyName)).Value; }
            set { ((Engine.Attributes.StringAttribute)t_attributeCollection.Get(t_folderPath_PropertyName)).Value = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Prefix")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Text)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.StringNotEmpty, "")]
        public string Prefix
        {
            get { return ((Engine.Attributes.StringAttribute)t_attributeCollection.Get(t_prefix_PropertyName)).Value; }
            set { ((Engine.Attributes.StringAttribute)t_attributeCollection.Get(t_prefix_PropertyName)).Value = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Format")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Text)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.StringNotEmpty, @"\A[0]+$")]
        public string Format
        {
            get { return ((Engine.Attributes.StringAttribute)t_attributeCollection.Get(t_format_PropertyName)).Value; }
            set { ((Engine.Attributes.StringAttribute)t_attributeCollection.Get(t_format_PropertyName)).Value = value; }
        }
    }
}
