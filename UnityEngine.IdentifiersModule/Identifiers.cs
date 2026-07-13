using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Identifiers
{
	[NativeHeader("Modules/Identifiers/Identifiers.h")]
	public static class Identifiers
	{
		public static string installationId
		{
			get
			{
				return Identifiers.GetInstallationId();
			}
		}

		[FreeFunction("UnityEngine_Identifiers_GetInstallationId")]
		private static string GetInstallationId()
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				Identifiers.GetInstallationId_Injected(out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetInstallationId_Injected(out ManagedSpanWrapper ret);
	}
}
