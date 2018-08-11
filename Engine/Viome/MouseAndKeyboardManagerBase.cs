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
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal class MouseAndKeyboardManagerBase
    {
        protected Engine.Viome t_workflow;

        public MouseAndKeyboardManagerBase(Engine.Viome w)
        {
            t_workflow = w;
        }

        internal void FeedMouseAction(Engine.MousePoint e)
        {
            if (t_workflow.CurrentDrawingBoardMode == DrawingBoardModes.Disabled)
            {
                return;
            }

            // correct coords relative to image position and zoom
            Engine.MousePoint correctedPoint = t_workflow.CoordinatesManager.MousePointRelativeToImagePositionAndZoom(e);

            switch (e.MouseAction)
            {
                case Engine.MouseActionType.MouseDown:
                    switch (t_workflow.CurrentDrawingBoardMode)
                    {
                        case DrawingBoardModes.None:
                        case DrawingBoardModes.SuspendDraw:
                            t_workflow.CurrentDrawingBoardMode = DrawingBoardModes.Draw;
                            ActivityPreProcess(correctedPoint);
                            // no threaded because drawing action would start before canvas is allowed to refresh its content.
                            t_workflow.AllowInvalidate();
                            ActivityProcess(correctedPoint);
                            break;
                    }
                    break;

                case Engine.MouseActionType.MouseMove:
                    switch (t_workflow.CurrentDrawingBoardMode)
                    {
                        case DrawingBoardModes.Draw:
                            ActivityProcess(correctedPoint);
                            break;
                    }
                    break;


                case Engine.MouseActionType.MouseUp:
                    t_workflow.CurrentDrawingBoardMode = DrawingBoardModes.SuspendDraw;

                    // should find a way to force display of final brush stroke although it will be shown
                    // by PaintualCanvas at next tick of timer unless t_allowInvalidate is set to false before

                    //t_motionAttribute.ThisIsLastMousePoint();
                    t_workflow.ThreadingQueue.RunAndForget(new Action(t_workflow.MotionAttribute.Clear)); // <MousePoint, int>(t_motionAttribute.AddMousePoint, correctedPoint);
                    t_workflow.ThreadingQueue.RunAndForget(new Action(t_workflow.DisallowInvalidate));
                    break;

                default:

                    break;
            }
        }

        internal void FeedKeyCode(KeyEventArgs e)
        {
            /*this.keyEventArgs = e;

            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (t_currentDrawingBoardMode == DrawingBoardModes.Pan || t_currentDrawingBoardMode == DrawingBoardModes.SuspendPan)
                    {
                        t_currentDrawingBoardMode = DrawingBoardModes.None;
                    }
                    else
                    {
                        t_currentDrawingBoardMode = DrawingBoardModes.Pan;
                    }
                    break;

                case Keys.B:
                    EnsureToolExists();

                    if (this.t_tool.GetType() == typeof(Engine.Tools.CanvasBrushTool))
                    {
                        RaiseToolAttributeRequested(WorkflowToolRequestType.CanvasAndMouseLocationRequest);
                    }

                    t_currentDrawingBoardMode = DrawingBoardModes.SuspendDraw;
                    break;
                case Keys.N:
                    t_currentDrawingBoardMode = DrawingBoardModes.None;
                    break;
                case Keys.Z:
                    if (e.Shift)
                    {
                        RaiseDrawingBoardActionRequested(WorkflowDrawingBoardRequestType.ZoomOut);
                    }
                    else
                    {
                        RaiseDrawingBoardActionRequested(WorkflowDrawingBoardRequestType.ZoomIn);
                    }
                    break;
                case Keys.R:
                    if (this.t_tool.GetType().FullName == "Engine.Tools.ImagesBrushTool"
                        || this.t_tool.GetType().FullName == "Engine.Tools.CanvasBrushTool")
                    {
                        throw new NotImplementedException();
                        //this.t_tool.Image.Rotate(20);
                    }
                    break;
                default:
                    break;
            }*/
        }

        private void ActivityProcess(Engine.MousePoint correctedPoint)
        {
            t_workflow.ThreadingQueue.RunAndReturn<MousePoint, int>(t_workflow.MotionAttribute.AddMousePoint, correctedPoint);
            t_workflow.ThreadingQueue.RunAndForget(t_workflow.CurrentActivity.Process);
        }

        private void ActivityPreProcess(Engine.MousePoint correctedPoint)
        {
            t_workflow.ThreadingQueue.RunAndReturn<MousePoint, int>(t_workflow.MotionAttribute.AddMousePoint, correctedPoint);
            t_workflow.ThreadingQueue.RunAndForget(t_workflow.CurrentActivity.PreProcess);
        }
    }
}
