using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.Animations
{
	/// <summary>
	///   <para>Handle for a Component property on an object in the AnimationStream.</para>
	/// </summary>
	[NativeHeader("Runtime/Animation/Director/AnimationStreamHandles.h")]
	public struct PropertyStreamHandle
	{
		/// <summary>
		///   <para>Returns whether or not the handle is valid.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream holding the animated values.</param>
		/// <returns>
		///   <para>Whether or not the handle is valid.</para>
		/// </returns>
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
				return this.animatorBindingsVersion != 0U;
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

		/// <summary>
		///   <para>Resolves the handle.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream holding the animated values.</param>
		public void Resolve(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
		}

		/// <summary>
		///   <para>Returns whether or not the handle is resolved.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream holding the animated values.</param>
		/// <returns>
		///   <para>Returns true if the handle is resolved, false otherwise.</para>
		/// </returns>
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
			if (!this.IsResolvedInternal(ref stream))
			{
				if (!this.createdByNative || !this.hasHandleIndex || !this.hasBindType)
				{
					throw new InvalidOperationException("The PropertyStreamHandle is invalid. Please use proper function to create the handle.");
				}
				if (!this.IsSameVersionAsStream(ref stream) || (this.hasHandleIndex && !this.hasValueArrayIndex))
				{
					this.ResolveInternal(ref stream);
				}
				if (this.hasHandleIndex && !this.hasValueArrayIndex)
				{
					throw new InvalidOperationException("The PropertyStreamHandle cannot be resolved.");
				}
			}
		}

		/// <summary>
		///   <para>Gets the float property value from a stream.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream holding the animated values.</param>
		/// <returns>
		///   <para>The float property value.</para>
		/// </returns>
		public float GetFloat(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
			if (this.bindType != 5)
			{
				throw new InvalidOperationException("GetValue type doesn't match PropertyStreamHandle bound type.");
			}
			return this.GetFloatInternal(ref stream);
		}

		/// <summary>
		///   <para>Sets the float property value into a stream.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream holding the animated values.</param>
		/// <param name="value">The new float property value.</param>
		public void SetFloat(AnimationStream stream, float value)
		{
			this.CheckIsValidAndResolve(ref stream);
			if (this.bindType != 5)
			{
				throw new InvalidOperationException("SetValue type doesn't match PropertyStreamHandle bound type.");
			}
			this.SetFloatInternal(ref stream, value);
		}

		/// <summary>
		///   <para>Gets the integer property value from a stream.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream holding the animated values.</param>
		/// <returns>
		///   <para>The integer property value.</para>
		/// </returns>
		public int GetInt(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
			if (this.bindType != 10 && this.bindType != 11)
			{
				throw new InvalidOperationException("GetValue type doesn't match PropertyStreamHandle bound type.");
			}
			return this.GetIntInternal(ref stream);
		}

		/// <summary>
		///   <para>Sets the integer property value into a stream.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream holding the animated values.</param>
		/// <param name="value">The new integer property value.</param>
		public void SetInt(AnimationStream stream, int value)
		{
			this.CheckIsValidAndResolve(ref stream);
			if (this.bindType != 10 && this.bindType != 11)
			{
				throw new InvalidOperationException("SetValue type doesn't match PropertyStreamHandle bound type.");
			}
			this.SetIntInternal(ref stream, value);
		}

		/// <summary>
		///   <para>Gets the boolean property value from a stream.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream holding the animated values.</param>
		/// <returns>
		///   <para>The boolean property value.</para>
		/// </returns>
		public bool GetBool(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
			if (this.bindType != 6 && this.bindType != 7)
			{
				throw new InvalidOperationException("GetValue type doesn't match PropertyStreamHandle bound type.");
			}
			return this.GetBoolInternal(ref stream);
		}

		/// <summary>
		///   <para>Sets the boolean property value into a stream.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream holding the animated values.</param>
		/// <param name="value">The new boolean property value.</param>
		public void SetBool(AnimationStream stream, bool value)
		{
			this.CheckIsValidAndResolve(ref stream);
			if (this.bindType != 6 && this.bindType != 7)
			{
				throw new InvalidOperationException("SetValue type doesn't match PropertyStreamHandle bound type.");
			}
			this.SetBoolInternal(ref stream, value);
		}

		[NativeMethod(Name = "Resolve", IsThreadSafe = true)]
		private void ResolveInternal(ref AnimationStream stream)
		{
			PropertyStreamHandle.ResolveInternal_Injected(ref this, ref stream);
		}

		[NativeMethod(Name = "GetFloat", IsThreadSafe = true)]
		private float GetFloatInternal(ref AnimationStream stream)
		{
			return PropertyStreamHandle.GetFloatInternal_Injected(ref this, ref stream);
		}

		[NativeMethod(Name = "SetFloat", IsThreadSafe = true)]
		private void SetFloatInternal(ref AnimationStream stream, float value)
		{
			PropertyStreamHandle.SetFloatInternal_Injected(ref this, ref stream, value);
		}

		[NativeMethod(Name = "GetInt", IsThreadSafe = true)]
		private int GetIntInternal(ref AnimationStream stream)
		{
			return PropertyStreamHandle.GetIntInternal_Injected(ref this, ref stream);
		}

		[NativeMethod(Name = "SetInt", IsThreadSafe = true)]
		private void SetIntInternal(ref AnimationStream stream, int value)
		{
			PropertyStreamHandle.SetIntInternal_Injected(ref this, ref stream, value);
		}

		[NativeMethod(Name = "GetBool", IsThreadSafe = true)]
		private bool GetBoolInternal(ref AnimationStream stream)
		{
			return PropertyStreamHandle.GetBoolInternal_Injected(ref this, ref stream);
		}

		[NativeMethod(Name = "SetBool", IsThreadSafe = true)]
		private void SetBoolInternal(ref AnimationStream stream, bool value)
		{
			PropertyStreamHandle.SetBoolInternal_Injected(ref this, ref stream, value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResolveInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFloatInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIntInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIntInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetBoolInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoolInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream, bool value);

		private uint m_AnimatorBindingsVersion;

		private int handleIndex;

		private int valueArrayIndex;

		private int bindType;
	}
}
