fx_version 'bodacious'
game 'gta5'

author 'Jazzuh'
version '1.0.0'
description 'Provides server authoritive time control'

fxdk_watch_command 'dotnet' {'watch', '--project', 'Client/Magicallity.Time.Client.csproj', 'publish', '--configuration', 'Release'}
fxdk_watch_command 'dotnet' {'watch', '--project', 'Server/Magicallity.Time.Server.csproj', 'publish', '--configuration', 'Release'}

file 'Client/bin/Release/**/publish/*.dll'

client_script 'Client/bin/Release/**/publish/*.net.dll'
server_script 'Server/bin/Release/**/publish/*.net.dll'

convar_category 'Hour' {
    '',
    {
        { "The hour at which day starts", "clock_day_start_hour", "CV_INT", 6 },
        { "The hour at which night starts", "clock_night_start_hour", "CV_INT", 21 },
    }
}

convar_category 'Minute' {
    '',
    {
        { "How many milliseconds it takes for a minute to pass during the day", "clock_day_milliseconds_per_minute", "CV_INT", 2000 },
        { "How many milliseconds it takes for a minute to pass during the night", "clock_night_milliseconds_per_minute", "CV_INT", 2000 },
    }
}

convar_category 'Clock' {
    '',
    {
        { "If the clock will start paused or not", "clock_paused", "CV_BOOL", false },
        { 'Decides if the clock will be synced to the client when they are spawned in', '$clock_sync_time_on_spawn', 'CV_BOOL', true }
    }
}