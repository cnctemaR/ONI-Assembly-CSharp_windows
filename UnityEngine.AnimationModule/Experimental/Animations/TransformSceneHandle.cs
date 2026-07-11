using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.Animations
{
	[NativeHeader("Runtime/Animation/ScriptBindings/AnimationStreamHandles.bindings.h")]
	[NativeHeader("Runtime/Animation/Director/AnimationSceneHandles.h")]
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
				return this.valid != 0U;
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
			if (!this.createdByNative || !this.hasTransformSceneHandleDefinitionIndex)
			{
				throw new InvalidOperationException("The TransformSceneHandle is invalid. Please use proper function to create the handle.");
			}
			if (!this.HasValidTransform(ref stream))
			{
				throw new NullReferenceException("The transform is invalid.");
			}
		}

		public Vector3 GetPosition(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetPositionInternal(ref stream);
		}

		public void SetPosition(AnimationStream stream, Vector3 position)
		{
			this.CheckIsValid(ref stream);
			this.SetPositionInternal(ref stream, position);
		}

		public Vector3 GetLocalPosition(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetLocalPositionInternal(ref stream);
		}

		public void SetLocalPosition(AnimationStream stream, Vector3 position)
		{
			this.CheckIsValid(ref stream);
			this.SetLocalPositionInternal(ref stream, position);
		}

		public Quaternion GetRotation(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetRotationInternal(ref stream);
		}

		public void SetRotation(AnimationStream stream, Quaternion rotation)
		{
			this.CheckIsValid(ref stream);
			this.SetRotationInternal(ref stream, rotation);
		}

		public Quaternion GetLocalRotation(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetLocalRotationInternal(ref stream);
		}

		public void SetLocalRotation(AnimationStream stream, Quaternion rotation)
		{
			this.CheckIsValid(ref stream);
			this.SetLocalRotationInternal(ref stream, rotation);
		}

		public Vector3 GetLocalScale(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetLocalScaleInternal(ref stream);
		}

		public void SetLocalScale(AnimationStream stream, Vector3 scale)
		{
			this.CheckIsValid(ref stream);
			this.SetLocalScaleInternal(ref stream, scale);
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

		[NativeMethod(Name = "TransformSceneHandleBindings::SetPositionInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void SetPositionInternal(ref AnimationStream stream, Vector3 position)
		{
			TransformSceneHandle.SetPositionInternal_Injected(ref this, ref stream, ref position);
		}

		[NativeMethod(Name = "TransformSceneHandleBindings::GetLocalPositionInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 GetLocalPositionInternal(ref AnimationStream stream)
		{
			Vector3 vector;
			TransformSceneHandle.GetLocalPositionInternal_Injected(ref this, ref stream, out vector);
			return vector;
		}

		[NativeMethod(Name = "TransformSceneHandleBindings::SetLocalPositionInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void SetLocalPositionInternal(ref AnimationStream stream, Vector3 position)
		{
			TransformSceneHandle.SetLocalPositionInternal_Injected(ref this, ref stream, ref position);
		}

		[NativeMethod(Name = "TransformSceneHandleBindings::GetRotationInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Quaternion GetRotationInternal(ref AnimationStream stream)
		{
			Quaternion quaternion;
			TransformSceneHandle.GetRotationInternal_Injected(ref this, ref stream, out quaternion);
			return quaternion;
		}

		[NativeMethod(Name = "TransformSceneHandleBindings::SetRotationInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void SetRotationInternal(ref AnimationStream stream, Quaternion rotation)
		{
			TransformSceneHandle.SetRotationInternal_Injected(ref this, ref stream, ref rotation);
		}

		[NativeMethod(Name = "TransformSceneHandleBindings::GetLocalRotationInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Quaternion GetLocalRotationInternal(ref AnimationStream stream)
		{
			Quaternion quaternion;
			TransformSceneHandle.GetLocalRotationInternal_Injected(ref this, ref stream, out quaternion);
			return quaternion;
		}

		[NativeMethod(Name = "TransformSceneHandleBindings::SetLocalRotationInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void SetLocalRotationInternal(ref AnimationStream stream, Quaternion rotation)
		{
			TransformSceneHandle.SetLocalRotationInternal_Injected(ref this, ref stream, ref rotation);
		}

		[NativeMethod(Name = "TransformSceneHandleBindings::GetLocalScaleInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 GetLocalScaleInternal(ref AnimationStream stream)
		{
			Vector3 vector;
			TransformSceneHandle.GetLocalScaleInternal_Injected(ref this, ref stream, out vector);
			return vector;
		}

		[NativeMethod(Name = "TransformSceneHandleBindings::SetLocalScaleInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void SetLocalScaleInternal(ref AnimationStream stream, Vector3 scale)
		{
			TransformSceneHandle.SetLocalScaleInternal_Injected(ref this, ref stream, ref scale);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasValidTransform_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPositionInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPositionInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, ref Vector3 position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalPositionInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLocalPositionInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, ref Vector3 position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRotationInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRotationInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalRotationInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLocalRotationInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalScaleInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLocalScaleInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, ref Vector3 scale);

		private uint valid;

		private int transformSceneHandleDefinitionIndex;
	}
}
