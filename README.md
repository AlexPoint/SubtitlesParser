## SubtitlesParser
===============

Universal subtitles parser which aims at supporting all subtitle formats.
For more info on subtitles formats, see this page: http://en.wikipedia.org/wiki/Category:Subtitle_file_formats

For now, 5 different formats are supported:
* SubRip	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/SrtParser.cs
* SubViewer	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/SubViewerParser.cs
* SubStationAlpha	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/SsaParser.cs
* MicroDvd	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/MicroDvdParser.cs
* Youtube specific XML format	https://github.com/AlexPoint/SubtitlesParser/blob/master/SubtitlesParser/Classes/Parsers/YtXmlFormatParser.cs


### Quickstart

You can check the Test project for subtitles files and more sample codes.

#### Universal parser

If you don't specify the subtitle format, the SubtitlesParser will try all the registered parsers (4 for now)

	var parser = new SubtitlesParser.Classes.Parsers.SubtitlesParser();
	using (var fileStream = File.OpenRead(pathToSrtFile)){
		var items = parser.ParseStream(fileStream);
	}


#### Specific parser

You can use a specific parser if you know the format of the files you parse.
For example, for parsing an srt file:

	var parser = new SubtitlesParser.Classes.Parsers.SrtParser();
	using (var fileStream = File.OpenRead(pathToSrtFile)){
		var items = parser.ParseStream(fileStream);
	}