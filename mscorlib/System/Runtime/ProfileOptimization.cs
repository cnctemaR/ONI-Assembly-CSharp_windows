using System;
using System.Security;
using Unity;

namespace System.Runtime
{
	public static class ProfileOptimization
	{
		[SecurityCritical]
		public static void SetProfileRoot(string directoryPath)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		[SecurityCritical]
		public static void StartProfile(string profile)
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
