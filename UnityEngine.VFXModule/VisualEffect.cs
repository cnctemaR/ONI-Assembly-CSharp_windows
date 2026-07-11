using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.VFX
{
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Modules/VFX/Public/VisualEffect.h")]
	public class VisualEffect : Behaviour
	{
		public extern bool pause
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float playRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern uint startSeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool resetSeedOnPlay
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool culled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern VisualEffectAsset visualEffectAsset
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public VFXEventAttribute CreateVFXEventAttribute()
		{
			VFXEventAttribute vfxeventAttribute;
			if (this.visualEffectAsset == null)
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Play(VFXEventAttribute eventAttribute);

		public void Play()
		{
			this.Play(null);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Stop(VFXEventAttribute eventAttribute);

		public void Stop()
		{
			this.Stop(null);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SendEvent(int eventNameID, VFXEventAttribute eventAttribute);

		public void SendEvent(string eventName)
		{
			this.SendEvent(Shader.PropertyToID(eventName), null);
		}

		public void SendEvent(string eventName, VFXEventAttribute eventAttribute)
		{
			this.SendEvent(Shader.PropertyToID(eventName), eventAttribute);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Reinit();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AdvanceOneFrame();

		[NativeName("ResetOverrideFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetOverride(int nameID);

		[NativeName("GetTextureDimensionFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern TextureDimension GetTextureDimension(int nameID);

		[NativeName("HasValueFromScript<bool>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasBool(int nameID);

		[NativeName("HasValueFromScript<int>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasInt(int nameID);

		[NativeName("HasValueFromScript<UInt32>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasUInt(int nameID);

		[NativeName("HasValueFromScript<float>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasFloat(int nameID);

		[NativeName("HasValueFromScript<Vector2f>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasVector2(int nameID);

		[NativeName("HasValueFromScript<Vector3f>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasVector3(int nameID);

		[NativeName("HasValueFromScript<Vector4f>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasVector4(int nameID);

		[NativeName("HasValueFromScript<Matrix4x4f>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasMatrix4x4(int nameID);

		[NativeName("HasValueFromScript<Texture*>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasTexture(int nameID);

		[NativeName("HasValueFromScript<AnimationCurve*>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasAnimationCurve(int nameID);

		[NativeName("HasValueFromScript<Gradient*>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasGradient(int nameID);

		[NativeName("HasValueFromScript<Mesh*>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasMesh(int nameID);

		[NativeName("SetValueFromScript<bool>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetBool(int nameID, bool b);

		[NativeName("SetValueFromScript<int>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetInt(int nameID, int i);

		[NativeName("SetValueFromScript<UInt32>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetUInt(int nameID, uint i);

		[NativeName("SetValueFromScript<float>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetFloat(int nameID, float f);

		[NativeName("SetValueFromScript<Vector2f>")]
		public void SetVector2(int nameID, Vector2 v)
		{
			this.SetVector2_Injected(nameID, ref v);
		}

		[NativeName("SetValueFromScript<Vector3f>")]
		public void SetVector3(int nameID, Vector3 v)
		{
			this.SetVector3_Injected(nameID, ref v);
		}

		[NativeName("SetValueFromScript<Vector4f>")]
		public void SetVector4(int nameID, Vector4 v)
		{
			this.SetVector4_Injected(nameID, ref v);
		}

		[NativeName("SetValueFromScript<Matrix4x4f>")]
		public void SetMatrix4x4(int nameID, Matrix4x4 v)
		{
			this.SetMatrix4x4_Injected(nameID, ref v);
		}

		[NativeName("SetValueFromScript<Texture*>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTexture(int nameID, Texture t);

		[NativeName("SetValueFromScript<AnimationCurve*>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetAnimationCurve(int nameID, AnimationCurve c);

		[NativeName("SetValueFromScript<Gradient*>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGradient(int nameID, Gradient g);

		[NativeName("SetValueFromScript<Mesh*>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetMesh(int nameID, Mesh m);

		[NativeName("GetValueFromScript<bool>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetBool(int nameID);

		[NativeName("GetValueFromScript<int>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetInt(int nameID);

		[NativeName("GetValueFromScript<UInt32>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern uint GetUInt(int nameID);

		[NativeName("GetValueFromScript<float>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetFloat(int nameID);

		[NativeName("GetValueFromScript<Vector2f>")]
		public Vector2 GetVector2(int nameID)
		{
			Vector2 vector;
			this.GetVector2_Injected(nameID, out vector);
			return vector;
		}

		[NativeName("GetValueFromScript<Vector3f>")]
		public Vector3 GetVector3(int nameID)
		{
			Vector3 vector;
			this.GetVector3_Injected(nameID, out vector);
			return vector;
		}

		[NativeName("GetValueFromScript<Vector4f>")]
		public Vector4 GetVector4(int nameID)
		{
			Vector4 vector;
			this.GetVector4_Injected(nameID, out vector);
			return vector;
		}

		[NativeName("GetValueFromScript<Matrix4x4f>")]
		public Matrix4x4 GetMatrix4x4(int nameID)
		{
			Matrix4x4 matrix4x;
			this.GetMatrix4x4_Injected(nameID, out matrix4x);
			return matrix4x;
		}

		[NativeName("GetValueFromScript<Texture*>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Texture GetTexture(int nameID);

		[NativeName("GetValueFromScript<Mesh*>")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Mesh GetMesh(int nameID);

		public Gradient GetGradient(int nameID)
		{
			Gradient gradient = new Gradient();
			this.Internal_GetGradient(nameID, gradient);
			return gradient;
		}

		[NativeName("Internal_GetGradientFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetGradient(int nameID, Gradient gradient);

		public AnimationCurve GetAnimationCurve(int nameID)
		{
			AnimationCurve animationCurve = new AnimationCurve();
			this.Internal_GetAnimationCurve(nameID, animationCurve);
			return animationCurve;
		}

		[NativeName("Internal_GetAnimationCurveFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetAnimationCurve(int nameID, AnimationCurve curve);

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

		public extern int aliveParticleCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetVector2_Injected(int nameID, ref Vector2 v);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetVector3_Injected(int nameID, ref Vector3 v);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetVector4_Injected(int nameID, ref Vector4 v);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetMatrix4x4_Injected(int nameID, ref Matrix4x4 v);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetVector2_Injected(int nameID, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetVector3_Injected(int nameID, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetVector4_Injected(int nameID, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetMatrix4x4_Injected(int nameID, out Matrix4x4 ret);
	}
}
