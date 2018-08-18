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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PaintualUI.Controls.PropertyPage
{
    public class TPropertyControl : Control, ITPropertyControl
    {
        static TPropertyControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TPropertyControl), new FrameworkPropertyMetadata(typeof(TPropertyControl)));
        }

        protected List<Engine.Validators.Validator> t_validators;

        public TPropertyControl()
        {
            t_validators = new List<Engine.Validators.Validator>();
        }

        public virtual void BuildControl(Engine.Effects.VisualPropertyItem pi)
        {

        }

        /// <summary>
        /// Use this method to build controls dynamically (ie radio button list based on a property).
        /// </summary>
        public virtual void BuildVisual()
        {

        }

        /// <summary>
        /// Updates the content of visual controls (textbox, drop lists, etc) to display values set by the engine.
        /// </summary>
        public virtual void UpdateVisual()
        {

        }

        public virtual void SignalError(string message)
        {
            ;
        }

        public virtual void ClearSignals()
        {
            ;
        }

        public virtual object EnteredValue { get; }

        public string LabelText { get; set; }

        public string PropertyName { get; set; }

        public Engine.PropertyDataTypes DataType { get; set; }

        public Engine.Attributes.ValueList ValueList { get; set; }

        public string Text { get; set; }

        public List<Engine.Validators.Validator> Validators { get => t_validators; }

        public object DefaultValue { get; set; }
    }
}
