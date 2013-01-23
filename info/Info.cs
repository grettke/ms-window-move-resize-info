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
using System.Collections;
using System.Collections.Generic;
using window.lib;

namespace window.app
{
    internal class Info
    {
        static void Main()
        {
            IList<WinInfo> windows = Win32.GetWindowInformation();
            ArrayList.Adapter((IList)windows).Sort();
            string fmt = "\"{0}\" ({1}) is {2}x{3} at ({4},{5})";
            Console.WriteLine(
                String.Format(
                "Sample: " + fmt, 
                "windowText",
                "\"handle\"",
                "\"width\"",
                "\"height\"",
                "\"x\"",
                "\"y\""));
            Console.WriteLine("-----------------------");

            foreach (WinInfo window in windows)
            {
                if (filter(window))
                {
                    string output =
                        String.Format(fmt,
                                      window.windowText,
                                      window.handle,
                                      window.width,
                                      window.height,
                                      window.x,
                                      window.y);
                    Console.WriteLine(output);
                }
            }
        }

        private static bool filter(WinInfo window)
        {
            return window.IsInTaskbar();
        }
    }
}