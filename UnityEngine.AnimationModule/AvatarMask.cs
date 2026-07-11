using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine
{
	/// <summary>
	///   <para>AvatarMask is used to mask out humanoid body parts and transforms.</para>
	/// </summary>
	[NativeHeader("Runtime/Animation/ScriptBindings/Animation.bindings.h")]
	[NativeHeader("Runtime/Animation/AvatarMask.h")]
	[MovedFrom("UnityEditor.Animations", true)]
	[UsedByNativeCode]
	public sealed class AvatarMask : Object
	{
		/// <summary>
		///   <para>Creates a new AvatarMask.</para>
		/// </summary>
		public AvatarMask()
		{
			AvatarMask.Internal_Create(this);
		}

		[FreeFunction("AnimationBindings::CreateAvatarMask")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] AvatarMask self);

		/// <summary>
		///   <para>The number of humanoid body parts.</para>
		/// </summary>
		[Obsolete("AvatarMask.humanoidBodyPartCount is deprecated, use AvatarMaskBodyPart.LastBodyPart instead.")]
		public int humanoidBodyPartCount
		{
			get
			{
				return 13;
			}
		}

		/// <summary>
		///   <para>Returns true if the humanoid body part at the given index is active.</para>
		/// </summary>
		/// <param name="index">The index of the humanoid body part.</param>
		[NativeMethod("GetBodyPart")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetHumanoidBodyPartActive(AvatarMaskBodyPart index);

		/// <summary>
		///   <para>Sets the humanoid body part at the given index to active or not.</para>
		/// </summary>
		/// <param name="index">The index of the humanoid body part.</param>
		/// <param name="value">Active or not.</param>
		[NativeMethod("SetBodyPart")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetHumanoidBodyPartActive(AvatarMaskBodyPart index, bool value);

		/// <summary>
		///   <para>Number of transforms.</para>
		/// </summary>
		public extern int transformCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public void AddTransformPath(Transform transform)
		{
			this.AddTransformPath(transform, true);
		}

		/// <summary>
		///   <para>Adds a transform path into the AvatarMask.</para>
		/// </summary>
		/// <param name="transform">The transform to add into the AvatarMask.</param>
		/// <param name="recursive">Whether to also add all children of the specified transform.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddTransformPath([NotNull] Transform transform, [DefaultValue("true")] bool recursive);

		public void RemoveTransformPath(Transform transform)
		{
			this.RemoveTransformPath(transform, true);
		}

		/// <summary>
		///   <para>Removes a transform path from the AvatarMask.</para>
		/// </summary>
		/// <param name="transform">The Transform that should be removed from the AvatarMask.</param>
		/// <param name="recursive">Whether to also remove all children of the specified transform.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RemoveTransformPath([NotNull] Transform transform, [DefaultValue("true")] bool recursive);

		/// <summary>
		///   <para>Returns the path of the transform at the given index.</para>
		/// </summary>
		/// <param name="index">The index of the transform.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetTransformPath(int index);

		/// <summary>
		///   <para>Sets the path of the transform at the given index.</para>
		/// </summary>
		/// <param name="index">The index of the transform.</param>
		/// <param name="path">The path of the transform.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTransformPath(int index, string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetTransformWeight(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTransformWeight(int index, float weight);

		/// <summary>
		///   <para>Returns true if the transform at the given index is active.</para>
		/// </summary>
		/// <param name="index">The index of the transform.</param>
		public bool GetTransformActive(int index)
		{
			return this.GetTransformWeight(index) > 0.5f;
		}

		/// <summary>
		///   <para>Sets the tranform at the given index to active or not.</para>
		/// </summary>
		/// <param name="index">The index of the transform.</param>
		/// <param name="value">Active or not.</param>
		public void SetTransformActive(int index, bool value)
		{
			this.SetTransformWeight(index, (!value) ? 0f : 1f);
		}

		internal extern bool hasFeetIK
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal void Copy(AvatarMask other)
		{
			for (AvatarMaskBodyPart avatarMaskBodyPart = AvatarMaskBodyPart.Root; avatarMaskBodyPart < AvatarMaskBodyPart.LastBodyPart; avatarMaskBodyPart++)
			{
				this.SetHumanoidBodyPartActive(avatarMaskBodyPart, other.GetHumanoidBodyPartActive(avatarMaskBodyPart));
			}
			this.transformCount = other.transformCount;
			for (int i = 0; i < other.transformCount; i++)
			{
				this.SetTransformPath(i, other.GetTransformPath(i));
				this.SetTransformActive(i, other.GetTransformActive(i));
			}
		}
	}
}
