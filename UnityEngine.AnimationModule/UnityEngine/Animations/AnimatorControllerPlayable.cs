using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[StaticAccessor("AnimatorControllerPlayableBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/Animation/AnimatorInfo.h")]
	[NativeHeader("Modules/Animation/RuntimeAnimatorController.h")]
	[NativeHeader("Modules/Animation/Director/AnimatorControllerPlayable.h")]
	[NativeHeader("Modules/Animation/ScriptBindings/Animator.bindings.h")]
	[NativeHeader("Modules/Animation/ScriptBindings/AnimatorControllerPlayable.bindings.h")]
	[RequiredByNativeCode]
	public struct AnimatorControllerPlayable : IPlayable, IEquatable<AnimatorControllerPlayable>
	{
		public static AnimatorControllerPlayable Null
		{
			get
			{
				return AnimatorControllerPlayable.m_NullPlayable;
			}
		}

		public static AnimatorControllerPlayable Create(PlayableGraph graph, RuntimeAnimatorController controller)
		{
			PlayableHandle playableHandle = AnimatorControllerPlayable.CreateHandle(graph, controller);
			return new AnimatorControllerPlayable(playableHandle);
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph, RuntimeAnimatorController controller)
		{
			PlayableHandle @null = PlayableHandle.Null;
			bool flag = !AnimatorControllerPlayable.CreateHandleInternal(graph, controller, ref @null);
			PlayableHandle playableHandle;
			if (flag)
			{
				playableHandle = PlayableHandle.Null;
			}
			else
			{
				playableHandle = @null;
			}
			return playableHandle;
		}

		internal AnimatorControllerPlayable(PlayableHandle handle)
		{
			this.m_Handle = PlayableHandle.Null;
			this.SetHandle(handle);
		}

		public PlayableHandle GetHandle()
		{
			return this.m_Handle;
		}

		public void SetHandle(PlayableHandle handle)
		{
			bool flag = this.m_Handle.IsValid();
			if (flag)
			{
				throw new InvalidOperationException("Cannot call IPlayable.SetHandle on an instance that already contains a valid handle.");
			}
			bool flag2 = handle.IsValid();
			if (flag2)
			{
				bool flag3 = !handle.IsPlayableOfType<AnimatorControllerPlayable>();
				if (flag3)
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimatorControllerPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		public static implicit operator Playable(AnimatorControllerPlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		public static explicit operator AnimatorControllerPlayable(Playable playable)
		{
			return new AnimatorControllerPlayable(playable.GetHandle());
		}

		public bool Equals(AnimatorControllerPlayable other)
		{
			return this.GetHandle() == other.GetHandle();
		}

		public float GetFloat(string name)
		{
			return AnimatorControllerPlayable.GetFloatString(ref this.m_Handle, name);
		}

		public float GetFloat(int id)
		{
			return AnimatorControllerPlayable.GetFloatID(ref this.m_Handle, id);
		}

		public void SetFloat(string name, float value)
		{
			AnimatorControllerPlayable.SetFloatString(ref this.m_Handle, name, value);
		}

		public void SetFloat(int id, float value)
		{
			AnimatorControllerPlayable.SetFloatID(ref this.m_Handle, id, value);
		}

		public bool GetBool(string name)
		{
			return AnimatorControllerPlayable.GetBoolString(ref this.m_Handle, name);
		}

		public bool GetBool(int id)
		{
			return AnimatorControllerPlayable.GetBoolID(ref this.m_Handle, id);
		}

		public void SetBool(string name, bool value)
		{
			AnimatorControllerPlayable.SetBoolString(ref this.m_Handle, name, value);
		}

		public void SetBool(int id, bool value)
		{
			AnimatorControllerPlayable.SetBoolID(ref this.m_Handle, id, value);
		}

		public int GetInteger(string name)
		{
			return AnimatorControllerPlayable.GetIntegerString(ref this.m_Handle, name);
		}

		public int GetInteger(int id)
		{
			return AnimatorControllerPlayable.GetIntegerID(ref this.m_Handle, id);
		}

		public void SetInteger(string name, int value)
		{
			AnimatorControllerPlayable.SetIntegerString(ref this.m_Handle, name, value);
		}

		public void SetInteger(int id, int value)
		{
			AnimatorControllerPlayable.SetIntegerID(ref this.m_Handle, id, value);
		}

		public void SetTrigger(string name)
		{
			AnimatorControllerPlayable.SetTriggerString(ref this.m_Handle, name);
		}

		public void SetTrigger(int id)
		{
			AnimatorControllerPlayable.SetTriggerID(ref this.m_Handle, id);
		}

		public void ResetTrigger(string name)
		{
			AnimatorControllerPlayable.ResetTriggerString(ref this.m_Handle, name);
		}

		public void ResetTrigger(int id)
		{
			AnimatorControllerPlayable.ResetTriggerID(ref this.m_Handle, id);
		}

		public bool IsParameterControlledByCurve(string name)
		{
			return AnimatorControllerPlayable.IsParameterControlledByCurveString(ref this.m_Handle, name);
		}

		public bool IsParameterControlledByCurve(int id)
		{
			return AnimatorControllerPlayable.IsParameterControlledByCurveID(ref this.m_Handle, id);
		}

		public int GetLayerCount()
		{
			return AnimatorControllerPlayable.GetLayerCountInternal(ref this.m_Handle);
		}

		public string GetLayerName(int layerIndex)
		{
			return AnimatorControllerPlayable.GetLayerNameInternal(ref this.m_Handle, layerIndex);
		}

		public int GetLayerIndex(string layerName)
		{
			return AnimatorControllerPlayable.GetLayerIndexInternal(ref this.m_Handle, layerName);
		}

		public float GetLayerWeight(int layerIndex)
		{
			return AnimatorControllerPlayable.GetLayerWeightInternal(ref this.m_Handle, layerIndex);
		}

		public void SetLayerWeight(int layerIndex, float weight)
		{
			AnimatorControllerPlayable.SetLayerWeightInternal(ref this.m_Handle, layerIndex, weight);
		}

		public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex)
		{
			return AnimatorControllerPlayable.GetCurrentAnimatorStateInfoInternal(ref this.m_Handle, layerIndex);
		}

		public AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex)
		{
			return AnimatorControllerPlayable.GetNextAnimatorStateInfoInternal(ref this.m_Handle, layerIndex);
		}

		public AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex)
		{
			return AnimatorControllerPlayable.GetAnimatorTransitionInfoInternal(ref this.m_Handle, layerIndex);
		}

		public AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex)
		{
			return AnimatorControllerPlayable.GetCurrentAnimatorClipInfoInternal(ref this.m_Handle, layerIndex);
		}

		public void GetCurrentAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
		{
			bool flag = clips == null;
			if (flag)
			{
				throw new ArgumentNullException("clips");
			}
			AnimatorControllerPlayable.GetAnimatorClipInfoInternal(ref this.m_Handle, layerIndex, true, clips);
		}

		public void GetNextAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
		{
			bool flag = clips == null;
			if (flag)
			{
				throw new ArgumentNullException("clips");
			}
			AnimatorControllerPlayable.GetAnimatorClipInfoInternal(ref this.m_Handle, layerIndex, false, clips);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAnimatorClipInfoInternal(ref PlayableHandle handle, int layerIndex, bool isCurrent, object clips);

		public int GetCurrentAnimatorClipInfoCount(int layerIndex)
		{
			return AnimatorControllerPlayable.GetAnimatorClipInfoCountInternal(ref this.m_Handle, layerIndex, true);
		}

		public int GetNextAnimatorClipInfoCount(int layerIndex)
		{
			return AnimatorControllerPlayable.GetAnimatorClipInfoCountInternal(ref this.m_Handle, layerIndex, false);
		}

		public AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex)
		{
			return AnimatorControllerPlayable.GetNextAnimatorClipInfoInternal(ref this.m_Handle, layerIndex);
		}

		public bool IsInTransition(int layerIndex)
		{
			return AnimatorControllerPlayable.IsInTransitionInternal(ref this.m_Handle, layerIndex);
		}

		public int GetParameterCount()
		{
			return AnimatorControllerPlayable.GetParameterCountInternal(ref this.m_Handle);
		}

		public AnimatorControllerParameter GetParameter(int index)
		{
			AnimatorControllerParameter parameterInternal = AnimatorControllerPlayable.GetParameterInternal(ref this.m_Handle, index);
			bool flag = parameterInternal.m_Type == (AnimatorControllerParameterType)0;
			if (flag)
			{
				throw new IndexOutOfRangeException("Invalid parameter index.");
			}
			return parameterInternal;
		}

		public void CrossFadeInFixedTime(string stateName, float transitionDuration)
		{
			AnimatorControllerPlayable.CrossFadeInFixedTimeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), transitionDuration, -1, 0f);
		}

		public void CrossFadeInFixedTime(string stateName, float transitionDuration, int layer)
		{
			AnimatorControllerPlayable.CrossFadeInFixedTimeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), transitionDuration, layer, 0f);
		}

		public void CrossFadeInFixedTime(string stateName, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTime)
		{
			AnimatorControllerPlayable.CrossFadeInFixedTimeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), transitionDuration, layer, fixedTime);
		}

		public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration)
		{
			AnimatorControllerPlayable.CrossFadeInFixedTimeInternal(ref this.m_Handle, stateNameHash, transitionDuration, -1, 0f);
		}

		public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration, int layer)
		{
			AnimatorControllerPlayable.CrossFadeInFixedTimeInternal(ref this.m_Handle, stateNameHash, transitionDuration, layer, 0f);
		}

		public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTime)
		{
			AnimatorControllerPlayable.CrossFadeInFixedTimeInternal(ref this.m_Handle, stateNameHash, transitionDuration, layer, fixedTime);
		}

		public void CrossFade(string stateName, float transitionDuration)
		{
			AnimatorControllerPlayable.CrossFadeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), transitionDuration, -1, float.NegativeInfinity);
		}

		public void CrossFade(string stateName, float transitionDuration, int layer)
		{
			AnimatorControllerPlayable.CrossFadeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), transitionDuration, layer, float.NegativeInfinity);
		}

		public void CrossFade(string stateName, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			AnimatorControllerPlayable.CrossFadeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), transitionDuration, layer, normalizedTime);
		}

		public void CrossFade(int stateNameHash, float transitionDuration)
		{
			AnimatorControllerPlayable.CrossFadeInternal(ref this.m_Handle, stateNameHash, transitionDuration, -1, float.NegativeInfinity);
		}

		public void CrossFade(int stateNameHash, float transitionDuration, int layer)
		{
			AnimatorControllerPlayable.CrossFadeInternal(ref this.m_Handle, stateNameHash, transitionDuration, layer, float.NegativeInfinity);
		}

		public void CrossFade(int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			AnimatorControllerPlayable.CrossFadeInternal(ref this.m_Handle, stateNameHash, transitionDuration, layer, normalizedTime);
		}

		public void PlayInFixedTime(string stateName)
		{
			AnimatorControllerPlayable.PlayInFixedTimeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), -1, float.NegativeInfinity);
		}

		public void PlayInFixedTime(string stateName, int layer)
		{
			AnimatorControllerPlayable.PlayInFixedTimeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), layer, float.NegativeInfinity);
		}

		public void PlayInFixedTime(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime)
		{
			AnimatorControllerPlayable.PlayInFixedTimeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), layer, fixedTime);
		}

		public void PlayInFixedTime(int stateNameHash)
		{
			AnimatorControllerPlayable.PlayInFixedTimeInternal(ref this.m_Handle, stateNameHash, -1, float.NegativeInfinity);
		}

		public void PlayInFixedTime(int stateNameHash, int layer)
		{
			AnimatorControllerPlayable.PlayInFixedTimeInternal(ref this.m_Handle, stateNameHash, layer, float.NegativeInfinity);
		}

		public void PlayInFixedTime(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime)
		{
			AnimatorControllerPlayable.PlayInFixedTimeInternal(ref this.m_Handle, stateNameHash, layer, fixedTime);
		}

		public void Play(string stateName)
		{
			AnimatorControllerPlayable.PlayInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), -1, float.NegativeInfinity);
		}

		public void Play(string stateName, int layer)
		{
			AnimatorControllerPlayable.PlayInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), layer, float.NegativeInfinity);
		}

		public void Play(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			AnimatorControllerPlayable.PlayInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), layer, normalizedTime);
		}

		public void Play(int stateNameHash)
		{
			AnimatorControllerPlayable.PlayInternal(ref this.m_Handle, stateNameHash, -1, float.NegativeInfinity);
		}

		public void Play(int stateNameHash, int layer)
		{
			AnimatorControllerPlayable.PlayInternal(ref this.m_Handle, stateNameHash, layer, float.NegativeInfinity);
		}

		public void Play(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			AnimatorControllerPlayable.PlayInternal(ref this.m_Handle, stateNameHash, layer, normalizedTime);
		}

		public void ResetControllerState([DefaultValue("true")] bool resetParameters = true)
		{
			AnimatorControllerPlayable.ResetControllerStateInternal(ref this.m_Handle, resetParameters);
		}

		public bool HasState(int layerIndex, int stateID)
		{
			return AnimatorControllerPlayable.HasStateInternal(ref this.m_Handle, layerIndex, stateID);
		}

		internal string ResolveHash(int hash)
		{
			return AnimatorControllerPlayable.ResolveHashInternal(ref this.m_Handle, hash);
		}

		[NativeThrows]
		private static bool CreateHandleInternal(PlayableGraph graph, RuntimeAnimatorController controller, ref PlayableHandle handle)
		{
			return AnimatorControllerPlayable.CreateHandleInternal_Injected(ref graph, Object.MarshalledUnityObject.Marshal<RuntimeAnimatorController>(controller), ref handle);
		}

		[NativeThrows]
		private static RuntimeAnimatorController GetAnimatorControllerInternal(ref PlayableHandle handle)
		{
			return Unmarshal.UnmarshalUnityObject<RuntimeAnimatorController>(AnimatorControllerPlayable.GetAnimatorControllerInternal_Injected(ref handle));
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetLayerCountInternal(ref PlayableHandle handle);

		[NativeThrows]
		private static string GetLayerNameInternal(ref PlayableHandle handle, int layerIndex)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				AnimatorControllerPlayable.GetLayerNameInternal_Injected(ref handle, layerIndex, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[NativeThrows]
		private unsafe static int GetLayerIndexInternal(ref PlayableHandle handle, string layerName)
		{
			int layerIndexInternal_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(layerName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = layerName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				layerIndexInternal_Injected = AnimatorControllerPlayable.GetLayerIndexInternal_Injected(ref handle, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return layerIndexInternal_Injected;
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetLayerWeightInternal(ref PlayableHandle handle, int layerIndex);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLayerWeightInternal(ref PlayableHandle handle, int layerIndex, float weight);

		[NativeThrows]
		private static AnimatorStateInfo GetCurrentAnimatorStateInfoInternal(ref PlayableHandle handle, int layerIndex)
		{
			AnimatorStateInfo animatorStateInfo;
			AnimatorControllerPlayable.GetCurrentAnimatorStateInfoInternal_Injected(ref handle, layerIndex, out animatorStateInfo);
			return animatorStateInfo;
		}

		[NativeThrows]
		private static AnimatorStateInfo GetNextAnimatorStateInfoInternal(ref PlayableHandle handle, int layerIndex)
		{
			AnimatorStateInfo animatorStateInfo;
			AnimatorControllerPlayable.GetNextAnimatorStateInfoInternal_Injected(ref handle, layerIndex, out animatorStateInfo);
			return animatorStateInfo;
		}

		[NativeThrows]
		private static AnimatorTransitionInfo GetAnimatorTransitionInfoInternal(ref PlayableHandle handle, int layerIndex)
		{
			AnimatorTransitionInfo animatorTransitionInfo;
			AnimatorControllerPlayable.GetAnimatorTransitionInfoInternal_Injected(ref handle, layerIndex, out animatorTransitionInfo);
			return animatorTransitionInfo;
		}

		[NativeThrows]
		private static AnimatorClipInfo[] GetCurrentAnimatorClipInfoInternal(ref PlayableHandle handle, int layerIndex)
		{
			AnimatorClipInfo[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				AnimatorControllerPlayable.GetCurrentAnimatorClipInfoInternal_Injected(ref handle, layerIndex, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				AnimatorClipInfo[] array;
				blittableArrayWrapper.Unmarshal<AnimatorClipInfo>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAnimatorClipInfoCountInternal(ref PlayableHandle handle, int layerIndex, bool current);

		[NativeThrows]
		private static AnimatorClipInfo[] GetNextAnimatorClipInfoInternal(ref PlayableHandle handle, int layerIndex)
		{
			AnimatorClipInfo[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				AnimatorControllerPlayable.GetNextAnimatorClipInfoInternal_Injected(ref handle, layerIndex, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				AnimatorClipInfo[] array;
				blittableArrayWrapper.Unmarshal<AnimatorClipInfo>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeThrows]
		private static string ResolveHashInternal(ref PlayableHandle handle, int hash)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				AnimatorControllerPlayable.ResolveHashInternal_Injected(ref handle, hash, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsInTransitionInternal(ref PlayableHandle handle, int layerIndex);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimatorControllerParameter GetParameterInternal(ref PlayableHandle handle, int index);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetParameterCountInternal(ref PlayableHandle handle);

		[ThreadSafe]
		private unsafe static int StringToHash(string name)
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
				num = AnimatorControllerPlayable.StringToHash_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CrossFadeInFixedTimeInternal(ref PlayableHandle handle, int stateNameHash, float transitionDuration, int layer, float fixedTime);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CrossFadeInternal(ref PlayableHandle handle, int stateNameHash, float transitionDuration, int layer, float normalizedTime);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PlayInFixedTimeInternal(ref PlayableHandle handle, int stateNameHash, int layer, float fixedTime);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PlayInternal(ref PlayableHandle handle, int stateNameHash, int layer, float normalizedTime);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetControllerStateInternal(ref PlayableHandle handle, bool resetParameters);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasStateInternal(ref PlayableHandle handle, int layerIndex, int stateID);

		[NativeThrows]
		private unsafe static void SetFloatString(ref PlayableHandle handle, string name, float value)
		{
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
				AnimatorControllerPlayable.SetFloatString_Injected(ref handle, ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatID(ref PlayableHandle handle, int id, float value);

		[NativeThrows]
		private unsafe static float GetFloatString(ref PlayableHandle handle, string name)
		{
			float floatString_Injected;
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
				floatString_Injected = AnimatorControllerPlayable.GetFloatString_Injected(ref handle, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return floatString_Injected;
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFloatID(ref PlayableHandle handle, int id);

		[NativeThrows]
		private unsafe static void SetBoolString(ref PlayableHandle handle, string name, bool value)
		{
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
				AnimatorControllerPlayable.SetBoolString_Injected(ref handle, ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoolID(ref PlayableHandle handle, int id, bool value);

		[NativeThrows]
		private unsafe static bool GetBoolString(ref PlayableHandle handle, string name)
		{
			bool boolString_Injected;
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
				boolString_Injected = AnimatorControllerPlayable.GetBoolString_Injected(ref handle, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return boolString_Injected;
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetBoolID(ref PlayableHandle handle, int id);

		[NativeThrows]
		private unsafe static void SetIntegerString(ref PlayableHandle handle, string name, int value)
		{
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
				AnimatorControllerPlayable.SetIntegerString_Injected(ref handle, ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIntegerID(ref PlayableHandle handle, int id, int value);

		[NativeThrows]
		private unsafe static int GetIntegerString(ref PlayableHandle handle, string name)
		{
			int integerString_Injected;
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
				integerString_Injected = AnimatorControllerPlayable.GetIntegerString_Injected(ref handle, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return integerString_Injected;
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIntegerID(ref PlayableHandle handle, int id);

		[NativeThrows]
		private unsafe static void SetTriggerString(ref PlayableHandle handle, string name)
		{
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
				AnimatorControllerPlayable.SetTriggerString_Injected(ref handle, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTriggerID(ref PlayableHandle handle, int id);

		[NativeThrows]
		private unsafe static void ResetTriggerString(ref PlayableHandle handle, string name)
		{
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
				AnimatorControllerPlayable.ResetTriggerString_Injected(ref handle, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetTriggerID(ref PlayableHandle handle, int id);

		[NativeThrows]
		private unsafe static bool IsParameterControlledByCurveString(ref PlayableHandle handle, string name)
		{
			bool flag;
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
				flag = AnimatorControllerPlayable.IsParameterControlledByCurveString_Injected(ref handle, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsParameterControlledByCurveID(ref PlayableHandle handle, int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateHandleInternal_Injected([In] ref PlayableGraph graph, IntPtr controller, ref PlayableHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetAnimatorControllerInternal_Injected(ref PlayableHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLayerNameInternal_Injected(ref PlayableHandle handle, int layerIndex, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetLayerIndexInternal_Injected(ref PlayableHandle handle, ref ManagedSpanWrapper layerName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCurrentAnimatorStateInfoInternal_Injected(ref PlayableHandle handle, int layerIndex, out AnimatorStateInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNextAnimatorStateInfoInternal_Injected(ref PlayableHandle handle, int layerIndex, out AnimatorStateInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAnimatorTransitionInfoInternal_Injected(ref PlayableHandle handle, int layerIndex, out AnimatorTransitionInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCurrentAnimatorClipInfoInternal_Injected(ref PlayableHandle handle, int layerIndex, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNextAnimatorClipInfoInternal_Injected(ref PlayableHandle handle, int layerIndex, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResolveHashInternal_Injected(ref PlayableHandle handle, int hash, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int StringToHash_Injected(ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatString_Injected(ref PlayableHandle handle, ref ManagedSpanWrapper name, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFloatString_Injected(ref PlayableHandle handle, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoolString_Injected(ref PlayableHandle handle, ref ManagedSpanWrapper name, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetBoolString_Injected(ref PlayableHandle handle, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIntegerString_Injected(ref PlayableHandle handle, ref ManagedSpanWrapper name, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIntegerString_Injected(ref PlayableHandle handle, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTriggerString_Injected(ref PlayableHandle handle, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetTriggerString_Injected(ref PlayableHandle handle, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsParameterControlledByCurveString_Injected(ref PlayableHandle handle, ref ManagedSpanWrapper name);

		private PlayableHandle m_Handle;

		private static readonly AnimatorControllerPlayable m_NullPlayable = new AnimatorControllerPlayable(PlayableHandle.Null);
	}
}
