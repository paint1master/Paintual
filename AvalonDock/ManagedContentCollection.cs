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
using System.Collections.ObjectModel;

namespace AvalonDock
{
    public class ManagedContentCollection<T> : ReadOnlyObservableCollection<T> where T : ManagedContent
    {
        internal ManagedContentCollection(DockingManager manager)
            : base(new ObservableCollection<T>())
        {
            Manager = manager;
        }


        /// <summary>
        /// Get associated <see cref="DockingManager"/> object
        /// </summary>
        public DockingManager Manager { get; private set; }

        /// <summary>
        /// Override collection changed event to setup manager property on <see cref="ManagedContent"/> objects
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (T cntAdded in e.NewItems)
                    cntAdded.Manager = Manager;
            }

            base.OnCollectionChanged(e);
        }

        /// <summary>
        /// Add a content to the list
        /// </summary>
        /// <param name="contentToAdd"></param>
        internal void Add(T contentToAdd)
        {
            if (!Items.Contains(contentToAdd))
                Items.Add(contentToAdd);
        }

        internal void Remove(T contentToRemove)
        {
            Items.Remove(contentToRemove);
        }
    }
}
