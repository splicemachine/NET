# NET
Support for the .NET framework

## Compiling the project with Visual Studio 2019.

- Download and install SimbaSDK on your local machine.
- Navigate to <SimbaSDK folder>\SimbaEngineSDK\10.0\DataAccessComponents\Bin\win\release and add Simba.ADO.Net.dll and Simba.DotNetDSI.dll to the GAC by running (as administrator):
  gacutil.exe /i Simba.ADO.Net.dll
  gacutil.exe /i Simba.DotNetDSI.dll
- If you have kept the default path of SimbaSDK (C:\Simba Technologies) during installation, you do not have to do anything else, otherwise, you have to remove the references to Simba.DotNetDSI and Simba.ADO.Net assemblies from SpliceMachine.Provider project and add them again as custom references pointing to the DDLs at the new location.