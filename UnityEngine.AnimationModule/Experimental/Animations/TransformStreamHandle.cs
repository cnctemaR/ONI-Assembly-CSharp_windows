using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.Animations
{
	/// <summary>
	///   <para>Position, rotation and scale of an object in the AnimationStream.</para>
	/// </summary>
	[NativeHeader("Runtime/Animation/Director/AnimationStreamHandles.h")]
	[NativeHeader("Runtime/Animation/ScriptBindings/AnimationStreamHandles.bindings.h")]
	public struct TransformStreamHandle
	{
		/// <summary>
		///   <para>Returns whether this is a valid handle.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream that hold the animated values.</param>
		/// <returns>
		///   <para>Whether this is a valid handle.</para>
		/// </returns>
		public bool IsValid(AnimationStream stream)
		{
			return this.IsValidInternal(ref stream);
		}

		private bool IsValidInternal(ref AnimationStream stream)
		{
			return stream.isValid && this.createdByNative && this.hasHandleIndex;
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

		private bool hasSkeletonIndex
		{
			get
			{
				return this.skeletonIndex != -1;
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
		///   <para>Bind this handle with an animated values from the AnimationStream.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream that hold the animated values.</param>
		public void Resolve(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
		}

		/// <summary>
		///   <para>Returns whether this handle is resolved.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream that hold the animated values.</param>
		/// <returns>
		///   <para>Returns true if the handle is resolved, false otherwise.</para>
		/// </returns>
		public bool IsResolved(AnimationStream stream)
		{
			return this.IsResolvedInternal(ref stream);
		}

		private bool IsResolvedInternal(ref AnimationStream stream)
		{
			return this.IsValidInternal(ref stream) && this.IsSameVersionAsStream(ref stream) && this.hasSkeletonIndex;
		}

		private void CheckIsValidAndResolve(ref AnimationStream stream)
		{
			stream.CheckIsValid();
			if (!this.IsResolvedInternal(ref stream))
			{
				if (!this.createdByNative || !this.hasHandleIndex)
				{
					throw new InvalidOperationException("The TransformStreamHandle is invalid. Please use proper function to create the handle.");
				}
				if (!this.IsSameVersionAsStream(ref stream) || (this.hasHandleIndex && !this.hasSkeletonIndex))
				{
					this.ResolveInternal(ref stream);
				}
				if (this.hasHandleIndex && !this.hasSkeletonIndex)
				{
					throw new InvalidOperationException("The TransformStreamHandle cannot be resolved.");
				}
			}
		}

		/// <summary>
		///   <para>Gets the position of the transform in world space.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream that hold the animated values.</param>
		/// <returns>
		///   <para>The position of the transform in world space.</para>
		/// </returns>
		public Vector3 GetPosition(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
			return this.GetPositionInternal(ref stream);
		}

		/// <summary>
		///   <para>Sets the position of the transform in world space.</para>
		/// </summary>
		/// <param name="position">The position of the transform in world space.</param>
		/// <param name="stream">The AnimationStream that hold the animated values.</param>
		public void SetPosition(AnimationStream stream, Vector3 position)
		{
			this.CheckIsValidAndResolve(ref stream);
			this.SetPositionInternal(ref stream, position);
		}

		/// <summary>
		///   <para>Gets the rotation of the transform in world space.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream that hold the animated values.</param>
		/// <returns>
		///   <para>The rotation of the transform in world space.</para>
		/// </returns>
		public Quaternion GetRotation(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
			return this.GetRotationInternal(ref stream);
		}

		/// <summary>
		///   <para>Sets the rotation of the transform in world space.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream that hold the animated values.</param>
		/// <param name="rotation">The rotation of the transform in world space.</param>
		public void SetRotation(AnimationStream stream, Quaternion rotation)
		{
			this.CheckIsValidAndResolve(ref stream);
			this.SetRotationInternal(ref stream, rotation);
		}

		/// <summary>
		///   <para>Gets the position of the transform relative to the parent.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream that hold the animated values.</param>
		/// <returns>
		///   <para>The position of the transform relative to the parent.</para>
		/// </returns>
		public Vector3 GetLocalPosition(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
			return this.GetLocalPositionInternal(ref stream);
		}

		/// <summary>
		///   <para>Sets the position of the transform relative to the parent.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream that hold the animated values.</param>
		/// <param name="position">The position of the transform relative to the parent.</param>
		public void SetLocalPosition(AnimationStream stream, Vector3 position)
		{
			this.CheckIsValidAndResolve(ref stream);
			this.SetLocalPositionInternal(ref stream, position);
		}

		/// <summary>
		///   <para>Gets the rotation of the transform relative to the parent.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream that hold the animated values.</param>
		/// <returns>
		///   <para>The rotation of the transform relative to the parent.</para>
		/// </returns>
		public Quaternion GetLocalRotation(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
			return this.GetLocalRotationInternal(ref stream);
		}

		/// <summary>
		///   <para>Sets the rotation of the transform relative to the parent.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream that hold the animated values.</param>
		/// <param name="rotation">The rotation of the transform relative to the parent.</param>
		public void SetLocalRotation(AnimationStream stream, Quaternion rotation)
		{
			this.CheckIsValidAndResolve(ref stream);
			this.SetLocalRotationInternal(ref stream, rotation);
		}

		/// <summary>
		///   <para>Gets the scale of the transform relative to the parent.</para>
		/// </summary>
		/// <param name="stream">The AnimationStream that hold the animated values.</param>
		/// <returns>
		///   <para>The scale of the transform relative to the parent.</para>
		/// </returns>
		public Vector3 GetLocalScale(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
			return this.GetLocalScaleInternal(ref stream);
		}

		/// <summary>
		///   <para>Sets the scale of the transform relative to the parent.</para>
		/// </summary>
		/// <param name="scale">The scale of the transform relative to the parent.</param>
		/// <param name="stream">The AnimationStream that hold the animated values.</param>
		public void SetLocalScale(AnimationStream stream, Vector3 scale)
		{
			this.CheckIsValidAndResolve(ref stream);
			this.SetLocalScaleInternal(ref stream, scale);
		}

		[NativeMethod(Name = "Resolve", IsThreadSafe = true)]
		private void ResolveInternal(ref AnimationStream stream)
		{
			TransformStreamHandle.ResolveInternal_Injected(ref this, ref stream);
		}

		[NativeMethod(Name = "TransformStreamHandleBindings::GetPositionInternal", IsFreeFunction = true, HasExplicitThis = true, IsThreadSafe = true)]
		private Vector3 GetPositionInternal(ref AnimationStream stream)
		{
			Vector3 vector;
			TransformStreamHandle.GetPositionInternal_Injected(ref this, ref stream, out vector);
			return vector;
		}

		[NativeMethod(Name = "TransformStreamHandleBindings::SetPositionInternal", IsFreeFunction = true, HasExplicitThis = true, IsThreadSafe = true)]
		private void SetPositionInternal(ref AnimationStream stream, Vector3 position)
		{
			TransformStreamHandle.SetPositionInternal_Injected(ref this, ref stream, ref position);
		}

		[NativeMethod(Name = "TransformStreamHandleBindings::GetRotationInternal", IsFreeFunction = true, HasExplicitThis = true, IsThreadSafe = true)]
		private Quaternion GetRotationInternal(ref AnimationStream stream)
		{
			Quaternion quaternion;
			TransformStreamHandle.GetRotationInternal_Injected(ref this, ref stream, out quaternion);
			return quaternion;
		}

		[NativeMethod(Name = "TransformStreamHandleBindings::SetRotationInternal", IsFreeFunction = true, HasExplicitThis = true, IsThreadSafe = true)]
		private void SetRotationInternal(ref AnimationStream stream, Quaternion rotation)
		{
			TransformStreamHandle.SetRotationInternal_Injected(ref this, ref stream, ref rotation);
		}

		[NativeMethod(Name = "TransformStreamHandleBindings::GetLocalPositionInternal", IsFreeFunction = true, HasExplicitThis = true, IsThreadSafe = true)]
		private Vector3 GetLocalPositionInternal(ref AnimationStream stream)
		{
			Vector3 vector;
			TransformStreamHandle.GetLocalPositionInternal_Injected(ref this, ref stream, out vector);
			return vector;
		}

		[NativeMethod(Name = "TransformStreamHandleBindings::SetLocalPositionInternal", IsFreeFunction = true, HasExplicitThis = true, IsThreadSafe = true)]
		private void SetLocalPositionInternal(ref AnimationStream stream, Vector3 position)
		{
			TransformStreamHandle.SetLocalPositionInternal_Injected(ref this, ref stream, ref position);
		}

		[NativeMethod(Name = "TransformStreamHandleBindings::GetLocalRotationInternal", IsFreeFunction = true, HasExplicitThis = true, IsThreadSafe = true)]
		private Quaternion GetLocalRotationInternal(ref AnimationStream stream)
		{
			Quaternion quaternion;
			TransformStreamHandle.GetLocalRotationInternal_Injected(ref this, ref stream, out quaternion);
			return quaternion;
		}

		[NativeMethod(Name = "TransformStreamHandleBindings::SetLocalRotationInternal", IsFreeFunction = true, HasExplicitThis = true, IsThreadSafe = true)]
		private void SetLocalRotationInternal(ref AnimationStream stream, Quaternion rotation)
		{
			TransformStreamHandle.SetLocalRotationInternal_Injected(ref this, ref stream, ref rotation);
		}

		[NativeMethod(Name = "TransformStreamHandleBindings::GetLocalScaleInternal", IsFreeFunction = true, HasExplicitThis = true, IsThreadSafe = true)]
		private Vector3 GetLocalScaleInternal(ref AnimationStream stream)
		{
			Vector3 vector;
			TransformStreamHandle.GetLocalScaleInternal_Injected(ref this, ref stream, out vector);
			return vector;
		}

		[NativeMethod(Name = "TransformStreamHandleBindings::SetLocalScaleInternal", IsFreeFunction = true, HasExplicitThis = true, IsThreadSafe = true)]
		private void SetLocalScaleInternal(ref AnimationStream stream, Vector3 scale)
		{
			TransformStreamHandle.SetLocalScaleInternal_Injected(ref this, ref stream, ref scale);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResolveInternal_Injected(ref TransformStreamHandle _unity_self, ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPositionInternal_Injected(ref TransformStreamHandle _unity_self, ref AnimationStream stream, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPositionInternal_Injected(ref TransformStreamHandle _unity_self, ref AnimationStream stream, ref Vector3 position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRotationInternal_Injected(ref TransformStreamHandle _unity_self, ref AnimationStream stream, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRotationInternal_Injected(ref TransformStreamHandle _unity_self, ref AnimationStream stream, ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalPositionInternal_Injected(ref TransformStreamHandle _unity_self, ref AnimationStream stream, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLocalPositionInternal_Injected(ref TransformStreamHandle _unity_self, ref AnimationStream stream, ref Vector3 position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalRotationInternal_Injected(ref TransformStreamHandle _unity_self, ref AnimationStream stream, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLocalRotationInternal_Injected(ref TransformStreamHandle _unity_self, ref AnimationStream stream, ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalScaleInternal_Injected(ref TransformStreamHandle _unity_self, ref AnimationStream stream, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLocalScaleInternal_Injected(ref TransformStreamHandle _unity_self, ref AnimationStream stream, ref Vector3 scale);

		private uint m_AnimatorBindingsVersion;

		private int handleIndex;

		private int skeletonIndex;
	}
}
