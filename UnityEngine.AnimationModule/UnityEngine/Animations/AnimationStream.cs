using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations
{
	[NativeHeader("Modules/Animation/Director/AnimationStream.h")]
	[RequiredByNativeCode]
	[NativeHeader("Modules/Animation/ScriptBindings/AnimationStream.bindings.h")]
	[MovedFrom("UnityEngine.Experimental.Animations")]
	public struct AnimationStream
	{
		internal uint animatorBindingsVersion
		{
			get
			{
				return this.m_AnimatorBindingsVersion;
			}
		}

		public bool isValid
		{
			get
			{
				return this.m_AnimatorBindingsVersion >= 2U && this.constant != IntPtr.Zero && this.input != IntPtr.Zero && this.output != IntPtr.Zero && this.workspace != IntPtr.Zero && this.animationHandleBinder != IntPtr.Zero;
			}
		}

		internal void CheckIsValid()
		{
			bool flag = !this.isValid;
			if (flag)
			{
				throw new InvalidOperationException("The AnimationStream is invalid.");
			}
		}

		public float deltaTime
		{
			get
			{
				this.CheckIsValid();
				return this.GetDeltaTime();
			}
		}

		public Vector3 velocity
		{
			get
			{
				this.CheckIsValid();
				return this.GetVelocity();
			}
			set
			{
				this.CheckIsValid();
				this.SetVelocity(value);
			}
		}

		public Vector3 angularVelocity
		{
			get
			{
				this.CheckIsValid();
				return this.GetAngularVelocity();
			}
			set
			{
				this.CheckIsValid();
				this.SetAngularVelocity(value);
			}
		}

		public Vector3 rootMotionPosition
		{
			get
			{
				this.CheckIsValid();
				return this.GetRootMotionPosition();
			}
		}

		public Quaternion rootMotionRotation
		{
			get
			{
				this.CheckIsValid();
				return this.GetRootMotionRotation();
			}
		}

		public bool isHumanStream
		{
			get
			{
				this.CheckIsValid();
				return this.GetIsHumanStream();
			}
		}

		public AnimationHumanStream AsHuman()
		{
			this.CheckIsValid();
			bool flag = !this.GetIsHumanStream();
			if (flag)
			{
				throw new InvalidOperationException("Cannot create an AnimationHumanStream for a generic rig.");
			}
			return this.GetHumanStream();
		}

		public int inputStreamCount
		{
			get
			{
				this.CheckIsValid();
				return this.GetInputStreamCount();
			}
		}

		public AnimationStream GetInputStream(int index)
		{
			this.CheckIsValid();
			return this.InternalGetInputStream(index);
		}

		public float GetInputWeight(int index)
		{
			this.CheckIsValid();
			return this.InternalGetInputWeight(index);
		}

		public void CopyAnimationStreamMotion(AnimationStream animationStream)
		{
			this.CheckIsValid();
			animationStream.CheckIsValid();
			this.CopyAnimationStreamMotionInternal(animationStream);
		}

		private void ReadSceneTransforms()
		{
			this.CheckIsValid();
			this.InternalReadSceneTransforms();
		}

		private void WriteSceneTransforms()
		{
			this.CheckIsValid();
			this.InternalWriteSceneTransforms();
		}

		[NativeMethod(Name = "AnimationStreamBindings::CopyAnimationStreamMotion", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void CopyAnimationStreamMotionInternal(AnimationStream animationStream)
		{
			AnimationStream.CopyAnimationStreamMotionInternal_Injected(ref this, ref animationStream);
		}

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetDeltaTime();

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetIsHumanStream();

		[NativeMethod(Name = "AnimationStreamBindings::GetVelocity", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 GetVelocity()
		{
			Vector3 vector;
			AnimationStream.GetVelocity_Injected(ref this, out vector);
			return vector;
		}

		[NativeMethod(Name = "AnimationStreamBindings::SetVelocity", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void SetVelocity(Vector3 velocity)
		{
			AnimationStream.SetVelocity_Injected(ref this, ref velocity);
		}

		[NativeMethod(Name = "AnimationStreamBindings::GetAngularVelocity", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 GetAngularVelocity()
		{
			Vector3 vector;
			AnimationStream.GetAngularVelocity_Injected(ref this, out vector);
			return vector;
		}

		[NativeMethod(Name = "AnimationStreamBindings::SetAngularVelocity", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void SetAngularVelocity(Vector3 velocity)
		{
			AnimationStream.SetAngularVelocity_Injected(ref this, ref velocity);
		}

		[NativeMethod(Name = "AnimationStreamBindings::GetRootMotionPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 GetRootMotionPosition()
		{
			Vector3 vector;
			AnimationStream.GetRootMotionPosition_Injected(ref this, out vector);
			return vector;
		}

		[NativeMethod(Name = "AnimationStreamBindings::GetRootMotionRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Quaternion GetRootMotionRotation()
		{
			Quaternion quaternion;
			AnimationStream.GetRootMotionRotation_Injected(ref this, out quaternion);
			return quaternion;
		}

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetInputStreamCount();

		[NativeMethod(Name = "GetInputStream", IsThreadSafe = true)]
		private AnimationStream InternalGetInputStream(int index)
		{
			AnimationStream animationStream;
			AnimationStream.InternalGetInputStream_Injected(ref this, index, out animationStream);
			return animationStream;
		}

		[NativeMethod(Name = "GetInputWeight", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float InternalGetInputWeight(int index);

		[NativeMethod(IsThreadSafe = true)]
		private AnimationHumanStream GetHumanStream()
		{
			AnimationHumanStream animationHumanStream;
			AnimationStream.GetHumanStream_Injected(ref this, out animationHumanStream);
			return animationHumanStream;
		}

		[NativeMethod(Name = "ReadSceneTransforms", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalReadSceneTransforms();

		[NativeMethod(Name = "WriteSceneTransforms", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalWriteSceneTransforms();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyAnimationStreamMotionInternal_Injected(ref AnimationStream _unity_self, [In] ref AnimationStream animationStream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVelocity_Injected(ref AnimationStream _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVelocity_Injected(ref AnimationStream _unity_self, [In] ref Vector3 velocity);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAngularVelocity_Injected(ref AnimationStream _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAngularVelocity_Injected(ref AnimationStream _unity_self, [In] ref Vector3 velocity);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRootMotionPosition_Injected(ref AnimationStream _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRootMotionRotation_Injected(ref AnimationStream _unity_self, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetInputStream_Injected(ref AnimationStream _unity_self, int index, out AnimationStream ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetHumanStream_Injected(ref AnimationStream _unity_self, out AnimationHumanStream ret);

		private uint m_AnimatorBindingsVersion;

		private IntPtr constant;

		private IntPtr input;

		private IntPtr output;

		private IntPtr workspace;

		private IntPtr inputStreamAccessor;

		private IntPtr animationHandleBinder;

		internal const int InvalidIndex = -1;
	}
}
