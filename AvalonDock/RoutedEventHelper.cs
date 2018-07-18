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
using System.Linq;
using System.Text;

namespace AvalonDock
{
    using System;
    using System.Windows;

    internal static class RoutedEventHelper
    {
        #region RoutedEvent Helper Methods

        /// <summary>
        /// A static helper method to raise a routed event on a target UIElement or ContentElement.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        /// <param name="args">RoutedEventArgs to use when raising the event</param>
        internal static void RaiseEvent(DependencyObject target, RoutedEventArgs args)
        {
            if (target is UIElement)
            {
                (target as UIElement).RaiseEvent(args);
            }
            else if (target is ContentElement)
            {
                (target as ContentElement).RaiseEvent(args);
            }
        }

        /// <summary>
        /// A static helper method that adds a handler for a routed event 
        /// to a target UIElement or ContentElement.
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="routedEvent">Event that will be handled</param>
        /// <param name="handler">Event handler to be added</param>
        internal static void AddHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
        {
            UIElement uie = element as UIElement;
            if (uie != null)
            {
                uie.AddHandler(routedEvent, handler);
            }
            else
            {
                ContentElement ce = element as ContentElement;
                if (ce != null)
                {
                    ce.AddHandler(routedEvent, handler);
                }
            }
        }

        /// <summary>
        /// A static helper method that removes a handler for a routed event 
        /// from a target UIElement or ContentElement.
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="routedEvent">Event that will no longer be handled</param>
        /// <param name="handler">Event handler to be removed</param>
        internal static void RemoveHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
        {
            UIElement uie = element as UIElement;
            if (uie != null)
            {
                uie.RemoveHandler(routedEvent, handler);
            }
            else
            {
                ContentElement ce = element as ContentElement;
                if (ce != null)
                {
                    ce.RemoveHandler(routedEvent, handler);
                }
            }
        }

        #endregion
    }
}
