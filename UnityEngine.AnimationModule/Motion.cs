using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Animation/Motion.h")]
	public class Motion : Object
	{
		protected Motion()
		{
		}

		public extern float averageDuration
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float averageAngularSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Vector3 averageSpeed
		{
			get
			{
				Vector3 vector;
				this.get_averageSpeed_Injected(out vector);
				return vector;
			}
		}

		public extern float apparentSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isLooping
		{
			[NativeMethod("IsLooping")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool legacy
		{
			[NativeMethod("IsLegacy")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isHumanMotion
		{
			[NativeMethod("IsHumanMotion")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("ValidateIfRetargetable is not supported anymore, please use isHumanMotion instead.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ValidateIfRetargetable(bool val)
		{
			return false;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("isAnimatorMotion is not supported anymore, please use !legacy instead.", true)]
		public bool isAnimatorMotion { get; }

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_averageSpeed_Injected(out Vector3 ret);
	}
}
