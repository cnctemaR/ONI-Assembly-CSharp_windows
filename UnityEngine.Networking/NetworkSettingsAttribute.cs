using System;

namespace UnityEngine.Networking
{
	[AttributeUsage(AttributeTargets.Class)]
	[Obsolete("The high level API classes are deprecated and will be removed in the future.")]
	public class NetworkSettingsAttribute : Attribute
	{
		public int channel = 0;

		public float sendInterval = 0.1f;
	}
}
