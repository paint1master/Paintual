﻿/**********************************************************

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
        private Engine.Workflow t_workflow;

        /// <summary>
        /// Remembers the property values entered by the user so when the user switches from drawing board to drawing board the UI of the
        /// VisualPropertyPage can be repopulated with the corresponding values.
        /// </summary>
        private Engine.Attributes.AttributeCollection t_properties;

        /// <summary>
        /// Flag set by the Build() method when all controls have been created to allow the tool, effect, or animation to start
        /// immediately so the user does not need to specifically click Apply button when the VPP is loaded with default values.
        /// </summary>
        private bool t_canAutoApply = false;

        public VisualPropertyPage()
        {
            InitializeComponent();

            /// this is now required because we used call .UpdateVisual() on all templated controls
            /// when creating them but now we need to call that method only when the VisualPropertyPage is fully loaded
            /// because templated controls are fully instantiated later than user controls (!!??!!)
            this.Loaded += VisualPropertyPage_Loaded; 
        }

        private void VisualPropertyPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateControlVisuals();

            if (t_canAutoApply)
            {
                AutoApply(true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        /// <remarks>Called by the VisualPropertyPageManager</remarks>
        internal void Build(Engine.Workflow w)
        {
            Clear();

            t_workflow = w;

            if (w.GraphicActivity == null)
            {
                return;
            }

            Engine.Tools.IGraphicActivity activity = w.GraphicActivity;
            Engine.Effects.VisualProperties vp = activity.GetVisualProperties();


            // this can happen for tools or effects that have no visual properties
            if (vp == null)
            {
                return;
            }

            if (vp.Count == 0)
            {
                // ensure no properties are left from a previous workflow-document
                t_properties = null;
                t_canAutoApply = false;

                return;
            }

            t_properties = new Engine.Attributes.AttributeCollection();

            Dictionary<string, Engine.Effects.VisualPropertyItem> items = vp.GetItems();

            foreach (KeyValuePair<string, Engine.Effects.VisualPropertyItem> key in items)
            {
                BuildControl(key);
            }

            t_canAutoApply = true;
        }

        /// <summary>
        /// Removes all controls from the FlowPanelContainer
        /// </summary>
        private void Clear()
        {
            // explicitly removes children controls and set them to null
            while (FlowPanelContainer.Children.Count > 0)
            {
                UIElement uie = FlowPanelContainer.Children[0];
                FlowPanelContainer.Children.Remove(uie);
                uie = null;
            }

            t_properties = null;
        }

        #region Build
        private void BuildControl(KeyValuePair<string, Engine.Effects.VisualPropertyItem> key)
        {
            Engine.Effects.VisualPropertyItem pi = key.Value;

            PaintualUI.Controls.PropertyPage.TPropertyControl ctrl = null;

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

                case Engine.Attributes.Meta.DisplayControlTypes.ColorVariance:
                    ctrl = BuildPropertyObject(pi);
                    break;

                case Engine.Attributes.Meta.DisplayControlTypes.RadioButtons:
                    ctrl = BuildPropertyRadioButtons(pi);
                    break;

                case Engine.Attributes.Meta.DisplayControlTypes.FolderSelector:
                    ctrl = BuildPropertyFolderSelector(pi);
                    break;

                case Engine.Attributes.Meta.DisplayControlTypes.Checkbox:
                    ctrl = BuildPropertyCheckBox(pi);
                    break;

                default:
                    throw new ArgumentException(String.Format("Either the control type is set to 'None' or the type \"{0}\" is not supported in the VisualBuilder", pi.DisplayControlType.ToString()));

            }

            FlowPanelContainer.Children.Add(ctrl);

            // since getting control by name seems not to work, can retrieve later by index from pi instance.
            pi.Index = FlowPanelContainer.Children.Count - 1;
        }

        private PaintualUI.Controls.PropertyPage.TPropertyControl BuildPropertyFolderSelector(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.TPropertyFolderSelector pfs = new PropertyPage.TPropertyFolderSelector();
            pfs.BuildControl(pi);

            t_properties.Add(pi.ActualPropertyName, new Engine.Attributes.StringAttribute());

            return pfs;
        }

        private PaintualUI.Controls.PropertyPage.TPropertyControl BuildPropertyIntBox(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.TPropertyIntBox pib = new TPropertyIntBox();
            pib.BuildControl(pi);

            t_properties.Add(pi.ActualPropertyName, new Engine.Attributes.IntAttribute());

            return pib;
        }

        private PaintualUI.Controls.PropertyPage.TPropertyControl BuildPropertyDoubleBox(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.TPropertyDoubleBox pdb = new TPropertyDoubleBox();
            pdb.BuildControl(pi);

            t_properties.Add(pi.ActualPropertyName, new Engine.Attributes.DoubleAttribute());

            return pdb;
        }

        private PaintualUI.Controls.PropertyPage.TPropertyControl BuildPropertyRadioButtons(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.TPropertyRadioButtons prb = new PropertyPage.TPropertyRadioButtons();
            prb.BuildControl(pi);

            // in this case we only store the selected value for future usage
            t_properties.Add(pi.ActualPropertyName, new Engine.Attributes.IntAttribute());

            return prb;
        }

        private PaintualUI.Controls.PropertyPage.TPropertyControl BuildPropertyTextBox(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.TPropertyTextBox ptb = new TPropertyTextBox();
            ptb.BuildControl(pi);

            t_properties.Add(pi.ActualPropertyName, new Engine.Attributes.StringAttribute());

            return ptb;
        }

        private PaintualUI.Controls.PropertyPage.TPropertyControl BuildPropertyCheckBox(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.TPropertyCheckBox ctrl = new TPropertyCheckBox();
            ctrl.BuildControl(pi);

            return ctrl;
        }

        private PaintualUI.Controls.PropertyPage.TPropertyControl BuildPropertyObject(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.TPropertyControl ctrl;

            switch (pi.TypeDeclaration.Name)
            {
                case "ColorVariance":
                    ctrl = new PaintualUI.Controls.PropertyPage.TPropertyColorVariance();
                    ctrl.BuildControl(pi);
                    break;
                default:
                    throw new Exception(String.Format("In VisualPropertyPage, the type declaration '{0}' is not supported.", pi.TypeDeclaration.Name));

            }

            return ctrl;
        }

        private void UpdateControlVisuals()
        {
            foreach (UIElement uie in FlowPanelContainer.Children)
            {
                PaintualUI.Controls.PropertyPage.TPropertyControl tpc = (PaintualUI.Controls.PropertyPage.TPropertyControl)uie;

                if (tpc != null)
                {
                    tpc.UpdateVisual();
                }
            }
        }
        #endregion // Build



        #region Fill
        /// <summary>
        /// Retrieves values passed to tool of effect and put them in controls. This is used when user changes active drawing board
        /// </summary>
        /// <param name="collectedValues">The collected VisualPropertyPage values saved in the tool or effect corresponding to the current drawing board</param>
        public void Fill(Engine.Workflow w)
        {
            Engine.Effects.VisualProperties vp = w.GraphicActivity.GetVisualProperties();

            if (vp == null)
            {
                return;
            }

            //t_properties = collectedValues;

            Dictionary<string, Engine.Effects.VisualPropertyItem> items = vp.GetItems();

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
            PaintualUI.Controls.PropertyPage.TPropertyIntBox pib = (PaintualUI.Controls.PropertyPage.TPropertyIntBox)this.FlowPanelContainer.Children[pi.Index];

            // get collected values from tool or effect
            Engine.Attributes.IAttribute attr = t_properties.Get(pi.ActualPropertyName);

            pib.DefaultValue = ((int)attr.Value).ToString();
            pib.UpdateVisual();
        }

        private void FillPropertyDoubleBox(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.TPropertyDoubleBox pdb = (PaintualUI.Controls.PropertyPage.TPropertyDoubleBox)this.FlowPanelContainer.Children[pi.Index];

            // get collected values from tool or effect
            Engine.Attributes.IAttribute attr = t_properties.Get(pi.ActualPropertyName);

            pdb.DefaultValue = ((double)attr.Value).ToString();
            pdb.UpdateVisual();
        }

        private void FillPropertyTextBox(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.TPropertyTextBox ptb = (PaintualUI.Controls.PropertyPage.TPropertyTextBox)this.FlowPanelContainer.Children[pi.Index];

            // get collected values from tool or effect
            Engine.Attributes.IAttribute attr = t_properties.Get(pi.ActualPropertyName);

            ptb.DefaultValue = (string)attr.Value;
            ptb.UpdateVisual();
        }

        private void FillPropertyRadioButtons(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.TPropertyRadioButtons prb = (PaintualUI.Controls.PropertyPage.TPropertyRadioButtons)this.FlowPanelContainer.Children[pi.Index];

            // get collected values from tool or effects
            Engine.Attributes.IAttribute attr = t_properties.Get(pi.ActualPropertyName);
            prb.DefaultValue = attr.Value;
            prb.UpdateVisual();
        }

        private void FillPropertyFolderSelector(Engine.Effects.VisualPropertyItem pi)
        {
            PaintualUI.Controls.PropertyPage.TPropertyFolderSelector pfs = (PaintualUI.Controls.PropertyPage.TPropertyFolderSelector)this.FlowPanelContainer.Children[pi.Index];

            // get collected values from tool or effect
            Engine.Attributes.IAttribute attr = t_properties.Get(pi.ActualPropertyName);

            pfs.DefaultValue = (string)attr.Value;
            pfs.UpdateVisual();
        }

        #endregion // Fill

        #region Process
        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            if (t_workflow.GraphicActivity == null)
            {
                MessageBox.Show("No effect or tool selected.");
                return;
            }

            AutoApply(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="auto">Determines whether the method is called automatically after the page has loaded or
        /// called by a user clicking on the Apply button (in the former, prevents effects to be applied).</param>
        private void AutoApply(bool auto)
        {
            bool hasErrors = false;

            // auto apply when page loaded should not launch effects.
            if(!auto)
            { 
                if (t_workflow.CurrentEffect != null)
                {
                    hasErrors = Apply();

                    if (!hasErrors)
                    {
                        t_workflow.CurrentEffect.Process();
                    }

                    return;
                }
            }

            if (t_workflow.CurrentTool != null)
            {
                t_workflow.CurrentTool.HasErrors = Apply();

                return;
            }

            t_canAutoApply = false;
        }

        /// <summary>
        /// Validates and passes the values to the selected GraphicActivity (tool, effect, animation).
        /// </summary>
        /// <returns>Indicates whether or not the validation process raised an error or not.</returns>
        private bool Apply()
        {
            Engine.Tools.IGraphicActivity ga = t_workflow.GraphicActivity;
            Type gaType = ga.GetType();

            bool hasErrors = false;

            foreach (System.Windows.UIElement uie in FlowPanelContainer.Children)
            {
                TPropertyControl ipc = (TPropertyControl)uie;

                // required when user has entered invalid value, signals are being displayed. when user enters corrected values
                // the validation process is executed again and in case of success, no signal must be displayed.
                ipc.ClearSignals();

                string propertyActualName = ipc.PropertyName; // i.e.: "Seed"
                object validatedValue = null;

                if (ipc.Validators.Count > 0)
                {
                    foreach (Engine.Validators.Validator v in ipc.Validators)
                    {
                        v.InputValue = (string)ipc.EnteredValue;
                        bool result = v.Validate();

                        if (!result)
                        {
                            ipc.SignalError(v.ErrorMessage);
                            hasErrors = true;
                            continue;
                        }
                    }

                    if (!hasErrors)
                    {
                        // using the interface IValidated which can return an object; specific validators return a value specific to the control data type
                        validatedValue = ((Engine.Validators.IValidated)ipc.Validators[ipc.Validators.Count - 1]).Validated;

                        // there is a special case where the selected value is an enum, this is being handled by the IntAttribute class
                        t_properties.Get(propertyActualName).SetValue(validatedValue);
                    }
                }
                else
                {
                    // no validator

                    validatedValue = ipc.EnteredValue;
                }

                PropertyInfo prop = gaType.GetProperty(propertyActualName, BindingFlags.Public | BindingFlags.Instance);

                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(ga, validatedValue, null);
                }
            }

            ga.CollectedPropertyValues = t_properties;

            return hasErrors;
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


