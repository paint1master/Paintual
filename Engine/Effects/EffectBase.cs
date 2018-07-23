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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Engine.Effects
{
    /// <summary>
    /// The base class for all effects
    /// </summary>
    public class EffectBase : Engine.Tools.IGraphicActivity
    {
        protected Engine.Surface.Canvas t_imageSource;
        protected Engine.Surface.Canvas t_imageProcessed;
        protected bool t_isImageProcessed = false;

        protected Engine.Effects.VisualProperties t_visualProperties;

        /// <summary>
        /// stores values entered in UI allowing user to switch from drawingboard to drawingboard without need to
        /// reenter property values.
        /// </summary>
        protected Engine.Attributes.AttributeCollection t_collectedAttributeValues;

        protected Engine.Viome t_workflow;

        public EffectBase()
        {
            t_visualProperties = new VisualProperties("Effect Base", typeof(EffectBase));
        }

        public void Initialize(Viome w)
        {
            t_workflow = w;
            t_imageSource = w.Canvas;
        }

        public virtual VisualProperties GetVisualProperties()
        {
            t_visualProperties.Fill();
            return t_visualProperties;
        }

        /// <summary>
        /// Processes the effect on a copy of the current image stored in the Workflow,
        /// then updates the workflow image
        /// </summary>
        public virtual void Process()
        {
            t_isImageProcessed = true;
            t_workflow.SetImage(t_imageProcessed, true);
        }

        /// <summary>
        /// Creates a new instance of an effect that will be attached to another Workflow.
        /// </summary>
        /// <param name="w">The workflow to which a copy of this effect is to be applied to.</param>
        /// <returns></returns>
        public virtual Engine.Tools.IGraphicActivity Duplicate(Engine.Viome w)
        {
            EffectBase eb = new EffectBase();
            eb.Initialize(w);

            return eb;
        }

        public virtual Engine.Attributes.AttributeCollection CollectedPropertyValues
        {
            get { return t_collectedAttributeValues; }
            set { t_collectedAttributeValues = value; }
        }

        public Engine.Surface.Canvas ImageSource
        {
            get { return t_imageSource; }
            set
            {
                t_imageSource = value;
            }
        }

        public Engine.Surface.Canvas ImageProcessed
        {
            get { return t_imageProcessed; }
        }

        public bool IsImageProcessed
        {
            get { return t_isImageProcessed; }
        }

        public bool HasVisualProperties
        {
            get
            {
                if (t_visualProperties != null && t_visualProperties.Count > 0)
                {
                    return true;
                }

                return false;
            }
        }

        public int Width
        {
            get { return t_imageSource.Width; }
        }

        public int Height
        {
            get { return t_imageSource.Height; }
        }

        // TODO : does EffectBase need a Dispose() ? yes, dispose of VisualProperties
    }
}