## SubtitlesParser

Universal subtitles parser which aims at supporting parsing for all subtitle formats, and writing some.
For more info on subtitles formats, see this page: http://en.wikipedia.org/wiki/Category:Subtitle_file_formats

It's available on Nuget:
> Install-Package SubtitlesParser

For now, 7 different formats are supported for parsing:
* MicroDvd	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/MicroDvdParser.cs
* SubRip	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/SrtParser.cs
* SubStationAlpha	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/SsaParser.cs
* SubViewer	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/SubViewerParser.cs
* TTML	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/TtmlParser.cs
* WebVTT	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/VttParser.cs
* Youtube specific XML format	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/YtXmlFormatParser.cs

And 2 formats are supported for writing: 
* SubRip    https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Writers/SrtWriter.cs
* SubstationAlpha   https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Writers/SsaWriter.cs

### Quickstart

You can check the Test project for subtitles files and more sample codes.

#### Universal parser

If you don't specify the subtitle format, the SubtitlesParser will try all the registered parsers (7 for now)

```csharp
var parser = new SubtitlesParser.Classes.Parsers.SubParser();
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

#### Specific writer 

You can use a specific writer to write a List of SubtitleItems to a stream.
```csharp
var writer = new SubtitlesParser.Classes.Writers.SrtWriter();
using (var fileStream = File.OpenWrite(pathToSrtFile)) {
	writer.WriteStream(fileStream, yourListOfSubtitleItems);
}
```

Async versions are also available (ie `writer.WriteStreamAsync(fileStream, yourListOfSubtitleItems);` instead). 
