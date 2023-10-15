fx_version 'bodacious'
game 'gta5'

author 'Jazzuh'
version '1.0.0'
description 'Test resource for time-controller proxy solution'

fxdk_watch_command 'dotnet' {'watch', '--project', 'Client/Magicallity.Time.Client.Proxy.Test.csproj', 'publish', '--configuration', 'Release'}
fxdk_watch_command 'dotnet' {'watch', '--project', 'Server/Magicallity.Time.Server.Proxy.Test.csproj', 'publish', '--configuration', 'Release'}

file 'Client/bin/Release/**/publish/*.dll'

client_script 'Client/bin/Release/**/publish/*.net.dll'
server_script 'Server/bin/Release/**/publish/*.net.dll'

mono_rt2 'Prerelease expiring 2023-12-31. See https://aka.cfx.re/mono-rt2-preview for info.'

dependancy 'time-controller'