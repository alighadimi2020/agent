Ario Tjarat Windows Agent

This version turns the tested activity/idle collector into a Windows Service.

GitHub:
1. Upload the files to the repository.
2. Run Actions -> Build Windows Service Agent.
3. Download the artifact ArioTjarat-Agent-Windows-Service.
4. Extract ArioTjarat.Agent.exe.

Windows test:
1. Put ArioTjarat.Agent.exe and Install-Agent.ps1 in the same folder.
2. Open PowerShell as Administrator.
3. Run:
   Set-ExecutionPolicy -Scope Process Bypass
   .\Install-Agent.ps1
4. The service starts automatically.
5. Data is stored in:
   C:\ProgramData\ArioTjarat\Agent

This version only collects active/idle state, device ID, machine name and Windows username.
Accounting software is not included.
