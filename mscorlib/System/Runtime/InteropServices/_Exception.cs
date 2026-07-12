using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[CLSCompliant(false)]
	[ComVisible(true)]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("b36b5c63-42ef-38bc-a07e-0b34c98f164a")]
	public interface _Exception
	{
		string HelpLink { get; set; }

		Exception InnerException { get; }

		string Message { get; }

		string Source { get; set; }

		string StackTrace { get; }

		MethodBase TargetSite { get; }

		bool Equals(object obj);

		Exception GetBaseException();

		int GetHashCode();

		void GetObjectData(SerializationInfo info, StreamingContext context);

		Type GetType();

		string ToString();
	}
}
