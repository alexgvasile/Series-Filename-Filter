# Series Filename Filter

A simple tool for renaming TV Series files. This tool was created out of a need to mass rename TV series for consumption by the [PLEX Media Server](https://www.plex.tv/).

As of version 1.0.0.0, this tool includes functionality to remove and/or replace words in episode filenames that have **original names** containing the following **non-case-sensitive** episode information pattern(s):
- Single Episode Files: `S01E01`
- Multi Episode Files: `S01E01E02`

This tool will format new filenames to abide by the rules set out by PLEX, found here: [Naming ‘Series’ & ‘Season’ Based TV Shows](https://support.plex.tv/articles/200220687-naming-series-season-based-tv-shows/).

## Hot To Use
Once at the menu screen, you'll have two funtional options. `[1] Simple Rename: Removals Only`, which will **not** allow you to change words, only to remove them, or `[2]Complex Rename: Removals and Replacements`, which will allow for replacements as well. You will be asked to enter 3 or 4 parameters based on your choice. These parameters are described below:
- **Main Directory**: Refers to the parent directory/folder containing the TV Series' seasons folders.
  - e.g.: `C:\Users\<USER_NAME>\Documents\Star Trek TNG\`
- **Delimiter**: Refers to a recurring character, or set of characters, that split up the words within a filename.
  - e.g. `ST_TNG_S01E04_The_Code_Of_Honor_1080p_HD.mp4` would be using the delimiter underscore `_`
  - e.g. `ST.TNG.S01E04.The.Code.Of.Honor.1080p.HD.mp4` would be using the delimiter period `.`
- **Filters**: Refers to the repeating words to be completely removed from the filename. These should be entered as a comma (,) delimited list. Spaces are retained so do not add any unless necessary and never in the case where your delimiter is already a space.
  - e.g. To remove the words `TNG`, `1080p` and `HD` in the above example, the filter list would look like this: `TNG,1080p,HD`
- **Replacements**: Refers to the pairs of words to be replaced and their replacement values. These should be entered as a list where elements of a * *Old Word/New word* * pair should be delimited by a comma (,) and complete pairs should be delimited by a semicolon (;).
  - e.g. To replace the word `ST` with `Star Trek` and the word `TNG` with `The Next Generation` in the example above, the replacement list would look like this: `ST,Star Trek;TNG,The Next Generation;`

## A Complete Example
Let's assume you want to rename the entire Star Trek The Next Generation series in your library. The original folder structure and episode naming scheme looks like this for a given episode:
- `C:\Users\<USER_NAME>\Documents\Star Trek TNG\Season 01\ST.TNG.S01E05.The.Last.Outpost.1080p.HD.mp4`.

You want to rename the acronyms `ST` and `TNG` and remove the extra details `1080p` and `HD`. Your parameters would then be:
- **Main Directory:** `C:\Users\<USER_NAME>\Documents\Star Trek TNG\`
- **Delimiter:** `.`
- **Filters:** `1080p,HD`
- **Replacements:** `ST,Star Trek;TNG,The Next Generation;`

The results would then look like this:
- **Old Filename**: `ST.TNG.S01E05.The.Last.Outpost.1080p.HD.mp4`
- **New Filename**: `Star Trek The Next Generation - s01e05 - The Last Outpost.mp4`

## Requirements & Installation
Series Filename Filter requires that you install [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework-runtime). Alternatively you can [Download the Installer](https://vault.alexvasile.com/series-filename-filter/Series%20Filename%20Filter.zip)

If you prefer you can also use the code in this repository to complile your own application.

## Art Credits
Icon made by [Freepik](https://www.freepik.com/) from [www.flaticon.com](https://www.flaticon.com/) is licensed by [CC BY 3.0](http://creativecommons.org/licenses/by/3.0/)
