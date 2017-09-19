## HKUST Barn Init

Apply personal settings to HKUST barn and CS lab computers.

### How to use

Preparation

1. Download the binaries and edit `HKUSTBarnInit.exe.config`
according to your needs
2. Put the `.exe` and `.config` files in your personal network drive
space in barn / CS lab

Use
1. Each time you logon, open the network drive and double click the tool

*NOTE: this tool restarts explorer.exe*

### What it does

You can switch features off in `.config`

1. Remove all pinned applications on taskbar
2. Set taskbar icons to never combine
3. Start file explorer to This PC rather than Quick Access
4. Show filename extensions
5. Disable "enhance mouse precision"
6. Set mouse speed

For RDP users: it can set a default username for a RDP domain name. You can also save
a copy of a `.rdp` file in your network drive. Then, you can start RDP after
running the tool and have username auto-filled.

### License

This code is put in public domain.