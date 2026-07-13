using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine
{
	[NativeHeader("Modules/Animation/AvatarMask.h")]
	[MovedFrom(true, "UnityEditor.Animations", "UnityEditor", null)]
	[UsedByNativeCode]
	[NativeHeader("Modules/Animation/ScriptBindings/Animation.bindings.h")]
	public sealed class AvatarMask : Object
	{
		public AvatarMask()
		{
			AvatarMask.Internal_Create(this);
		}

		[FreeFunction("AnimationBindings::CreateAvatarMask")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] AvatarMask self);

		[Obsolete("AvatarMask.humanoidBodyPartCount is deprecated, use AvatarMaskBodyPart.LastBodyPart instead.")]
		public int humanoidBodyPartCount
		{
			get
			{
				return 13;
			}
		}

		[NativeMethod("GetBodyPart")]
		public bool GetHumanoidBodyPartActive(AvatarMaskBodyPart index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AvatarMask>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AvatarMask.GetHumanoidBodyPartActive_Injected(intPtr, index);
		}

		[NativeMethod("SetBodyPart")]
		public void SetHumanoidBodyPartActive(AvatarMaskBodyPart index, bool value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AvatarMask>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AvatarMask.SetHumanoidBodyPartActive_Injected(intPtr, index, value);
		}

		public int transformCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AvatarMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AvatarMask.get_transformCount_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AvatarMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AvatarMask.set_transformCount_Injected(intPtr, value);
			}
		}

		public void AddTransformPath(Transform transform)
		{
			this.AddTransformPath(transform, true);
		}

		public void AddTransformPath([NotNull] Transform transform, [DefaultValue("true")] bool recursive)
		{
			if (transform == null)
			{
				ThrowHelper.ThrowArgumentNullException(transform, "transform");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AvatarMask>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Transform>(transform);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(transform, "transform");
			}
			AvatarMask.AddTransformPath_Injected(intPtr, intPtr2, recursive);
		}

		public void RemoveTransformPath(Transform transform)
		{
			this.RemoveTransformPath(transform, true);
		}

		public void RemoveTransformPath([NotNull] Transform transform, [DefaultValue("true")] bool recursive)
		{
			if (transform == null)
			{
				ThrowHelper.ThrowArgumentNullException(transform, "transform");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AvatarMask>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Transform>(transform);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(transform, "transform");
			}
			AvatarMask.RemoveTransformPath_Injected(intPtr, intPtr2, recursive);
		}

		public string GetTransformPath(int index)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AvatarMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				AvatarMask.GetTransformPath_Injected(intPtr, index, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public unsafe void SetTransformPath(int index, string path)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AvatarMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = path.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				AvatarMask.SetTransformPath_Injected(intPtr, index, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		private float GetTransformWeight(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AvatarMask>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AvatarMask.GetTransformWeight_Injected(intPtr, index);
		}

		private void SetTransformWeight(int index, float weight)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AvatarMask>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AvatarMask.SetTransformWeight_Injected(intPtr, index, weight);
		}

		public bool GetTransformActive(int index)
		{
			return this.GetTransformWeight(index) > 0.5f;
		}

		public void SetTransformActive(int index, bool value)
		{
			this.SetTransformWeight(index, value ? 1f : 0f);
		}

		internal bool hasFeetIK
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AvatarMask>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AvatarMask.get_hasFeetIK_Injected(intPtr);
			}
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetHumanoidBodyPartActive_Injected(IntPtr _unity_self, AvatarMaskBodyPart index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetHumanoidBodyPartActive_Injected(IntPtr _unity_self, AvatarMaskBodyPart index, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_transformCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_transformCount_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddTransformPath_Injected(IntPtr _unity_self, IntPtr transform, [DefaultValue("true")] bool recursive);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveTransformPath_Injected(IntPtr _unity_self, IntPtr transform, [DefaultValue("true")] bool recursive);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTransformPath_Injected(IntPtr _unity_self, int index, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTransformPath_Injected(IntPtr _unity_self, int index, ref ManagedSpanWrapper path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetTransformWeight_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTransformWeight_Injected(IntPtr _unity_self, int index, float weight);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasFeetIK_Injected(IntPtr _unity_self);
	}
}
