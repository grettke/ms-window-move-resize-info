/*
Copyright (c) 2007 Grant Rettke (grettke@acm.org)

All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;

namespace window.lib
{
    /// <summary>
    /// Responsible for providing window information in a user friendly manner.
    /// </summary>
    public class WinInfo : IComparable
    {
        /// <summary>
        /// Grabbed in the EnumWindows callback.
        /// </summary>
        public Int64 handle;

        /// <summary>
        /// Grabbed from GetParent.
        /// </summary>
        public Int64 parentHandle;

        /// <summary>
        /// Grabbed from GetWindowLongPtr(GWL.GWLP_HWNDPARENT).
        /// </summary>
        public Int64 ownerHandle;

        /// <summary>
        /// Grabbed from GetWindowClass.
        /// </summary>
        public string className;

        /// <summary>
        /// Grabbed from GetWindowRect; "left".
        /// </summary>
        public int x;

        /// <summary>
        /// Grabbed from GetWindowRect; "top".
        /// </summary>
        public int y;

        /// <summary>
        /// Grabbed from GetWindowRect; "right - left".
        /// </summary>
        public int width;

        /// <summary>
        /// Grabbed from GetWindowRect; "bottom - top".
        /// </summary>
        public int height;

        /// <summary>
        /// Grabbed from IsWindowVisible.
        /// </summary>
        public bool visible;

        /// <summary>
        /// Grabbed from IsWindowVisible.
        /// </summary>
        public bool ownerVisible;

        /// <summary>
        /// Grabbed from IsToolWindow.
        /// </summary>
        public bool isToolWindow;

        /// <summary>
        /// Grabbed from IsToolWindow.
        /// </summary>
        public bool ownerIsToolWindow;

        /// <summary>
        /// Contents of the title bar;
        /// </summary>
        public string windowText;

        /// <summary>
        /// Does this window have a parent?
        /// </summary>
        /// <returns></returns>
        public bool hasParent()
        {
            return parentHandle != 0;
        }

        /// <summary>
        /// Sort on the window text to make it more human readable.
        /// </summary>
        public int CompareTo(object o)
        {
            WinInfo that = (WinInfo)o;
            return windowText.ToUpper().CompareTo(that.windowText.ToUpper());
        }

        /// <summary>
        /// Is this a top level window?
        /// Attribution: 
        ///     Karl E. Peterson   
        ///     http://groups.google.com/group/microsoft.public.vb.winapi/browse_thread/thread/3864e1de6fb398bf/5b8faf4e05e7a9f0?hl=en&lnk=st&q=enumerate%20taskbar%20windows#5b8faf4e05e7a9f0
        /// </summary>
        public bool IsInTaskbar()
        {
            bool result = false;
            if(visible)
            {
                if(parentHandle == 0)
                {
                    if(ownerHandle == 0 || ownerIsToolWindow)
                    {
                        if(!isToolWindow)
                        {
                            result = true;
                        }
                    }
                }
            }
            return result;
        }
    }
}
