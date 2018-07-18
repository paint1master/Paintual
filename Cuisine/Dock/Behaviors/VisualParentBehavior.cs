/**********************************************************

Part of the Synergy code created by Ashish Kaila (https://www.codeproject.com/Articles/140209/Building-a-Docking-Window-Management-Solution-in-W),
code which is licensed under The Code Project Open License (CPOL).
Details of the license can be found in the accompanying file : cpol_license.htm

**********************************************************/

//
// Copyright(C) MixModes Inc. 2011
// 

using System;
using System.Windows;
using System.Windows.Interactivity;
using Cuisine.Extensions;

namespace Cuisine.Behaviors
{
    /// <summary>
    /// Behavior that is attached to a visual parent of specified type that may or may not be immidiate parent of the behavior
    /// </summary>
    /// <typeparam name="T">Type of visual parent</typeparam>
    /// <remarks>
    ///     1. The first parent that matches the specified type is assumed to be the visual parent that behavior shall use
    ///     2. Do not use AssociatedObject but rather VisualParent when using derived behaviors of this type
    /// </remarks>
    public abstract class VisualParentBehavior<T> : Behavior<FrameworkElement> where T:FrameworkElement
    {                
        /// <summary>
        /// Visual parent
        /// </summary>
        /// <remarks>
        /// During initialization VisualParent is null since the behavior may be applied to
        /// a FrameworkElement a parent of which is still not attached to the visual tree. 
        /// Hence this property must never be used from OnAttached method.
        /// </remarks>
        protected T VisualParent
        {
            get
            {
                _visualParent = _visualParent ?? FindVisualParent();
                return _visualParent;
            }
        }

        /// <summary>
        /// Finds the visual parent
        /// </summary>
        /// <exception cref="InvalidOperationException">Parent does not exist in visual tree</exception>
        private T FindVisualParent()
        {
            T parent = AssociatedObject.GetVisualParent<T>();

            if (parent == null)
            {
                throw new InvalidOperationException("No parent found in visual tree");
            }

            return parent;
        }

        // Private members
        private T _visualParent;
    }
}
