using System;
using System.ComponentModel;
using System.IO;

namespace System.Xml
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
	public interface IApplicationResourceStreamResolver
	{
		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		Stream GetApplicationResourceStream(Uri relativeUri);
	}
}
