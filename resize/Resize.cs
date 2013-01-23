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
using window.lib;

namespace window.app
{
    internal class Resize
    {
        static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: wresize <handle> <width> <height>");
                return 1;
            }

            Int64 window;
            UInt16 width, height;

            if (!Win32.ValidHandle(args[0]))
            {
                string errMes =
                    "\"{0}\" is not a valid handle, only integers greater than " +
                    "or equal to zero are allowed";
                Console.WriteLine(String.Format(errMes, args[0]));
                return 1;
            }

            window = Int64.Parse(args[0]);

            if (!Win32.ValidSide(args[1]))
            {
                Console.WriteLine(String.Format("\"{0}\" is not a valid width", args[1]));
                return 1;
            }

            width = UInt16.Parse(args[1]);

            if (!Win32.ValidSide(args[2]))
            {
                Console.WriteLine(String.Format("\"{0}\" is not a valid height", args[2]));
                return 1;
            }

            height = UInt16.Parse(args[2]);

            try
            {
                Win32.Resize(window, width, height);
            }
            catch (Exception e)
            {
                Console.WriteLine("Aborting:");
                Console.WriteLine(e.Message);
            }

            return 0;
        }
    }
}