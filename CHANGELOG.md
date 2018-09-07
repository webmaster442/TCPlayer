#2.1 - 2018.09.07
 * Just source code release
 * Reverted source after a failed rewrite attempt to 2.0
 * Updated internal libaries
 * Updated bass.dll components
 * Updated project to .net 4.7
 * Assembly is now strongly signed & checks hash code of native dll files
 * ManagedBass.Pinvoke is now compiled from code for signing

#2.0 - 2017.02.17
 * Fixed Network stream playback
 * Network streams now display stram tags (Shoutcast & Icecast)
 * Redesigned Media info
 * Added web radio direrctory
 * Updated Bass.dll components
 * Can set sample rate when changing output device
 * Support for translations. currently available languages: English, Hungarian
 * Added MP4 chapter support
	- If the file doesn't have chapters then the chapters are created automaticaly
 * Redesigned window
	- Added open button
	- window is bigger
	- Audio Spectrum visual
	- Displays played item index & item count in playlist
 * This is the last release that targets the .NET Framework 4.5 Future versions will require 4.6

#1.07 - 2016.11.05
 * Fixed broken bassmidi.dll
 * Removed OFR support, because the current version of the bass plugin is broken, will reenable it when it's fixed
 
#1.06 - 2016.11.04

 * Added options dialog, with various settings
 * Improved Lister plugin code
 * Default soundfont is removed, can be configured via the settings
 * Support for multimedia keys on keyboards (can be disabled)
 * Support for track change notifications (can be disabled)
 * Playlist view now displays artist & track title, when available
 * Player background is now affected by the accent color
 * Added a packer plugin to send multiple files to the player.
 * Launched website: https://webmaster442.github.io/TCPlayer/

#1.05 - 2016.10.23

 * Added support for: APE, MPC, MP+, MPP, OFR, OFS, SPX, TTA, DSF, OPUS
 * Updated Bass.dll components
 * Saves volume on exit

#1.04 - 2016.10.03

 * Fixed Total commander plugin loading issue specific to x86 versions

#1.03 - 2016.10.01

 * Fixed Total commander plugin loading in TC 9 beta & x86 versions
 * Fixed .NET Framework targeting (4.5.2 -> 4.5.0)

#1.02 - 2016.09.27

 * MP4 handler removed from plugin, because usually it's a video
 * Added MIDI & Tracker formats playback

# 1.01 - 2016.09.10
 
 * Next track function fixed
 * Player is now correctly displayed if another file added to playlist
 * Removed an unused variable from Total Commander plugin code

# 1.0 - 2016.09.09

 * Initial Release
