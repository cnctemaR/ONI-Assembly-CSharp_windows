using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.VFX
{
	[NativeHeader("Modules/VFX/Public/ScriptBindings/VisualEffectBindings.h")]
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Modules/VFX/Public/VisualEffect.h")]
	public class VisualEffect : Behaviour
	{
		public bool pause
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VisualEffect.get_pause_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VisualEffect.set_pause_Injected(intPtr, value);
			}
		}

		public float playRate
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VisualEffect.get_playRate_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VisualEffect.set_playRate_Injected(intPtr, value);
			}
		}

		public uint startSeed
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VisualEffect.get_startSeed_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VisualEffect.set_startSeed_Injected(intPtr, value);
			}
		}

		public bool resetSeedOnPlay
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VisualEffect.get_resetSeedOnPlay_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VisualEffect.set_resetSeedOnPlay_Injected(intPtr, value);
			}
		}

		public int initialEventID
		{
			[FreeFunction(Name = "VisualEffectBindings::GetInitialEventID", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VisualEffect.get_initialEventID_Injected(intPtr);
			}
			[FreeFunction(Name = "VisualEffectBindings::SetInitialEventID", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VisualEffect.set_initialEventID_Injected(intPtr, value);
			}
		}

		public unsafe string initialEventName
		{
			[FreeFunction(Name = "VisualEffectBindings::GetInitialEventName", HasExplicitThis = true)]
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					VisualEffect.get_initialEventName_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
			[FreeFunction(Name = "VisualEffectBindings::SetInitialEventName", HasExplicitThis = true)]
			set
			{
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					VisualEffect.set_initialEventName_Injected(intPtr, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		public bool culled
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VisualEffect.get_culled_Injected(intPtr);
			}
		}

		public VisualEffectAsset visualEffectAsset
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<VisualEffectAsset>(VisualEffect.get_visualEffectAsset_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VisualEffect.set_visualEffectAsset_Injected(intPtr, Object.MarshalledUnityObject.Marshal<VisualEffectAsset>(value));
			}
		}

		public VFXEventAttribute CreateVFXEventAttribute()
		{
			bool flag = this.visualEffectAsset == null;
			VFXEventAttribute vfxeventAttribute;
			if (flag)
			{
				vfxeventAttribute = null;
			}
			else
			{
				VFXEventAttribute vfxeventAttribute2 = VFXEventAttribute.Internal_InstanciateVFXEventAttribute(this.visualEffectAsset);
				vfxeventAttribute = vfxeventAttribute2;
			}
			return vfxeventAttribute;
		}

		private void CheckValidVFXEventAttribute(VFXEventAttribute eventAttribute)
		{
			bool flag = eventAttribute != null && eventAttribute.vfxAsset != this.visualEffectAsset;
			if (flag)
			{
				throw new InvalidOperationException("Invalid VFXEventAttribute provided to VisualEffect. It has been created with another VisualEffectAsset. Use CreateVFXEventAttribute.");
			}
		}

		[FreeFunction(Name = "VisualEffectBindings::SendEventFromScript", HasExplicitThis = true)]
		private void SendEventFromScript(int eventNameID, VFXEventAttribute eventAttribute)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.SendEventFromScript_Injected(intPtr, eventNameID, (eventAttribute == null) ? ((IntPtr)0) : VFXEventAttribute.BindingsMarshaller.ConvertToNative(eventAttribute));
		}

		public void SendEvent(int eventNameID, VFXEventAttribute eventAttribute)
		{
			this.CheckValidVFXEventAttribute(eventAttribute);
			this.SendEventFromScript(eventNameID, eventAttribute);
		}

		public void SendEvent(string eventName, VFXEventAttribute eventAttribute)
		{
			this.SendEvent(Shader.PropertyToID(eventName), eventAttribute);
		}

		public void SendEvent(int eventNameID)
		{
			this.SendEventFromScript(eventNameID, null);
		}

		public void SendEvent(string eventName)
		{
			this.SendEvent(Shader.PropertyToID(eventName), null);
		}

		public void Play(VFXEventAttribute eventAttribute)
		{
			this.SendEvent(VisualEffectAsset.PlayEventID, eventAttribute);
		}

		public void Play()
		{
			this.SendEvent(VisualEffectAsset.PlayEventID);
		}

		public void Stop(VFXEventAttribute eventAttribute)
		{
			this.SendEvent(VisualEffectAsset.StopEventID, eventAttribute);
		}

		public void Stop()
		{
			this.SendEvent(VisualEffectAsset.StopEventID);
		}

		public void Reinit()
		{
			this.Reinit(true);
		}

		internal void Reinit(bool sendInitialEventAndPrewarm = true)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.Reinit_Injected(intPtr, sendInitialEventAndPrewarm);
		}

		public void AdvanceOneFrame()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.AdvanceOneFrame_Injected(intPtr);
		}

		internal void RecreateData()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.RecreateData_Injected(intPtr);
		}

		[FreeFunction(Name = "VisualEffectBindings::GetGPUTaskMarkerName", HasExplicitThis = true, ThrowsException = true)]
		[NativeConditional("ENABLE_PROFILER")]
		private string GetGPUTaskMarkerName(int nameID, int taskIndex)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				VisualEffect.GetGPUTaskMarkerName_Injected(intPtr, nameID, taskIndex, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[NativeConditional("ENABLE_PROFILER")]
		[FreeFunction(Name = "VisualEffectBindings::GetCPUEffectMarkerName", HasExplicitThis = true, ThrowsException = true)]
		internal string GetCPUEffectMarkerName(int markerIndex)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				VisualEffect.GetCPUEffectMarkerName_Injected(intPtr, markerIndex, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[NativeConditional("ENABLE_PROFILER")]
		[FreeFunction(Name = "VisualEffectBindings::GetCPUSystemMarkerName", HasExplicitThis = true, ThrowsException = true)]
		private string GetCPUSystemMarkerName(int nameID)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				VisualEffect.GetCPUSystemMarkerName_Injected(intPtr, nameID, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[FreeFunction(Name = "VisualEffectBindings::RegisterForProfiling", HasExplicitThis = true, ThrowsException = false)]
		[NativeConditional("ENABLE_PROFILER")]
		internal void RegisterForProfiling()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.RegisterForProfiling_Injected(intPtr);
		}

		[FreeFunction(Name = "VisualEffectBindings::UnregisterForProfiling", HasExplicitThis = true, ThrowsException = false)]
		[NativeConditional("ENABLE_PROFILER")]
		internal void UnregisterForProfiling()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.UnregisterForProfiling_Injected(intPtr);
		}

		[FreeFunction(Name = "VisualEffectBindings::IsRegisteredForProfiling", HasExplicitThis = true, ThrowsException = false)]
		[NativeConditional("ENABLE_PROFILER")]
		internal bool IsRegisteredForProfiling()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.IsRegisteredForProfiling_Injected(intPtr);
		}

		[FreeFunction(Name = "VisualEffectBindings::ResetOverrideFromScript", HasExplicitThis = true)]
		public void ResetOverride(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.ResetOverride_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::GetTextureDimensionFromScript", HasExplicitThis = true)]
		public TextureDimension GetTextureDimension(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.GetTextureDimension_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::HasValueFromScript<bool>", HasExplicitThis = true)]
		public bool HasBool(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.HasBool_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::HasValueFromScript<int>", HasExplicitThis = true)]
		public bool HasInt(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.HasInt_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::HasValueFromScript<UInt32>", HasExplicitThis = true)]
		public bool HasUInt(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.HasUInt_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::HasValueFromScript<float>", HasExplicitThis = true)]
		public bool HasFloat(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.HasFloat_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::HasValueFromScript<Vector2f>", HasExplicitThis = true)]
		public bool HasVector2(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.HasVector2_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::HasValueFromScript<Vector3f>", HasExplicitThis = true)]
		public bool HasVector3(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.HasVector3_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::HasValueFromScript<Vector4f>", HasExplicitThis = true)]
		public bool HasVector4(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.HasVector4_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::HasValueFromScript<Matrix4x4f>", HasExplicitThis = true)]
		public bool HasMatrix4x4(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.HasMatrix4x4_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::HasValueFromScript<Texture*>", HasExplicitThis = true)]
		public bool HasTexture(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.HasTexture_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::HasValueFromScript<AnimationCurve*>", HasExplicitThis = true)]
		public bool HasAnimationCurve(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.HasAnimationCurve_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::HasValueFromScript<Gradient*>", HasExplicitThis = true)]
		public bool HasGradient(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.HasGradient_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::HasValueFromScript<Mesh*>", HasExplicitThis = true)]
		public bool HasMesh(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.HasMesh_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::HasValueFromScript<SkinnedMeshRenderer*>", HasExplicitThis = true)]
		public bool HasSkinnedMeshRenderer(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.HasSkinnedMeshRenderer_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::HasValueFromScript<GraphicsBuffer*>", HasExplicitThis = true)]
		public bool HasGraphicsBuffer(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.HasGraphicsBuffer_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::SetValueFromScript<bool>", HasExplicitThis = true)]
		public void SetBool(int nameID, bool b)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.SetBool_Injected(intPtr, nameID, b);
		}

		[FreeFunction(Name = "VisualEffectBindings::SetValueFromScript<int>", HasExplicitThis = true)]
		public void SetInt(int nameID, int i)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.SetInt_Injected(intPtr, nameID, i);
		}

		[FreeFunction(Name = "VisualEffectBindings::SetValueFromScript<UInt32>", HasExplicitThis = true)]
		public void SetUInt(int nameID, uint i)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.SetUInt_Injected(intPtr, nameID, i);
		}

		[FreeFunction(Name = "VisualEffectBindings::SetValueFromScript<float>", HasExplicitThis = true)]
		public void SetFloat(int nameID, float f)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.SetFloat_Injected(intPtr, nameID, f);
		}

		[FreeFunction(Name = "VisualEffectBindings::SetValueFromScript<Vector2f>", HasExplicitThis = true)]
		public void SetVector2(int nameID, Vector2 v)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.SetVector2_Injected(intPtr, nameID, ref v);
		}

		[FreeFunction(Name = "VisualEffectBindings::SetValueFromScript<Vector3f>", HasExplicitThis = true)]
		public void SetVector3(int nameID, Vector3 v)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.SetVector3_Injected(intPtr, nameID, ref v);
		}

		[FreeFunction(Name = "VisualEffectBindings::SetValueFromScript<Vector4f>", HasExplicitThis = true)]
		public void SetVector4(int nameID, Vector4 v)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.SetVector4_Injected(intPtr, nameID, ref v);
		}

		[FreeFunction(Name = "VisualEffectBindings::SetValueFromScript<Matrix4x4f>", HasExplicitThis = true)]
		public void SetMatrix4x4(int nameID, Matrix4x4 v)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.SetMatrix4x4_Injected(intPtr, nameID, ref v);
		}

		[FreeFunction(Name = "VisualEffectBindings::SetValueFromScript<Texture*>", HasExplicitThis = true)]
		public void SetTexture(int nameID, [NotNull] Texture t)
		{
			if (t == null)
			{
				ThrowHelper.ThrowArgumentNullException(t, "t");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Texture>(t);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(t, "t");
			}
			VisualEffect.SetTexture_Injected(intPtr, nameID, intPtr2);
		}

		[FreeFunction(Name = "VisualEffectBindings::SetValueFromScript<AnimationCurve*>", HasExplicitThis = true)]
		public void SetAnimationCurve(int nameID, [NotNull] AnimationCurve c)
		{
			if (c == null)
			{
				ThrowHelper.ThrowArgumentNullException(c, "c");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = AnimationCurve.BindingsMarshaller.ConvertToNative(c);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(c, "c");
			}
			VisualEffect.SetAnimationCurve_Injected(intPtr, nameID, intPtr2);
		}

		[FreeFunction(Name = "VisualEffectBindings::SetValueFromScript<Gradient*>", HasExplicitThis = true)]
		public void SetGradient(int nameID, [NotNull] Gradient g)
		{
			if (g == null)
			{
				ThrowHelper.ThrowArgumentNullException(g, "g");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Gradient.BindingsMarshaller.ConvertToNative(g);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(g, "g");
			}
			VisualEffect.SetGradient_Injected(intPtr, nameID, intPtr2);
		}

		[FreeFunction(Name = "VisualEffectBindings::SetValueFromScript<Mesh*>", HasExplicitThis = true)]
		public void SetMesh(int nameID, [NotNull] Mesh m)
		{
			if (m == null)
			{
				ThrowHelper.ThrowArgumentNullException(m, "m");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(m);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(m, "m");
			}
			VisualEffect.SetMesh_Injected(intPtr, nameID, intPtr2);
		}

		[FreeFunction(Name = "VisualEffectBindings::SetValueFromScript<SkinnedMeshRenderer*>", HasExplicitThis = true)]
		public void SetSkinnedMeshRenderer(int nameID, SkinnedMeshRenderer m)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.SetSkinnedMeshRenderer_Injected(intPtr, nameID, Object.MarshalledUnityObject.Marshal<SkinnedMeshRenderer>(m));
		}

		[FreeFunction(Name = "VisualEffectBindings::SetValueFromScript<GraphicsBuffer*>", HasExplicitThis = true)]
		public void SetGraphicsBuffer(int nameID, GraphicsBuffer g)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.SetGraphicsBuffer_Injected(intPtr, nameID, (g == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(g));
		}

		[FreeFunction(Name = "VisualEffectBindings::GetValueFromScript<bool>", HasExplicitThis = true)]
		public bool GetBool(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.GetBool_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::GetValueFromScript<int>", HasExplicitThis = true)]
		public int GetInt(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.GetInt_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::GetValueFromScript<UInt32>", HasExplicitThis = true)]
		public uint GetUInt(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.GetUInt_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::GetValueFromScript<float>", HasExplicitThis = true)]
		public float GetFloat(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.GetFloat_Injected(intPtr, nameID);
		}

		[FreeFunction(Name = "VisualEffectBindings::GetValueFromScript<Vector2f>", HasExplicitThis = true)]
		public Vector2 GetVector2(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector;
			VisualEffect.GetVector2_Injected(intPtr, nameID, out vector);
			return vector;
		}

		[FreeFunction(Name = "VisualEffectBindings::GetValueFromScript<Vector3f>", HasExplicitThis = true)]
		public Vector3 GetVector3(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			VisualEffect.GetVector3_Injected(intPtr, nameID, out vector);
			return vector;
		}

		[FreeFunction(Name = "VisualEffectBindings::GetValueFromScript<Vector4f>", HasExplicitThis = true)]
		public Vector4 GetVector4(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector4 vector;
			VisualEffect.GetVector4_Injected(intPtr, nameID, out vector);
			return vector;
		}

		[FreeFunction(Name = "VisualEffectBindings::GetValueFromScript<Matrix4x4f>", HasExplicitThis = true)]
		public Matrix4x4 GetMatrix4x4(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Matrix4x4 matrix4x;
			VisualEffect.GetMatrix4x4_Injected(intPtr, nameID, out matrix4x);
			return matrix4x;
		}

		[FreeFunction(Name = "VisualEffectBindings::GetValueFromScript<Texture*>", HasExplicitThis = true)]
		public Texture GetTexture(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture>(VisualEffect.GetTexture_Injected(intPtr, nameID));
		}

		[FreeFunction(Name = "VisualEffectBindings::GetValueFromScript<Mesh*>", HasExplicitThis = true)]
		public Mesh GetMesh(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Mesh>(VisualEffect.GetMesh_Injected(intPtr, nameID));
		}

		[FreeFunction(Name = "VisualEffectBindings::GetValueFromScript<SkinnedMeshRenderer*>", HasExplicitThis = true)]
		public SkinnedMeshRenderer GetSkinnedMeshRenderer(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<SkinnedMeshRenderer>(VisualEffect.GetSkinnedMeshRenderer_Injected(intPtr, nameID));
		}

		[FreeFunction(Name = "VisualEffectBindings::GetValueFromScript<GraphicsBuffer*>", HasExplicitThis = true)]
		internal GraphicsBuffer GetGraphicsBuffer(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr graphicsBuffer_Injected = VisualEffect.GetGraphicsBuffer_Injected(intPtr, nameID);
			return (graphicsBuffer_Injected == 0) ? null : GraphicsBuffer.BindingsMarshaller.ConvertToManaged(graphicsBuffer_Injected);
		}

		public Gradient GetGradient(int nameID)
		{
			Gradient gradient = new Gradient();
			this.Internal_GetGradient(nameID, gradient);
			return gradient;
		}

		[FreeFunction(Name = "VisualEffectBindings::Internal_GetGradientFromScript", HasExplicitThis = true)]
		private void Internal_GetGradient(int nameID, Gradient gradient)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.Internal_GetGradient_Injected(intPtr, nameID, (gradient == null) ? ((IntPtr)0) : Gradient.BindingsMarshaller.ConvertToNative(gradient));
		}

		public AnimationCurve GetAnimationCurve(int nameID)
		{
			AnimationCurve animationCurve = new AnimationCurve();
			this.Internal_GetAnimationCurve(nameID, animationCurve);
			return animationCurve;
		}

		[FreeFunction(Name = "VisualEffectBindings::Internal_GetAnimationCurveFromScript", HasExplicitThis = true)]
		private void Internal_GetAnimationCurve(int nameID, AnimationCurve curve)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.Internal_GetAnimationCurve_Injected(intPtr, nameID, (curve == null) ? ((IntPtr)0) : AnimationCurve.BindingsMarshaller.ConvertToNative(curve));
		}

		[FreeFunction(Name = "VisualEffectBindings::GetParticleSystemInfo", HasExplicitThis = true, ThrowsException = true)]
		public VFXParticleSystemInfo GetParticleSystemInfo(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VFXParticleSystemInfo vfxparticleSystemInfo;
			VisualEffect.GetParticleSystemInfo_Injected(intPtr, nameID, out vfxparticleSystemInfo);
			return vfxparticleSystemInfo;
		}

		[FreeFunction(Name = "VisualEffectBindings::GetSpawnSystemInfo", HasExplicitThis = true, ThrowsException = true)]
		private void GetSpawnSystemInfo(int nameID, IntPtr spawnerState)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.GetSpawnSystemInfo_Injected(intPtr, nameID, spawnerState);
		}

		public bool HasAnySystemAwake()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualEffect.HasAnySystemAwake_Injected(intPtr);
		}

		[FreeFunction(Name = "VisualEffectBindings::GetComputedBounds", HasExplicitThis = true)]
		internal Bounds GetComputedBounds(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Bounds bounds;
			VisualEffect.GetComputedBounds_Injected(intPtr, nameID, out bounds);
			return bounds;
		}

		[FreeFunction(Name = "VisualEffectBindings::GetCurrentBoundsPadding", HasExplicitThis = true)]
		internal Vector3 GetCurrentBoundsPadding(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			VisualEffect.GetCurrentBoundsPadding_Injected(intPtr, nameID, out vector);
			return vector;
		}

		public void GetSpawnSystemInfo(int nameID, VFXSpawnerState spawnState)
		{
			bool flag = spawnState == null;
			if (flag)
			{
				throw new NullReferenceException("GetSpawnSystemInfo expects a non null VFXSpawnerState.");
			}
			IntPtr ptr = spawnState.GetPtr();
			bool flag2 = ptr == IntPtr.Zero;
			if (flag2)
			{
				throw new NullReferenceException("GetSpawnSystemInfo use an unexpected not owned VFXSpawnerState.");
			}
			this.GetSpawnSystemInfo(nameID, ptr);
		}

		public VFXSpawnerState GetSpawnSystemInfo(int nameID)
		{
			VFXSpawnerState vfxspawnerState = new VFXSpawnerState();
			this.GetSpawnSystemInfo(nameID, vfxspawnerState);
			return vfxspawnerState;
		}

		public bool HasSystem(int nameID)
		{
			VisualEffectAsset visualEffectAsset = this.visualEffectAsset;
			return visualEffectAsset != null && visualEffectAsset.HasSystem(nameID);
		}

		public void GetSystemNames(List<string> names)
		{
			bool flag = names == null;
			if (flag)
			{
				throw new ArgumentNullException("names");
			}
			VisualEffectAsset visualEffectAsset = this.visualEffectAsset;
			bool flag2 = visualEffectAsset;
			if (flag2)
			{
				visualEffectAsset.GetSystemNames(names);
			}
			else
			{
				names.Clear();
			}
		}

		public void GetParticleSystemNames(List<string> names)
		{
			bool flag = names == null;
			if (flag)
			{
				throw new ArgumentNullException("names");
			}
			VisualEffectAsset visualEffectAsset = this.visualEffectAsset;
			bool flag2 = visualEffectAsset;
			if (flag2)
			{
				visualEffectAsset.GetParticleSystemNames(names);
			}
			else
			{
				names.Clear();
			}
		}

		public void GetOutputEventNames(List<string> names)
		{
			bool flag = names == null;
			if (flag)
			{
				throw new ArgumentNullException("names");
			}
			VisualEffectAsset visualEffectAsset = this.visualEffectAsset;
			bool flag2 = visualEffectAsset;
			if (flag2)
			{
				visualEffectAsset.GetOutputEventNames(names);
			}
			else
			{
				names.Clear();
			}
		}

		public void GetSpawnSystemNames(List<string> names)
		{
			bool flag = names == null;
			if (flag)
			{
				throw new ArgumentNullException("names");
			}
			VisualEffectAsset visualEffectAsset = this.visualEffectAsset;
			bool flag2 = visualEffectAsset;
			if (flag2)
			{
				visualEffectAsset.GetSpawnSystemNames(names);
			}
			else
			{
				names.Clear();
			}
		}

		public void ResetOverride(string name)
		{
			this.ResetOverride(Shader.PropertyToID(name));
		}

		public bool HasInt(string name)
		{
			return this.HasInt(Shader.PropertyToID(name));
		}

		public bool HasUInt(string name)
		{
			return this.HasUInt(Shader.PropertyToID(name));
		}

		public bool HasFloat(string name)
		{
			return this.HasFloat(Shader.PropertyToID(name));
		}

		public bool HasVector2(string name)
		{
			return this.HasVector2(Shader.PropertyToID(name));
		}

		public bool HasVector3(string name)
		{
			return this.HasVector3(Shader.PropertyToID(name));
		}

		public bool HasVector4(string name)
		{
			return this.HasVector4(Shader.PropertyToID(name));
		}

		public bool HasMatrix4x4(string name)
		{
			return this.HasMatrix4x4(Shader.PropertyToID(name));
		}

		public bool HasTexture(string name)
		{
			return this.HasTexture(Shader.PropertyToID(name));
		}

		public TextureDimension GetTextureDimension(string name)
		{
			return this.GetTextureDimension(Shader.PropertyToID(name));
		}

		public bool HasAnimationCurve(string name)
		{
			return this.HasAnimationCurve(Shader.PropertyToID(name));
		}

		public bool HasGradient(string name)
		{
			return this.HasGradient(Shader.PropertyToID(name));
		}

		public bool HasMesh(string name)
		{
			return this.HasMesh(Shader.PropertyToID(name));
		}

		public bool HasSkinnedMeshRenderer(string name)
		{
			return this.HasSkinnedMeshRenderer(Shader.PropertyToID(name));
		}

		public bool HasGraphicsBuffer(string name)
		{
			return this.HasGraphicsBuffer(Shader.PropertyToID(name));
		}

		public bool HasBool(string name)
		{
			return this.HasBool(Shader.PropertyToID(name));
		}

		public void SetInt(string name, int i)
		{
			this.SetInt(Shader.PropertyToID(name), i);
		}

		public void SetUInt(string name, uint i)
		{
			this.SetUInt(Shader.PropertyToID(name), i);
		}

		public void SetFloat(string name, float f)
		{
			this.SetFloat(Shader.PropertyToID(name), f);
		}

		public void SetVector2(string name, Vector2 v)
		{
			this.SetVector2(Shader.PropertyToID(name), v);
		}

		public void SetVector3(string name, Vector3 v)
		{
			this.SetVector3(Shader.PropertyToID(name), v);
		}

		public void SetVector4(string name, Vector4 v)
		{
			this.SetVector4(Shader.PropertyToID(name), v);
		}

		public void SetMatrix4x4(string name, Matrix4x4 v)
		{
			this.SetMatrix4x4(Shader.PropertyToID(name), v);
		}

		public void SetTexture(string name, Texture t)
		{
			this.SetTexture(Shader.PropertyToID(name), t);
		}

		public void SetAnimationCurve(string name, AnimationCurve c)
		{
			this.SetAnimationCurve(Shader.PropertyToID(name), c);
		}

		public void SetGradient(string name, Gradient g)
		{
			this.SetGradient(Shader.PropertyToID(name), g);
		}

		public void SetMesh(string name, Mesh m)
		{
			this.SetMesh(Shader.PropertyToID(name), m);
		}

		public void SetSkinnedMeshRenderer(string name, SkinnedMeshRenderer m)
		{
			this.SetSkinnedMeshRenderer(Shader.PropertyToID(name), m);
		}

		public void SetGraphicsBuffer(string name, GraphicsBuffer g)
		{
			this.SetGraphicsBuffer(Shader.PropertyToID(name), g);
		}

		public void SetBool(string name, bool b)
		{
			this.SetBool(Shader.PropertyToID(name), b);
		}

		public int GetInt(string name)
		{
			return this.GetInt(Shader.PropertyToID(name));
		}

		public uint GetUInt(string name)
		{
			return this.GetUInt(Shader.PropertyToID(name));
		}

		public float GetFloat(string name)
		{
			return this.GetFloat(Shader.PropertyToID(name));
		}

		public Vector2 GetVector2(string name)
		{
			return this.GetVector2(Shader.PropertyToID(name));
		}

		public Vector3 GetVector3(string name)
		{
			return this.GetVector3(Shader.PropertyToID(name));
		}

		public Vector4 GetVector4(string name)
		{
			return this.GetVector4(Shader.PropertyToID(name));
		}

		public Matrix4x4 GetMatrix4x4(string name)
		{
			return this.GetMatrix4x4(Shader.PropertyToID(name));
		}

		public Texture GetTexture(string name)
		{
			return this.GetTexture(Shader.PropertyToID(name));
		}

		public Mesh GetMesh(string name)
		{
			return this.GetMesh(Shader.PropertyToID(name));
		}

		public SkinnedMeshRenderer GetSkinnedMeshRenderer(string name)
		{
			return this.GetSkinnedMeshRenderer(Shader.PropertyToID(name));
		}

		internal GraphicsBuffer GetGraphicsBuffer(string name)
		{
			return this.GetGraphicsBuffer(Shader.PropertyToID(name));
		}

		public bool GetBool(string name)
		{
			return this.GetBool(Shader.PropertyToID(name));
		}

		public AnimationCurve GetAnimationCurve(string name)
		{
			return this.GetAnimationCurve(Shader.PropertyToID(name));
		}

		public Gradient GetGradient(string name)
		{
			return this.GetGradient(Shader.PropertyToID(name));
		}

		public bool HasSystem(string name)
		{
			return this.HasSystem(Shader.PropertyToID(name));
		}

		public VFXParticleSystemInfo GetParticleSystemInfo(string name)
		{
			return this.GetParticleSystemInfo(Shader.PropertyToID(name));
		}

		internal string GetGPUTaskMarkerName(string systemName, int taskIndex)
		{
			return this.GetGPUTaskMarkerName(Shader.PropertyToID(systemName), taskIndex);
		}

		internal string GetCPUSystemMarkerName(string systemName)
		{
			return this.GetCPUSystemMarkerName(Shader.PropertyToID(systemName));
		}

		internal string GetCPUEffectMarkerName(VisualEffect.VFXCPUEffectMarkers markerId)
		{
			return this.GetCPUEffectMarkerName((int)markerId);
		}

		public VFXSpawnerState GetSpawnSystemInfo(string name)
		{
			return this.GetSpawnSystemInfo(Shader.PropertyToID(name));
		}

		internal Bounds GetComputedBounds(string name)
		{
			return this.GetComputedBounds(Shader.PropertyToID(name));
		}

		internal Vector3 GetCurrentBoundsPadding(string name)
		{
			return this.GetCurrentBoundsPadding(Shader.PropertyToID(name));
		}

		public int aliveParticleCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VisualEffect.get_aliveParticleCount_Injected(intPtr);
			}
		}

		internal float time
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VisualEffect.get_time_Injected(intPtr);
			}
		}

		public void Simulate(float stepDeltaTime, uint stepCount = 1U)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffect>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualEffect.Simulate_Injected(intPtr, stepDeltaTime, stepCount);
		}

		[RequiredByNativeCode]
		private static VFXEventAttribute InvokeGetCachedEventAttributeForOutputEvent_Internal(VisualEffect source)
		{
			bool flag = source.outputEventReceived == null;
			VFXEventAttribute vfxeventAttribute;
			if (flag)
			{
				vfxeventAttribute = null;
			}
			else
			{
				bool flag2 = source.m_cachedEventAttribute == null;
				if (flag2)
				{
					source.m_cachedEventAttribute = source.CreateVFXEventAttribute();
				}
				vfxeventAttribute = source.m_cachedEventAttribute;
			}
			return vfxeventAttribute;
		}

		[RequiredByNativeCode]
		private static void InvokeOutputEventReceived_Internal(VisualEffect source, int eventNameId)
		{
			VFXOutputEventArgs e = new VFXOutputEventArgs(eventNameId, source.m_cachedEventAttribute);
			source.outputEventReceived(e);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_pause_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_pause_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_playRate_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_playRate_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_startSeed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startSeed_Injected(IntPtr _unity_self, uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_resetSeedOnPlay_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_resetSeedOnPlay_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_initialEventID_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_initialEventID_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_initialEventName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_initialEventName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_culled_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_visualEffectAsset_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_visualEffectAsset_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SendEventFromScript_Injected(IntPtr _unity_self, int eventNameID, IntPtr eventAttribute);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Reinit_Injected(IntPtr _unity_self, bool sendInitialEventAndPrewarm);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AdvanceOneFrame_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RecreateData_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGPUTaskMarkerName_Injected(IntPtr _unity_self, int nameID, int taskIndex, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCPUEffectMarkerName_Injected(IntPtr _unity_self, int markerIndex, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCPUSystemMarkerName_Injected(IntPtr _unity_self, int nameID, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RegisterForProfiling_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UnregisterForProfiling_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsRegisteredForProfiling_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetOverride_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureDimension GetTextureDimension_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasBool_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasInt_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasUInt_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasFloat_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasVector2_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasVector3_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasVector4_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasMatrix4x4_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasTexture_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasAnimationCurve_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasGradient_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasMesh_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasSkinnedMeshRenderer_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasGraphicsBuffer_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBool_Injected(IntPtr _unity_self, int nameID, bool b);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetInt_Injected(IntPtr _unity_self, int nameID, int i);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetUInt_Injected(IntPtr _unity_self, int nameID, uint i);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloat_Injected(IntPtr _unity_self, int nameID, float f);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVector2_Injected(IntPtr _unity_self, int nameID, [In] ref Vector2 v);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVector3_Injected(IntPtr _unity_self, int nameID, [In] ref Vector3 v);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVector4_Injected(IntPtr _unity_self, int nameID, [In] ref Vector4 v);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMatrix4x4_Injected(IntPtr _unity_self, int nameID, [In] ref Matrix4x4 v);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTexture_Injected(IntPtr _unity_self, int nameID, IntPtr t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAnimationCurve_Injected(IntPtr _unity_self, int nameID, IntPtr c);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGradient_Injected(IntPtr _unity_self, int nameID, IntPtr g);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMesh_Injected(IntPtr _unity_self, int nameID, IntPtr m);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSkinnedMeshRenderer_Injected(IntPtr _unity_self, int nameID, IntPtr m);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGraphicsBuffer_Injected(IntPtr _unity_self, int nameID, IntPtr g);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetBool_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetInt_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetUInt_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFloat_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVector2_Injected(IntPtr _unity_self, int nameID, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVector3_Injected(IntPtr _unity_self, int nameID, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVector4_Injected(IntPtr _unity_self, int nameID, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMatrix4x4_Injected(IntPtr _unity_self, int nameID, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetTexture_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetMesh_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetSkinnedMeshRenderer_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetGraphicsBuffer_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetGradient_Injected(IntPtr _unity_self, int nameID, IntPtr gradient);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetAnimationCurve_Injected(IntPtr _unity_self, int nameID, IntPtr curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetParticleSystemInfo_Injected(IntPtr _unity_self, int nameID, out VFXParticleSystemInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSpawnSystemInfo_Injected(IntPtr _unity_self, int nameID, IntPtr spawnerState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasAnySystemAwake_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetComputedBounds_Injected(IntPtr _unity_self, int nameID, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCurrentBoundsPadding_Injected(IntPtr _unity_self, int nameID, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_aliveParticleCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_time_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Simulate_Injected(IntPtr _unity_self, float stepDeltaTime, uint stepCount);

		private VFXEventAttribute m_cachedEventAttribute;

		public Action<VFXOutputEventArgs> outputEventReceived;

		internal enum VFXCPUEffectMarkers
		{
			FullUpdate,
			ProcessUpdate,
			EvaluateExpressions
		}
	}
}
