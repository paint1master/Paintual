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
    /// Document container state
    /// </summary>
    public enum DocumentContainerState
    {
        /// <summary>
        /// Document container is empty
        /// </summary>
        Empty,

        /// <summary>
        /// Document container contains one or more documents
        /// </summary>
        ContainsDocuments,

        /// <summary>
        /// Document container contains other document containers and is split horizontally
        /// </summary>
        SplitHorizontally,

        /// <summary>
        /// Document container contains other document containers and is split vertically
        /// </summary>
        SplitVertically
    }
}
