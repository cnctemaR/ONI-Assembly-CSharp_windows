using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations
{
	[NativeHeader("Modules/Animation/ScriptBindings/AnimationHumanStream.bindings.h")]
	[RequiredByNativeCode]
	[NativeHeader("Modules/Animation/Director/AnimationHumanStream.h")]
	[MovedFrom("UnityEngine.Experimental.Animations")]
	public struct AnimationHumanStream
	{
		public bool isValid
		{
			get
			{
				return this.stream != IntPtr.Zero;
			}
		}

		private void ThrowIfInvalid()
		{
			bool flag = !this.isValid;
			if (flag)
			{
				throw new InvalidOperationException("The AnimationHumanStream is invalid.");
			}
		}

		public float humanScale
		{
			get
			{
				this.ThrowIfInvalid();
				return this.GetHumanScale();
			}
		}

		public float leftFootHeight
		{
			get
			{
				this.ThrowIfInvalid();
				return this.GetFootHeight(true);
			}
		}

		public float rightFootHeight
		{
			get
			{
				this.ThrowIfInvalid();
				return this.GetFootHeight(false);
			}
		}

		public Vector3 bodyLocalPosition
		{
			get
			{
				this.ThrowIfInvalid();
				return this.InternalGetBodyLocalPosition();
			}
			set
			{
				this.ThrowIfInvalid();
				this.InternalSetBodyLocalPosition(value);
			}
		}

		public Quaternion bodyLocalRotation
		{
			get
			{
				this.ThrowIfInvalid();
				return this.InternalGetBodyLocalRotation();
			}
			set
			{
				this.ThrowIfInvalid();
				this.InternalSetBodyLocalRotation(value);
			}
		}

		public Vector3 bodyPosition
		{
			get
			{
				this.ThrowIfInvalid();
				return this.InternalGetBodyPosition();
			}
			set
			{
				this.ThrowIfInvalid();
				this.InternalSetBodyPosition(value);
			}
		}

		public Quaternion bodyRotation
		{
			get
			{
				this.ThrowIfInvalid();
				return this.InternalGetBodyRotation();
			}
			set
			{
				this.ThrowIfInvalid();
				this.InternalSetBodyRotation(value);
			}
		}

		public float GetMuscle(MuscleHandle muscle)
		{
			this.ThrowIfInvalid();
			return this.InternalGetMuscle(muscle);
		}

		public void SetMuscle(MuscleHandle muscle, float value)
		{
			this.ThrowIfInvalid();
			this.InternalSetMuscle(muscle, value);
		}

		public Vector3 leftFootVelocity
		{
			get
			{
				this.ThrowIfInvalid();
				return this.GetLeftFootVelocity();
			}
		}

		public Vector3 rightFootVelocity
		{
			get
			{
				this.ThrowIfInvalid();
				return this.GetRightFootVelocity();
			}
		}

		public void ResetToStancePose()
		{
			this.ThrowIfInvalid();
			this.InternalResetToStancePose();
		}

		public Vector3 GetGoalPositionFromPose(AvatarIKGoal index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetGoalPositionFromPose(index);
		}

		public Quaternion GetGoalRotationFromPose(AvatarIKGoal index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetGoalRotationFromPose(index);
		}

		public Vector3 GetGoalLocalPosition(AvatarIKGoal index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetGoalLocalPosition(index);
		}

		public void SetGoalLocalPosition(AvatarIKGoal index, Vector3 pos)
		{
			this.ThrowIfInvalid();
			this.InternalSetGoalLocalPosition(index, pos);
		}

		public Quaternion GetGoalLocalRotation(AvatarIKGoal index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetGoalLocalRotation(index);
		}

		public void SetGoalLocalRotation(AvatarIKGoal index, Quaternion rot)
		{
			this.ThrowIfInvalid();
			this.InternalSetGoalLocalRotation(index, rot);
		}

		public Vector3 GetGoalPosition(AvatarIKGoal index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetGoalPosition(index);
		}

		public void SetGoalPosition(AvatarIKGoal index, Vector3 pos)
		{
			this.ThrowIfInvalid();
			this.InternalSetGoalPosition(index, pos);
		}

		public Quaternion GetGoalRotation(AvatarIKGoal index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetGoalRotation(index);
		}

		public void SetGoalRotation(AvatarIKGoal index, Quaternion rot)
		{
			this.ThrowIfInvalid();
			this.InternalSetGoalRotation(index, rot);
		}

		public void SetGoalWeightPosition(AvatarIKGoal index, float value)
		{
			this.ThrowIfInvalid();
			this.InternalSetGoalWeightPosition(index, value);
		}

		public void SetGoalWeightRotation(AvatarIKGoal index, float value)
		{
			this.ThrowIfInvalid();
			this.InternalSetGoalWeightRotation(index, value);
		}

		public float GetGoalWeightPosition(AvatarIKGoal index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetGoalWeightPosition(index);
		}

		public float GetGoalWeightRotation(AvatarIKGoal index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetGoalWeightRotation(index);
		}

		public Vector3 GetHintPosition(AvatarIKHint index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetHintPosition(index);
		}

		public void SetHintPosition(AvatarIKHint index, Vector3 pos)
		{
			this.ThrowIfInvalid();
			this.InternalSetHintPosition(index, pos);
		}

		public void SetHintWeightPosition(AvatarIKHint index, float value)
		{
			this.ThrowIfInvalid();
			this.InternalSetHintWeightPosition(index, value);
		}

		public float GetHintWeightPosition(AvatarIKHint index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetHintWeightPosition(index);
		}

		public void SetLookAtPosition(Vector3 lookAtPosition)
		{
			this.ThrowIfInvalid();
			this.InternalSetLookAtPosition(lookAtPosition);
		}

		public void SetLookAtClampWeight(float weight)
		{
			this.ThrowIfInvalid();
			this.InternalSetLookAtClampWeight(weight);
		}

		public void SetLookAtBodyWeight(float weight)
		{
			this.ThrowIfInvalid();
			this.InternalSetLookAtBodyWeight(weight);
		}

		public void SetLookAtHeadWeight(float weight)
		{
			this.ThrowIfInvalid();
			this.InternalSetLookAtHeadWeight(weight);
		}

		public void SetLookAtEyesWeight(float weight)
		{
			this.ThrowIfInvalid();
			this.InternalSetLookAtEyesWeight(weight);
		}

		public void SolveIK()
		{
			this.ThrowIfInvalid();
			this.InternalSolveIK();
		}

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetHumanScale();

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetFootHeight(bool left);

		[NativeMethod(Name = "ResetToStancePose", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalResetToStancePose();

		[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalPositionFromPose", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 InternalGetGoalPositionFromPose(AvatarIKGoal index)
		{
			Vector3 vector;
			AnimationHumanStream.InternalGetGoalPositionFromPose_Injected(ref this, index, out vector);
			return vector;
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalRotationFromPose", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Quaternion InternalGetGoalRotationFromPose(AvatarIKGoal index)
		{
			Quaternion quaternion;
			AnimationHumanStream.InternalGetGoalRotationFromPose_Injected(ref this, index, out quaternion);
			return quaternion;
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::GetBodyLocalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 InternalGetBodyLocalPosition()
		{
			Vector3 vector;
			AnimationHumanStream.InternalGetBodyLocalPosition_Injected(ref this, out vector);
			return vector;
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::SetBodyLocalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetBodyLocalPosition(Vector3 value)
		{
			AnimationHumanStream.InternalSetBodyLocalPosition_Injected(ref this, ref value);
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::GetBodyLocalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Quaternion InternalGetBodyLocalRotation()
		{
			Quaternion quaternion;
			AnimationHumanStream.InternalGetBodyLocalRotation_Injected(ref this, out quaternion);
			return quaternion;
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::SetBodyLocalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetBodyLocalRotation(Quaternion value)
		{
			AnimationHumanStream.InternalSetBodyLocalRotation_Injected(ref this, ref value);
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::GetBodyPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 InternalGetBodyPosition()
		{
			Vector3 vector;
			AnimationHumanStream.InternalGetBodyPosition_Injected(ref this, out vector);
			return vector;
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::SetBodyPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetBodyPosition(Vector3 value)
		{
			AnimationHumanStream.InternalSetBodyPosition_Injected(ref this, ref value);
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::GetBodyRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Quaternion InternalGetBodyRotation()
		{
			Quaternion quaternion;
			AnimationHumanStream.InternalGetBodyRotation_Injected(ref this, out quaternion);
			return quaternion;
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::SetBodyRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetBodyRotation(Quaternion value)
		{
			AnimationHumanStream.InternalSetBodyRotation_Injected(ref this, ref value);
		}

		[NativeMethod(Name = "GetMuscle", IsThreadSafe = true)]
		private float InternalGetMuscle(MuscleHandle muscle)
		{
			return AnimationHumanStream.InternalGetMuscle_Injected(ref this, ref muscle);
		}

		[NativeMethod(Name = "SetMuscle", IsThreadSafe = true)]
		private void InternalSetMuscle(MuscleHandle muscle, float value)
		{
			AnimationHumanStream.InternalSetMuscle_Injected(ref this, ref muscle, value);
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::GetLeftFootVelocity", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 GetLeftFootVelocity()
		{
			Vector3 vector;
			AnimationHumanStream.GetLeftFootVelocity_Injected(ref this, out vector);
			return vector;
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::GetRightFootVelocity", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 GetRightFootVelocity()
		{
			Vector3 vector;
			AnimationHumanStream.GetRightFootVelocity_Injected(ref this, out vector);
			return vector;
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalLocalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 InternalGetGoalLocalPosition(AvatarIKGoal index)
		{
			Vector3 vector;
			AnimationHumanStream.InternalGetGoalLocalPosition_Injected(ref this, index, out vector);
			return vector;
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::SetGoalLocalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetGoalLocalPosition(AvatarIKGoal index, Vector3 pos)
		{
			AnimationHumanStream.InternalSetGoalLocalPosition_Injected(ref this, index, ref pos);
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalLocalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Quaternion InternalGetGoalLocalRotation(AvatarIKGoal index)
		{
			Quaternion quaternion;
			AnimationHumanStream.InternalGetGoalLocalRotation_Injected(ref this, index, out quaternion);
			return quaternion;
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::SetGoalLocalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetGoalLocalRotation(AvatarIKGoal index, Quaternion rot)
		{
			AnimationHumanStream.InternalSetGoalLocalRotation_Injected(ref this, index, ref rot);
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 InternalGetGoalPosition(AvatarIKGoal index)
		{
			Vector3 vector;
			AnimationHumanStream.InternalGetGoalPosition_Injected(ref this, index, out vector);
			return vector;
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::SetGoalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetGoalPosition(AvatarIKGoal index, Vector3 pos)
		{
			AnimationHumanStream.InternalSetGoalPosition_Injected(ref this, index, ref pos);
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Quaternion InternalGetGoalRotation(AvatarIKGoal index)
		{
			Quaternion quaternion;
			AnimationHumanStream.InternalGetGoalRotation_Injected(ref this, index, out quaternion);
			return quaternion;
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::SetGoalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetGoalRotation(AvatarIKGoal index, Quaternion rot)
		{
			AnimationHumanStream.InternalSetGoalRotation_Injected(ref this, index, ref rot);
		}

		[NativeMethod(Name = "SetGoalWeightPosition", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetGoalWeightPosition(AvatarIKGoal index, float value);

		[NativeMethod(Name = "SetGoalWeightRotation", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetGoalWeightRotation(AvatarIKGoal index, float value);

		[NativeMethod(Name = "GetGoalWeightPosition", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float InternalGetGoalWeightPosition(AvatarIKGoal index);

		[NativeMethod(Name = "GetGoalWeightRotation", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float InternalGetGoalWeightRotation(AvatarIKGoal index);

		[NativeMethod(Name = "AnimationHumanStreamBindings::GetHintPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 InternalGetHintPosition(AvatarIKHint index)
		{
			Vector3 vector;
			AnimationHumanStream.InternalGetHintPosition_Injected(ref this, index, out vector);
			return vector;
		}

		[NativeMethod(Name = "AnimationHumanStreamBindings::SetHintPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetHintPosition(AvatarIKHint index, Vector3 pos)
		{
			AnimationHumanStream.InternalSetHintPosition_Injected(ref this, index, ref pos);
		}

		[NativeMethod(Name = "SetHintWeightPosition", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetHintWeightPosition(AvatarIKHint index, float value);

		[NativeMethod(Name = "GetHintWeightPosition", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float InternalGetHintWeightPosition(AvatarIKHint index);

		[NativeMethod(Name = "AnimationHumanStreamBindings::SetLookAtPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetLookAtPosition(Vector3 lookAtPosition)
		{
			AnimationHumanStream.InternalSetLookAtPosition_Injected(ref this, ref lookAtPosition);
		}

		[NativeMethod(Name = "SetLookAtClampWeight", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetLookAtClampWeight(float weight);

		[NativeMethod(Name = "SetLookAtBodyWeight", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetLookAtBodyWeight(float weight);

		[NativeMethod(Name = "SetLookAtHeadWeight", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetLookAtHeadWeight(float weight);

		[NativeMethod(Name = "SetLookAtEyesWeight", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetLookAtEyesWeight(float weight);

		[NativeMethod(Name = "SolveIK", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSolveIK();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetGoalPositionFromPose_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetGoalRotationFromPose_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetBodyLocalPosition_Injected(ref AnimationHumanStream _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetBodyLocalPosition_Injected(ref AnimationHumanStream _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetBodyLocalRotation_Injected(ref AnimationHumanStream _unity_self, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetBodyLocalRotation_Injected(ref AnimationHumanStream _unity_self, [In] ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetBodyPosition_Injected(ref AnimationHumanStream _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetBodyPosition_Injected(ref AnimationHumanStream _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetBodyRotation_Injected(ref AnimationHumanStream _unity_self, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetBodyRotation_Injected(ref AnimationHumanStream _unity_self, [In] ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float InternalGetMuscle_Injected(ref AnimationHumanStream _unity_self, [In] ref MuscleHandle muscle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetMuscle_Injected(ref AnimationHumanStream _unity_self, [In] ref MuscleHandle muscle, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLeftFootVelocity_Injected(ref AnimationHumanStream _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRightFootVelocity_Injected(ref AnimationHumanStream _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetGoalLocalPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetGoalLocalPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, [In] ref Vector3 pos);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetGoalLocalRotation_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetGoalLocalRotation_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, [In] ref Quaternion rot);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetGoalPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetGoalPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, [In] ref Vector3 pos);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetGoalRotation_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetGoalRotation_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, [In] ref Quaternion rot);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetHintPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKHint index, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetHintPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKHint index, [In] ref Vector3 pos);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetLookAtPosition_Injected(ref AnimationHumanStream _unity_self, [In] ref Vector3 lookAtPosition);

		private IntPtr stream;
	}
}
