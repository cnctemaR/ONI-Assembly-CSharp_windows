using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.Animations
{
	[NativeHeader("Runtime/Animation/Director/AnimationSceneHandles.h")]
	public struct PropertySceneHandle
	{
		public bool IsValid(AnimationStream stream)
		{
			return this.IsValidInternal(ref stream);
		}

		private bool IsValidInternal(ref AnimationStream stream)
		{
			return stream.isValid && this.createdByNative && this.hasHandleIndex && this.HasValidTransform(ref stream);
		}

		private bool createdByNative
		{
			get
			{
				return this.valid != 0U;
			}
		}

		private bool hasHandleIndex
		{
			get
			{
				return this.handleIndex != -1;
			}
		}

		public void Resolve(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			this.ResolveInternal(ref stream);
		}

		public bool IsResolved(AnimationStream stream)
		{
			return this.IsValidInternal(ref stream) && this.IsBound(ref stream);
		}

		private void CheckIsValid(ref AnimationStream stream)
		{
			stream.CheckIsValid();
			if (!this.createdByNative || !this.hasHandleIndex)
			{
				throw new InvalidOperationException("The PropertySceneHandle is invalid. Please use proper function to create the handle.");
			}
			if (!this.HasValidTransform(ref stream))
			{
				throw new NullReferenceException("The transform is invalid.");
			}
		}

		public float GetFloat(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetFloatInternal(ref stream);
		}

		public void SetFloat(AnimationStream stream, float value)
		{
			this.CheckIsValid(ref stream);
			this.SetFloatInternal(ref stream, value);
		}

		public int GetInt(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetIntInternal(ref stream);
		}

		public void SetInt(AnimationStream stream, int value)
		{
			this.CheckIsValid(ref stream);
			this.SetIntInternal(ref stream, value);
		}

		public bool GetBool(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetBoolInternal(ref stream);
		}

		public void SetBool(AnimationStream stream, bool value)
		{
			this.CheckIsValid(ref stream);
			this.SetBoolInternal(ref stream, value);
		}

		[ThreadSafe]
		private bool HasValidTransform(ref AnimationStream stream)
		{
			return PropertySceneHandle.HasValidTransform_Injected(ref this, ref stream);
		}

		[ThreadSafe]
		private bool IsBound(ref AnimationStream stream)
		{
			return PropertySceneHandle.IsBound_Injected(ref this, ref stream);
		}

		[NativeMethod(Name = "Resolve", IsThreadSafe = true)]
		private void ResolveInternal(ref AnimationStream stream)
		{
			PropertySceneHandle.ResolveInternal_Injected(ref this, ref stream);
		}

		[NativeMethod(Name = "GetFloat", IsThreadSafe = true)]
		private float GetFloatInternal(ref AnimationStream stream)
		{
			return PropertySceneHandle.GetFloatInternal_Injected(ref this, ref stream);
		}

		[NativeMethod(Name = "SetFloat", IsThreadSafe = true)]
		private void SetFloatInternal(ref AnimationStream stream, float value)
		{
			PropertySceneHandle.SetFloatInternal_Injected(ref this, ref stream, value);
		}

		[NativeMethod(Name = "GetInt", IsThreadSafe = true)]
		private int GetIntInternal(ref AnimationStream stream)
		{
			return PropertySceneHandle.GetIntInternal_Injected(ref this, ref stream);
		}

		[NativeMethod(Name = "SetInt", IsThreadSafe = true)]
		private void SetIntInternal(ref AnimationStream stream, int value)
		{
			PropertySceneHandle.SetIntInternal_Injected(ref this, ref stream, value);
		}

		[NativeMethod(Name = "GetBool", IsThreadSafe = true)]
		private bool GetBoolInternal(ref AnimationStream stream)
		{
			return PropertySceneHandle.GetBoolInternal_Injected(ref this, ref stream);
		}

		[NativeMethod(Name = "SetBool", IsThreadSafe = true)]
		private void SetBoolInternal(ref AnimationStream stream, bool value)
		{
			PropertySceneHandle.SetBoolInternal_Injected(ref this, ref stream, value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasValidTransform_Injected(ref PropertySceneHandle _unity_self, ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsBound_Injected(ref PropertySceneHandle _unity_self, ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResolveInternal_Injected(ref PropertySceneHandle _unity_self, ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFloatInternal_Injected(ref PropertySceneHandle _unity_self, ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatInternal_Injected(ref PropertySceneHandle _unity_self, ref AnimationStream stream, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIntInternal_Injected(ref PropertySceneHandle _unity_self, ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIntInternal_Injected(ref PropertySceneHandle _unity_self, ref AnimationStream stream, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetBoolInternal_Injected(ref PropertySceneHandle _unity_self, ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoolInternal_Injected(ref PropertySceneHandle _unity_self, ref AnimationStream stream, bool value);

		private uint valid;

		private int handleIndex;
	}
}
