@echo off
cls

rem "tools\nuget.exe" "install" "FAKE" "-OutputDirectory" "tools" "-ExcludeVersion"
rem "tools\nuget.exe" "install" "xunit.runner.console" "-OutputDirectory" "tools" "-ExcludeVersion"
"tools\FAKE\tools\Fake.exe" build.fsx %*
pause