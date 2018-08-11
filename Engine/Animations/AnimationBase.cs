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

namespace Engine.Animations
{
    public class AnimationBase : Engine.Tools.IGraphicActivity
    {

        // TODO : find a way to display/merge multiple images on screen. Animation creates full size images
        // of a one shot effect sequence. Then those sequences are put one on top of the other as layers
        // each layer has alpha to 0 to see through. When another sequence is added on top, one is removed at the bottom
        // the whole layer bunch is captured as image 001 002 003 etc and then a movie program can bind them together as .mp4


        protected Engine.Surface.Canvas t_imageSource;
        protected Engine.Surface.Canvas t_imageProcessed;
        protected bool t_isImageProcessed = false;

        protected Engine.Effects.VisualProperties t_visualProperties;

        /// <summary>
        /// stores values entered in UI allowing user to switch from drawingboard to drawingboard without need to
        /// reenter property values.
        /// </summary>
        protected Engine.Attributes.AttributeCollection t_collectedAttributeValues;

        protected Engine.Workflow t_workflow;

        public AnimationBase()
        {
            t_visualProperties = new Engine.Effects.VisualProperties("Animation Base", typeof(AnimationBase));
        }

        public void Initialize(Engine.Workflow w)
        {
            t_workflow = w;
            t_imageSource = w.Canvas;
        }

        public virtual Engine.Effects.VisualProperties GetVisualProperties()
        {
            t_visualProperties.Fill();
            return t_visualProperties;
        }

        /// <summary>
        /// Allows an effect to do some work before the actual process (compatibility with tool)
        /// </summary>
        public virtual void PreProcess()
        {

        }

        /// <summary>
        /// Processes the effect on a copy of the current image stored in the Workflow,
        /// then updates the workflow image
        /// </summary>
        /// <remarks>This method is called at the end of Process() in every class that inherits EffectBase.</remarks>
        public virtual void Process()
        {

        }

        // to be called by UI because Effect is being processed in separate thread
        public virtual void ProcessCompleted()
        {
            /// there is a bug here. Sometimes the final image is not refreshed on screen due to bad synchronization
            /// between threads and last image updated on screen. see solution in Effects
            /*t_isImageProcessed = true;

            t_workflow.SetImage(t_imageProcessed, false);


            // leaving a little time to thread stuff to be cleaned before updating final image
            System.Threading.Thread.Sleep(100);
            t_workflow.DisallowInvalidate();
            RaiseProcessEnded();*/
        }


        /// <summary>
        /// Creates a new instance of an effect that will be attached to another Workflow.
        /// </summary>
        /// <param name="w">The workflow to which a copy of this effect is to be applied to.</param>
        /// <returns></returns>
        public virtual Engine.Tools.IGraphicActivity Duplicate(Engine.Workflow w)
        {
            AnimationBase ab = new AnimationBase();
            ab.Initialize(w);

            return ab;
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

        public virtual string Name { get => "AnimationBase"; }

        public int Width
        {
            get { return t_imageSource.Width; }
        }

        public int Height
        {
            get { return t_imageSource.Height; }
        }
    }
}
