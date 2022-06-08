@echo off
:loop
ping -n 2 127.0.0.1>nul
tasklist|find /i "Hearthstone.exe">nul
if %errorlevel%==1 (
copy /y "filename1" "filename"
del "filename1"
del %0
exit
)
goto :loop