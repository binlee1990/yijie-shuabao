@echo off
chcp 65001 >nul
cd /d "%~dp0"
where dotnet >nul 2>nul
if errorlevel 1 (
  echo 请先安装 .NET 8 SDK。
  pause
  exit /b 1
)
if not defined GODOT_NET (
  echo 请设置 GODOT_NET 为 Godot 4.5 .NET 版可执行文件的完整路径。
  echo 或直接用 Godot .NET 编辑器导入 project.godot。
  pause
  exit /b 1
)
dotnet build Shuabao.csproj
if errorlevel 1 (
  pause
  exit /b 1
)
"%GODOT_NET%" --path . --editor --import
if errorlevel 1 exit /b 1
"%GODOT_NET%" --path .
