using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics2D/Public/CircleCollider2D.h")]
	public sealed class CircleCollider2D : Collider2D
	{
		public float radius
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CircleCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CircleCollider2D.get_radius_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CircleCollider2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CircleCollider2D.set_radius_Injected(intPtr, value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("CircleCollider2D.center has been obsolete. Use CircleCollider2D.offset instead (UnityUpgradable) -> offset", true)]
		public Vector2 center
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_radius_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_radius_Injected(IntPtr _unity_self, float value);
	}
}
