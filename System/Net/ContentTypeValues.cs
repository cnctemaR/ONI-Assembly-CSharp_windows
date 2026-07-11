using System;

namespace System.Net
{
	internal enum ContentTypeValues
	{
		ChangeCipherSpec = 20,
		Alert,
		HandShake,
		AppData,
		Unrecognized = 255
	}
}
