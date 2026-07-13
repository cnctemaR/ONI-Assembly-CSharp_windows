using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering
{
	public static class RenderPipelineGraphicsSettingsExtensions
	{
		public static void SetValueAndNotify<T>(this IRenderPipelineGraphicsSettings settings, ref T currentPropertyValue, T newValue, [CallerMemberName] string propertyName = null)
		{
			throw new Exception("Changing values of a property in standalone builds is forbidden.");
		}

		public static void NotifyValueChanged(this IRenderPipelineGraphicsSettings settings, [CallerMemberName] string propertyName = null)
		{
			throw new Exception("Notifying change of a property in standalone builds is forbidden.");
		}
	}
}
