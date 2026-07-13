using System;
using System.Runtime.InteropServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics
{
	[ExcludeFromDocs]
	[RequiredByNativeCode(GenerateProxy = true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class SubsystemsAnalyticInfo : SubsystemsAnalyticBase
	{
		public SubsystemsAnalyticInfo()
			: base("SubsystemInfo")
		{
		}

		[RequiredByNativeCode]
		internal static SubsystemsAnalyticInfo CreateSubsystemsAnalyticInfo()
		{
			return new SubsystemsAnalyticInfo();
		}

		private string id;

		private string plugin_name;

		private string version;

		private string library_name;
	}
}
