using System;
using System.Runtime.InteropServices;
using UnityEngine.Analytics;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEditor.Analytics
{
	[ExcludeFromDocs]
	[RequiredByNativeCode(GenerateProxy = true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ProjectSettingsInformationAnalytic : AnalyticsEventBase
	{
		public ProjectSettingsInformationAnalytic()
			: base("navigation_project_settings_info", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		internal static ProjectSettingsInformationAnalytic CreateProjectSettingsInformationAnalytic()
		{
			return new ProjectSettingsInformationAnalytic();
		}

		private int agent_types_count;

		private int areas_count;
	}
}
