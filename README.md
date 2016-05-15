## SubtitlesParser
===============

Universal subtitles parser which aims at supporting all subtitle formats.
For more info on subtitles formats, see this page: http://en.wikipedia.org/wiki/Category:Subtitle_file_formats

It's available on Nuget:
> Install-Package SubtitlesParser

For now, 7 different formats are supported:
* MicroDvd	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/MicroDvdParser.cs
* SubRip	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/SrtParser.cs
* SubStationAlpha	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/SsaParser.cs
* SubViewer	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/SubViewerParser.cs
* TTML	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/TtmlParser.cs
* WebVTT	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/VttParser.cs
* Youtube specific XML format	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/YtXmlFormatParser.cs


### Quickstart

You can check the Test project for subtitles files and more sample codes.

#### Universal parser

If you don't specify the subtitle format, the SubtitlesParser will try all the registered parsers (7 for now)

```csharp
var parser = new SubtitlesParser.Classes.Parsers.SubtitlesParser();
using (var fileStream = File.OpenRead(pathToSrtFile)){
	var items = parser.ParseStream(fileStream);
}
```

#### Specific parser

You can use a specific parser if you know the format of the files you parse.
For example, for parsing an srt file:

```csharp
var parser = new SubtitlesParser.Classes.Parsers.SrtParser();
using (var fileStream = File.OpenRead(pathToSrtFile)){
	var items = parser.ParseStream(fileStream);
}
```
