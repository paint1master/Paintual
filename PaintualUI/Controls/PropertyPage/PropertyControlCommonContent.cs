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

namespace PaintualUI.Controls.PropertyPage
{
    /// <summary>
    /// Holds properties that are common to Property_XYZ_Box controls
    /// </summary>
    /// <remarks>Seemed not possible to have UserControls derive from other UserControls, maybe a CustomControl could do it later.</remarks>
    public class PropertyControlCommonContent
    {
        protected string text;
        protected string labelText;

        protected string propertyName;
        protected Engine.PropertyDataTypes dataType;
        protected Engine.Attributes.ValueList valueList;
        protected object defaultValue;
        protected Engine.Validators.Validator validator;

        public PropertyControlCommonContent()
        {

        }

        public string LabelText
        {
            get { return labelText; }
            set { labelText = value; }
        }

        public string PropertyName
        {
            get { return propertyName; }
            set { propertyName = value; }
        }

        public Engine.PropertyDataTypes DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        public Engine.Attributes.ValueList ValueList
        {
            get { return valueList; }
            set { valueList = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public Engine.Validators.Validator Validator
        {
            get { return validator; }
            set { validator = value; }
        }

        public object DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }
    }
}
