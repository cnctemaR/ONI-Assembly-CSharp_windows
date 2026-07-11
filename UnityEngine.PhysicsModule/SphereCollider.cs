using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>A sphere-shaped primitive collider.</para>
	/// </summary>
	[NativeHeader("Runtime/Dynamics/SphereCollider.h")]
	[RequiredByNativeCode]
	public class SphereCollider : Collider
	{
		/// <summary>
		///   <para>The center of the sphere in the object's local space.</para>
		/// </summary>
		public Vector3 center
		{
			get
			{
				Vector3 vector;
				this.get_center_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_center_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The radius of the sphere measured in the object's local space.</para>
		/// </summary>
		public extern float radius
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_center_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_center_Injected(ref Vector3 value);
	}
}
