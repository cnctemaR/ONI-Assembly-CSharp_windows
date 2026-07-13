using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations
{
	[NativeHeader("Modules/Animation/Director/AnimationSceneHandles.h")]
	[MovedFrom("UnityEngine.Experimental.Animations")]
	[NativeHeader("Modules/Animation/ScriptBindings/AnimationStreamHandles.bindings.h")]
	public struct TransformSceneHandle
	{
		public bool IsValid(AnimationStream stream)
		{
			return stream.isValid && this.createdByNative && this.hasTransformSceneHandleDefinitionIndex && this.HasValidTransform(ref stream);
		}

		private bool createdByNative
		{
			get
			{
				return this.valid > 0U;
			}
		}

		private bool hasTransformSceneHandleDefinitionIndex
		{
			get
			{
				return this.transformSceneHandleDefinitionIndex != -1;
			}
		}

		private void CheckIsValid(ref AnimationStream stream)
		{
			stream.CheckIsValid();
			bool flag = !this.createdByNative || !this.hasTransformSceneHandleDefinitionIndex;
			if (flag)
			{
				throw new InvalidOperationException("The TransformSceneHandle is invalid. Please use proper function to create the handle.");
			}
			bool flag2 = !this.HasValidTransform(ref stream);
			if (flag2)
			{
				throw new NullReferenceException("The transform is invalid.");
			}
		}

		public Vector3 GetPosition(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetPositionInternal(ref stream);
		}

		[Obsolete("SceneHandle is now read-only; it was problematic with the engine multithreading and determinism", true)]
		public void SetPosition(AnimationStream stream, Vector3 position)
		{
		}

		public Vector3 GetLocalPosition(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetLocalPositionInternal(ref stream);
		}

		[Obsolete("SceneHandle is now read-only; it was problematic with the engine multithreading and determinism", true)]
		public void SetLocalPosition(AnimationStream stream, Vector3 position)
		{
		}

		public Quaternion GetRotation(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetRotationInternal(ref stream);
		}

		[Obsolete("SceneHandle is now read-only; it was problematic with the engine multithreading and determinism", true)]
		public void SetRotation(AnimationStream stream, Quaternion rotation)
		{
		}

		public Quaternion GetLocalRotation(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetLocalRotationInternal(ref stream);
		}

		[Obsolete("SceneHandle is now read-only; it was problematic with the engine multithreading and determinism", true)]
		public void SetLocalRotation(AnimationStream stream, Quaternion rotation)
		{
		}

		public Vector3 GetLocalScale(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetLocalScaleInternal(ref stream);
		}

		public void GetLocalTRS(AnimationStream stream, out Vector3 position, out Quaternion rotation, out Vector3 scale)
		{
			this.CheckIsValid(ref stream);
			this.GetLocalTRSInternal(ref stream, out position, out rotation, out scale);
		}

		public Matrix4x4 GetLocalToParentMatrix(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetLocalToParentMatrixInternal(ref stream);
		}

		public void GetGlobalTR(AnimationStream stream, out Vector3 position, out Quaternion rotation)
		{
			this.CheckIsValid(ref stream);
			this.GetGlobalTRInternal(ref stream, out position, out rotation);
		}

		public Matrix4x4 GetLocalToWorldMatrix(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetLocalToWorldMatrixInternal(ref stream);
		}

		[Obsolete("SceneHandle is now read-only; it was problematic with the engine multithreading and determinism", true)]
		public void SetLocalScale(AnimationStream stream, Vector3 scale)
		{
		}

		[ThreadSafe]
		private bool HasValidTransform(ref AnimationStream stream)
		{
			return TransformSceneHandle.HasValidTransform_Injected(ref this, ref stream);
		}

		[NativeMethod(Name = "TransformSceneHandleBindings::GetPositionInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 GetPositionInternal(ref AnimationStream stream)
		{
			Vector3 vector;
			TransformSceneHandle.GetPositionInternal_Injected(ref this, ref stream, out vector);
			return vector;
		}

		[NativeMethod(Name = "TransformSceneHandleBindings::GetLocalPositionInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 GetLocalPositionInternal(ref AnimationStream stream)
		{
			Vector3 vector;
			TransformSceneHandle.GetLocalPositionInternal_Injected(ref this, ref stream, out vector);
			return vector;
		}

		[NativeMethod(Name = "TransformSceneHandleBindings::GetRotationInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Quaternion GetRotationInternal(ref AnimationStream stream)
		{
			Quaternion quaternion;
			TransformSceneHandle.GetRotationInternal_Injected(ref this, ref stream, out quaternion);
			return quaternion;
		}

		[NativeMethod(Name = "TransformSceneHandleBindings::GetLocalRotationInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Quaternion GetLocalRotationInternal(ref AnimationStream stream)
		{
			Quaternion quaternion;
			TransformSceneHandle.GetLocalRotationInternal_Injected(ref this, ref stream, out quaternion);
			return quaternion;
		}

		[NativeMethod(Name = "TransformSceneHandleBindings::GetLocalScaleInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 GetLocalScaleInternal(ref AnimationStream stream)
		{
			Vector3 vector;
			TransformSceneHandle.GetLocalScaleInternal_Injected(ref this, ref stream, out vector);
			return vector;
		}

		[NativeMethod(Name = "TransformSceneHandleBindings::GetLocalTRSInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void GetLocalTRSInternal(ref AnimationStream stream, out Vector3 position, out Quaternion rotation, out Vector3 scale)
		{
			TransformSceneHandle.GetLocalTRSInternal_Injected(ref this, ref stream, out position, out rotation, out scale);
		}

		[NativeMethod(Name = "TransformSceneHandleBindings::GetLocalToParentMatrixInternal", IsFreeFunction = true, HasExplicitThis = true, IsThreadSafe = true)]
		private Matrix4x4 GetLocalToParentMatrixInternal(ref AnimationStream stream)
		{
			Matrix4x4 matrix4x;
			TransformSceneHandle.GetLocalToParentMatrixInternal_Injected(ref this, ref stream, out matrix4x);
			return matrix4x;
		}

		[NativeMethod(Name = "TransformSceneHandleBindings::GetGlobalTRInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void GetGlobalTRInternal(ref AnimationStream stream, out Vector3 position, out Quaternion rotation)
		{
			TransformSceneHandle.GetGlobalTRInternal_Injected(ref this, ref stream, out position, out rotation);
		}

		[NativeMethod(Name = "TransformSceneHandleBindings::GetLocalToWorldMatrixInternal", IsFreeFunction = true, HasExplicitThis = true, IsThreadSafe = true)]
		private Matrix4x4 GetLocalToWorldMatrixInternal(ref AnimationStream stream)
		{
			Matrix4x4 matrix4x;
			TransformSceneHandle.GetLocalToWorldMatrixInternal_Injected(ref this, ref stream, out matrix4x);
			return matrix4x;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasValidTransform_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPositionInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalPositionInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRotationInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalRotationInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalScaleInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalTRSInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Vector3 position, out Quaternion rotation, out Vector3 scale);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalToParentMatrixInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGlobalTRInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Vector3 position, out Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalToWorldMatrixInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Matrix4x4 ret);

		private uint valid;

		private int transformSceneHandleDefinitionIndex;
	}
}
