using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations
{
	[MovedFrom("UnityEngine.Experimental.Animations")]
	[NativeHeader("Modules/Animation/Director/AnimationSceneHandles.h")]
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
				return this.valid > 0U;
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
			bool flag = !this.createdByNative || !this.hasHandleIndex;
			if (flag)
			{
				throw new InvalidOperationException("The PropertySceneHandle is invalid. Please use proper function to create the handle.");
			}
			bool flag2 = !this.HasValidTransform(ref stream);
			if (flag2)
			{
				throw new NullReferenceException("The transform is invalid.");
			}
		}

		public float GetFloat(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetFloatInternal(ref stream);
		}

		[Obsolete("SceneHandle is now read-only; it was problematic with the engine multithreading and determinism", true)]
		public void SetFloat(AnimationStream stream, float value)
		{
		}

		public int GetInt(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetIntInternal(ref stream);
		}

		public EntityId GetEntityId(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetEntityIdInternal(ref stream);
		}

		[Obsolete("SceneHandle is now read-only; it was problematic with the engine multithreading and determinism", true)]
		public void SetInt(AnimationStream stream, int value)
		{
		}

		public bool GetBool(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetBoolInternal(ref stream);
		}

		[Obsolete("SceneHandle is now read-only; it was problematic with the engine multithreading and determinism", true)]
		public void SetBool(AnimationStream stream, bool value)
		{
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool HasValidTransform(ref AnimationStream stream);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsBound(ref AnimationStream stream);

		[NativeMethod(Name = "Resolve", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ResolveInternal(ref AnimationStream stream);

		[NativeMethod(Name = "GetFloat", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetFloatInternal(ref AnimationStream stream);

		[NativeMethod(Name = "GetInt", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetIntInternal(ref AnimationStream stream);

		[NativeMethod(Name = "GetEntityId", IsThreadSafe = true)]
		private EntityId GetEntityIdInternal(ref AnimationStream stream)
		{
			EntityId entityId;
			PropertySceneHandle.GetEntityIdInternal_Injected(ref this, ref stream, out entityId);
			return entityId;
		}

		[NativeMethod(Name = "GetBool", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetBoolInternal(ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetEntityIdInternal_Injected(ref PropertySceneHandle _unity_self, ref AnimationStream stream, out EntityId ret);

		private uint valid;

		private int handleIndex;
	}
}
