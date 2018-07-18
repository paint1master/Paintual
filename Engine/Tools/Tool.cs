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
using Engine.Surface;

namespace Engine.Tools
{
    public abstract class Tool : IGraphicActivity
    {
        protected Engine.Viome t_VIOM;

        protected Engine.Surface.Canvas t_imageSource;
        protected Engine.Surface.Canvas t_imageProcessed;

        protected Engine.Effects.VisualProperties t_visualProperties;

        protected Engine.Attributes.AttributeCollection t_attributeCollection;

        // value set by the ValuePropertyPage: when a user has not entered all necessary values in property controls,
        // the tool cannot proceed
        protected bool t_hasErrors = false;

        /// <summary>
        /// stores values entered in UI allowing user to switch from drawingboard to drawingboard without need to
        /// reenter property values.
        /// </summary>
        protected Engine.Attributes.AttributeCollection t_collectedAttributeValues;

        public virtual void Initialize(Engine.Viome w)
        {
            t_VIOM = w;
            t_attributeCollection = new Attributes.AttributeCollection();
            t_imageSource = t_VIOM.Canvas;

            // clear the drawing surface of all work layers created by previous tools or effects
            t_VIOM.SelectionGlassRequest(SelectionGlassRequestType.Delete);
        }

        /// <summary>
        /// Called by the Workflow when MouseDown event is received by UI
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public abstract void BeforeDraw(int x, int y);

        public virtual int Draw(List<MousePoint> points)
        {
            foreach (MousePoint p in points)
            {
                Draw(p);
            }

            // a return value is needed for the BackgroundQueue
            return 0;
        }

        /// <summary>
        /// The drawing procedure
        /// </summary>
        public virtual void Process()
        {
            if (t_VIOM.MotionAttribute.PointCount == 0)
            {
                return;
            }

            List<MousePoint> points = t_VIOM.MotionAttribute.LinearInterpolate();

            foreach (MousePoint p in points)
            {
                Draw(p);
            }
        }

        internal virtual void Draw(MousePoint p)
        {

        }

        /// <summary>
        /// Called by the Workflow when MouseUp event is received by UI. Allows tool to do any post processing
        /// before final commit to surface
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public abstract int AfterDraw(Point p);

        public virtual Engine.Attributes.AttributeCollection CollectedPropertyValues
        {
            get { return t_collectedAttributeValues; }
            set { t_collectedAttributeValues = value; }            
        }

        public abstract IGraphicActivity Duplicate(Viome w);

        public virtual Engine.Effects.VisualProperties GetVisualProperties()
        {
            t_visualProperties.Fill();
            return t_visualProperties;
        }

        public Engine.Surface.Canvas ImageSource
        {
            get { return t_imageSource; }
        }

        public Canvas ImageProcessed
        {
            get { return t_imageProcessed; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>The AttributeCollection instance is not directly accessible because each Tool can expose specific
        /// Attributes as properties through the VisualProperties mechanism.</remarks>
        protected Engine.Attributes.AttributeCollection Attributes
        {
            get { return t_attributeCollection; }
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

        public virtual bool HasErrors
        {
            get { return t_hasErrors; }
            set { t_hasErrors = value; }
        }
    }
}
