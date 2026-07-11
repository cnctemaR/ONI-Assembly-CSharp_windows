using System;

namespace System.Net.Mime
{
	internal enum MimeMultiPartType
	{
		Mixed,
		Alternative,
		Parallel,
		Related,
		Unknown = -1
	}
}
