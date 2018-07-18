/**********************************************************

Part of the Synergy code created by Ashish Kaila (https://www.codeproject.com/Articles/140209/Building-a-Docking-Window-Management-Solution-in-W),
code which is licensed under The Code Project Open License (CPOL).
Details of the license can be found in the accompanying file : cpol_license.htm

**********************************************************/

//
// Copyright(C) MixModes Inc. 2011
// 

namespace Cuisine.Windows
{
    /// <summary>
    /// Dock pane state
    /// </summary>
    public enum DockPaneState
    {
        /// <summary>
        /// Docked to the side
        /// </summary>
        Docked,

        /// <summary>
        /// Auto hidden
        /// </summary>
        AutoHide,

        /// <summary>
        /// Floating on top
        /// </summary>
        Floating,

        /// <summary>
        /// Dock pane is set as content
        /// </summary>
        Content
    }
}
