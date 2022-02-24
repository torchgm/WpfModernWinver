# ModernWinver

## This application is deprecated. A new version of Modern Winver compatible with Windows 10 and 11 is available [here](https://github.com/torchgm/NewModernWinver).


A modern replacement for the About Windows screen relying on ModernWPF and FluentWPF, providing details on Windows and your PC.

![ModernWinver](https://cdn.discordapp.com/attachments/272509873479221249/805509239816585256/dragfrom.202101311846062843.png) 

## Features
 - Provides information on your current installation of Windows
 - Gives you a quick overview of system specs and resource usage
 - Presents quick access to your current wallpaper and accent colour
 - Allows you to customise your secondary accent colour (used for file selection, text highlighting etc.)

## Installation
Right now there's no official installation, and the executable will work standalone. If you want to be able to start it from the command line, I recommend you copy it to `C:\Windows\` and rename it something short like `mwv.exe` for now, or just run `copy /-Y ModernWinver.exe C:\Windows\mwv.exe` as administrator from the folder in which you downloaded ModernWinver.exe. Once you've done this, you can open ModernWinver by typing `mwv` into Start, Command Prompt or Run and pressing enter. I'll probably make a more competent installer in the future but for now this is pretty quick and effective.
