using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Light Probe Group.</para>
	/// </summary>
	[NativeHeader("Runtime/Graphics/LightProbeGroup.h")]
	public sealed class LightProbeGroup : Behaviour
	{
		/// <summary>
		///   <para>Editor only function to access and modify probe positions.</para>
		/// </summary>
		public Vector3[] probePositions
		{
			get
			{
				return null;
			}
		}
	}
}
