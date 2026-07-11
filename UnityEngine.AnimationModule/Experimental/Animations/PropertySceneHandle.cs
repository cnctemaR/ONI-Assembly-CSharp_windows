using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.Animations
{
	/// <summary>
	///   <para>Handle for a Component property on an object in the scene.</para>
	/// </summary>
	[NativeHeader("Runtime/Animation/Director/AnimationSceneHandles.h")]
	public struct PropertySceneHandle
	{
		/// <summary>
		///   <para>Returns whether or not the handle is valid.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream managing this handle.</param>
		/// <returns>
		///   <para>Whether or not the handle is valid.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Resolves the handle.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream managing this handle.</param>
		public void Resolve(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			this.ResolveInternal(ref stream);
		}

		/// <summary>
		///   <para>Returns whether or not the handle is resolved.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream managing this handle.</param>
		/// <returns>
		///   <para>Returns true if the handle is resolved, false otherwise.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Gets the float property value from an object in the scene.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream managing this handle.</param>
		/// <returns>
		///   <para>The float property value.</para>
		/// </returns>
		public float GetFloat(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetFloatInternal(ref stream);
		}

		/// <summary>
		///   <para>Sets the float property value to an object in the scene.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream managing this handle.</param>
		/// <param name="value">The new float property value.</param>
		public void SetFloat(AnimationStream stream, float value)
		{
			this.CheckIsValid(ref stream);
			this.SetFloatInternal(ref stream, value);
		}

		/// <summary>
		///   <para>Gets the integer property value from an object in the scene.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream managing this handle.</param>
		/// <returns>
		///   <para>The integer property value.</para>
		/// </returns>
		public int GetInt(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetIntInternal(ref stream);
		}

		/// <summary>
		///   <para>Sets the integer property value to an object in the scene.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream managing this handle.</param>
		/// <param name="value">The new integer property value.</param>
		public void SetInt(AnimationStream stream, int value)
		{
			this.CheckIsValid(ref stream);
			this.SetIntInternal(ref stream, value);
		}

		/// <summary>
		///   <para>Gets the boolean property value from an object in the scene.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream managing this handle.</param>
		/// <returns>
		///   <para>The boolean property value.</para>
		/// </returns>
		public bool GetBool(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetBoolInternal(ref stream);
		}

		/// <summary>
		///   <para>Sets the boolean property value to an object in the scene.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream managing this handle.</param>
		/// <param name="value">The new boolean property value.</param>
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
