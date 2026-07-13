using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics/FixedJoint.h")]
	[NativeClass("Unity::FixedJoint")]
	[RequireComponent(typeof(Rigidbody))]
	public class FixedJoint : Joint
	{
	}
}
