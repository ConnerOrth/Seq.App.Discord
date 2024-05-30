# Seq.App.Discord.SixSided

An app for Seq (http://getseq.net) that forwards messages to Discord through webhooks.

## Changes

### 1.0.2

- Fixed a small typo

### 1.0.1

- Added build script
- Added missing icon

### 1.0.0

- Updated project to .NET 8
- Added option to specify roles to mention in the webhookmessage.
- Removed obsolete code

### 0.4.0

- Static color configuration option added

### 0.3.0

- Color of a message now depends on a message level
- Notifier bot name and avatar are now configurable
- Title of a message now can contain one property value in addition to event level

### 0.2.2

- Timeout of http requests increased

### 0.2.1

- Enhanced logging of http errors

### 0.2.0

- Correct attributes added to DiscordReactor class

### 0.1.2

- Cosmetic version bump

### 0.1.1

- Initial revision

## Building NuGet Package

From solution root, run:

- msbuild
- nuget pack ./Seq.App.Discord/Seq.App.Discord.csproj

## Credits

Inspired by [stayhard](https://github.com/stayhard/Seq.App.HipChat)
Forked from [glkn's Seq.App.Discord](https://github.com/glkn/Seq.App.Discord)