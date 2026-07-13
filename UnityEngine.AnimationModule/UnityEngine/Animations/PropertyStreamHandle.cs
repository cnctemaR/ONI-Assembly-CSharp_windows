using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations
{
	[NativeHeader("Modules/Animation/Director/AnimationStreamHandles.h")]
	[MovedFrom("UnityEngine.Experimental.Animations")]
	public struct PropertyStreamHandle
	{
		public bool IsValid(AnimationStream stream)
		{
			return this.IsValidInternal(ref stream);
		}

		private bool IsValidInternal(ref AnimationStream stream)
		{
			return stream.isValid && this.createdByNative && this.hasHandleIndex && this.hasBindType;
		}

		private bool createdByNative
		{
			get
			{
				return this.animatorBindingsVersion > 0U;
			}
		}

		private bool IsSameVersionAsStream(ref AnimationStream stream)
		{
			return this.animatorBindingsVersion == stream.animatorBindingsVersion;
		}

		private bool hasHandleIndex
		{
			get
			{
				return this.handleIndex != -1;
			}
		}

		private bool hasValueArrayIndex
		{
			get
			{
				return this.valueArrayIndex != -1;
			}
		}

		private bool hasBindType
		{
			get
			{
				return this.bindType != 0;
			}
		}

		internal uint animatorBindingsVersion
		{
			get
			{
				return this.m_AnimatorBindingsVersion;
			}
			private set
			{
				this.m_AnimatorBindingsVersion = value;
			}
		}

		public void Resolve(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
		}

		public bool IsResolved(AnimationStream stream)
		{
			return this.IsResolvedInternal(ref stream);
		}

		private bool IsResolvedInternal(ref AnimationStream stream)
		{
			return this.IsValidInternal(ref stream) && this.IsSameVersionAsStream(ref stream) && this.hasValueArrayIndex;
		}

		private void CheckIsValidAndResolve(ref AnimationStream stream)
		{
			stream.CheckIsValid();
			bool flag = this.IsResolvedInternal(ref stream);
			if (!flag)
			{
				bool flag2 = !this.createdByNative || !this.hasHandleIndex || !this.hasBindType;
				if (flag2)
				{
					throw new InvalidOperationException("The PropertyStreamHandle is invalid. Please use proper function to create the handle.");
				}
				bool flag3 = !this.IsSameVersionAsStream(ref stream) || (this.hasHandleIndex && !this.hasValueArrayIndex);
				if (flag3)
				{
					this.ResolveInternal(ref stream);
				}
				bool flag4 = this.hasHandleIndex && !this.hasValueArrayIndex;
				if (flag4)
				{
					throw new InvalidOperationException("The PropertyStreamHandle cannot be resolved.");
				}
			}
		}

		public float GetFloat(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
			bool flag = this.bindType != 5;
			if (flag)
			{
				throw new InvalidOperationException("GetValue type doesn't match PropertyStreamHandle bound type.");
			}
			return this.GetFloatInternal(ref stream);
		}

		public void SetFloat(AnimationStream stream, float value)
		{
			this.CheckIsValidAndResolve(ref stream);
			bool flag = this.bindType != 5;
			if (flag)
			{
				throw new InvalidOperationException("SetValue type doesn't match PropertyStreamHandle bound type.");
			}
			this.SetFloatInternal(ref stream, value);
		}

		public int GetInt(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
			bool flag = this.bindType == 9;
			int num;
			if (flag)
			{
				Debug.LogWarning("Please Use GetEntityId directly to get the value of an ObjectReference PropertyStreamHandle.");
				num = this.GetEntityId(stream);
			}
			else
			{
				bool flag2 = this.bindType != 10 && this.bindType != 11;
				if (flag2)
				{
					throw new InvalidOperationException("GetValue type doesn't match PropertyStreamHandle bound type.");
				}
				num = this.GetIntInternal(ref stream);
			}
			return num;
		}

		public void SetInt(AnimationStream stream, int value)
		{
			this.CheckIsValidAndResolve(ref stream);
			bool flag = this.bindType == 9;
			if (flag)
			{
				Debug.LogWarning("Please Use SetEntityId directly to set the value of an ObjectReference PropertyStreamHandle.");
				this.SetEntityId(stream, value);
			}
			else
			{
				bool flag2 = this.bindType != 10 && this.bindType != 11;
				if (flag2)
				{
					throw new InvalidOperationException("SetValue type doesn't match PropertyStreamHandle bound type.");
				}
				this.SetIntInternal(ref stream, value);
			}
		}

		public EntityId GetEntityId(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
			bool flag = this.bindType != 9;
			if (flag)
			{
				throw new InvalidOperationException("GetValue type doesn't match PropertyStreamHandle bound type.");
			}
			return this.GetEntityIdInternal(ref stream);
		}

		public void SetEntityId(AnimationStream stream, EntityId value)
		{
			this.CheckIsValidAndResolve(ref stream);
			bool flag = this.bindType != 9;
			if (flag)
			{
				throw new InvalidOperationException("SetValue type doesn't match PropertyStreamHandle bound type.");
			}
			this.SetEntityIdInternal(ref stream, value);
		}

		public bool GetBool(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
			bool flag = this.bindType != 6 && this.bindType != 7;
			if (flag)
			{
				throw new InvalidOperationException("GetValue type doesn't match PropertyStreamHandle bound type.");
			}
			return this.GetBoolInternal(ref stream);
		}

		public void SetBool(AnimationStream stream, bool value)
		{
			this.CheckIsValidAndResolve(ref stream);
			bool flag = this.bindType != 6 && this.bindType != 7;
			if (flag)
			{
				throw new InvalidOperationException("SetValue type doesn't match PropertyStreamHandle bound type.");
			}
			this.SetBoolInternal(ref stream, value);
		}

		public bool GetReadMask(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
			return this.GetReadMaskInternal(ref stream);
		}

		[NativeMethod(Name = "Resolve", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ResolveInternal(ref AnimationStream stream);

		[NativeMethod(Name = "GetFloat", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetFloatInternal(ref AnimationStream stream);

		[NativeMethod(Name = "SetFloat", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatInternal(ref AnimationStream stream, float value);

		[NativeMethod(Name = "GetInt", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetIntInternal(ref AnimationStream stream);

		[NativeMethod(Name = "SetInt", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetIntInternal(ref AnimationStream stream, int value);

		[NativeMethod(Name = "GetEntityId", IsThreadSafe = true)]
		private EntityId GetEntityIdInternal(ref AnimationStream stream)
		{
			EntityId entityId;
			PropertyStreamHandle.GetEntityIdInternal_Injected(ref this, ref stream, out entityId);
			return entityId;
		}

		[NativeMethod(Name = "SetEntityId", IsThreadSafe = true)]
		private void SetEntityIdInternal(ref AnimationStream stream, EntityId value)
		{
			PropertyStreamHandle.SetEntityIdInternal_Injected(ref this, ref stream, ref value);
		}

		[NativeMethod(Name = "GetBool", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetBoolInternal(ref AnimationStream stream);

		[NativeMethod(Name = "SetBool", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetBoolInternal(ref AnimationStream stream, bool value);

		[NativeMethod(Name = "GetReadMask", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetReadMaskInternal(ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetEntityIdInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream, out EntityId ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetEntityIdInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream, [In] ref EntityId value);

		private uint m_AnimatorBindingsVersion;

		private int handleIndex;

		private int valueArrayIndex;

		private int bindType;
	}
}
