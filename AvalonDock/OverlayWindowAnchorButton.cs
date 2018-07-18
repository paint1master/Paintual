/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license , a copy thereof being
   reproduced below:

   Copyright (c) 2007-2013, Xceed Software Inc.
   All rights reserved.

   Redistribution and use in source and binary forms, with or without modification,
   are permitted provided that the following conditions are met:

   * Redistributions of source code must retain the above copyright notice,
     this list of conditions and the following disclaimer.

   * Redistributions in binary form must reproduce the above copyright notice,
     this list of conditions and the following disclaimer in the documentation 
     and/or other materials provided with the distribution.

   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
   AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
   THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
   ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
   FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
   (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
   LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED
   AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
   OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
   OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AvalonDock
{
    class OverlayWindowDockingButton : IDropSurface
    {
        OverlayWindow _owner;
        FrameworkElement _btnDock;

        public OverlayWindowDockingButton(FrameworkElement btnDock, OverlayWindow owner)
            : this(btnDock, owner, true)
        {

        }
        public OverlayWindowDockingButton(FrameworkElement btnDock, OverlayWindow owner, bool enabled)
        {
            if (btnDock == null)
                throw new ArgumentNullException("btnDock");
            if (owner == null)
                throw new ArgumentNullException("owner");

            _btnDock = btnDock;
            _owner = owner;
            Enabled = enabled;
        }

        bool _enabled = true;

        public bool Enabled
        {
            get { return _enabled; }
            set 
            {
                _enabled = value;

                if (_enabled)
                    _btnDock.Visibility = Visibility.Visible;
                else
                    _btnDock.Visibility = Visibility.Hidden;
            }
        }



        #region IDropSurface Membri di



        Rect IDropSurface.SurfaceRectangle
        {
            get
            {
                if (!(this as IDropSurface).IsSurfaceVisible)
                    return Rect.Empty;

                if (PresentationSource.FromVisual(_btnDock) == null)
                    return Rect.Empty;

                return new Rect(HelperFunc.PointToScreenWithoutFlowDirection(_btnDock, new Point()), new Size(_btnDock.ActualWidth, _btnDock.ActualHeight));
            }
        }

        void IDropSurface.OnDragEnter(Point point)
        {
            if (!Enabled)
                return;

            _owner.OnDragEnter(this, point);
        }

        void IDropSurface.OnDragOver(Point point)
        {
            if (!Enabled)
                return;

            _owner.OnDragOver(this, point);
        }

        void IDropSurface.OnDragLeave(Point point)
        {
            if (!Enabled)
                return;

            _owner.OnDragLeave(this, point);
        }

        bool IDropSurface.OnDrop(Point point)
        {
            if (!Enabled)
                return false;

            return _owner.OnDrop(this, point);
        }

        bool IDropSurface.IsSurfaceVisible
        {
            get { return (_owner.IsLoaded && _btnDock != null); }
        }

        #endregion
    }
}
