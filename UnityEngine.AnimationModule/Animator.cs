using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Interface to control the Mecanim animation system.</para>
	/// </summary>
	[UsedByNativeCode]
	[NativeHeader("Runtime/Animation/ScriptBindings/AnimatorControllerParameter.bindings.h")]
	[NativeHeader("Runtime/Animation/ScriptBindings/Animator.bindings.h")]
	[NativeHeader("Runtime/Animation/Animator.h")]
	public class Animator : Behaviour
	{
		/// <summary>
		///   <para>Returns true if the current rig is optimizable with AnimatorUtility.OptimizeTransformHierarchy.</para>
		/// </summary>
		public extern bool isOptimizable
		{
			[NativeMethod("IsOptimizable")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns true if the current rig is humanoid, false if it is generic.</para>
		/// </summary>
		public extern bool isHuman
		{
			[NativeMethod("IsHuman")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns true if the current rig has root motion.</para>
		/// </summary>
		public extern bool hasRootMotion
		{
			[NativeMethod("HasRootMotion")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool isRootPositionOrRotationControlledByCurves
		{
			[NativeMethod("IsRootTranslationOrRotationControllerByCurves")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns the scale of the current Avatar for a humanoid rig, (1 by default if the rig is generic).</para>
		/// </summary>
		public extern float humanScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns whether the animator is initialized successfully.</para>
		/// </summary>
		public extern bool isInitialized
		{
			[NativeMethod("IsInitialized")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns the value of the given float parameter.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		/// <returns>
		///   <para>The value of the parameter.</para>
		/// </returns>
		public float GetFloat(string name)
		{
			return this.GetFloatString(name);
		}

		/// <summary>
		///   <para>Returns the value of the given float parameter.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		/// <returns>
		///   <para>The value of the parameter.</para>
		/// </returns>
		public float GetFloat(int id)
		{
			return this.GetFloatID(id);
		}

		/// <summary>
		///   <para>Send float values to the Animator to affect transitions.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		/// <param name="value">The new parameter value.</param>
		/// <param name="dampTime">The damper total time.</param>
		/// <param name="deltaTime">The delta time to give to the damper.</param>
		public void SetFloat(string name, float value)
		{
			this.SetFloatString(name, value);
		}

		/// <summary>
		///   <para>Send float values to the Animator to affect transitions.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		/// <param name="value">The new parameter value.</param>
		/// <param name="dampTime">The damper total time.</param>
		/// <param name="deltaTime">The delta time to give to the damper.</param>
		public void SetFloat(string name, float value, float dampTime, float deltaTime)
		{
			this.SetFloatStringDamp(name, value, dampTime, deltaTime);
		}

		/// <summary>
		///   <para>Send float values to the Animator to affect transitions.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		/// <param name="value">The new parameter value.</param>
		/// <param name="dampTime">The damper total time.</param>
		/// <param name="deltaTime">The delta time to give to the damper.</param>
		public void SetFloat(int id, float value)
		{
			this.SetFloatID(id, value);
		}

		/// <summary>
		///   <para>Send float values to the Animator to affect transitions.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		/// <param name="value">The new parameter value.</param>
		/// <param name="dampTime">The damper total time.</param>
		/// <param name="deltaTime">The delta time to give to the damper.</param>
		public void SetFloat(int id, float value, float dampTime, float deltaTime)
		{
			this.SetFloatIDDamp(id, value, dampTime, deltaTime);
		}

		/// <summary>
		///   <para>Returns the value of the given boolean parameter.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		/// <returns>
		///   <para>The value of the parameter.</para>
		/// </returns>
		public bool GetBool(string name)
		{
			return this.GetBoolString(name);
		}

		/// <summary>
		///   <para>Returns the value of the given boolean parameter.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		/// <returns>
		///   <para>The value of the parameter.</para>
		/// </returns>
		public bool GetBool(int id)
		{
			return this.GetBoolID(id);
		}

		/// <summary>
		///   <para>Sets the value of the given boolean parameter.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		/// <param name="value">The new parameter value.</param>
		public void SetBool(string name, bool value)
		{
			this.SetBoolString(name, value);
		}

		/// <summary>
		///   <para>Sets the value of the given boolean parameter.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		/// <param name="value">The new parameter value.</param>
		public void SetBool(int id, bool value)
		{
			this.SetBoolID(id, value);
		}

		/// <summary>
		///   <para>Returns the value of the given integer parameter.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		/// <returns>
		///   <para>The value of the parameter.</para>
		/// </returns>
		public int GetInteger(string name)
		{
			return this.GetIntegerString(name);
		}

		/// <summary>
		///   <para>Returns the value of the given integer parameter.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		/// <returns>
		///   <para>The value of the parameter.</para>
		/// </returns>
		public int GetInteger(int id)
		{
			return this.GetIntegerID(id);
		}

		/// <summary>
		///   <para>Sets the value of the given integer parameter.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		/// <param name="value">The new parameter value.</param>
		public void SetInteger(string name, int value)
		{
			this.SetIntegerString(name, value);
		}

		/// <summary>
		///   <para>Sets the value of the given integer parameter.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		/// <param name="value">The new parameter value.</param>
		public void SetInteger(int id, int value)
		{
			this.SetIntegerID(id, value);
		}

		/// <summary>
		///   <para>Sets the value of the given trigger parameter.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		public void SetTrigger(string name)
		{
			this.SetTriggerString(name);
		}

		/// <summary>
		///   <para>Sets the value of the given trigger parameter.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		public void SetTrigger(int id)
		{
			this.SetTriggerID(id);
		}

		/// <summary>
		///   <para>Resets the value of the given trigger parameter.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		public void ResetTrigger(string name)
		{
			this.ResetTriggerString(name);
		}

		/// <summary>
		///   <para>Resets the value of the given trigger parameter.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		public void ResetTrigger(int id)
		{
			this.ResetTriggerID(id);
		}

		/// <summary>
		///   <para>Returns true if the parameter is controlled by a curve, false otherwise.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		/// <returns>
		///   <para>True if the parameter is controlled by a curve, false otherwise.</para>
		/// </returns>
		public bool IsParameterControlledByCurve(string name)
		{
			return this.IsParameterControlledByCurveString(name);
		}

		/// <summary>
		///   <para>Returns true if the parameter is controlled by a curve, false otherwise.</para>
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="id">The parameter ID.</param>
		/// <returns>
		///   <para>True if the parameter is controlled by a curve, false otherwise.</para>
		/// </returns>
		public bool IsParameterControlledByCurve(int id)
		{
			return this.IsParameterControlledByCurveID(id);
		}

		/// <summary>
		///   <para>Gets the avatar delta position for the last evaluated frame.</para>
		/// </summary>
		public Vector3 deltaPosition
		{
			get
			{
				Vector3 vector;
				this.get_deltaPosition_Injected(out vector);
				return vector;
			}
		}

		/// <summary>
		///   <para>Gets the avatar delta rotation for the last evaluated frame.</para>
		/// </summary>
		public Quaternion deltaRotation
		{
			get
			{
				Quaternion quaternion;
				this.get_deltaRotation_Injected(out quaternion);
				return quaternion;
			}
		}

		/// <summary>
		///   <para>Gets the avatar velocity  for the last evaluated frame.</para>
		/// </summary>
		public Vector3 velocity
		{
			get
			{
				Vector3 vector;
				this.get_velocity_Injected(out vector);
				return vector;
			}
		}

		/// <summary>
		///   <para>Gets the avatar angular velocity for the last evaluated frame.</para>
		/// </summary>
		public Vector3 angularVelocity
		{
			get
			{
				Vector3 vector;
				this.get_angularVelocity_Injected(out vector);
				return vector;
			}
		}

		/// <summary>
		///   <para>The root position, the position of the game object.</para>
		/// </summary>
		public Vector3 rootPosition
		{
			[NativeMethod("GetAvatarPosition")]
			get
			{
				Vector3 vector;
				this.get_rootPosition_Injected(out vector);
				return vector;
			}
			[NativeMethod("SetAvatarPosition")]
			set
			{
				this.set_rootPosition_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The root rotation, the rotation of the game object.</para>
		/// </summary>
		public Quaternion rootRotation
		{
			[NativeMethod("GetAvatarRotation")]
			get
			{
				Quaternion quaternion;
				this.get_rootRotation_Injected(out quaternion);
				return quaternion;
			}
			[NativeMethod("SetAvatarRotation")]
			set
			{
				this.set_rootRotation_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Should root motion be applied?</para>
		/// </summary>
		public extern bool applyRootMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>When linearVelocityBlending is set to true, the root motion velocity and angular velocity will be blended linearly.</para>
		/// </summary>
		public extern bool linearVelocityBlending
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>When turned on, animations will be executed in the physics loop. This is only useful in conjunction with kinematic rigidbodies.</para>
		/// </summary>
		[Obsolete("Animator.animatePhysics has been deprecated. Use Animator.updateMode instead.")]
		public bool animatePhysics
		{
			get
			{
				return this.updateMode == AnimatorUpdateMode.AnimatePhysics;
			}
			set
			{
				this.updateMode = ((!value) ? AnimatorUpdateMode.Normal : AnimatorUpdateMode.AnimatePhysics);
			}
		}

		/// <summary>
		///   <para>Specifies the update mode of the Animator.</para>
		/// </summary>
		public extern AnimatorUpdateMode updateMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Returns true if the object has a transform hierarchy.</para>
		/// </summary>
		public extern bool hasTransformHierarchy
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool allowConstantClipSamplingOptimization
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The current gravity weight based on current animations that are played.</para>
		/// </summary>
		public extern float gravityWeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The position of the body center of mass.</para>
		/// </summary>
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
				Vector3 vector;
				this.get_bodyPositionInternal_Injected(out vector);
				return vector;
			}
			[NativeMethod("SetBodyPosition")]
			set
			{
				this.set_bodyPositionInternal_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The rotation of the body center of mass.</para>
		/// </summary>
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
				Quaternion quaternion;
				this.get_bodyRotationInternal_Injected(out quaternion);
				return quaternion;
			}
			[NativeMethod("SetBodyRotation")]
			set
			{
				this.set_bodyRotationInternal_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Gets the position of an IK goal.</para>
		/// </summary>
		/// <param name="goal">The AvatarIKGoal that is queried.</param>
		/// <returns>
		///   <para>Return the current position of this IK goal in world space.</para>
		/// </returns>
		public Vector3 GetIKPosition(AvatarIKGoal goal)
		{
			this.CheckIfInIKPass();
			return this.GetGoalPosition(goal);
		}

		private Vector3 GetGoalPosition(AvatarIKGoal goal)
		{
			Vector3 vector;
			this.GetGoalPosition_Injected(goal, out vector);
			return vector;
		}

		/// <summary>
		///   <para>Sets the position of an IK goal.</para>
		/// </summary>
		/// <param name="goal">The AvatarIKGoal that is set.</param>
		/// <param name="goalPosition">The position in world space.</param>
		public void SetIKPosition(AvatarIKGoal goal, Vector3 goalPosition)
		{
			this.CheckIfInIKPass();
			this.SetGoalPosition(goal, goalPosition);
		}

		private void SetGoalPosition(AvatarIKGoal goal, Vector3 goalPosition)
		{
			this.SetGoalPosition_Injected(goal, ref goalPosition);
		}

		/// <summary>
		///   <para>Gets the rotation of an IK goal.</para>
		/// </summary>
		/// <param name="goal">The AvatarIKGoal that is is queried.</param>
		public Quaternion GetIKRotation(AvatarIKGoal goal)
		{
			this.CheckIfInIKPass();
			return this.GetGoalRotation(goal);
		}

		private Quaternion GetGoalRotation(AvatarIKGoal goal)
		{
			Quaternion quaternion;
			this.GetGoalRotation_Injected(goal, out quaternion);
			return quaternion;
		}

		/// <summary>
		///   <para>Sets the rotation of an IK goal.</para>
		/// </summary>
		/// <param name="goal">The AvatarIKGoal that is set.</param>
		/// <param name="goalRotation">The rotation in world space.</param>
		public void SetIKRotation(AvatarIKGoal goal, Quaternion goalRotation)
		{
			this.CheckIfInIKPass();
			this.SetGoalRotation(goal, goalRotation);
		}

		private void SetGoalRotation(AvatarIKGoal goal, Quaternion goalRotation)
		{
			this.SetGoalRotation_Injected(goal, ref goalRotation);
		}

		/// <summary>
		///   <para>Gets the translative weight of an IK goal (0 = at the original animation before IK, 1 = at the goal).</para>
		/// </summary>
		/// <param name="goal">The AvatarIKGoal that is queried.</param>
		public float GetIKPositionWeight(AvatarIKGoal goal)
		{
			this.CheckIfInIKPass();
			return this.GetGoalWeightPosition(goal);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetGoalWeightPosition(AvatarIKGoal goal);

		/// <summary>
		///   <para>Sets the translative weight of an IK goal (0 = at the original animation before IK, 1 = at the goal).</para>
		/// </summary>
		/// <param name="goal">The AvatarIKGoal that is set.</param>
		/// <param name="value">The translative weight.</param>
		public void SetIKPositionWeight(AvatarIKGoal goal, float value)
		{
			this.CheckIfInIKPass();
			this.SetGoalWeightPosition(goal, value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetGoalWeightPosition(AvatarIKGoal goal, float value);

		/// <summary>
		///   <para>Gets the rotational weight of an IK goal (0 = rotation before IK, 1 = rotation at the IK goal).</para>
		/// </summary>
		/// <param name="goal">The AvatarIKGoal that is queried.</param>
		public float GetIKRotationWeight(AvatarIKGoal goal)
		{
			this.CheckIfInIKPass();
			return this.GetGoalWeightRotation(goal);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetGoalWeightRotation(AvatarIKGoal goal);

		/// <summary>
		///   <para>Sets the rotational weight of an IK goal (0 = rotation before IK, 1 = rotation at the IK goal).</para>
		/// </summary>
		/// <param name="goal">The AvatarIKGoal that is set.</param>
		/// <param name="value">The rotational weight.</param>
		public void SetIKRotationWeight(AvatarIKGoal goal, float value)
		{
			this.CheckIfInIKPass();
			this.SetGoalWeightRotation(goal, value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetGoalWeightRotation(AvatarIKGoal goal, float value);

		/// <summary>
		///   <para>Gets the position of an IK hint.</para>
		/// </summary>
		/// <param name="hint">The AvatarIKHint that is queried.</param>
		/// <returns>
		///   <para>Return the current position of this IK hint in world space.</para>
		/// </returns>
		public Vector3 GetIKHintPosition(AvatarIKHint hint)
		{
			this.CheckIfInIKPass();
			return this.GetHintPosition(hint);
		}

		private Vector3 GetHintPosition(AvatarIKHint hint)
		{
			Vector3 vector;
			this.GetHintPosition_Injected(hint, out vector);
			return vector;
		}

		/// <summary>
		///   <para>Sets the position of an IK hint.</para>
		/// </summary>
		/// <param name="hint">The AvatarIKHint that is set.</param>
		/// <param name="hintPosition">The position in world space.</param>
		public void SetIKHintPosition(AvatarIKHint hint, Vector3 hintPosition)
		{
			this.CheckIfInIKPass();
			this.SetHintPosition(hint, hintPosition);
		}

		private void SetHintPosition(AvatarIKHint hint, Vector3 hintPosition)
		{
			this.SetHintPosition_Injected(hint, ref hintPosition);
		}

		/// <summary>
		///   <para>Gets the translative weight of an IK Hint (0 = at the original animation before IK, 1 = at the hint).</para>
		/// </summary>
		/// <param name="hint">The AvatarIKHint that is queried.</param>
		/// <returns>
		///   <para>Return translative weight.</para>
		/// </returns>
		public float GetIKHintPositionWeight(AvatarIKHint hint)
		{
			this.CheckIfInIKPass();
			return this.GetHintWeightPosition(hint);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetHintWeightPosition(AvatarIKHint hint);

		/// <summary>
		///   <para>Sets the translative weight of an IK hint (0 = at the original animation before IK, 1 = at the hint).</para>
		/// </summary>
		/// <param name="hint">The AvatarIKHint that is set.</param>
		/// <param name="value">The translative weight.</param>
		public void SetIKHintPositionWeight(AvatarIKHint hint, float value)
		{
			this.CheckIfInIKPass();
			this.SetHintWeightPosition(hint, value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetHintWeightPosition(AvatarIKHint hint, float value);

		/// <summary>
		///   <para>Sets the look at position.</para>
		/// </summary>
		/// <param name="lookAtPosition">The position to lookAt.</param>
		public void SetLookAtPosition(Vector3 lookAtPosition)
		{
			this.CheckIfInIKPass();
			this.SetLookAtPositionInternal(lookAtPosition);
		}

		[NativeMethod("SetLookAtPosition")]
		private void SetLookAtPositionInternal(Vector3 lookAtPosition)
		{
			this.SetLookAtPositionInternal_Injected(ref lookAtPosition);
		}

		/// <summary>
		///   <para>Set look at weights.</para>
		/// </summary>
		/// <param name="weight">(0-1) the global weight of the LookAt, multiplier for other parameters.</param>
		/// <param name="bodyWeight">(0-1) determines how much the body is involved in the LookAt.</param>
		/// <param name="headWeight">(0-1) determines how much the head is involved in the LookAt.</param>
		/// <param name="eyesWeight">(0-1) determines how much the eyes are involved in the LookAt.</param>
		/// <param name="clampWeight">(0-1) 0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).</param>
		public void SetLookAtWeight(float weight)
		{
			this.CheckIfInIKPass();
			this.SetLookAtWeightInternal(weight, 0f, 1f, 0f, 0.5f);
		}

		/// <summary>
		///   <para>Set look at weights.</para>
		/// </summary>
		/// <param name="weight">(0-1) the global weight of the LookAt, multiplier for other parameters.</param>
		/// <param name="bodyWeight">(0-1) determines how much the body is involved in the LookAt.</param>
		/// <param name="headWeight">(0-1) determines how much the head is involved in the LookAt.</param>
		/// <param name="eyesWeight">(0-1) determines how much the eyes are involved in the LookAt.</param>
		/// <param name="clampWeight">(0-1) 0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).</param>
		public void SetLookAtWeight(float weight, float bodyWeight)
		{
			this.CheckIfInIKPass();
			this.SetLookAtWeightInternal(weight, bodyWeight, 1f, 0f, 0.5f);
		}

		/// <summary>
		///   <para>Set look at weights.</para>
		/// </summary>
		/// <param name="weight">(0-1) the global weight of the LookAt, multiplier for other parameters.</param>
		/// <param name="bodyWeight">(0-1) determines how much the body is involved in the LookAt.</param>
		/// <param name="headWeight">(0-1) determines how much the head is involved in the LookAt.</param>
		/// <param name="eyesWeight">(0-1) determines how much the eyes are involved in the LookAt.</param>
		/// <param name="clampWeight">(0-1) 0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).</param>
		public void SetLookAtWeight(float weight, float bodyWeight, float headWeight)
		{
			this.CheckIfInIKPass();
			this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, 0f, 0.5f);
		}

		/// <summary>
		///   <para>Set look at weights.</para>
		/// </summary>
		/// <param name="weight">(0-1) the global weight of the LookAt, multiplier for other parameters.</param>
		/// <param name="bodyWeight">(0-1) determines how much the body is involved in the LookAt.</param>
		/// <param name="headWeight">(0-1) determines how much the head is involved in the LookAt.</param>
		/// <param name="eyesWeight">(0-1) determines how much the eyes are involved in the LookAt.</param>
		/// <param name="clampWeight">(0-1) 0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).</param>
		public void SetLookAtWeight(float weight, float bodyWeight, float headWeight, float eyesWeight)
		{
			this.CheckIfInIKPass();
			this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, 0.5f);
		}

		/// <summary>
		///   <para>Set look at weights.</para>
		/// </summary>
		/// <param name="weight">(0-1) the global weight of the LookAt, multiplier for other parameters.</param>
		/// <param name="bodyWeight">(0-1) determines how much the body is involved in the LookAt.</param>
		/// <param name="headWeight">(0-1) determines how much the head is involved in the LookAt.</param>
		/// <param name="eyesWeight">(0-1) determines how much the eyes are involved in the LookAt.</param>
		/// <param name="clampWeight">(0-1) 0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).</param>
		public void SetLookAtWeight(float weight, [DefaultValue("0.0f")] float bodyWeight, [DefaultValue("1.0f")] float headWeight, [DefaultValue("0.0f")] float eyesWeight, [DefaultValue("0.5f")] float clampWeight)
		{
			this.CheckIfInIKPass();
			this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
		}

		[NativeMethod("SetLookAtWeight")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetLookAtWeightInternal(float weight, float bodyWeight, float headWeight, float eyesWeight, float clampWeight);

		/// <summary>
		///   <para>Sets local rotation of a human bone during a IK pass.</para>
		/// </summary>
		/// <param name="humanBoneId">The human bone Id.</param>
		/// <param name="rotation">The local rotation.</param>
		public void SetBoneLocalRotation(HumanBodyBones humanBoneId, Quaternion rotation)
		{
			this.CheckIfInIKPass();
			this.SetBoneLocalRotationInternal(HumanTrait.GetBoneIndexFromMono((int)humanBoneId), rotation);
		}

		[NativeMethod("SetBoneLocalRotation")]
		private void SetBoneLocalRotationInternal(int humanBoneId, Quaternion rotation)
		{
			this.SetBoneLocalRotationInternal_Injected(humanBoneId, ref rotation);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern ScriptableObject GetBehaviour([NotNull] Type type);

		public T GetBehaviour<T>() where T : StateMachineBehaviour
		{
			return this.GetBehaviour(typeof(T)) as T;
		}

		private static T[] ConvertStateMachineBehaviour<T>(ScriptableObject[] rawObjects) where T : StateMachineBehaviour
		{
			T[] array;
			if (rawObjects == null)
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
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern ScriptableObject[] InternalGetBehaviours([NotNull] Type type);

		public StateMachineBehaviour[] GetBehaviours(int fullPathHash, int layerIndex)
		{
			return this.InternalGetBehavioursByKey(fullPathHash, layerIndex, typeof(StateMachineBehaviour)) as StateMachineBehaviour[];
		}

		[FreeFunction(Name = "AnimatorBindings::InternalGetBehavioursByKey", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern ScriptableObject[] InternalGetBehavioursByKey(int fullPathHash, int layerIndex, [NotNull] Type type);

		/// <summary>
		///   <para>Automatic stabilization of feet during transition and blending.</para>
		/// </summary>
		public extern bool stabilizeFeet
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Returns the number of layers in the controller.</para>
		/// </summary>
		public extern int layerCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns the layer name.</para>
		/// </summary>
		/// <param name="layerIndex">The layer index.</param>
		/// <returns>
		///   <para>The layer name.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetLayerName(int layerIndex);

		/// <summary>
		///   <para>Returns the index of the layer with the given name.</para>
		/// </summary>
		/// <param name="layerName">The layer name.</param>
		/// <returns>
		///   <para>The layer index.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetLayerIndex(string layerName);

		/// <summary>
		///   <para>Returns the weight of the layer at the specified index.</para>
		/// </summary>
		/// <param name="layerIndex">The layer index.</param>
		/// <returns>
		///   <para>The layer weight.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetLayerWeight(int layerIndex);

		/// <summary>
		///   <para>Sets the weight of the layer at the given index.</para>
		/// </summary>
		/// <param name="layerIndex">The layer index.</param>
		/// <param name="weight">The new layer weight.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetLayerWeight(int layerIndex, float weight);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetAnimatorStateInfo(int layerIndex, StateInfoIndex stateInfoIndex, out AnimatorStateInfo info);

		/// <summary>
		///   <para>Returns an AnimatorStateInfo with the information on the current state.</para>
		/// </summary>
		/// <param name="layerIndex">The layer index.</param>
		/// <returns>
		///   <para>An AnimatorStateInfo with the information on the current state.</para>
		/// </returns>
		public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex)
		{
			AnimatorStateInfo animatorStateInfo;
			this.GetAnimatorStateInfo(layerIndex, StateInfoIndex.CurrentState, out animatorStateInfo);
			return animatorStateInfo;
		}

		/// <summary>
		///   <para>Returns an AnimatorStateInfo with the information on the next state.</para>
		/// </summary>
		/// <param name="layerIndex">The layer index.</param>
		/// <returns>
		///   <para>An AnimatorStateInfo with the information on the next state.</para>
		/// </returns>
		public AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex)
		{
			AnimatorStateInfo animatorStateInfo;
			this.GetAnimatorStateInfo(layerIndex, StateInfoIndex.NextState, out animatorStateInfo);
			return animatorStateInfo;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetAnimatorTransitionInfo(int layerIndex, out AnimatorTransitionInfo info);

		/// <summary>
		///   <para>Returns an AnimatorTransitionInfo with the informations on the current transition.</para>
		/// </summary>
		/// <param name="layerIndex">The layer's index.</param>
		/// <returns>
		///   <para>An AnimatorTransitionInfo with the informations on the current transition.</para>
		/// </returns>
		public AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex)
		{
			AnimatorTransitionInfo animatorTransitionInfo;
			this.GetAnimatorTransitionInfo(layerIndex, out animatorTransitionInfo);
			return animatorTransitionInfo;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetAnimatorClipInfoCount(int layerIndex, bool current);

		/// <summary>
		///   <para>Returns the number of AnimatorClipInfo in the current state.</para>
		/// </summary>
		/// <param name="layerIndex">The layer index.</param>
		/// <returns>
		///   <para>The number of AnimatorClipInfo in the current state.</para>
		/// </returns>
		public int GetCurrentAnimatorClipInfoCount(int layerIndex)
		{
			return this.GetAnimatorClipInfoCount(layerIndex, true);
		}

		/// <summary>
		///   <para>Returns the number of AnimatorClipInfo in the next state.</para>
		/// </summary>
		/// <param name="layerIndex">The layer index.</param>
		/// <returns>
		///   <para>The number of AnimatorClipInfo in the next state.</para>
		/// </returns>
		public int GetNextAnimatorClipInfoCount(int layerIndex)
		{
			return this.GetAnimatorClipInfoCount(layerIndex, false);
		}

		/// <summary>
		///   <para>Returns an array of all the AnimatorClipInfo in the current state of the given layer.</para>
		/// </summary>
		/// <param name="layerIndex">The layer index.</param>
		/// <returns>
		///   <para>An array of all the AnimatorClipInfo in the current state.</para>
		/// </returns>
		[FreeFunction(Name = "AnimatorBindings::GetCurrentAnimatorClipInfo", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex);

		/// <summary>
		///   <para>Returns an array of all the AnimatorClipInfo in the next state of the given layer.</para>
		/// </summary>
		/// <param name="layerIndex">The layer index.</param>
		/// <returns>
		///   <para>An array of all the AnimatorClipInfo in the next state.</para>
		/// </returns>
		[FreeFunction(Name = "AnimatorBindings::GetNextAnimatorClipInfo", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex);

		public void GetCurrentAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
		{
			if (clips == null)
			{
				throw new ArgumentNullException("clips");
			}
			this.GetAnimatorClipInfoInternal(layerIndex, true, clips);
		}

		[FreeFunction(Name = "AnimatorBindings::GetAnimatorClipInfoInternal", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetAnimatorClipInfoInternal(int layerIndex, bool isCurrent, object clips);

		[NativeConditional("ENABLE_DOTNET")]
		[FreeFunction(Name = "AnimatorBindings::GetAnimatorClipInfoInternalWinRT", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AnimatorClipInfo[] GetAnimatorClipInfoInternalWinRT(int layerIndex, bool isCurrent);

		public void GetNextAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
		{
			if (clips == null)
			{
				throw new ArgumentNullException("clips");
			}
			this.GetAnimatorClipInfoInternal(layerIndex, false, clips);
		}

		/// <summary>
		///   <para>Returns true if there is a transition on the given layer, false otherwise.</para>
		/// </summary>
		/// <param name="layerIndex">The layer index.</param>
		/// <returns>
		///   <para>True if there is a transition on the given layer, false otherwise.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsInTransition(int layerIndex);

		/// <summary>
		///   <para>The AnimatorControllerParameter list used by the animator. (Read Only)</para>
		/// </summary>
		public extern AnimatorControllerParameter[] parameters
		{
			[FreeFunction(Name = "AnimatorBindings::GetParameters", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns the number of parameters in the controller.</para>
		/// </summary>
		public extern int parameterCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>See AnimatorController.parameters.</para>
		/// </summary>
		/// <param name="index"></param>
		public AnimatorControllerParameter GetParameter(int index)
		{
			AnimatorControllerParameter[] parameters = this.parameters;
			if (index < 0 && index >= this.parameters.Length)
			{
				throw new IndexOutOfRangeException("Index must be between 0 and " + this.parameters.Length);
			}
			return parameters[index];
		}

		/// <summary>
		///   <para>Blends pivot point between body center of mass and feet pivot.</para>
		/// </summary>
		public extern float feetPivotActive
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Gets the pivot weight.</para>
		/// </summary>
		public extern float pivotWeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Get the current position of the pivot.</para>
		/// </summary>
		public Vector3 pivotPosition
		{
			get
			{
				Vector3 vector;
				this.get_pivotPosition_Injected(out vector);
				return vector;
			}
		}

		private void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, int targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime, float targetNormalizedTime)
		{
			this.MatchTarget_Injected(ref matchPosition, ref matchRotation, targetBodyPart, ref weightMask, startNormalizedTime, targetNormalizedTime);
		}

		public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime)
		{
			this.MatchTarget(matchPosition, matchRotation, (int)targetBodyPart, weightMask, startNormalizedTime, 1f);
		}

		/// <summary>
		///   <para>Automatically adjust the GameObject position and rotation.</para>
		/// </summary>
		/// <param name="matchPosition">The position we want the body part to reach.</param>
		/// <param name="matchRotation">The rotation in which we want the body part to be.</param>
		/// <param name="targetBodyPart">The body part that is involved in the match.</param>
		/// <param name="weightMask">Structure that contains weights for matching position and rotation.</param>
		/// <param name="startNormalizedTime">Start time within the animation clip (0 - beginning of clip, 1 - end of clip).</param>
		/// <param name="targetNormalizedTime">End time within the animation clip (0 - beginning of clip, 1 - end of clip), values greater than 1 can be set to trigger a match after a certain number of loops. Ex: 2.3 means at 30% of 2nd loop.</param>
		public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime, [DefaultValue("1")] float targetNormalizedTime)
		{
			this.MatchTarget(matchPosition, matchRotation, (int)targetBodyPart, weightMask, startNormalizedTime, targetNormalizedTime);
		}

		/// <summary>
		///   <para>Interrupts the automatic target matching.</para>
		/// </summary>
		/// <param name="completeMatch"></param>
		public void InterruptMatchTarget()
		{
			this.InterruptMatchTarget(true);
		}

		/// <summary>
		///   <para>Interrupts the automatic target matching.</para>
		/// </summary>
		/// <param name="completeMatch"></param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InterruptMatchTarget([DefaultValue("true")] bool completeMatch);

		/// <summary>
		///   <para>If automatic matching is active.</para>
		/// </summary>
		public extern bool isMatchingTarget
		{
			[NativeMethod("IsMatchingTarget")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The playback speed of the Animator. 1 is normal playback speed.</para>
		/// </summary>
		public extern float speed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
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

		/// <summary>
		///   <para>Creates a crossfade from the current state to any other state using times in seconds.</para>
		/// </summary>
		/// <param name="stateName">The name of the state.</param>
		/// <param name="stateHashName">The hash name of the state.</param>
		/// <param name="fixedTransitionDuration">The duration of the transition (in seconds).</param>
		/// <param name="layer">The layer where the crossfade occurs.</param>
		/// <param name="fixedTimeOffset">The time of the state (in seconds).</param>
		/// <param name="normalizedTransitionTime">The time of the transition (normalized).</param>
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

		/// <summary>
		///   <para>Creates a crossfade from the current state to any other state using times in seconds.</para>
		/// </summary>
		/// <param name="stateName">The name of the state.</param>
		/// <param name="stateHashName">The hash name of the state.</param>
		/// <param name="fixedTransitionDuration">The duration of the transition (in seconds).</param>
		/// <param name="layer">The layer where the crossfade occurs.</param>
		/// <param name="fixedTimeOffset">The time of the state (in seconds).</param>
		/// <param name="normalizedTransitionTime">The time of the transition (normalized).</param>
		[FreeFunction(Name = "AnimatorBindings::CrossFadeInFixedTime", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CrossFadeInFixedTime(int stateHashName, float fixedTransitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTimeOffset, [DefaultValue("0.0f")] float normalizedTransitionTime);

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

		/// <summary>
		///   <para>Creates a crossfade from the current state to any other state using normalized times.</para>
		/// </summary>
		/// <param name="stateName">The name of the state.</param>
		/// <param name="stateHashName">The hash name of the state.</param>
		/// <param name="normalizedTransitionDuration">The duration of the transition (normalized).</param>
		/// <param name="layer">The layer where the crossfade occurs.</param>
		/// <param name="normalizedTimeOffset">The time of the state (normalized).</param>
		/// <param name="normalizedTransitionTime">The time of the transition (normalized).</param>
		public void CrossFade(string stateName, float normalizedTransitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTimeOffset, [DefaultValue("0.0f")] float normalizedTransitionTime)
		{
			this.CrossFade(Animator.StringToHash(stateName), normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
		}

		/// <summary>
		///   <para>Creates a crossfade from the current state to any other state using normalized times.</para>
		/// </summary>
		/// <param name="stateName">The name of the state.</param>
		/// <param name="stateHashName">The hash name of the state.</param>
		/// <param name="normalizedTransitionDuration">The duration of the transition (normalized).</param>
		/// <param name="layer">The layer where the crossfade occurs.</param>
		/// <param name="normalizedTimeOffset">The time of the state (normalized).</param>
		/// <param name="normalizedTransitionTime">The time of the transition (normalized).</param>
		[FreeFunction(Name = "AnimatorBindings::CrossFade", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CrossFade(int stateHashName, float normalizedTransitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float normalizedTimeOffset, [DefaultValue("0.0f")] float normalizedTransitionTime);

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

		/// <summary>
		///   <para>Plays a state.</para>
		/// </summary>
		/// <param name="stateName">The state name.</param>
		/// <param name="stateNameHash">The state hash name. If statNameHash is 0, it changes the current state time.</param>
		/// <param name="layer">The layer index. If layer is -1, it plays the first state with the given state name or hash.</param>
		/// <param name="fixedTime">The time offset (in seconds).</param>
		public void PlayInFixedTime(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime)
		{
			this.PlayInFixedTime(Animator.StringToHash(stateName), layer, fixedTime);
		}

		/// <summary>
		///   <para>Plays a state.</para>
		/// </summary>
		/// <param name="stateName">The state name.</param>
		/// <param name="stateNameHash">The state hash name. If statNameHash is 0, it changes the current state time.</param>
		/// <param name="layer">The layer index. If layer is -1, it plays the first state with the given state name or hash.</param>
		/// <param name="fixedTime">The time offset (in seconds).</param>
		[FreeFunction(Name = "AnimatorBindings::PlayInFixedTime", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void PlayInFixedTime(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime);

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

		/// <summary>
		///   <para>Plays a state.</para>
		/// </summary>
		/// <param name="stateName">The state name.</param>
		/// <param name="stateNameHash">The state hash name. If statNameHash is 0, it changes the current state time.</param>
		/// <param name="layer">The layer index. If layer is -1, it plays the first state with the given state name or hash.</param>
		/// <param name="normalizedTime">The time offset (in percentage).</param>
		public void Play(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			this.Play(Animator.StringToHash(stateName), layer, normalizedTime);
		}

		/// <summary>
		///   <para>Plays a state.</para>
		/// </summary>
		/// <param name="stateName">The state name.</param>
		/// <param name="stateNameHash">The state hash name. If statNameHash is 0, it changes the current state time.</param>
		/// <param name="layer">The layer index. If layer is -1, it plays the first state with the given state name or hash.</param>
		/// <param name="normalizedTime">The time offset (in percentage).</param>
		[FreeFunction(Name = "AnimatorBindings::Play", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Play(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime);

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

		/// <summary>
		///   <para>Sets an AvatarTarget and a targetNormalizedTime for the current state.</para>
		/// </summary>
		/// <param name="targetIndex">The avatar body part that is queried.</param>
		/// <param name="targetNormalizedTime">The current state Time that is queried.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTarget(AvatarTarget targetIndex, float targetNormalizedTime);

		/// <summary>
		///   <para>Returns the position of the target specified by SetTarget.</para>
		/// </summary>
		public Vector3 targetPosition
		{
			get
			{
				Vector3 vector;
				this.get_targetPosition_Injected(out vector);
				return vector;
			}
		}

		/// <summary>
		///   <para>Returns the rotation of the target specified by SetTarget.</para>
		/// </summary>
		public Quaternion targetRotation
		{
			get
			{
				Quaternion quaternion;
				this.get_targetRotation_Injected(out quaternion);
				return quaternion;
			}
		}

		/// <summary>
		///   <para>Returns true if the transform is controlled by the Animator\.</para>
		/// </summary>
		/// <param name="transform">The transform that is queried.</param>
		[Obsolete("Use mask and layers to control subset of transfroms in a skeleton.", true)]
		public bool IsControlled(Transform transform)
		{
			return false;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool IsBoneTransform(Transform transform);

		internal extern Transform avatarRoot
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns Transform mapped to this human bone id.</para>
		/// </summary>
		/// <param name="humanBoneId">The human bone that is queried, see enum HumanBodyBones for a list of possible values.</param>
		public Transform GetBoneTransform(HumanBodyBones humanBoneId)
		{
			if (humanBoneId < HumanBodyBones.Hips || humanBoneId >= HumanBodyBones.LastBone)
			{
				throw new IndexOutOfRangeException("humanBoneId must be between 0 and " + HumanBodyBones.LastBone);
			}
			return this.GetBoneTransformInternal(HumanTrait.GetBoneIndexFromMono((int)humanBoneId));
		}

		[NativeMethod("GetBoneTransform")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Transform GetBoneTransformInternal(int humanBoneId);

		/// <summary>
		///   <para>Controls culling of this Animator component.</para>
		/// </summary>
		public extern AnimatorCullingMode cullingMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Sets the animator in playback mode.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StartPlayback();

		/// <summary>
		///   <para>Stops the animator playback mode. When playback stops, the avatar resumes getting control from game logic.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StopPlayback();

		/// <summary>
		///   <para>Sets the playback position in the recording buffer.</para>
		/// </summary>
		public extern float playbackTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Sets the animator in recording mode, and allocates a circular buffer of size frameCount.</para>
		/// </summary>
		/// <param name="frameCount">The number of frames (updates) that will be recorded. If frameCount is 0, the recording will continue until the user calls StopRecording. The maximum value for frameCount is 10000.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StartRecording(int frameCount);

		/// <summary>
		///   <para>Stops animator record mode.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StopRecording();

		/// <summary>
		///   <para>Start time of the first frame of the buffer relative to the frame at which StartRecording was called.</para>
		/// </summary>
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetRecorderStartTime();

		/// <summary>
		///   <para>End time of the recorded clip relative to when StartRecording was called.</para>
		/// </summary>
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetRecorderStopTime();

		/// <summary>
		///   <para>Gets the mode of the Animator recorder.</para>
		/// </summary>
		public extern AnimatorRecorderMode recorderMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The runtime representation of AnimatorController that controls the Animator.</para>
		/// </summary>
		public extern RuntimeAnimatorController runtimeAnimatorController
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Returns true if Animator has any playables assigned to it.</para>
		/// </summary>
		public extern bool hasBoundPlayables
		{
			[NativeMethod("HasBoundPlayables")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void ClearInternalControllerPlayable();

		/// <summary>
		///   <para>Returns true if the state exists in this layer, false otherwise.</para>
		/// </summary>
		/// <param name="layerIndex">The layer index.</param>
		/// <param name="stateID">The state ID.</param>
		/// <returns>
		///   <para>True if the state exists in this layer, false otherwise.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasState(int layerIndex, int stateID);

		/// <summary>
		///   <para>Generates an parameter id from a string.</para>
		/// </summary>
		/// <param name="name">The string to convert to Id.</param>
		[NativeMethod(Name = "ScriptingStringToCRC32", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int StringToHash(string name);

		/// <summary>
		///   <para>Gets/Sets the current Avatar.</para>
		/// </summary>
		public extern Avatar avatar
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetStats();

		/// <summary>
		///   <para>The PlayableGraph created by the Animator.</para>
		/// </summary>
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
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetCurrentGraph(ref PlayableGraph graph);

		private void CheckIfInIKPass()
		{
			if (this.logWarnings && !this.IsInIKPass())
			{
				Debug.LogWarning("Setting and getting Body Position/Rotation, IK Goals, Lookat and BoneLocalRotation should only be done in OnAnimatorIK or OnStateIK");
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsInIKPass();

		[FreeFunction(Name = "AnimatorBindings::SetFloatString", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatString(string name, float value);

		[FreeFunction(Name = "AnimatorBindings::SetFloatID", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatID(int id, float value);

		[FreeFunction(Name = "AnimatorBindings::GetFloatString", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetFloatString(string name);

		[FreeFunction(Name = "AnimatorBindings::GetFloatID", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetFloatID(int id);

		[FreeFunction(Name = "AnimatorBindings::SetBoolString", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetBoolString(string name, bool value);

		[FreeFunction(Name = "AnimatorBindings::SetBoolID", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetBoolID(int id, bool value);

		[FreeFunction(Name = "AnimatorBindings::GetBoolString", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetBoolString(string name);

		[FreeFunction(Name = "AnimatorBindings::GetBoolID", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetBoolID(int id);

		[FreeFunction(Name = "AnimatorBindings::SetIntegerString", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetIntegerString(string name, int value);

		[FreeFunction(Name = "AnimatorBindings::SetIntegerID", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetIntegerID(int id, int value);

		[FreeFunction(Name = "AnimatorBindings::GetIntegerString", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetIntegerString(string name);

		[FreeFunction(Name = "AnimatorBindings::GetIntegerID", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetIntegerID(int id);

		[FreeFunction(Name = "AnimatorBindings::SetTriggerString", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTriggerString(string name);

		[FreeFunction(Name = "AnimatorBindings::SetTriggerID", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTriggerID(int id);

		[FreeFunction(Name = "AnimatorBindings::ResetTriggerString", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ResetTriggerString(string name);

		[FreeFunction(Name = "AnimatorBindings::ResetTriggerID", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ResetTriggerID(int id);

		[FreeFunction(Name = "AnimatorBindings::IsParameterControlledByCurveString", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsParameterControlledByCurveString(string name);

		[FreeFunction(Name = "AnimatorBindings::IsParameterControlledByCurveID", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsParameterControlledByCurveID(int id);

		[FreeFunction(Name = "AnimatorBindings::SetFloatStringDamp", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatStringDamp(string name, float value, float dampTime, float deltaTime);

		[FreeFunction(Name = "AnimatorBindings::SetFloatIDDamp", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatIDDamp(int id, float value, float dampTime, float deltaTime);

		/// <summary>
		///   <para>Additional layers affects the center of mass.</para>
		/// </summary>
		public extern bool layersAffectMassCenter
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Get left foot bottom height.</para>
		/// </summary>
		public extern float leftFeetBottomHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Get right foot bottom height.</para>
		/// </summary>
		public extern float rightFeetBottomHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeConditional("UNITY_EDITOR")]
		internal extern bool supportsOnAnimatorMove
		{
			[NativeMethod("SupportsOnAnimatorMove")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeConditional("UNITY_EDITOR")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void OnUpdateModeChanged();

		[NativeConditional("UNITY_EDITOR")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void OnCullingModeChanged();

		[NativeConditional("UNITY_EDITOR")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void WriteDefaultPose();

		/// <summary>
		///   <para>Evaluates the animator based on deltaTime.</para>
		/// </summary>
		/// <param name="deltaTime">The time delta.</param>
		[NativeMethod("UpdateWithDelta")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Update(float deltaTime);

		/// <summary>
		///   <para>Rebind all the animated properties and mesh data with the Animator.</para>
		/// </summary>
		public void Rebind()
		{
			this.Rebind(true);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Rebind(bool writeDefaultValues);

		/// <summary>
		///   <para>Apply the default Root Motion.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ApplyBuiltinRootMotion();

		[NativeConditional("UNITY_EDITOR")]
		internal void EvaluateController()
		{
			this.EvaluateController(0f);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void EvaluateController(float deltaTime);

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
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string GetAnimatorStateName(int layerIndex, bool current);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string ResolveHash(int hash);

		public extern bool logWarnings
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Sets whether the Animator sends events of type AnimationEvent.</para>
		/// </summary>
		public extern bool fireEvents
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Controls the behaviour of the Animator component when a GameObject is disabled.</para>
		/// </summary>
		public extern bool keepAnimatorControllerStateOnDisable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Gets the value of a vector parameter.</para>
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		[Obsolete("GetVector is deprecated.")]
		public Vector3 GetVector(string name)
		{
			return Vector3.zero;
		}

		/// <summary>
		///   <para>Gets the value of a vector parameter.</para>
		/// </summary>
		/// <param name="id">The id of the parameter. The id is generated using Animator::StringToHash.</param>
		[Obsolete("GetVector is deprecated.")]
		public Vector3 GetVector(int id)
		{
			return Vector3.zero;
		}

		/// <summary>
		///   <para>Sets the value of a vector parameter.</para>
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="value">The new value for the parameter.</param>
		[Obsolete("SetVector is deprecated.")]
		public void SetVector(string name, Vector3 value)
		{
		}

		/// <summary>
		///   <para>Sets the value of a vector parameter.</para>
		/// </summary>
		/// <param name="id">The id of the parameter. The id is generated using Animator::StringToHash.</param>
		/// <param name="value">The new value for the parameter.</param>
		[Obsolete("SetVector is deprecated.")]
		public void SetVector(int id, Vector3 value)
		{
		}

		/// <summary>
		///   <para>Gets the value of a quaternion parameter.</para>
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		[Obsolete("GetQuaternion is deprecated.")]
		public Quaternion GetQuaternion(string name)
		{
			return Quaternion.identity;
		}

		/// <summary>
		///   <para>Gets the value of a quaternion parameter.</para>
		/// </summary>
		/// <param name="id">The id of the parameter. The id is generated using Animator::StringToHash.</param>
		[Obsolete("GetQuaternion is deprecated.")]
		public Quaternion GetQuaternion(int id)
		{
			return Quaternion.identity;
		}

		/// <summary>
		///   <para>Sets the value of a quaternion parameter.</para>
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="value">The new value for the parameter.</param>
		[Obsolete("SetQuaternion is deprecated.")]
		public void SetQuaternion(string name, Quaternion value)
		{
		}

		/// <summary>
		///   <para>Sets the value of a quaternion parameter.</para>
		/// </summary>
		/// <param name="id">Of the parameter. The id is generated using Animator::StringToHash.</param>
		/// <param name="value">The new value for the parameter.</param>
		[Obsolete("SetQuaternion is deprecated.")]
		public void SetQuaternion(int id, Quaternion value)
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_deltaPosition_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_deltaRotation_Injected(out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_velocity_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_angularVelocity_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_rootPosition_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_rootPosition_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_rootRotation_Injected(out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_rootRotation_Injected(ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_bodyPositionInternal_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_bodyPositionInternal_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_bodyRotationInternal_Injected(out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_bodyRotationInternal_Injected(ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetGoalPosition_Injected(AvatarIKGoal goal, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetGoalPosition_Injected(AvatarIKGoal goal, ref Vector3 goalPosition);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetGoalRotation_Injected(AvatarIKGoal goal, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetGoalRotation_Injected(AvatarIKGoal goal, ref Quaternion goalRotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetHintPosition_Injected(AvatarIKHint hint, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetHintPosition_Injected(AvatarIKHint hint, ref Vector3 hintPosition);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetLookAtPositionInternal_Injected(ref Vector3 lookAtPosition);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetBoneLocalRotationInternal_Injected(int humanBoneId, ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_pivotPosition_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void MatchTarget_Injected(ref Vector3 matchPosition, ref Quaternion matchRotation, int targetBodyPart, ref MatchTargetWeightMask weightMask, float startNormalizedTime, float targetNormalizedTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_targetPosition_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_targetRotation_Injected(out Quaternion ret);
	}
}
