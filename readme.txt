***********************************************************************
ms-window-move-resize-info
Grant Rettke
http://www.wisdomandwonder.com/
***********************************************************************

I. Description

ms-window-move-resize-info is a project to provide both the libraries 
and helper programs to do three things:

1.	Get information about the windows on your MS Windows desktop.
2.	Move windows on your MS Windows desktop.
3.	Resize windows on your MS Windows desktop.

I wanted to do these three things for two reasons: to lay out windows
precisely on large monitors and to force windows to a suitable size for
use in screencasts. Surprisingly, no programs exist (as yet) in a 
“ready to use” form to quickly and easily do these three things. 
(To be specific, no programs that weren't "fishy", "unfinished", or
incomplete. Believe you me, I like coding, but I would rather play
guitar!)

II. Usage

The library requires .NET 2.0.

The console apps require .NET 2.0.

	Use winfo.exe to get information on windows, most importantly
	the handle. The handle is used in the move and resize commands.
	Usage: winfo

	Use wmove.exe to move windows: Usage: move <handle> <x> <y>

	Use wresize.exe to resize windows: Usage: resize <handle> <width> <height>

With exception of generics, there are no features that would make 
it difficult to back port the library.

III. Releases

		1.00	-	All desired functionality implemented.
		1.01	-	Implemented functionality that folks might actually
					want:
						Only display windows that are in the taskbar.
						*Don't* tweak the window text values.

IV. License

	BSD: the best license!

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
