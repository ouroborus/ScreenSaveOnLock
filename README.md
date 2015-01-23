# ScreenSaveOnLock
Anti-idle but use screen saver if workstation is locked.

I wanted my system setup such that it would not activate the screen saver (and lock the computer) unless I had locked the computer myself. The screen saver timer had been locked by the domain security policy and the timer is global in that it ignores whether the computer is locked or not.

This program is the result of those needs and acts as anti-idle and screen saver trigger.

The program monitors workstation lock state and screen saver active state. The state transition times are also tracked. If the screen saver is currently running, the program just idles. If the screen saver isn't running, the shift key is tapped every 50s. (This is comfortably under the minimum time you can set the screen saver timer to. Using the shift key prevents interference if you are actually using the computer.) If the console is locked and has been locked for certain amount of time (currently 50s), the screen saver is activated.

There are number of necessary functions that are not made available through the .NET framework. These are instead accessed through User32.dll and C#'s pinvoke capability. Windows doesn't provide the current lock state instead passing events when the computer becomes locked or unlocked. Conversely, Windows doesn't provide screen saver start and stop events, instead giving you current screen saver state. This program also tracks logon and logoff events and will only do anything if a user is currently logged in.

This program lives in the task tray. Making a "windowless" task tray program seems to be a difficult thing based on the varied solutions available via Google. In order to catch events, you need an event loop but with Windows Forms it's difficult to activate the event loop without also creating a main form. There are few things that, when brought together, make it easy to create a "windowless" task tray program: Create a Windows Forms project; add a notify icon to the main form; update the main form, setting `WindowState` to `Minimized` and `ShowInTaskbar` to `False`. The rest of the UI elements (icon menu, config, about, etc.) can be created as there own forms or hung off the main form.

TODO: Add user config, add about dialog.
