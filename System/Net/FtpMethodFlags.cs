using System;

namespace System.Net
{
	[Flags]
	internal enum FtpMethodFlags
	{
		None = 0,
		IsDownload = 1,
		IsUpload = 2,
		TakesParameter = 4,
		MayTakeParameter = 8,
		DoesNotTakeParameter = 16,
		ParameterIsDirectory = 32,
		ShouldParseForResponseUri = 64,
		HasHttpCommand = 128,
		MustChangeWorkingDirectoryToPath = 256
	}
}
