using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Animation/ScriptBindings/Animation.bindings.h")]
	[NativeHeader("Modules/Animation/HumanPoseHandler.h")]
	public class HumanPoseHandler : IDisposable
	{
		[FreeFunction("AnimationBindings::CreateHumanPoseHandler")]
		private static IntPtr Internal_CreateFromRoot(Avatar avatar, Transform root)
		{
			return HumanPoseHandler.Internal_CreateFromRoot_Injected(Object.MarshalledUnityObject.Marshal<Avatar>(avatar), Object.MarshalledUnityObject.Marshal<Transform>(root));
		}

		[FreeFunction("AnimationBindings::CreateHumanPoseHandler", IsThreadSafe = true)]
		private static IntPtr Internal_CreateFromJointPaths(Avatar avatar, string[] jointPaths)
		{
			return HumanPoseHandler.Internal_CreateFromJointPaths_Injected(Object.MarshalledUnityObject.Marshal<Avatar>(avatar), jointPaths);
		}

		[FreeFunction("AnimationBindings::DestroyHumanPoseHandler")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr ptr);

		private unsafe void GetHumanPose(out Vector3 bodyPosition, out Quaternion bodyRotation, [Out] float[] muscles, [Out] Vector3[] ikGoalPositions, [Out] Quaternion[] ikGoalRotations)
		{
			try
			{
				IntPtr intPtr = HumanPoseHandler.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (muscles != null)
				{
					fixed (float[] array = muscles)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				BlittableArrayWrapper blittableArrayWrapper2;
				if (ikGoalPositions != null)
				{
					fixed (Vector3[] array2 = ikGoalPositions)
					{
						if (array2.Length != 0)
						{
							blittableArrayWrapper2 = new BlittableArrayWrapper((void*)(&array2[0]), array2.Length);
						}
					}
				}
				BlittableArrayWrapper blittableArrayWrapper3;
				if (ikGoalRotations != null)
				{
					fixed (Quaternion[] array3 = ikGoalRotations)
					{
						if (array3.Length != 0)
						{
							blittableArrayWrapper3 = new BlittableArrayWrapper((void*)(&array3[0]), array3.Length);
						}
					}
				}
				HumanPoseHandler.GetHumanPose_Injected(intPtr, out bodyPosition, out bodyRotation, out blittableArrayWrapper, out blittableArrayWrapper2, out blittableArrayWrapper3);
			}
			finally
			{
				float[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<float>(ref array);
				Vector3[] array2;
				BlittableArrayWrapper blittableArrayWrapper2;
				blittableArrayWrapper2.Unmarshal<Vector3>(ref array2);
				Quaternion[] array3;
				BlittableArrayWrapper blittableArrayWrapper3;
				blittableArrayWrapper3.Unmarshal<Quaternion>(ref array3);
			}
		}

		private unsafe void SetHumanPose(ref Vector3 bodyPosition, ref Quaternion bodyRotation, float[] muscles)
		{
			IntPtr intPtr = HumanPoseHandler.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<float> span = new Span<float>(muscles);
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				HumanPoseHandler.SetHumanPose_Injected(intPtr, ref bodyPosition, ref bodyRotation, ref managedSpanWrapper);
			}
		}

		[ThreadSafe]
		private unsafe void GetInternalHumanPose(out Vector3 bodyPosition, out Quaternion bodyRotation, [Out] float[] muscles, [Out] Vector3[] ikGoalPositions, [Out] Quaternion[] ikGoalRotation)
		{
			try
			{
				IntPtr intPtr = HumanPoseHandler.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (muscles != null)
				{
					fixed (float[] array = muscles)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				BlittableArrayWrapper blittableArrayWrapper2;
				if (ikGoalPositions != null)
				{
					fixed (Vector3[] array2 = ikGoalPositions)
					{
						if (array2.Length != 0)
						{
							blittableArrayWrapper2 = new BlittableArrayWrapper((void*)(&array2[0]), array2.Length);
						}
					}
				}
				BlittableArrayWrapper blittableArrayWrapper3;
				if (ikGoalRotation != null)
				{
					fixed (Quaternion[] array3 = ikGoalRotation)
					{
						if (array3.Length != 0)
						{
							blittableArrayWrapper3 = new BlittableArrayWrapper((void*)(&array3[0]), array3.Length);
						}
					}
				}
				HumanPoseHandler.GetInternalHumanPose_Injected(intPtr, out bodyPosition, out bodyRotation, out blittableArrayWrapper, out blittableArrayWrapper2, out blittableArrayWrapper3);
			}
			finally
			{
				float[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<float>(ref array);
				Vector3[] array2;
				BlittableArrayWrapper blittableArrayWrapper2;
				blittableArrayWrapper2.Unmarshal<Vector3>(ref array2);
				Quaternion[] array3;
				BlittableArrayWrapper blittableArrayWrapper3;
				blittableArrayWrapper3.Unmarshal<Quaternion>(ref array3);
			}
		}

		[ThreadSafe]
		private unsafe void SetInternalHumanPose(ref Vector3 bodyPosition, ref Quaternion bodyRotation, float[] muscles)
		{
			IntPtr intPtr = HumanPoseHandler.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<float> span = new Span<float>(muscles);
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				HumanPoseHandler.SetInternalHumanPose_Injected(intPtr, ref bodyPosition, ref bodyRotation, ref managedSpanWrapper);
			}
		}

		[ThreadSafe]
		private unsafe void GetInternalAvatarPose(void* avatarPose, int avatarPoseLength)
		{
			IntPtr intPtr = HumanPoseHandler.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HumanPoseHandler.GetInternalAvatarPose_Injected(intPtr, avatarPose, avatarPoseLength);
		}

		[ThreadSafe]
		private unsafe void SetInternalAvatarPose(void* avatarPose, int avatarPoseLength)
		{
			IntPtr intPtr = HumanPoseHandler.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HumanPoseHandler.SetInternalAvatarPose_Injected(intPtr, avatarPose, avatarPoseLength);
		}

		public void Dispose()
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
			{
				HumanPoseHandler.Internal_Destroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		public HumanPoseHandler(Avatar avatar, Transform root)
		{
			this.m_Ptr = IntPtr.Zero;
			bool flag = root == null;
			if (flag)
			{
				throw new ArgumentNullException("HumanPoseHandler root Transform is null");
			}
			bool flag2 = avatar == null;
			if (flag2)
			{
				throw new ArgumentNullException("HumanPoseHandler avatar is null");
			}
			bool flag3 = !avatar.isValid;
			if (flag3)
			{
				throw new ArgumentException("HumanPoseHandler avatar is invalid");
			}
			bool flag4 = !avatar.isHuman;
			if (flag4)
			{
				throw new ArgumentException("HumanPoseHandler avatar is not human");
			}
			this.m_Ptr = HumanPoseHandler.Internal_CreateFromRoot(avatar, root);
		}

		public HumanPoseHandler(Avatar avatar, string[] jointPaths)
		{
			this.m_Ptr = IntPtr.Zero;
			bool flag = jointPaths == null;
			if (flag)
			{
				throw new ArgumentNullException("HumanPoseHandler jointPaths array is null");
			}
			bool flag2 = avatar == null;
			if (flag2)
			{
				throw new ArgumentNullException("HumanPoseHandler avatar is null");
			}
			bool flag3 = !avatar.isValid;
			if (flag3)
			{
				throw new ArgumentException("HumanPoseHandler avatar is invalid");
			}
			bool flag4 = !avatar.isHuman;
			if (flag4)
			{
				throw new ArgumentException("HumanPoseHandler avatar is not human");
			}
			this.m_Ptr = HumanPoseHandler.Internal_CreateFromJointPaths(avatar, jointPaths);
		}

		private static void CalculateIKOffsets(in Quaternion[] sourceRotations, ref Quaternion[] destRotations)
		{
			for (int i = 0; i < 4; i++)
			{
				destRotations[i] = sourceRotations[i] * HumanPose.s_IKGoalOffsets[i];
			}
		}

		public void GetHumanPose(ref HumanPose humanPose)
		{
			bool flag = this.m_Ptr == IntPtr.Zero;
			if (flag)
			{
				throw new NullReferenceException("HumanPoseHandler is not initialized properly");
			}
			humanPose.Init();
			this.GetHumanPose(out humanPose.bodyPosition, out humanPose.bodyRotation, humanPose.muscles, humanPose.m_IkGoalPositions, humanPose.m_IkGoalRotations);
			HumanPoseHandler.CalculateIKOffsets(in humanPose.m_IkGoalRotations, ref humanPose.m_OffsetIkGoalRotations);
		}

		public void SetHumanPose(ref HumanPose humanPose)
		{
			bool flag = this.m_Ptr == IntPtr.Zero;
			if (flag)
			{
				throw new NullReferenceException("HumanPoseHandler is not initialized properly");
			}
			humanPose.Init();
			this.SetHumanPose(ref humanPose.bodyPosition, ref humanPose.bodyRotation, humanPose.muscles);
		}

		public void GetInternalHumanPose(ref HumanPose humanPose)
		{
			bool flag = this.m_Ptr == IntPtr.Zero;
			if (flag)
			{
				throw new NullReferenceException("HumanPoseHandler is not initialized properly");
			}
			humanPose.Init();
			this.GetInternalHumanPose(out humanPose.bodyPosition, out humanPose.bodyRotation, humanPose.muscles, humanPose.m_IkGoalPositions, humanPose.m_IkGoalRotations);
			HumanPoseHandler.CalculateIKOffsets(in humanPose.m_IkGoalRotations, ref humanPose.m_OffsetIkGoalRotations);
		}

		public void SetInternalHumanPose(ref HumanPose humanPose)
		{
			bool flag = this.m_Ptr == IntPtr.Zero;
			if (flag)
			{
				throw new NullReferenceException("HumanPoseHandler is not initialized properly");
			}
			humanPose.Init();
			this.SetInternalHumanPose(ref humanPose.bodyPosition, ref humanPose.bodyRotation, humanPose.muscles);
		}

		public void GetInternalAvatarPose(NativeArray<float> avatarPose)
		{
			bool flag = this.m_Ptr == IntPtr.Zero;
			if (flag)
			{
				throw new NullReferenceException("HumanPoseHandler is not initialized properly");
			}
			this.GetInternalAvatarPose(avatarPose.GetUnsafePtr<float>(), avatarPose.Length);
		}

		public void SetInternalAvatarPose(NativeArray<float> avatarPose)
		{
			bool flag = this.m_Ptr == IntPtr.Zero;
			if (flag)
			{
				throw new NullReferenceException("HumanPoseHandler is not initialized properly");
			}
			this.SetInternalAvatarPose(avatarPose.GetUnsafeReadOnlyPtr<float>(), avatarPose.Length);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_CreateFromRoot_Injected(IntPtr avatar, IntPtr root);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_CreateFromJointPaths_Injected(IntPtr avatar, string[] jointPaths);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetHumanPose_Injected(IntPtr _unity_self, out Vector3 bodyPosition, out Quaternion bodyRotation, out BlittableArrayWrapper muscles, out BlittableArrayWrapper ikGoalPositions, out BlittableArrayWrapper ikGoalRotations);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetHumanPose_Injected(IntPtr _unity_self, ref Vector3 bodyPosition, ref Quaternion bodyRotation, ref ManagedSpanWrapper muscles);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetInternalHumanPose_Injected(IntPtr _unity_self, out Vector3 bodyPosition, out Quaternion bodyRotation, out BlittableArrayWrapper muscles, out BlittableArrayWrapper ikGoalPositions, out BlittableArrayWrapper ikGoalRotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetInternalHumanPose_Injected(IntPtr _unity_self, ref Vector3 bodyPosition, ref Quaternion bodyRotation, ref ManagedSpanWrapper muscles);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void GetInternalAvatarPose_Injected(IntPtr _unity_self, void* avatarPose, int avatarPoseLength);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetInternalAvatarPose_Injected(IntPtr _unity_self, void* avatarPose, int avatarPoseLength);

		internal IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(HumanPoseHandler humanPoseHandler)
			{
				return humanPoseHandler.m_Ptr;
			}
		}
	}
}
