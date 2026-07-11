using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>The Fixed joint groups together 2 rigidbodies, making them stick together in their bound position.</para>
	/// </summary>
	[NativeClass("Unity::FixedJoint")]
	[NativeHeader("Runtime/Dynamics/FixedJoint.h")]
	public class FixedJoint : Joint
	{
	}
}
