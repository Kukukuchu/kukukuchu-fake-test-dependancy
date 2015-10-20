@echo off
cls
"packages\FAKE\tools\Fake.exe" build.fsx target=Publish
pause
