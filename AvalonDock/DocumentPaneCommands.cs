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
using System.Windows.Input;

namespace AvalonDock
{
    public sealed class DocumentPaneCommands
    {
        private static object syncRoot = new object();


        private static RoutedUICommand closeAllButThisCommand = null;
        public static RoutedUICommand CloseAllButThis
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == closeAllButThisCommand)
                    {
                        closeAllButThisCommand = new RoutedUICommand(AvalonDock.Properties.Resources.DocumentPaneCommands_CloseAllButThis, "CloseAllButThis", typeof(DocumentPaneCommands));
                    }
                }
                return closeAllButThisCommand;
            }
        }

        private static RoutedUICommand closeThisCommand = null;
        public static RoutedUICommand CloseThis
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == closeThisCommand)
                    {
                        closeThisCommand = new RoutedUICommand(AvalonDock.Properties.Resources.DocumentPaneCommands_CloseThis, "Close", typeof(DocumentPaneCommands));
                    }
                }
                return closeThisCommand;
            }
        }

        private static RoutedUICommand newHTabGroupCommand = null;
        public static RoutedUICommand NewHorizontalTabGroup
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == newHTabGroupCommand)
                    {
                        newHTabGroupCommand = new RoutedUICommand(AvalonDock.Properties.Resources.DocumentPaneCommands_NewHorizontalTabGroup, "NewHorizontalTabGroup", typeof(DocumentPaneCommands));
                    }
                }
                return newHTabGroupCommand;
            }
        }

        private static RoutedUICommand newVTabGroupCommand = null;
        public static RoutedUICommand NewVerticalTabGroup
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == newVTabGroupCommand)
                    {
                        newVTabGroupCommand = new RoutedUICommand(AvalonDock.Properties.Resources.DocumentPaneCommands_NewVerticalTabGroup, "NewVerticalTabGroup", typeof(DocumentPaneCommands));
                    }
                }
                return newVTabGroupCommand;
            }
        }


        //private static RoutedUICommand activateDocument = null;
        //public static RoutedUICommand ActivateDocument
        //{
        //    get
        //    {
        //        lock (syncRoot)
        //        {
        //            if (null == activateDocument)
        //            {
        //                activateDocument = new RoutedUICommand("Activate Document", "ActivateDocuement", typeof(DocumentPaneCommands));
        //            }
        //        }
        //        return activateDocument;
        //    }
        //}

        // #region Activate Document Command
        //public static RoutedCommand ActivateDocumentCommand = new RoutedCommand();

        //public void ExecutedActivateDocumentCommand(object sender,
        //    ExecutedRoutedEventArgs e)
        //{
        //    ManagedContent doc = e.Parameter as ManagedContent;
        //    if (doc != null)
        //    {
        //        doc.Activate();
        //    }
        //}

        //public void CanExecuteActivateDocumentCommand(object sender,
        //    CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = true;
        //}

        // #endregion

    }
}
