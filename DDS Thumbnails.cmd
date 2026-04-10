reg add "HKCR\.dds\ShellEx\{e357fccd-a995-4576-b01f-234630154e96}" /ve /d "{5D6BF029-A6BA-417A-8523-120492B1DCE3}" /f
reg add "HKCR\CLSID\{5D6BF029-A6BA-417A-8523-120492B1DCE3}" /ve /d "Renderdoc Thumbnail Handler" /f
reg add "HKCR\CLSID\{5D6BF029-A6BA-417A-8523-120492B1DCE3}\InprocServer32" /ve /d "%~dp0RenderDoc.dll" /f
pause
