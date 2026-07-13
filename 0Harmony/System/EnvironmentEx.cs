using System;

namespace System
{
	internal static class EnvironmentEx
	{
		public static int CurrentManagedThreadId
		{
			get
			{
				return Environment.CurrentManagedThreadId;
			}
		}
	}
}
