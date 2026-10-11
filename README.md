# JevOnScreenKeyboard

Valheim has no on-screen keyboard for gamepads outside Steam Big Picture. This mod adds one. It pops up when a text field gets focus and goes away when you reach for the keyboard or mouse. It works with Xbox, PlayStation and Switch controllers and speaks every language the game does.

Screenshots are in [SCREENSHOTS.md](https://github.com/JevMods/JevOnScreenKeyboard/blob/main/SCREENSHOTS.md).

## Features

- Works in chat, console, signs, portals, map pins, and world and character names.
- Follows your game language, with the layout people actually use there: AZERTY, QWERTZ, Cyrillic, Thai, Hangul, kana and so on. A key next to `#+=` swaps to English for chat commands.
- RT on a letter shows accented variants (é, ñ, ø), so names with them work in any language.
- Korean letters combine into syllables as you type.
- Japanese gets hiragana, katakana on Shift and kanji suggestions. Chinese uses pinyin with candidates on the top row.
- Greek, Hindi, Macedonian and Serbian only exist as placeholders in the game, so they get the English layout.
- Button glyphs match your controller: Xbox, PlayStation or Switch.

## Controls

Nothing needs to be held. D-pad, A, X, Y and the bumpers repeat if you do hold them.

- D-pad or left stick: move. A: press the key.
- X: backspace. Y: space. LB and RB: move the caret.
- LT: Shift. Start: confirm. B: close.
- RT on a letter: accent variants show on the number row. A picks one, RT or B cancels.
- RT while typing Chinese or Japanese: jump to the candidates. Space takes the first one, `»` flips the page.

## Configuration

The settings are in `BepInEx/config/JevMods.JevOnScreenKeyboard.cfg`, created on first launch.

- `Scale`: size of the keyboard, from 0.5 to 2 (default `1`).

## Feedback

Found a bug or have an idea for a change? Open an issue on the [GitHub issues page](https://github.com/JevMods/JevOnScreenKeyboard/issues). Refactoring suggestions are welcome too. I'll go through everything as fast as I can.

## Building from source

To build you need Windows, the .NET SDK 8 or newer (with the .NET Framework 4.8 developer pack), Valheim and BepInEx. The easiest way to get BepInEx is to install BepInExPack_Valheim in a Thunderstore Mod Manager profile.

```
git clone https://github.com/JevMods/JevOnScreenKeyboard.git
cd JevOnScreenKeyboard
dotnet build src/JevOnScreenKeyboard -c Release
```

Then copy `src/JevOnScreenKeyboard/bin/Release/JevOnScreenKeyboard.dll` into `BepInEx/plugins`.

The build looks for Valheim through Steam and for BepInEx in the Default profile of the Thunderstore Mod Manager. If yours live elsewhere, pass the paths:

```
dotnet build src/JevOnScreenKeyboard -c Release -p:ValheimDir="D:\Games\Valheim" -p:BepInExDir="D:\Games\Valheim\BepInEx"
```

You can also set the `VALHEIM_INSTALL` environment variable. With no Valheim install at all, point `ManagedDir` at the `valheim_server_Data/Managed` folder of the free Valheim Dedicated Server (Steam app 896660). The GitHub workflow does exactly that.

## Notes

- Client-side: only the player who wants the keyboard needs to install it.
- Formerly published as JevControllerKeyboard.
- Source: https://github.com/JevMods/JevOnScreenKeyboard
