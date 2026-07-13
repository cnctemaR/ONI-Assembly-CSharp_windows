using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeHeader("Modules/Animation/ScriptBindings/AnimatorControllerParameter.bindings.h")]
	[NativeHeader("Modules/Animation/ScriptBindings/Animator.bindings.h")]
	[NativeHeader("Modules/Animation/Animator.h")]
	public class Animator : Behaviour
	{
		public bool isOptimizable
		{
			[NativeMethod("IsOptimizable")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_isOptimizable_Injected(intPtr);
			}
		}

		public bool isHuman
		{
			[NativeMethod("IsHuman")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_isHuman_Injected(intPtr);
			}
		}

		public bool hasRootMotion
		{
			[NativeMethod("HasRootMotion")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_hasRootMotion_Injected(intPtr);
			}
		}

		internal bool isRootPositionOrRotationControlledByCurves
		{
			[NativeMethod("IsRootTranslationOrRotationControllerByCurves")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_isRootPositionOrRotationControlledByCurves_Injected(intPtr);
			}
		}

		public float humanScale
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_humanScale_Injected(intPtr);
			}
		}

		public bool isInitialized
		{
			[NativeMethod("IsInitialized")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_isInitialized_Injected(intPtr);
			}
		}

		public float GetFloat(string name)
		{
			return this.GetFloatString(name);
		}

		public float GetFloat(int id)
		{
			return this.GetFloatID(id);
		}

		public void SetFloat(string name, float value)
		{
			this.SetFloatString(name, value);
		}

		public void SetFloat(string name, float value, float dampTime, float deltaTime)
		{
			this.SetFloatStringDamp(name, value, dampTime, deltaTime);
		}

		public void SetFloat(int id, float value)
		{
			this.SetFloatID(id, value);
		}

		public void SetFloat(int id, float value, float dampTime, float deltaTime)
		{
			this.SetFloatIDDamp(id, value, dampTime, deltaTime);
		}

		public bool GetBool(string name)
		{
			return this.GetBoolString(name);
		}

		public bool GetBool(int id)
		{
			return this.GetBoolID(id);
		}

		public void SetBool(string name, bool value)
		{
			this.SetBoolString(name, value);
		}

		public void SetBool(int id, bool value)
		{
			this.SetBoolID(id, value);
		}

		public int GetInteger(string name)
		{
			return this.GetIntegerString(name);
		}

		public int GetInteger(int id)
		{
			return this.GetIntegerID(id);
		}

		public void SetInteger(string name, int value)
		{
			this.SetIntegerString(name, value);
		}

		public void SetInteger(int id, int value)
		{
			this.SetIntegerID(id, value);
		}

		public void SetTrigger(string name)
		{
			this.SetTriggerString(name);
		}

		public void SetTrigger(int id)
		{
			this.SetTriggerID(id);
		}

		public void ResetTrigger(string name)
		{
			this.ResetTriggerString(name);
		}

		public void ResetTrigger(int id)
		{
			this.ResetTriggerID(id);
		}

		public bool IsParameterControlledByCurve(string name)
		{
			return this.IsParameterControlledByCurveString(name);
		}

		public bool IsParameterControlledByCurve(int id)
		{
			return this.IsParameterControlledByCurveID(id);
		}

		public Vector3 deltaPosition
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Animator.get_deltaPosition_Injected(intPtr, out vector);
				return vector;
			}
		}

		public Quaternion deltaRotation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Quaternion quaternion;
				Animator.get_deltaRotation_Injected(intPtr, out quaternion);
				return quaternion;
			}
		}

		public Vector3 velocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Animator.get_velocity_Injected(intPtr, out vector);
				return vector;
			}
		}

		public Vector3 angularVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Animator.get_angularVelocity_Injected(intPtr, out vector);
				return vector;
			}
		}

		public Vector3 rootPosition
		{
			[NativeMethod("GetAvatarPosition")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Animator.get_rootPosition_Injected(intPtr, out vector);
				return vector;
			}
			[NativeMethod("SetAvatarPosition")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_rootPosition_Injected(intPtr, ref value);
			}
		}

		public Quaternion rootRotation
		{
			[NativeMethod("GetAvatarRotation")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Quaternion quaternion;
				Animator.get_rootRotation_Injected(intPtr, out quaternion);
				return quaternion;
			}
			[NativeMethod("SetAvatarRotation")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_rootRotation_Injected(intPtr, ref value);
			}
		}

		public bool applyRootMotion
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_applyRootMotion_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_applyRootMotion_Injected(intPtr, value);
			}
		}

		[Obsolete("Animator.linearVelocityBlending is no longer used and has been deprecated.")]
		public bool linearVelocityBlending
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_linearVelocityBlending_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_linearVelocityBlending_Injected(intPtr, value);
			}
		}

		public bool animatePhysics
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_animatePhysics_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_animatePhysics_Injected(intPtr, value);
			}
		}

		public AnimatorUpdateMode updateMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_updateMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_updateMode_Injected(intPtr, value);
			}
		}

		public bool hasTransformHierarchy
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_hasTransformHierarchy_Injected(intPtr);
			}
		}

		internal bool allowConstantClipSamplingOptimization
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_allowConstantClipSamplingOptimization_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_allowConstantClipSamplingOptimization_Injected(intPtr, value);
			}
		}

		public float gravityWeight
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_gravityWeight_Injected(intPtr);
			}
		}

		public Vector3 bodyPosition
		{
			get
			{
				this.CheckIfInIKPass();
				return this.bodyPositionInternal;
			}
			set
			{
				this.CheckIfInIKPass();
				this.bodyPositionInternal = value;
			}
		}

		internal Vector3 bodyPositionInternal
		{
			[NativeMethod("GetBodyPosition")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Animator.get_bodyPositionInternal_Injected(intPtr, out vector);
				return vector;
			}
			[NativeMethod("SetBodyPosition")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_bodyPositionInternal_Injected(intPtr, ref value);
			}
		}

		public Quaternion bodyRotation
		{
			get
			{
				this.CheckIfInIKPass();
				return this.bodyRotationInternal;
			}
			set
			{
				this.CheckIfInIKPass();
				this.bodyRotationInternal = value;
			}
		}

		internal Quaternion bodyRotationInternal
		{
			[NativeMethod("GetBodyRotation")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Quaternion quaternion;
				Animator.get_bodyRotationInternal_Injected(intPtr, out quaternion);
				return quaternion;
			}
			[NativeMethod("SetBodyRotation")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_bodyRotationInternal_Injected(intPtr, ref value);
			}
		}

		public Vector3 GetIKPosition(AvatarIKGoal goal)
		{
			this.CheckIfInIKPass();
			return this.GetGoalPosition(goal);
		}

		private Vector3 GetGoalPosition(AvatarIKGoal goal)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Animator.GetGoalPosition_Injected(intPtr, goal, out vector);
			return vector;
		}

		public void SetIKPosition(AvatarIKGoal goal, Vector3 goalPosition)
		{
			this.CheckIfInIKPass();
			this.SetGoalPosition(goal, goalPosition);
		}

		private void SetGoalPosition(AvatarIKGoal goal, Vector3 goalPosition)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.SetGoalPosition_Injected(intPtr, goal, ref goalPosition);
		}

		public Quaternion GetIKRotation(AvatarIKGoal goal)
		{
			this.CheckIfInIKPass();
			return this.GetGoalRotation(goal);
		}

		private Quaternion GetGoalRotation(AvatarIKGoal goal)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Quaternion quaternion;
			Animator.GetGoalRotation_Injected(intPtr, goal, out quaternion);
			return quaternion;
		}

		public void SetIKRotation(AvatarIKGoal goal, Quaternion goalRotation)
		{
			this.CheckIfInIKPass();
			this.SetGoalRotation(goal, goalRotation);
		}

		private void SetGoalRotation(AvatarIKGoal goal, Quaternion goalRotation)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.SetGoalRotation_Injected(intPtr, goal, ref goalRotation);
		}

		public float GetIKPositionWeight(AvatarIKGoal goal)
		{
			this.CheckIfInIKPass();
			return this.GetGoalWeightPosition(goal);
		}

		private float GetGoalWeightPosition(AvatarIKGoal goal)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.GetGoalWeightPosition_Injected(intPtr, goal);
		}

		public void SetIKPositionWeight(AvatarIKGoal goal, float value)
		{
			this.CheckIfInIKPass();
			this.SetGoalWeightPosition(goal, value);
		}

		private void SetGoalWeightPosition(AvatarIKGoal goal, float value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.SetGoalWeightPosition_Injected(intPtr, goal, value);
		}

		public float GetIKRotationWeight(AvatarIKGoal goal)
		{
			this.CheckIfInIKPass();
			return this.GetGoalWeightRotation(goal);
		}

		private float GetGoalWeightRotation(AvatarIKGoal goal)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.GetGoalWeightRotation_Injected(intPtr, goal);
		}

		public void SetIKRotationWeight(AvatarIKGoal goal, float value)
		{
			this.CheckIfInIKPass();
			this.SetGoalWeightRotation(goal, value);
		}

		private void SetGoalWeightRotation(AvatarIKGoal goal, float value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.SetGoalWeightRotation_Injected(intPtr, goal, value);
		}

		public Vector3 GetIKHintPosition(AvatarIKHint hint)
		{
			this.CheckIfInIKPass();
			return this.GetHintPosition(hint);
		}

		private Vector3 GetHintPosition(AvatarIKHint hint)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Animator.GetHintPosition_Injected(intPtr, hint, out vector);
			return vector;
		}

		public void SetIKHintPosition(AvatarIKHint hint, Vector3 hintPosition)
		{
			this.CheckIfInIKPass();
			this.SetHintPosition(hint, hintPosition);
		}

		private void SetHintPosition(AvatarIKHint hint, Vector3 hintPosition)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.SetHintPosition_Injected(intPtr, hint, ref hintPosition);
		}

		public float GetIKHintPositionWeight(AvatarIKHint hint)
		{
			this.CheckIfInIKPass();
			return this.GetHintWeightPosition(hint);
		}

		private float GetHintWeightPosition(AvatarIKHint hint)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.GetHintWeightPosition_Injected(intPtr, hint);
		}

		public void SetIKHintPositionWeight(AvatarIKHint hint, float value)
		{
			this.CheckIfInIKPass();
			this.SetHintWeightPosition(hint, value);
		}

		private void SetHintWeightPosition(AvatarIKHint hint, float value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.SetHintWeightPosition_Injected(intPtr, hint, value);
		}

		public void SetLookAtPosition(Vector3 lookAtPosition)
		{
			this.CheckIfInIKPass();
			this.SetLookAtPositionInternal(lookAtPosition);
		}

		[NativeMethod("SetLookAtPosition")]
		private void SetLookAtPositionInternal(Vector3 lookAtPosition)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.SetLookAtPositionInternal_Injected(intPtr, ref lookAtPosition);
		}

		public void SetLookAtWeight(float weight)
		{
			this.CheckIfInIKPass();
			this.SetLookAtWeightInternal(weight, 0f, 1f, 0f, 0.5f);
		}

		public void SetLookAtWeight(float weight, float bodyWeight)
		{
			this.CheckIfInIKPass();
			this.SetLookAtWeightInternal(weight, bodyWeight, 1f, 0f, 0.5f);
		}

		public void SetLookAtWeight(float weight, float bodyWeight, float headWeight)
		{
			this.CheckIfInIKPass();
			this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, 0f, 0.5f);
		}

		public void SetLookAtWeight(float weight, float bodyWeight, float headWeight, float eyesWeight)
		{
			this.CheckIfInIKPass();
			this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, 0.5f);
		}

		public void SetLookAtWeight(float weight, [DefaultValue("0.0f")] float bodyWeight, [DefaultValue("1.0f")] float headWeight, [DefaultValue("0.0f")] float eyesWeight, [DefaultValue("0.5f")] float clampWeight)
		{
			this.CheckIfInIKPass();
			this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
		}

		[NativeMethod("SetLookAtWeight")]
		private void SetLookAtWeightInternal(float weight, float bodyWeight, float headWeight, float eyesWeight, float clampWeight)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.SetLookAtWeightInternal_Injected(intPtr, weight, bodyWeight, headWeight, eyesWeight, clampWeight);
		}

		public void SetBoneLocalRotation(HumanBodyBones humanBoneId, Quaternion rotation)
		{
			this.CheckIfInIKPass();
			this.SetBoneLocalRotationInternal(HumanTrait.GetBoneIndexFromMono((int)humanBoneId), rotation);
		}

		[NativeMethod("SetBoneLocalRotation")]
		private void SetBoneLocalRotationInternal(int humanBoneId, Quaternion rotation)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.SetBoneLocalRotationInternal_Injected(intPtr, humanBoneId, ref rotation);
		}

		private ScriptableObject GetBehaviour([NotNull] Type type)
		{
			if (type == null)
			{
				ThrowHelper.ThrowArgumentNullException(type, "type");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<ScriptableObject>(Animator.GetBehaviour_Injected(intPtr, type));
		}

		public T GetBehaviour<T>() where T : StateMachineBehaviour
		{
			return this.GetBehaviour(typeof(T)) as T;
		}

		private static T[] ConvertStateMachineBehaviour<T>(ScriptableObject[] rawObjects) where T : StateMachineBehaviour
		{
			bool flag = rawObjects == null;
			T[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				T[] array2 = new T[rawObjects.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = (T)((object)rawObjects[i]);
				}
				array = array2;
			}
			return array;
		}

		public T[] GetBehaviours<T>() where T : StateMachineBehaviour
		{
			return Animator.ConvertStateMachineBehaviour<T>(this.InternalGetBehaviours(typeof(T)));
		}

		[FreeFunction(Name = "AnimatorBindings::InternalGetBehaviours", HasExplicitThis = true)]
		internal ScriptableObject[] InternalGetBehaviours([NotNull] Type type)
		{
			if (type == null)
			{
				ThrowHelper.ThrowArgumentNullException(type, "type");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.InternalGetBehaviours_Injected(intPtr, type);
		}

		public StateMachineBehaviour[] GetBehaviours(int fullPathHash, int layerIndex)
		{
			return this.InternalGetBehavioursByKey(fullPathHash, layerIndex, typeof(StateMachineBehaviour)) as StateMachineBehaviour[];
		}

		[FreeFunction(Name = "AnimatorBindings::InternalGetBehavioursByKey", HasExplicitThis = true)]
		internal ScriptableObject[] InternalGetBehavioursByKey(int fullPathHash, int layerIndex, [NotNull] Type type)
		{
			if (type == null)
			{
				ThrowHelper.ThrowArgumentNullException(type, "type");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.InternalGetBehavioursByKey_Injected(intPtr, fullPathHash, layerIndex, type);
		}

		public bool stabilizeFeet
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_stabilizeFeet_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_stabilizeFeet_Injected(intPtr, value);
			}
		}

		public int layerCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_layerCount_Injected(intPtr);
			}
		}

		public string GetLayerName(int layerIndex)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				Animator.GetLayerName_Injected(intPtr, layerIndex, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public unsafe int GetLayerIndex(string layerName)
		{
			int layerIndex_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(layerName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = layerName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				layerIndex_Injected = Animator.GetLayerIndex_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return layerIndex_Injected;
		}

		public float GetLayerWeight(int layerIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.GetLayerWeight_Injected(intPtr, layerIndex);
		}

		public void SetLayerWeight(int layerIndex, float weight)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.SetLayerWeight_Injected(intPtr, layerIndex, weight);
		}

		private void GetAnimatorStateInfo(int layerIndex, StateInfoIndex stateInfoIndex, out AnimatorStateInfo info)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.GetAnimatorStateInfo_Injected(intPtr, layerIndex, stateInfoIndex, out info);
		}

		public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex)
		{
			AnimatorStateInfo animatorStateInfo;
			this.GetAnimatorStateInfo(layerIndex, StateInfoIndex.CurrentState, out animatorStateInfo);
			return animatorStateInfo;
		}

		public AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex)
		{
			AnimatorStateInfo animatorStateInfo;
			this.GetAnimatorStateInfo(layerIndex, StateInfoIndex.NextState, out animatorStateInfo);
			return animatorStateInfo;
		}

		private void GetAnimatorTransitionInfo(int layerIndex, out AnimatorTransitionInfo info)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.GetAnimatorTransitionInfo_Injected(intPtr, layerIndex, out info);
		}

		public AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex)
		{
			AnimatorTransitionInfo animatorTransitionInfo;
			this.GetAnimatorTransitionInfo(layerIndex, out animatorTransitionInfo);
			return animatorTransitionInfo;
		}

		internal int GetAnimatorClipInfoCount(int layerIndex, bool current)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.GetAnimatorClipInfoCount_Injected(intPtr, layerIndex, current);
		}

		public int GetCurrentAnimatorClipInfoCount(int layerIndex)
		{
			return this.GetAnimatorClipInfoCount(layerIndex, true);
		}

		public int GetNextAnimatorClipInfoCount(int layerIndex)
		{
			return this.GetAnimatorClipInfoCount(layerIndex, false);
		}

		[FreeFunction(Name = "AnimatorBindings::GetCurrentAnimatorClipInfo", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.GetCurrentAnimatorClipInfo_Injected(intPtr, layerIndex);
		}

		[FreeFunction(Name = "AnimatorBindings::GetNextAnimatorClipInfo", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.GetNextAnimatorClipInfo_Injected(intPtr, layerIndex);
		}

		public void GetCurrentAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
		{
			bool flag = clips == null;
			if (flag)
			{
				throw new ArgumentNullException("clips");
			}
			this.GetAnimatorClipInfoInternal(layerIndex, true, clips);
		}

		[FreeFunction(Name = "AnimatorBindings::GetAnimatorClipInfoInternal", HasExplicitThis = true)]
		private void GetAnimatorClipInfoInternal(int layerIndex, bool isCurrent, object clips)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.GetAnimatorClipInfoInternal_Injected(intPtr, layerIndex, isCurrent, clips);
		}

		public void GetNextAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
		{
			bool flag = clips == null;
			if (flag)
			{
				throw new ArgumentNullException("clips");
			}
			this.GetAnimatorClipInfoInternal(layerIndex, false, clips);
		}

		public bool IsInTransition(int layerIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.IsInTransition_Injected(intPtr, layerIndex);
		}

		public AnimatorControllerParameter[] parameters
		{
			[FreeFunction(Name = "AnimatorBindings::GetParameters", HasExplicitThis = true)]
			[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_parameters_Injected(intPtr);
			}
		}

		public int parameterCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_parameterCount_Injected(intPtr);
			}
		}

		[FreeFunction(Name = "AnimatorBindings::GetParameterInternal", HasExplicitThis = true)]
		private AnimatorControllerParameter GetParameterInternal(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.GetParameterInternal_Injected(intPtr, index);
		}

		public AnimatorControllerParameter GetParameter(int index)
		{
			AnimatorControllerParameter parameterInternal = this.GetParameterInternal(index);
			bool flag = parameterInternal.m_Type == (AnimatorControllerParameterType)0;
			if (flag)
			{
				throw new IndexOutOfRangeException("Index must be between 0 and " + this.parameterCount.ToString());
			}
			return parameterInternal;
		}

		public float feetPivotActive
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_feetPivotActive_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_feetPivotActive_Injected(intPtr, value);
			}
		}

		public float pivotWeight
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_pivotWeight_Injected(intPtr);
			}
		}

		public Vector3 pivotPosition
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Animator.get_pivotPosition_Injected(intPtr, out vector);
				return vector;
			}
		}

		private void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, int targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime, float targetNormalizedTime, bool completeMatch)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.MatchTarget_Injected(intPtr, ref matchPosition, ref matchRotation, targetBodyPart, ref weightMask, startNormalizedTime, targetNormalizedTime, completeMatch);
		}

		public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime)
		{
			this.MatchTarget(matchPosition, matchRotation, (int)targetBodyPart, weightMask, startNormalizedTime, 1f, true);
		}

		public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime, [DefaultValue("1")] float targetNormalizedTime)
		{
			this.MatchTarget(matchPosition, matchRotation, (int)targetBodyPart, weightMask, startNormalizedTime, targetNormalizedTime, true);
		}

		public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime, [DefaultValue("1")] float targetNormalizedTime, [DefaultValue("true")] bool completeMatch)
		{
			this.MatchTarget(matchPosition, matchRotation, (int)targetBodyPart, weightMask, startNormalizedTime, targetNormalizedTime, completeMatch);
		}

		public void InterruptMatchTarget()
		{
			this.InterruptMatchTarget(true);
		}

		public void InterruptMatchTarget([DefaultValue("true")] bool completeMatch)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.InterruptMatchTarget_Injected(intPtr, completeMatch);
		}

		public bool isMatchingTarget
		{
			[NativeMethod("IsMatchingTarget")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_isMatchingTarget_Injected(intPtr);
			}
		}

		public float speed
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_speed_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_speed_Injected(intPtr, value);
			}
		}

		[Obsolete("ForceStateNormalizedTime is deprecated. Please use Play or CrossFade instead.")]
		public void ForceStateNormalizedTime(float normalizedTime)
		{
			this.Play(0, 0, normalizedTime);
		}

		public void CrossFadeInFixedTime(string stateName, float fixedTransitionDuration)
		{
			float num = 0f;
			float num2 = 0f;
			int num3 = -1;
			this.CrossFadeInFixedTime(Animator.StringToHash(stateName), fixedTransitionDuration, num3, num2, num);
		}

		public void CrossFadeInFixedTime(string stateName, float fixedTransitionDuration, int layer)
		{
			float num = 0f;
			float num2 = 0f;
			this.CrossFadeInFixedTime(Animator.StringToHash(stateName), fixedTransitionDuration, layer, num2, num);
		}

		public void CrossFadeInFixedTime(string stateName, float fixedTransitionDuration, int layer, float fixedTimeOffset)
		{
			float num = 0f;
			this.CrossFadeInFixedTime(Animator.StringToHash(stateName), fixedTransitionDuration, layer, fixedTimeOffset, num);
		}

		public void CrossFadeInFixedTime(string stateName, float fixedTransitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTimeOffset, [DefaultValue("0.0f")] float normalizedTransitionTime)
		{
			this.CrossFadeInFixedTime(Animator.StringToHash(stateName), fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
		}

		public void CrossFadeInFixedTime(int stateHashName, float fixedTransitionDuration, int layer, float fixedTimeOffset)
		{
			float num = 0f;
			this.CrossFadeInFixedTime(stateHashName, fixedTransitionDuration, layer, fixedTimeOffset, num);
		}

		public void CrossFadeInFixedTime(int stateHashName, float fixedTransitionDuration, int layer)
		{
			float num = 0f;
			float num2 = 0f;
			this.CrossFadeInFixedTime(stateHashName, fixedTransitionDuration, layer, num2, num);
		}

		public void CrossFadeInFixedTime(int stateHashName, float fixedTransitionDuration)
		{
			float num = 0f;
			float num2 = 0f;
			int num3 = -1;
			this.CrossFadeInFixedTime(stateHashName, fixedTransitionDuration, num3, num2, num);
		}

		[FreeFunction(Name = "AnimatorBindings::CrossFadeInFixedTime", HasExplicitThis = true)]
		public void CrossFadeInFixedTime(int stateHashName, float fixedTransitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTimeOffset, [DefaultValue("0.0f")] float normalizedTransitionTime)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.CrossFadeInFixedTime_Injected(intPtr, stateHashName, fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
		}

		[FreeFunction(Name = "AnimatorBindings::WriteDefaultValues", HasExplicitThis = true)]
		public void WriteDefaultValues()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.WriteDefaultValues_Injected(intPtr);
		}

		public void CrossFade(string stateName, float normalizedTransitionDuration, int layer, float normalizedTimeOffset)
		{
			float num = 0f;
			this.CrossFade(stateName, normalizedTransitionDuration, layer, normalizedTimeOffset, num);
		}

		public void CrossFade(string stateName, float normalizedTransitionDuration, int layer)
		{
			float num = 0f;
			float negativeInfinity = float.NegativeInfinity;
			this.CrossFade(stateName, normalizedTransitionDuration, layer, negativeInfinity, num);
		}

		public void CrossFade(string stateName, float normalizedTransitionDuration)
		{
			float num = 0f;
			float negativeInfinity = float.NegativeInfinity;
			int num2 = -1;
			this.CrossFade(stateName, normalizedTransitionDuration, num2, negativeInfinity, num);
		}

		public void CrossFade(string stateName, float normalizedTransitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTimeOffset, [DefaultValue("0.0f")] float normalizedTransitionTime)
		{
			this.CrossFade(Animator.StringToHash(stateName), normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
		}

		[FreeFunction(Name = "AnimatorBindings::CrossFade", HasExplicitThis = true)]
		public void CrossFade(int stateHashName, float normalizedTransitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float normalizedTimeOffset, [DefaultValue("0.0f")] float normalizedTransitionTime)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.CrossFade_Injected(intPtr, stateHashName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
		}

		public void CrossFade(int stateHashName, float normalizedTransitionDuration, int layer, float normalizedTimeOffset)
		{
			float num = 0f;
			this.CrossFade(stateHashName, normalizedTransitionDuration, layer, normalizedTimeOffset, num);
		}

		public void CrossFade(int stateHashName, float normalizedTransitionDuration, int layer)
		{
			float num = 0f;
			float negativeInfinity = float.NegativeInfinity;
			this.CrossFade(stateHashName, normalizedTransitionDuration, layer, negativeInfinity, num);
		}

		public void CrossFade(int stateHashName, float normalizedTransitionDuration)
		{
			float num = 0f;
			float negativeInfinity = float.NegativeInfinity;
			int num2 = -1;
			this.CrossFade(stateHashName, normalizedTransitionDuration, num2, negativeInfinity, num);
		}

		public void PlayInFixedTime(string stateName, int layer)
		{
			float negativeInfinity = float.NegativeInfinity;
			this.PlayInFixedTime(stateName, layer, negativeInfinity);
		}

		public void PlayInFixedTime(string stateName)
		{
			float negativeInfinity = float.NegativeInfinity;
			int num = -1;
			this.PlayInFixedTime(stateName, num, negativeInfinity);
		}

		public void PlayInFixedTime(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime)
		{
			this.PlayInFixedTime(Animator.StringToHash(stateName), layer, fixedTime);
		}

		[FreeFunction(Name = "AnimatorBindings::PlayInFixedTime", HasExplicitThis = true)]
		public void PlayInFixedTime(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.PlayInFixedTime_Injected(intPtr, stateNameHash, layer, fixedTime);
		}

		public void PlayInFixedTime(int stateNameHash, int layer)
		{
			float negativeInfinity = float.NegativeInfinity;
			this.PlayInFixedTime(stateNameHash, layer, negativeInfinity);
		}

		public void PlayInFixedTime(int stateNameHash)
		{
			float negativeInfinity = float.NegativeInfinity;
			int num = -1;
			this.PlayInFixedTime(stateNameHash, num, negativeInfinity);
		}

		public void Play(string stateName, int layer)
		{
			float negativeInfinity = float.NegativeInfinity;
			this.Play(stateName, layer, negativeInfinity);
		}

		public void Play(string stateName)
		{
			float negativeInfinity = float.NegativeInfinity;
			int num = -1;
			this.Play(stateName, num, negativeInfinity);
		}

		public void Play(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			this.Play(Animator.StringToHash(stateName), layer, normalizedTime);
		}

		[FreeFunction(Name = "AnimatorBindings::Play", HasExplicitThis = true)]
		public void Play(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.Play_Injected(intPtr, stateNameHash, layer, normalizedTime);
		}

		public void Play(int stateNameHash, int layer)
		{
			float negativeInfinity = float.NegativeInfinity;
			this.Play(stateNameHash, layer, negativeInfinity);
		}

		public void Play(int stateNameHash)
		{
			float negativeInfinity = float.NegativeInfinity;
			int num = -1;
			this.Play(stateNameHash, num, negativeInfinity);
		}

		public void ResetControllerState([DefaultValue("true")] bool resetParameters = true)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.ResetControllerState_Injected(intPtr, resetParameters);
		}

		public void SetTarget(AvatarTarget targetIndex, float targetNormalizedTime)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.SetTarget_Injected(intPtr, targetIndex, targetNormalizedTime);
		}

		public Vector3 targetPosition
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Animator.get_targetPosition_Injected(intPtr, out vector);
				return vector;
			}
		}

		public Quaternion targetRotation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Quaternion quaternion;
				Animator.get_targetRotation_Injected(intPtr, out quaternion);
				return quaternion;
			}
		}

		[Obsolete("Use mask and layers to control subset of transfroms in a skeleton.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsControlled(Transform transform)
		{
			return false;
		}

		internal bool IsBoneTransform(Transform transform)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.IsBoneTransform_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Transform>(transform));
		}

		public Transform avatarRoot
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Transform>(Animator.get_avatarRoot_Injected(intPtr));
			}
		}

		public Transform GetBoneTransform(HumanBodyBones humanBoneId)
		{
			bool flag = this.avatar == null;
			if (flag)
			{
				throw new InvalidOperationException("Avatar is null.");
			}
			bool flag2 = !this.avatar.isValid;
			if (flag2)
			{
				throw new InvalidOperationException("Avatar is not valid.");
			}
			bool flag3 = !this.avatar.isHuman;
			if (flag3)
			{
				throw new InvalidOperationException("Avatar is not of type humanoid.");
			}
			bool flag4 = humanBoneId < HumanBodyBones.Hips || humanBoneId >= HumanBodyBones.LastBone;
			if (flag4)
			{
				throw new IndexOutOfRangeException("humanBoneId must be between 0 and " + HumanBodyBones.LastBone.ToString());
			}
			return this.GetBoneTransformInternal(HumanTrait.GetBoneIndexFromMono((int)humanBoneId));
		}

		[NativeMethod("GetBoneTransform")]
		internal Transform GetBoneTransformInternal(int humanBoneId)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Transform>(Animator.GetBoneTransformInternal_Injected(intPtr, humanBoneId));
		}

		public AnimatorCullingMode cullingMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_cullingMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_cullingMode_Injected(intPtr, value);
			}
		}

		public void StartPlayback()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.StartPlayback_Injected(intPtr);
		}

		public void StopPlayback()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.StopPlayback_Injected(intPtr);
		}

		public float playbackTime
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_playbackTime_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_playbackTime_Injected(intPtr, value);
			}
		}

		public void StartRecording(int frameCount)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.StartRecording_Injected(intPtr, frameCount);
		}

		public void StopRecording()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.StopRecording_Injected(intPtr);
		}

		public float recorderStartTime
		{
			get
			{
				return this.GetRecorderStartTime();
			}
			set
			{
			}
		}

		private float GetRecorderStartTime()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.GetRecorderStartTime_Injected(intPtr);
		}

		public float recorderStopTime
		{
			get
			{
				return this.GetRecorderStopTime();
			}
			set
			{
			}
		}

		private float GetRecorderStopTime()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.GetRecorderStopTime_Injected(intPtr);
		}

		public AnimatorRecorderMode recorderMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_recorderMode_Injected(intPtr);
			}
		}

		public RuntimeAnimatorController runtimeAnimatorController
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<RuntimeAnimatorController>(Animator.get_runtimeAnimatorController_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_runtimeAnimatorController_Injected(intPtr, Object.MarshalledUnityObject.Marshal<RuntimeAnimatorController>(value));
			}
		}

		public bool hasBoundPlayables
		{
			[NativeMethod("HasBoundPlayables")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_hasBoundPlayables_Injected(intPtr);
			}
		}

		internal void ClearInternalControllerPlayable()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.ClearInternalControllerPlayable_Injected(intPtr);
		}

		public bool HasState(int layerIndex, int stateID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.HasState_Injected(intPtr, layerIndex, stateID);
		}

		[NativeMethod(Name = "ScriptingStringToCRC32", IsThreadSafe = true)]
		public unsafe static int StringToHash(string name)
		{
			int num;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				num = Animator.StringToHash_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		public Avatar avatar
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Avatar>(Animator.get_avatar_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_avatar_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Avatar>(value));
			}
		}

		internal string GetStats()
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				Animator.GetStats_Injected(intPtr, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public PlayableGraph playableGraph
		{
			get
			{
				PlayableGraph playableGraph = default(PlayableGraph);
				this.GetCurrentGraph(ref playableGraph);
				return playableGraph;
			}
		}

		[FreeFunction(Name = "AnimatorBindings::GetCurrentGraph", HasExplicitThis = true)]
		private void GetCurrentGraph(ref PlayableGraph graph)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.GetCurrentGraph_Injected(intPtr, ref graph);
		}

		private void CheckIfInIKPass()
		{
			bool flag = this.logWarnings && !this.IsInIKPass();
			if (flag)
			{
				Debug.LogWarning("Setting and getting Body Position/Rotation, IK Goals, Lookat and BoneLocalRotation should only be done in OnAnimatorIK or OnStateIK");
			}
		}

		private bool IsInIKPass()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.IsInIKPass_Injected(intPtr);
		}

		[FreeFunction(Name = "AnimatorBindings::SetFloatString", HasExplicitThis = true)]
		private unsafe void SetFloatString(string name, float value)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Animator.SetFloatString_Injected(intPtr, ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction(Name = "AnimatorBindings::SetFloatID", HasExplicitThis = true)]
		private void SetFloatID(int id, float value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.SetFloatID_Injected(intPtr, id, value);
		}

		[FreeFunction(Name = "AnimatorBindings::GetFloatString", HasExplicitThis = true)]
		private unsafe float GetFloatString(string name)
		{
			float floatString_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				floatString_Injected = Animator.GetFloatString_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return floatString_Injected;
		}

		[FreeFunction(Name = "AnimatorBindings::GetFloatID", HasExplicitThis = true)]
		private float GetFloatID(int id)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.GetFloatID_Injected(intPtr, id);
		}

		[FreeFunction(Name = "AnimatorBindings::SetBoolString", HasExplicitThis = true)]
		private unsafe void SetBoolString(string name, bool value)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Animator.SetBoolString_Injected(intPtr, ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction(Name = "AnimatorBindings::SetBoolID", HasExplicitThis = true)]
		private void SetBoolID(int id, bool value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.SetBoolID_Injected(intPtr, id, value);
		}

		[FreeFunction(Name = "AnimatorBindings::GetBoolString", HasExplicitThis = true)]
		private unsafe bool GetBoolString(string name)
		{
			bool boolString_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				boolString_Injected = Animator.GetBoolString_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return boolString_Injected;
		}

		[FreeFunction(Name = "AnimatorBindings::GetBoolID", HasExplicitThis = true)]
		private bool GetBoolID(int id)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.GetBoolID_Injected(intPtr, id);
		}

		[FreeFunction(Name = "AnimatorBindings::SetIntegerString", HasExplicitThis = true)]
		private unsafe void SetIntegerString(string name, int value)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Animator.SetIntegerString_Injected(intPtr, ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction(Name = "AnimatorBindings::SetIntegerID", HasExplicitThis = true)]
		private void SetIntegerID(int id, int value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.SetIntegerID_Injected(intPtr, id, value);
		}

		[FreeFunction(Name = "AnimatorBindings::GetIntegerString", HasExplicitThis = true)]
		private unsafe int GetIntegerString(string name)
		{
			int integerString_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				integerString_Injected = Animator.GetIntegerString_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return integerString_Injected;
		}

		[FreeFunction(Name = "AnimatorBindings::GetIntegerID", HasExplicitThis = true)]
		private int GetIntegerID(int id)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.GetIntegerID_Injected(intPtr, id);
		}

		[FreeFunction(Name = "AnimatorBindings::SetTriggerString", HasExplicitThis = true)]
		private unsafe void SetTriggerString(string name)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Animator.SetTriggerString_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction(Name = "AnimatorBindings::SetTriggerID", HasExplicitThis = true)]
		private void SetTriggerID(int id)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.SetTriggerID_Injected(intPtr, id);
		}

		[FreeFunction(Name = "AnimatorBindings::ResetTriggerString", HasExplicitThis = true)]
		private unsafe void ResetTriggerString(string name)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Animator.ResetTriggerString_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction(Name = "AnimatorBindings::ResetTriggerID", HasExplicitThis = true)]
		private void ResetTriggerID(int id)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.ResetTriggerID_Injected(intPtr, id);
		}

		[FreeFunction(Name = "AnimatorBindings::IsParameterControlledByCurveString", HasExplicitThis = true)]
		private unsafe bool IsParameterControlledByCurveString(string name)
		{
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = Animator.IsParameterControlledByCurveString_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[FreeFunction(Name = "AnimatorBindings::IsParameterControlledByCurveID", HasExplicitThis = true)]
		private bool IsParameterControlledByCurveID(int id)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animator.IsParameterControlledByCurveID_Injected(intPtr, id);
		}

		[FreeFunction(Name = "AnimatorBindings::SetFloatStringDamp", HasExplicitThis = true)]
		private unsafe void SetFloatStringDamp(string name, float value, float dampTime, float deltaTime)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Animator.SetFloatStringDamp_Injected(intPtr, ref managedSpanWrapper, value, dampTime, deltaTime);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction(Name = "AnimatorBindings::SetFloatIDDamp", HasExplicitThis = true)]
		private void SetFloatIDDamp(int id, float value, float dampTime, float deltaTime)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.SetFloatIDDamp_Injected(intPtr, id, value, dampTime, deltaTime);
		}

		public bool layersAffectMassCenter
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_layersAffectMassCenter_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_layersAffectMassCenter_Injected(intPtr, value);
			}
		}

		public float leftFeetBottomHeight
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_leftFeetBottomHeight_Injected(intPtr);
			}
		}

		public float rightFeetBottomHeight
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_rightFeetBottomHeight_Injected(intPtr);
			}
		}

		[NativeConditional("UNITY_EDITOR")]
		internal bool supportsOnAnimatorMove
		{
			[NativeMethod("SupportsOnAnimatorMove")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_supportsOnAnimatorMove_Injected(intPtr);
			}
		}

		[NativeConditional("UNITY_EDITOR")]
		internal void OnUpdateModeChanged()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.OnUpdateModeChanged_Injected(intPtr);
		}

		[NativeConditional("UNITY_EDITOR")]
		internal void OnCullingModeChanged()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.OnCullingModeChanged_Injected(intPtr);
		}

		[NativeConditional("UNITY_EDITOR")]
		internal void WriteDefaultPose()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.WriteDefaultPose_Injected(intPtr);
		}

		[NativeMethod("UpdateWithDelta")]
		public void Update(float deltaTime)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.Update_Injected(intPtr, deltaTime);
		}

		public void Rebind()
		{
			this.Rebind(true);
		}

		private void Rebind(bool writeDefaultValues)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.Rebind_Injected(intPtr, writeDefaultValues);
		}

		public void ApplyBuiltinRootMotion()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.ApplyBuiltinRootMotion_Injected(intPtr);
		}

		[NativeConditional("UNITY_EDITOR")]
		internal void EvaluateController()
		{
			this.EvaluateController(0f);
		}

		private void EvaluateController(float deltaTime)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animator.EvaluateController_Injected(intPtr, deltaTime);
		}

		[NativeConditional("UNITY_EDITOR")]
		internal string GetCurrentStateName(int layerIndex)
		{
			return this.GetAnimatorStateName(layerIndex, true);
		}

		[NativeConditional("UNITY_EDITOR")]
		internal string GetNextStateName(int layerIndex)
		{
			return this.GetAnimatorStateName(layerIndex, false);
		}

		[NativeConditional("UNITY_EDITOR")]
		private string GetAnimatorStateName(int layerIndex, bool current)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				Animator.GetAnimatorStateName_Injected(intPtr, layerIndex, current, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		internal string ResolveHash(int hash)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				Animator.ResolveHash_Injected(intPtr, hash, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public bool logWarnings
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_logWarnings_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_logWarnings_Injected(intPtr, value);
			}
		}

		public bool fireEvents
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_fireEvents_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_fireEvents_Injected(intPtr, value);
			}
		}

		[Obsolete("keepAnimatorControllerStateOnDisable is deprecated, use keepAnimatorStateOnDisable instead. (UnityUpgradable) -> keepAnimatorStateOnDisable", false)]
		public bool keepAnimatorControllerStateOnDisable
		{
			get
			{
				return this.keepAnimatorStateOnDisable;
			}
			set
			{
				this.keepAnimatorStateOnDisable = value;
			}
		}

		public bool keepAnimatorStateOnDisable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_keepAnimatorStateOnDisable_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_keepAnimatorStateOnDisable_Injected(intPtr, value);
			}
		}

		public bool writeDefaultValuesOnDisable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animator.get_writeDefaultValuesOnDisable_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animator.set_writeDefaultValuesOnDisable_Injected(intPtr, value);
			}
		}

		[Obsolete("GetVector is deprecated.")]
		public Vector3 GetVector(string name)
		{
			return Vector3.zero;
		}

		[Obsolete("GetVector is deprecated.")]
		public Vector3 GetVector(int id)
		{
			return Vector3.zero;
		}

		[Obsolete("SetVector is deprecated.")]
		public void SetVector(string name, Vector3 value)
		{
		}

		[Obsolete("SetVector is deprecated.")]
		public void SetVector(int id, Vector3 value)
		{
		}

		[Obsolete("GetQuaternion is deprecated.")]
		public Quaternion GetQuaternion(string name)
		{
			return Quaternion.identity;
		}

		[Obsolete("GetQuaternion is deprecated.")]
		public Quaternion GetQuaternion(int id)
		{
			return Quaternion.identity;
		}

		[Obsolete("SetQuaternion is deprecated.")]
		public void SetQuaternion(string name, Quaternion value)
		{
		}

		[Obsolete("SetQuaternion is deprecated.")]
		public void SetQuaternion(int id, Quaternion value)
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isOptimizable_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isHuman_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasRootMotion_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isRootPositionOrRotationControlledByCurves_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_humanScale_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isInitialized_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_deltaPosition_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_deltaRotation_Injected(IntPtr _unity_self, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_velocity_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_angularVelocity_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rootPosition_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rootPosition_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rootRotation_Injected(IntPtr _unity_self, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rootRotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_applyRootMotion_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_applyRootMotion_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_linearVelocityBlending_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_linearVelocityBlending_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_animatePhysics_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_animatePhysics_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimatorUpdateMode get_updateMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_updateMode_Injected(IntPtr _unity_self, AnimatorUpdateMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasTransformHierarchy_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_allowConstantClipSamplingOptimization_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_allowConstantClipSamplingOptimization_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_gravityWeight_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_bodyPositionInternal_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bodyPositionInternal_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_bodyRotationInternal_Injected(IntPtr _unity_self, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bodyRotationInternal_Injected(IntPtr _unity_self, [In] ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGoalPosition_Injected(IntPtr _unity_self, AvatarIKGoal goal, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGoalPosition_Injected(IntPtr _unity_self, AvatarIKGoal goal, [In] ref Vector3 goalPosition);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGoalRotation_Injected(IntPtr _unity_self, AvatarIKGoal goal, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGoalRotation_Injected(IntPtr _unity_self, AvatarIKGoal goal, [In] ref Quaternion goalRotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetGoalWeightPosition_Injected(IntPtr _unity_self, AvatarIKGoal goal);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGoalWeightPosition_Injected(IntPtr _unity_self, AvatarIKGoal goal, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetGoalWeightRotation_Injected(IntPtr _unity_self, AvatarIKGoal goal);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGoalWeightRotation_Injected(IntPtr _unity_self, AvatarIKGoal goal, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetHintPosition_Injected(IntPtr _unity_self, AvatarIKHint hint, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetHintPosition_Injected(IntPtr _unity_self, AvatarIKHint hint, [In] ref Vector3 hintPosition);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetHintWeightPosition_Injected(IntPtr _unity_self, AvatarIKHint hint);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetHintWeightPosition_Injected(IntPtr _unity_self, AvatarIKHint hint, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLookAtPositionInternal_Injected(IntPtr _unity_self, [In] ref Vector3 lookAtPosition);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLookAtWeightInternal_Injected(IntPtr _unity_self, float weight, float bodyWeight, float headWeight, float eyesWeight, float clampWeight);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoneLocalRotationInternal_Injected(IntPtr _unity_self, int humanBoneId, [In] ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetBehaviour_Injected(IntPtr _unity_self, Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ScriptableObject[] InternalGetBehaviours_Injected(IntPtr _unity_self, Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ScriptableObject[] InternalGetBehavioursByKey_Injected(IntPtr _unity_self, int fullPathHash, int layerIndex, Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_stabilizeFeet_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_stabilizeFeet_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_layerCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLayerName_Injected(IntPtr _unity_self, int layerIndex, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetLayerIndex_Injected(IntPtr _unity_self, ref ManagedSpanWrapper layerName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetLayerWeight_Injected(IntPtr _unity_self, int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLayerWeight_Injected(IntPtr _unity_self, int layerIndex, float weight);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAnimatorStateInfo_Injected(IntPtr _unity_self, int layerIndex, StateInfoIndex stateInfoIndex, out AnimatorStateInfo info);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAnimatorTransitionInfo_Injected(IntPtr _unity_self, int layerIndex, out AnimatorTransitionInfo info);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAnimatorClipInfoCount_Injected(IntPtr _unity_self, int layerIndex, bool current);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimatorClipInfo[] GetCurrentAnimatorClipInfo_Injected(IntPtr _unity_self, int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimatorClipInfo[] GetNextAnimatorClipInfo_Injected(IntPtr _unity_self, int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAnimatorClipInfoInternal_Injected(IntPtr _unity_self, int layerIndex, bool isCurrent, object clips);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsInTransition_Injected(IntPtr _unity_self, int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimatorControllerParameter[] get_parameters_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_parameterCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimatorControllerParameter GetParameterInternal_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_feetPivotActive_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_feetPivotActive_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_pivotWeight_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_pivotPosition_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MatchTarget_Injected(IntPtr _unity_self, [In] ref Vector3 matchPosition, [In] ref Quaternion matchRotation, int targetBodyPart, [In] ref MatchTargetWeightMask weightMask, float startNormalizedTime, float targetNormalizedTime, bool completeMatch);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InterruptMatchTarget_Injected(IntPtr _unity_self, [DefaultValue("true")] bool completeMatch);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isMatchingTarget_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_speed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_speed_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CrossFadeInFixedTime_Injected(IntPtr _unity_self, int stateHashName, float fixedTransitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTimeOffset, [DefaultValue("0.0f")] float normalizedTransitionTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WriteDefaultValues_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CrossFade_Injected(IntPtr _unity_self, int stateHashName, float normalizedTransitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float normalizedTimeOffset, [DefaultValue("0.0f")] float normalizedTransitionTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PlayInFixedTime_Injected(IntPtr _unity_self, int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Play_Injected(IntPtr _unity_self, int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetControllerState_Injected(IntPtr _unity_self, [DefaultValue("true")] bool resetParameters);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTarget_Injected(IntPtr _unity_self, AvatarTarget targetIndex, float targetNormalizedTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_targetPosition_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_targetRotation_Injected(IntPtr _unity_self, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsBoneTransform_Injected(IntPtr _unity_self, IntPtr transform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_avatarRoot_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetBoneTransformInternal_Injected(IntPtr _unity_self, int humanBoneId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimatorCullingMode get_cullingMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cullingMode_Injected(IntPtr _unity_self, AnimatorCullingMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StartPlayback_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StopPlayback_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_playbackTime_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_playbackTime_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StartRecording_Injected(IntPtr _unity_self, int frameCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StopRecording_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetRecorderStartTime_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetRecorderStopTime_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimatorRecorderMode get_recorderMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_runtimeAnimatorController_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_runtimeAnimatorController_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasBoundPlayables_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearInternalControllerPlayable_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasState_Injected(IntPtr _unity_self, int layerIndex, int stateID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int StringToHash_Injected(ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_avatar_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_avatar_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetStats_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCurrentGraph_Injected(IntPtr _unity_self, ref PlayableGraph graph);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsInIKPass_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatID_Injected(IntPtr _unity_self, int id, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFloatString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFloatID_Injected(IntPtr _unity_self, int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoolString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoolID_Injected(IntPtr _unity_self, int id, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetBoolString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetBoolID_Injected(IntPtr _unity_self, int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIntegerString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIntegerID_Injected(IntPtr _unity_self, int id, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIntegerString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIntegerID_Injected(IntPtr _unity_self, int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTriggerString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTriggerID_Injected(IntPtr _unity_self, int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetTriggerString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetTriggerID_Injected(IntPtr _unity_self, int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsParameterControlledByCurveString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsParameterControlledByCurveID_Injected(IntPtr _unity_self, int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatStringDamp_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, float value, float dampTime, float deltaTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatIDDamp_Injected(IntPtr _unity_self, int id, float value, float dampTime, float deltaTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_layersAffectMassCenter_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_layersAffectMassCenter_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_leftFeetBottomHeight_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_rightFeetBottomHeight_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_supportsOnAnimatorMove_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OnUpdateModeChanged_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OnCullingModeChanged_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WriteDefaultPose_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Update_Injected(IntPtr _unity_self, float deltaTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Rebind_Injected(IntPtr _unity_self, bool writeDefaultValues);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ApplyBuiltinRootMotion_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EvaluateController_Injected(IntPtr _unity_self, float deltaTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAnimatorStateName_Injected(IntPtr _unity_self, int layerIndex, bool current, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResolveHash_Injected(IntPtr _unity_self, int hash, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_logWarnings_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_logWarnings_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_fireEvents_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_fireEvents_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_keepAnimatorStateOnDisable_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_keepAnimatorStateOnDisable_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_writeDefaultValuesOnDisable_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_writeDefaultValuesOnDisable_Injected(IntPtr _unity_self, bool value);
	}
}
