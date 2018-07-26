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
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using Engine;

namespace PaintualUI.Controls.PropertyPage
{
    /// <summary>
    /// Interaction logic for VisualPropertyPage.xaml
    /// </summary>
    public partial class VisualPropertyPage : UserControl
    {
        private Engine.Viome t_workflow;
        private Engine.Effects.VisualProperties t_visualProperties;

        /// <summary>
        /// holds property values, used when switching from drawing board to drawing board so that the UI and the Graphic activity
        /// remember values set by user. Current instance is saved in the corresponding tool or effect instance.
        /// </summary>
        private Engine.Attributes.AttributeCollection t_properties;

        public VisualPropertyPage() => InitializeComponent();

        public void Build(Engine.Effects.VisualProperties vp)
        {
            Clear();

            // this can happen for tools or effects that do not have visual properties
            if (vp == null)
            {
                return;
            }

            t_visualProperties = vp;
            t_properties = new Engine.Attributes.AttributeCollection();

            Dictionary<string, Engine.Effects.VisualPropertyItem> items = vp.GetItems();

            foreach (KeyValuePair<string, Engine.Effects.VisualPropertyItem> key in items)
            {
                BuildControl(key);
            }
        }

        public void Clear()
        {
            FlowPanelContainer.Children.Clear();
            t_visualProperties = null;
            t_properties = null;
        }

        public void SetWorkflow(Engine.Viome w)
        {
            t_workflow = w;
        }

        #region Build
        private void BuildControl(KeyValuePair<string, Engine.Effects.VisualPropertyItem> key)
        {
            Engine.Effects.VisualPropertyItem pi = key.Value;

            System.Windows.Controls.UserControl ctrl = null;

            switch (pi.DisplayControlType)
            {
                case Engine.Attributes.Meta.DisplayControlTypes.Textbox:
                    switch (pi.DataType)
                    {
                        case Engine.PropertyDataTypes.Int:
                            ctrl = BuildPropertyIntBox(pi);
                            break;

                        case Engine.PropertyDataTypes.Double:
                            ctrl = BuildPropertyDoubleBox(pi);
                            break;

                        case Engine.PropertyDataTypes.Text:
                            ctrl = BuildPropertyTextBox(pi);
                            break;

                        default:
                            throw new Exception();
                    }

                    break;

                case Engine.Attributes.Meta.DisplayControlTypes.RadioButtons:
                    ctrl = BuildPropertyRadioButtons(pi);
                    break;

                case Engine.Attributes.Meta.DisplayControlTypes.FolderSelector:
                    ctrl = BuildPropertyFolderSelector(pi);
                    break;

                default:
                    throw new ArgumentException(String.Format("Either the control type is set to 'None' or the type \"{0}\" is not supported in the VisualBuilder", pi.DisplayControlType.ToString()));

            }

            FlowPanelContainer.Children.Add(ctrl);

            // since getting control by name seems not to work, can retrieve later by index from pi instance.
            pi.Index = FlowPanelContainer.Children.Count - 1;
        }

        private System.Windows.Controls.UserControl BuildPropertyFolderSelector(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.PropertyFolderSelector pfs = new PropertyPage.PropertyFolderSelector();
            pfs.Name = pi.ActualPropertyName;
            pfs.CommonContent.PropertyName = pi.ActualPropertyName;
            pfs.CommonContent.LabelText = pi.DisplayName;
            pfs.CommonContent.DataType = pi.DataType;
            pfs.UpdateVisual();

            t_properties.Add(pi.ActualPropertyName, new Engine.Attributes.StringAttribute());

            if (pi.ValidatorType != Engine.Attributes.Meta.ValidatorTypes.Undefined)
            {
                switch (pi.ValidatorType)
                {
                    case Engine.Attributes.Meta.ValidatorTypes.StringNotEmpty:
                        pfs.CommonContent.Validator = new Engine.Validators.StringValidator();
                        break;
                    default:
                        throw new Exception(String.Format("In VisualPropertyPage, the validator type '{0}' is not supported", pi.ValidatorType));
                }
            }

            return pfs;
        }

        private System.Windows.Controls.UserControl BuildPropertyIntBox(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.PropertyIntBox pib = new PropertyIntBox();
            pib.Name = pi.ActualPropertyName;
            pib.CommonContent.PropertyName = pi.ActualPropertyName;
            pib.CommonContent.LabelText = pi.DisplayName;
            pib.CommonContent.DataType = pi.DataType;
            pib.UpdateVisual();

            t_properties.Add(pi.ActualPropertyName, new Engine.Attributes.IntAttribute());

            if (pi.ValidatorType != Engine.Attributes.Meta.ValidatorTypes.Undefined)
            {
                switch (pi.ValidatorType)
                {
                    case Engine.Attributes.Meta.ValidatorTypes.Int:
                        pib.CommonContent.Validator = new Engine.Validators.IntValidator();
                        break;
                    default:
                        throw new Exception(String.Format("In VisualPropertyPage, the validator type '{0}' is not supported.", pi.ValidatorType));
                }
            }

            return pib;
        }

        private System.Windows.Controls.UserControl BuildPropertyDoubleBox(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.PropertyDoubleBox pdb = new PropertyDoubleBox();
            pdb.Name = pi.ActualPropertyName;
            pdb.CommonContent.PropertyName = pi.ActualPropertyName;
            pdb.CommonContent.LabelText = pi.DisplayName;
            pdb.CommonContent.DataType = pi.DataType;
            pdb.UpdateVisual();

            t_properties.Add(pi.ActualPropertyName, new Engine.Attributes.DoubleAttribute());

            if (pi.ValidatorType != Engine.Attributes.Meta.ValidatorTypes.Undefined)
            {
                switch (pi.ValidatorType)
                {
                    case Engine.Attributes.Meta.ValidatorTypes.Double:
                        pdb.CommonContent.Validator = new Engine.Validators.DoubleValidator();
                        break;
                    default:
                        throw new Exception(String.Format("In VisualPropertyPage, the validator type '{0}' is not supported", pi.ValidatorType));
                }
            }

            return pdb;
        }

        private System.Windows.Controls.UserControl BuildPropertyRadioButtons(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.PropertyRadioButtons prb = new PropertyPage.PropertyRadioButtons();
            prb.Name = pi.ActualPropertyName;
            prb.CommonContent.PropertyName = pi.ActualPropertyName;
            prb.CommonContent.LabelText = pi.DisplayName;
            prb.CommonContent.DataType = pi.DataType;
            prb.CommonContent.ValueList = pi.ValueList;
            prb.CommonContent.DefaultValue = pi.DefaultValue;

            // in this case we only store the selected value for future usage
            t_properties.Add(pi.ActualPropertyName, new Engine.Attributes.IntAttribute());

            prb.UpdateVisual();

            if (pi.ValidatorType != Engine.Attributes.Meta.ValidatorTypes.Undefined)
            {
                switch (pi.ValidatorType)
                {
                    case Engine.Attributes.Meta.ValidatorTypes.ValueList:
                        prb.CommonContent.Validator = new Engine.Validators.ValueListValidator(pi.ValueList);
                        break;
                    default:
                        throw new Exception(String.Format("In VisualPropertyPage, the validator type '{0}' is not supported.", pi.ValidatorType));
                }
            }

            return prb;
        }

        private System.Windows.Controls.UserControl BuildPropertyTextBox(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.PropertyTextBox ptb = new PropertyTextBox();
            ptb.Name = pi.ActualPropertyName;
            ptb.CommonContent.PropertyName = pi.ActualPropertyName;
            ptb.CommonContent.LabelText = pi.DisplayName;
            ptb.CommonContent.DataType = pi.DataType;
            ptb.UpdateVisual();

            t_properties.Add(pi.ActualPropertyName, new Engine.Attributes.StringAttribute());

            if (pi.ValidatorType != Engine.Attributes.Meta.ValidatorTypes.Undefined)
            {
                switch (pi.ValidatorType)
                {
                    case Engine.Attributes.Meta.ValidatorTypes.StringNotEmpty:
                        Engine.Validators.StringValidator strValid = new Engine.Validators.StringValidator(pi.RegularExpression);
                        strValid.CannotBeEmpty = true;
                        ptb.CommonContent.Validator = strValid;
                        break;
                    default:
                        throw new Exception(String.Format("In VisualPropertyPage, the validator type '{0}' is not supported", pi.ValidatorType));
                }
            }

            return ptb;
        }
        #endregion // Build

        #region Fill
        /// <summary>
        /// Retrieves values passed to tool of effect and put them in controls. This is used when user changes active drawing board
        /// </summary>
        /// <param name="collectedValues">The collected VisualPropertyPage values saved in the tool or effect corresponding to the current drawing board</param>
        public void Fill(Engine.Attributes.AttributeCollection collectedValues)
        {
            // the following should never happen if Build() is called before Fill() in VisualPropertyPageManager
            if (t_visualProperties == null)
            {
                return;
            }

            t_properties = collectedValues;

            Dictionary<string, Engine.Effects.VisualPropertyItem> items = t_visualProperties.GetItems();

            foreach (KeyValuePair<string, Engine.Effects.VisualPropertyItem> key in items)
            {
                FillControl(key);
            }
        }

        private void FillControl(KeyValuePair<string, Engine.Effects.VisualPropertyItem> key)
        {
            Engine.Effects.VisualPropertyItem pi = key.Value;

            switch (pi.DisplayControlType)
            {
                case Engine.Attributes.Meta.DisplayControlTypes.Textbox:
                    switch (pi.DataType)
                    {
                        case Engine.PropertyDataTypes.Int:
                            FillPropertyIntBox(pi);
                            break;

                        case Engine.PropertyDataTypes.Double:
                            FillPropertyDoubleBox(pi);
                            break;

                        case Engine.PropertyDataTypes.Text:
                            FillPropertyTextBox(pi);
                            break;

                        default:
                            throw new Exception();
                    }

                    break;

                case Engine.Attributes.Meta.DisplayControlTypes.RadioButtons:
                    FillPropertyRadioButtons(pi);
                    break;

                case Engine.Attributes.Meta.DisplayControlTypes.FolderSelector:
                    FillPropertyFolderSelector(pi);
                    break;

                default:
                    throw new ArgumentException(String.Format("In VisualPropertyPage.FillControl() : either the control type is set to 'None' or the type \"{0}\" is not supported in the VisualBuilder", pi.DisplayControlType.ToString()));

            }
        }

        private void FillPropertyIntBox(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.PropertyIntBox pib = (PaintualUI.Controls.PropertyPage.PropertyIntBox)this.FlowPanelContainer.Children[pi.Index];

            // get collected values from tool or effect
            Engine.Attributes.IAttribute attr = t_properties.Get(pi.ActualPropertyName);

            pib.TextBox.Text = ((int)attr.Value).ToString();
        }

        private void FillPropertyDoubleBox(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.PropertyDoubleBox pdb = (PaintualUI.Controls.PropertyPage.PropertyDoubleBox)this.FlowPanelContainer.Children[pi.Index];

            // get collected values from tool or effect
            Engine.Attributes.IAttribute attr = t_properties.Get(pi.ActualPropertyName);

            pdb.TextBox.Text = ((double)attr.Value).ToString();
        }

        private void FillPropertyTextBox(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.PropertyTextBox ptb = (PaintualUI.Controls.PropertyPage.PropertyTextBox)this.FlowPanelContainer.Children[pi.Index];

            // get collected values from tool or effect
            Engine.Attributes.IAttribute attr = t_properties.Get(pi.ActualPropertyName);

            ptb.TextBox.Text = (string)attr.Value;
        }

        private void FillPropertyRadioButtons(Engine.Effects.VisualPropertyItem pi)
        {

        }

        private void FillPropertyFolderSelector(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.PropertyFolderSelector pfs = (PaintualUI.Controls.PropertyPage.PropertyFolderSelector)this.FlowPanelContainer.Children[pi.Index];

            // get collected values from tool or effect
            Engine.Attributes.IAttribute attr = t_properties.Get(pi.ActualPropertyName);

            pfs.TextBox.Text = (string)attr.Value;
        }


        #endregion // Fill

        #region Process
        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            if (t_workflow.CurrentActivity == null)
            {
                MessageBox.Show("No effect or tool selected.");
                return;
            }

            bool hasErrors = false;

            if (t_workflow.CurrentEffect != null)
            {
                hasErrors = ProcessEffect(sender, e);

                if (!hasErrors)
                {
                    t_workflow.CurrentEffect.Process();
                }

                return;
            }

            if (t_workflow.CurrentTool != null)
            {
                t_workflow.CurrentTool.HasErrors = ProcessTool(sender, e);

                return;
            }
        }

        private bool ProcessEffect(object sender, RoutedEventArgs e)
        {
            Engine.Effects.EffectBase effect = t_workflow.CurrentEffect;
            Type effectType = effect.GetType();

            bool hasErrors = false;

            foreach (System.Windows.UIElement uie in FlowPanelContainer.Children)
            {
                IPropertyControl ipc = (IPropertyControl)uie;

                string propertyActualName = ipc.CommonContent.PropertyName; // i.e.: "Seed"
                object validatedValue = null;

                if (ipc.CommonContent.Validator != null)
                {
                    ipc.CommonContent.Validator.InputValue = ipc.EnteredValue;
                    bool result = ipc.CommonContent.Validator.Validate();

                    if (!result)
                    {
                        ipc.SignalError(ipc.CommonContent.Validator.ErrorMessage);
                        hasErrors = true;
                        continue;
                    }

                    validatedValue = ((Engine.Validators.IValidated)ipc.CommonContent.Validator).Validated;

                    // there is a special case where the selected value is an enum, this is being handled by the IntAttribute class
                    t_properties.Get(propertyActualName).SetValue(validatedValue); 
                }

                PropertyInfo prop = effectType.GetProperty(propertyActualName, BindingFlags.Public | BindingFlags.Instance);

                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(effect, validatedValue, null);
                }
            }

            effect.CollectedPropertyValues = t_properties;

            return hasErrors;
        }

        private bool ProcessTool(object sender, RoutedEventArgs e)
        {
            Engine.Tools.Tool tool = t_workflow.CurrentTool;
            Type toolType = tool.GetType();

            bool hasErrors = false;

            foreach (System.Windows.UIElement uie in FlowPanelContainer.Children)
            {
                IPropertyControl ipc = (IPropertyControl)uie;

                // required when user has entered invalid value, signals are being displayed. when user enters corrected values
                // the validation process is executed again and in case of success, no signal must be displayed.
                ipc.ClearSignals();

                string propertyActualName = ipc.CommonContent.PropertyName; // i.e.: "Seed"
                object validatedValue = null;

                if (ipc.CommonContent.Validator != null)
                {
                    ipc.CommonContent.Validator.InputValue = ipc.EnteredValue;
                    bool result = ipc.CommonContent.Validator.Validate();

                    if (!result)
                    {
                        ipc.SignalError(ipc.CommonContent.Validator.ErrorMessage);
                        hasErrors = true;
                        continue;
                    }

                    validatedValue = ((Engine.Validators.IValidated)ipc.CommonContent.Validator).Validated;

                    // there is a special case where the selected value is an enum, this is being handled by the IntAttribute class
                    t_properties.Get(propertyActualName).SetValue(validatedValue);
                }

                PropertyInfo prop = toolType.GetProperty(propertyActualName, BindingFlags.Public | BindingFlags.Instance);

                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(tool, validatedValue, null);
                }
            }

            tool.CollectedPropertyValues = t_properties;

            return hasErrors;

            // tool.Process would call the sequence to draw on the canvas
            // here we only needed to pass to the tool the properties set by user in the UI
            //tool.Process();
        }
        #endregion // Process

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Operation not activated yet.");
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            Engine.Calc.SizeComparison scomp = Engine.Calc.Math.IsWiderOrHigher(sizeInfo.NewSize.Width, sizeInfo.NewSize.Height);

            switch (scomp)
            {
                case Engine.Calc.SizeComparison.IsWider:
                    FlowPanelContainer.HorizontalAlignment = HorizontalAlignment.Left;
                    FlowPanelContainer.VerticalAlignment = VerticalAlignment.Top;
                    ButtonsPanelContainer.HorizontalAlignment = HorizontalAlignment.Right;
                    ButtonsPanelContainer.VerticalAlignment = VerticalAlignment.Top;
                    break;

                case Engine.Calc.SizeComparison.IsHeigher:
                    FlowPanelContainer.HorizontalAlignment = HorizontalAlignment.Left;
                    FlowPanelContainer.VerticalAlignment = VerticalAlignment.Top;
                    ButtonsPanelContainer.HorizontalAlignment = HorizontalAlignment.Right;
                    ButtonsPanelContainer.VerticalAlignment = VerticalAlignment.Bottom;
                    break;
            }
        }
    }
}


