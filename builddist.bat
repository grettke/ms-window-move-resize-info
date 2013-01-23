rd dist /s /q
md dist
devenv wintools.sln /Clean
devenv wintools.sln /Build Release
copy lib\bin\release\lib.dll dist
copy info\bin\release\winfo.exe dist
copy move\bin\release\wmove.exe dist
copy resize\bin\release\wresize.exe dist
copy readme.txt dist
