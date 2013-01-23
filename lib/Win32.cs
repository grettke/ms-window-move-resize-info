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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;

namespace window.lib
{
    /// <summary>
    /// Attribution:
    ///     
    ///     All of the code in the region named "Win32 Calls" came from 
    ///     
    ///         http://pinvoke.net
    /// 
    ///     and/or
    /// 
    ///         http://msdn.microsoft.com
    /// 
    ///     For all other code see individual attributions.
    /// 
    /// Comments:
    /// 
    ///     If you plan to use this code as a reference, please be sure to read up on
    ///     P/Invoke before doing so, and also have a look at both the MSDN and PInvoke.net
    ///     documentation; it is important that you understand both the what *and* the how.
    /// </summary>
    public class Win32
    {
        #region Win32 Calls
        
        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);
        
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left_, int top_, int right_, int bottom_)
            {
                Left = left_;
                Top = top_;
                Right = right_;
                Bottom = bottom_;
            }

            public int Height { get { return Bottom - Top; } }
            public int Width { get { return Right - Left; } }
            public Size Size { get { return new Size(Width, Height); } }

            public Point Location { get { return new Point(Left, Top); } }

            public Rectangle ToRectangle()
            { return Rectangle.FromLTRB(Left, Top, Right, Bottom); }

            public static RECT FromRectangle(Rectangle rectangle)
            {
                return new RECT(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
            }

            public override int GetHashCode()
            {
                return Left ^ ((Top << 13) | (Top >> 0x13))
                  ^ ((Width << 0x1a) | (Width >> 6))
                  ^ ((Height << 7) | (Height >> 0x19));
            }

            public static implicit operator Rectangle(RECT rect)
            {
                return rect.ToRectangle();
            }

            public static implicit operator RECT(Rectangle rect)
            {
                return FromRectangle(rect);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint RealGetWindowClass(IntPtr hwnd, StringBuilder pszType,
           uint cchType);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth,
           int nHeight, bool bRepaint);

        [DllImport("kernel32.dll")]
        static extern void SetLastError(uint dwErrCode);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        public enum GWL
        {
            /// <summary>
            /// Retrieves the pointer to the window procedure, or a handle representing 
            /// the pointer to the window procedure. 
            /// </summary>
            GWLP_WNDPROC = (-4),
            /// <summary>
            /// Retrieves a handle to the application instance.
            /// </summary>
            GWLP_HINSTANCE = (-6),
            /// <summary>
            /// Retrieves a handle to the parent window, if there is one.
            /// </summary>
            GWLP_HWNDPARENT = (-8),
            /// <summary>
            /// Retrieves the window styles.
            /// </summary>
            GWL_STYLE = (-16),
            /// <summary>
            /// Retrieves the extended window styles.
            /// </summary>
            GWL_EXSTYLE = (-20),
            /// <summary>
            /// Retrieves the user data associated with the window.
            /// </summary>
            GWLP_USERDATA = (-21),
            /// <summary>
            /// Retrieves the identifier of the window.
            /// </summary>
            GWLP_ID = (-12)
        }

        /// <summary>
        /// Looked up value on a few websites and verified in:
        /// C:\Program Files\Microsoft Visual Studio 8\VC\PlatformSDK\Include\WinUser.h
        /// </summary>
        private const Int64 WS_EX_TOOLWINDOW = 0x00000080L;

        #endregion

        #region Private Functionality - Get Window Handles

        /// <summary>
        /// Shared collection between the initiator of EnumWindow and its callback.
        /// </summary>
        private static IList<IntPtr> getWindowHandlesTemp;

        /// <summary>
        /// EnumWindowProc delegate for the callback.
        /// </summary>
        private static EnumWindowsProc enumWindowsProcDelegate = 
            new EnumWindowsProc(getWindowHandlesImpl);

        /// <summary>
        /// Callback for EnumWindows.
        /// </summary>
        /// <param name="hWnd">Handle to a window.</param>
        /// <param name="lParam">Specifies the application-defined value given in 
        /// EnumWindows or EnumDesktopWindows.</param>
        /// <returns>Handles to all windows</returns>
        private static bool getWindowHandlesImpl(IntPtr hWnd, IntPtr lParam)
        {
            getWindowHandlesTemp.Add(hWnd);
            return true;
        }

        /// <summary>
        /// Initiate the Win32 EnumWindows function and return the handle collection.
        /// </summary>
        /// <returns>Handles to all windows</returns>
        private static IList<IntPtr> getWindowHandles()
        {
            SetLastError(0);
            getWindowHandlesTemp = new List<IntPtr>();
            EnumWindows(enumWindowsProcDelegate, IntPtr.Zero);
            if (Marshal.GetLastWin32Error() != 0)
            {
                string mes = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                throw new Exception("Error iterating over windows handles: " + mes);
            }
            IList<IntPtr> result = getWindowHandlesTemp;
            getWindowHandlesTemp = null;
            return result;
        }

        /// <summary>
        /// Helper gets the window rectangle, or null.
        /// </summary>
        static RECT? GetWindowRectHelper(IntPtr hWnd)
        {
            SetLastError(0);
            RECT windowRect;
            if (GetWindowRect(hWnd, out windowRect))
            {
                return windowRect;
            }
            else
            {
                if (Marshal.GetLastWin32Error() != 0)
                {
                    string mes = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                    Console.WriteLine(
                        "Error getting window rectangle for handle \"" + hWnd + "\": " + mes);
                }
                return null;
            }
        }

        /// <summary>
        /// Helper moves the window or throws an exception with the error message.
        /// </summary>
        private static void MoveWindowHelper(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, 
            bool bRepaint)
        {
            SetLastError(0);
            MoveWindow(hWnd, X, Y, nWidth, nHeight, bRepaint);
            if (Marshal.GetLastWin32Error() != 0)
            {
                string mes = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                throw new Exception("Error moving window with handle \"" + hWnd + "\": " + mes + "\n");
            }
        }

        #endregion

        #region Private Functionality - General

        /// <summary>
        /// Get window information.
        /// Attribution: PInvoke.net.
        /// </summary>
        public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            SetLastError(0);
            IntPtr result;
            if (IntPtr.Size == 8)
            {
                result = GetWindowLongPtr64(hWnd, nIndex);
            }
            else
            {
                result = GetWindowLongPtr32(hWnd, nIndex);
            }
            if (Marshal.GetLastWin32Error() != 0)
            {
                string mes = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                throw new Exception(
                    "Error getting " +  Enum.GetName(typeof(GWL), nIndex) + 
                    " window information for \"" + hWnd + "\": " + mes + "\n");
            }
            return result;
        }

        /// <summary>
        /// Get the text in a window's title bar
        /// </summary>
        /// <param name="hWnd">the window handle</param>
        /// <returns>the text</returns>
        private static string GetWindowText(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();   
        }

        /// <summary>
        /// Get the parent handle for a given window.
        /// </summary>
        /// <param name="hWnd">the child window handle</param>
        /// <returns>the parent window handle</returns>
        private static IntPtr GetWindowParent(IntPtr hWnd)
        {
            SetLastError(0);
            IntPtr result = GetParent(hWnd);
            if (Marshal.GetLastWin32Error() != 0)
            {
                string mes = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                throw new Exception("Error getting the parent for \"" + hWnd + "\": " + mes + "\n");
            }

            return result;
        }

        /// <summary>
        /// Get the class for a given window.
        /// </summary>
        /// <param name="hWnd">the window handle</param>
        /// <returns>the class name</returns>
        private static string GetWindowClass(IntPtr hWnd)
        {
            SetLastError(0);
            StringBuilder sb = new StringBuilder(255);
            RealGetWindowClass(hWnd, sb, (uint) (sb.Length + 1));
            if (Marshal.GetLastWin32Error() != 0)
            {
                string mes = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                Debug.WriteLine("Error looking up window class for \"" + hWnd + "\": " + mes);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Is the window a tool window?
        /// Attribution: 
        ///     Karl E. Peterson   
        ///     http://vb.mvps.org/samples/project.asp?id=AltTab
        /// </summary>
        private static bool IsToolWindow(IntPtr hWnd)
        {
            IntPtr style = GetWindowLongPtr(hWnd, (int) GWL.GWL_EXSTYLE);
            Int64 intStyle = style.ToInt64();
            bool result = (intStyle & WS_EX_TOOLWINDOW) == WS_EX_TOOLWINDOW;
            return result;
        }

        #endregion

        #region Public Features

        /// <summary>
        /// Get available information on all child windows.
        /// </summary>
        /// <returns></returns>
        public static IList<WinInfo> GetWindowInformation()
        {
            List<WinInfo> result = new List<WinInfo>();
            IList<IntPtr> handles = getWindowHandles();
            foreach (IntPtr hWnd in handles)
            {
                WinInfo info = new WinInfo();
                RECT? possibleRect = GetWindowRectHelper(hWnd);
                if (possibleRect.HasValue)
                {
                    info.handle = hWnd.ToInt64();
                    info.windowText = GetWindowText(hWnd);
                    info.className = GetWindowClass(hWnd);
                    try
                    {
                        info.parentHandle = GetWindowParent(hWnd).ToInt64();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        continue;
                    } 
                    
                    RECT windowRect = possibleRect.Value;
                    info.x = windowRect.Left;
                    info.y = windowRect.Top;
                    info.width = windowRect.Width;
                    info.height = windowRect.Height;

                    info.visible = IsWindowVisible(hWnd);
                    
                    try
                    {
                        IntPtr hWndOwner = GetWindowLongPtr(hWnd, (int)GWL.GWLP_HWNDPARENT);
                        info.ownerHandle = hWndOwner.ToInt64();
                        info.ownerVisible = IsWindowVisible(hWndOwner);
                        info.ownerIsToolWindow = IsToolWindow(hWndOwner);
                    }
                    catch(Exception e)
                    {
                        Debug.WriteLine("Error getting window owner information: " + e.Message +
                            ", skipping");
                        continue;
                    }

                    info.isToolWindow = IsToolWindow(hWnd);

                    result.Add(info);
                }
            }
            return result;
        }

        /// <summary>
        /// Move the given window to a new location
        /// </summary>
        /// <param name="hWnd">window to move</param>
        /// <param name="x">new x location</param>
        /// <param name="y">new y location</param>
        public static void Move(Int64 hWnd, Int32 x, Int32 y)
        {
            IntPtr hWndPtr = new IntPtr(hWnd);
            RECT? possibleRect = GetWindowRectHelper(hWndPtr);
            if (possibleRect.HasValue)
            {
                RECT windowRect = possibleRect.Value;
                MoveWindowHelper(
                    hWndPtr,
                    x, y,
                    windowRect.Width,
                    windowRect.Height,
                    true);
            }   
            else
            {
                throw new Exception("Couldn't look up the window information");
            }
        }

        /// <summary>
        /// Resize the given window
        /// </summary>
        /// <param name="hWnd">window to resize</param>
        /// <param name="width">new width</param>
        /// <param name="height">new height</param>
        public static void Resize(Int64 hWnd, UInt16 width, UInt16 height)
        {
            IntPtr hWndPtr = new IntPtr(hWnd);
            RECT? possibleRect = GetWindowRectHelper(hWndPtr);
            if (possibleRect.HasValue)
            {
                RECT windowRect = possibleRect.Value;
                MoveWindowHelper(
                    hWndPtr,
                    windowRect.Left, windowRect.Top,
                    width,
                    height,
                    true);
            }
            else
            {
                throw new Exception("Couldn't look up the window information");
            }   
        }

        #endregion

        #region Client Validation

        public static bool ValidHandle(Int64 handle)
        {
            return handle >= 0;
        }

        public static bool ValidHandle(string handle)
        {
            try
            {
                Int64 temp = Int64.Parse(handle);
                return ValidHandle(temp);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ValidCoordinate(string coord)
        {
            try
            {
                Int32.Parse(coord);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool ValidSide(UInt16 side)
        {
            return side > 0;
        }

        public static bool ValidSide(string side)
        {
            try
            {
                UInt16 temp = UInt16.Parse(side);
                return ValidSide(temp);
            }
            catch (Exception)
            {
                return false;
            }   
        }

        #endregion
    }
}