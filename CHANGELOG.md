## [1.0.0] - 16/06/2025
### First Release
- Created Package

## [1.0.1] - 16/06/2025
## Minor Improvements
- Added Gizmos for MonoBehaviour classes

## [1.1.0] - 19/06/2025
## Console Added
- Added a ready-to-go Console using UI Toolkit
- Added a set of ready-to-go fonts for the Console
- Added a few new tests for the Tokenizer and Binder

## [1.1.1] - 20/02/2025
## Bug Fixes and removed debug messages
- Removed some debug messages from the console
- Fixed a bug in the Command Binder that was causing speech marks to be included in string tokens after binding
- Included a copy of UnityDefaultThemes in the package so that the console would render correctly once imported to another project

## [1.2.1] - 20/02/2025
## Console feature update
- Fixed some layout issues with the Console
- Added a scroll bar which appears when the console exceeds a maximum size
- Made it so the console correctly scrolls all the way to the bottom when a new message is added
- Added a command history to the Console. Using the up and down arrow keys allows scrolling through history to quickly copy commands

## [1.3.1] - 20/02/2025
## extOSC feature update
- Added support for commands over OSC using the extOSC plugin (https://assetstore.unity.com/packages/tools/input-management/extosc-open-sound-control-72005)
- Added support for VContainer Dependency Injection if package is installed