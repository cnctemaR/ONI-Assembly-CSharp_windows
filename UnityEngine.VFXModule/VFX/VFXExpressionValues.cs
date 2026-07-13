using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.VFX
{
	[NativeType(Header = "Modules/VFX/Public/VFXExpressionValues.h")]
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class VFXExpressionValues
	{
		private VFXExpressionValues()
		{
		}

		[RequiredByNativeCode]
		internal static VFXExpressionValues CreateExpressionValuesWrapper(IntPtr ptr)
		{
			return new VFXExpressionValues
			{
				m_Ptr = ptr
			};
		}

		[NativeName("GetValueFromScript<bool>")]
		[NativeThrows]
		public bool GetBool(int nameID)
		{
			IntPtr intPtr = VFXExpressionValues.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VFXExpressionValues.GetBool_Injected(intPtr, nameID);
		}

		[NativeName("GetValueFromScript<int>")]
		[NativeThrows]
		public int GetInt(int nameID)
		{
			IntPtr intPtr = VFXExpressionValues.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VFXExpressionValues.GetInt_Injected(intPtr, nameID);
		}

		[NativeThrows]
		[NativeName("GetValueFromScript<UInt32>")]
		public uint GetUInt(int nameID)
		{
			IntPtr intPtr = VFXExpressionValues.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VFXExpressionValues.GetUInt_Injected(intPtr, nameID);
		}

		[NativeThrows]
		[NativeName("GetValueFromScript<float>")]
		public float GetFloat(int nameID)
		{
			IntPtr intPtr = VFXExpressionValues.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VFXExpressionValues.GetFloat_Injected(intPtr, nameID);
		}

		[NativeThrows]
		[NativeName("GetValueFromScript<Vector2f>")]
		public Vector2 GetVector2(int nameID)
		{
			IntPtr intPtr = VFXExpressionValues.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector;
			VFXExpressionValues.GetVector2_Injected(intPtr, nameID, out vector);
			return vector;
		}

		[NativeThrows]
		[NativeName("GetValueFromScript<Vector3f>")]
		public Vector3 GetVector3(int nameID)
		{
			IntPtr intPtr = VFXExpressionValues.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			VFXExpressionValues.GetVector3_Injected(intPtr, nameID, out vector);
			return vector;
		}

		[NativeThrows]
		[NativeName("GetValueFromScript<Vector4f>")]
		public Vector4 GetVector4(int nameID)
		{
			IntPtr intPtr = VFXExpressionValues.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector4 vector;
			VFXExpressionValues.GetVector4_Injected(intPtr, nameID, out vector);
			return vector;
		}

		[NativeThrows]
		[NativeName("GetValueFromScript<Matrix4x4f>")]
		public Matrix4x4 GetMatrix4x4(int nameID)
		{
			IntPtr intPtr = VFXExpressionValues.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Matrix4x4 matrix4x;
			VFXExpressionValues.GetMatrix4x4_Injected(intPtr, nameID, out matrix4x);
			return matrix4x;
		}

		[NativeName("GetValueFromScript<Texture*>")]
		[NativeThrows]
		public Texture GetTexture(int nameID)
		{
			IntPtr intPtr = VFXExpressionValues.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture>(VFXExpressionValues.GetTexture_Injected(intPtr, nameID));
		}

		[NativeName("GetValueFromScript<Mesh*>")]
		[NativeThrows]
		public Mesh GetMesh(int nameID)
		{
			IntPtr intPtr = VFXExpressionValues.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Mesh>(VFXExpressionValues.GetMesh_Injected(intPtr, nameID));
		}

		public AnimationCurve GetAnimationCurve(int nameID)
		{
			AnimationCurve animationCurve = new AnimationCurve();
			this.Internal_GetAnimationCurveFromScript(nameID, animationCurve);
			return animationCurve;
		}

		[NativeThrows]
		internal void Internal_GetAnimationCurveFromScript(int nameID, AnimationCurve curve)
		{
			IntPtr intPtr = VFXExpressionValues.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VFXExpressionValues.Internal_GetAnimationCurveFromScript_Injected(intPtr, nameID, (curve == null) ? ((IntPtr)0) : AnimationCurve.BindingsMarshaller.ConvertToNative(curve));
		}

		public Gradient GetGradient(int nameID)
		{
			Gradient gradient = new Gradient();
			this.Internal_GetGradientFromScript(nameID, gradient);
			return gradient;
		}

		[NativeThrows]
		internal void Internal_GetGradientFromScript(int nameID, Gradient gradient)
		{
			IntPtr intPtr = VFXExpressionValues.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VFXExpressionValues.Internal_GetGradientFromScript_Injected(intPtr, nameID, (gradient == null) ? ((IntPtr)0) : Gradient.BindingsMarshaller.ConvertToNative(gradient));
		}

		public bool GetBool(string name)
		{
			return this.GetBool(Shader.PropertyToID(name));
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

		public AnimationCurve GetAnimationCurve(string name)
		{
			return this.GetAnimationCurve(Shader.PropertyToID(name));
		}

		public Gradient GetGradient(string name)
		{
			return this.GetGradient(Shader.PropertyToID(name));
		}

		public Mesh GetMesh(string name)
		{
			return this.GetMesh(Shader.PropertyToID(name));
		}

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
		private static extern void Internal_GetAnimationCurveFromScript_Injected(IntPtr _unity_self, int nameID, IntPtr curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetGradientFromScript_Injected(IntPtr _unity_self, int nameID, IntPtr gradient);

		internal IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(VFXExpressionValues vFXExpressionValues)
			{
				return vFXExpressionValues.m_Ptr;
			}
		}
	}
}
