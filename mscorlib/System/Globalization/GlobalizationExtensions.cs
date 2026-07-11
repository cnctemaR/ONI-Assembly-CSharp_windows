using System;
using Unity;

namespace System.Globalization
{
	public static class GlobalizationExtensions
	{
		public static StringComparer GetStringComparer(this CompareInfo compareInfo, CompareOptions options)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
