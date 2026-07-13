using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.VFX
{
	[NativeType(Header = "Modules/VFX/Public/VFXEventAttribute.h")]
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class VFXEventAttribute : IDisposable
	{
		private VFXEventAttribute(IntPtr ptr, bool owner, VisualEffectAsset vfxAsset)
		{
			this.m_Ptr = ptr;
			this.m_Owner = owner;
			this.m_VfxAsset = vfxAsset;
		}

		private VFXEventAttribute(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

		private VFXEventAttribute()
			: this(IntPtr.Zero, false, null)
		{
		}

		internal static VFXEventAttribute CreateEventAttributeWrapper()
		{
			return new VFXEventAttribute(IntPtr.Zero, false, null);
		}

		internal void SetWrapValue(IntPtr ptrToEventAttribute)
		{
			bool owner = this.m_Owner;
			if (owner)
			{
				throw new Exception("VFXSpawnerState : SetWrapValue is reserved to CreateWrapper object");
			}
			this.m_Ptr = ptrToEventAttribute;
		}

		public VFXEventAttribute(VFXEventAttribute original)
		{
			bool flag = original == null;
			if (flag)
			{
				throw new ArgumentNullException("VFXEventAttribute expect a non null attribute");
			}
			this.m_Ptr = VFXEventAttribute.Internal_Create();
			this.m_VfxAsset = original.m_VfxAsset;
			this.Internal_InitFromEventAttribute(original);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr Internal_Create();

		internal static VFXEventAttribute Internal_InstanciateVFXEventAttribute(VisualEffectAsset vfxAsset)
		{
			VFXEventAttribute vfxeventAttribute = new VFXEventAttribute(VFXEventAttribute.Internal_Create(), true, vfxAsset);
			vfxeventAttribute.Internal_InitFromAsset(vfxAsset);
			return vfxeventAttribute;
		}

		internal void Internal_InitFromAsset(VisualEffectAsset vfxAsset)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VFXEventAttribute.Internal_InitFromAsset_Injected(intPtr, Object.MarshalledUnityObject.Marshal<VisualEffectAsset>(vfxAsset));
		}

		internal void Internal_InitFromEventAttribute(VFXEventAttribute vfxEventAttribute)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VFXEventAttribute.Internal_InitFromEventAttribute_Injected(intPtr, (vfxEventAttribute == null) ? ((IntPtr)0) : VFXEventAttribute.BindingsMarshaller.ConvertToNative(vfxEventAttribute));
		}

		internal VisualEffectAsset vfxAsset
		{
			get
			{
				return this.m_VfxAsset;
			}
		}

		private void Release()
		{
			bool flag = this.m_Owner && this.m_Ptr != IntPtr.Zero;
			if (flag)
			{
				VFXEventAttribute.Internal_Destroy(this.m_Ptr);
			}
			this.m_Ptr = IntPtr.Zero;
			this.m_VfxAsset = null;
		}

		~VFXEventAttribute()
		{
			this.Release();
		}

		public void Dispose()
		{
			this.Release();
			GC.SuppressFinalize(this);
		}

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_Destroy(IntPtr ptr);

		[NativeName("HasValueFromScript<bool>")]
		public bool HasBool(int nameID)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VFXEventAttribute.HasBool_Injected(intPtr, nameID);
		}

		[NativeName("HasValueFromScript<int>")]
		public bool HasInt(int nameID)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VFXEventAttribute.HasInt_Injected(intPtr, nameID);
		}

		[NativeName("HasValueFromScript<UInt32>")]
		public bool HasUint(int nameID)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VFXEventAttribute.HasUint_Injected(intPtr, nameID);
		}

		[NativeName("HasValueFromScript<float>")]
		public bool HasFloat(int nameID)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VFXEventAttribute.HasFloat_Injected(intPtr, nameID);
		}

		[NativeName("HasValueFromScript<Vector2f>")]
		public bool HasVector2(int nameID)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VFXEventAttribute.HasVector2_Injected(intPtr, nameID);
		}

		[NativeName("HasValueFromScript<Vector3f>")]
		public bool HasVector3(int nameID)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VFXEventAttribute.HasVector3_Injected(intPtr, nameID);
		}

		[NativeName("HasValueFromScript<Vector4f>")]
		public bool HasVector4(int nameID)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VFXEventAttribute.HasVector4_Injected(intPtr, nameID);
		}

		[NativeName("HasValueFromScript<Matrix4x4f>")]
		public bool HasMatrix4x4(int nameID)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VFXEventAttribute.HasMatrix4x4_Injected(intPtr, nameID);
		}

		[NativeName("SetValueFromScript<bool>")]
		public void SetBool(int nameID, bool b)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VFXEventAttribute.SetBool_Injected(intPtr, nameID, b);
		}

		[NativeName("SetValueFromScript<int>")]
		public void SetInt(int nameID, int i)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VFXEventAttribute.SetInt_Injected(intPtr, nameID, i);
		}

		[NativeName("SetValueFromScript<UInt32>")]
		public void SetUint(int nameID, uint i)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VFXEventAttribute.SetUint_Injected(intPtr, nameID, i);
		}

		[NativeName("SetValueFromScript<float>")]
		public void SetFloat(int nameID, float f)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VFXEventAttribute.SetFloat_Injected(intPtr, nameID, f);
		}

		[NativeName("SetValueFromScript<Vector2f>")]
		public void SetVector2(int nameID, Vector2 v)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VFXEventAttribute.SetVector2_Injected(intPtr, nameID, ref v);
		}

		[NativeName("SetValueFromScript<Vector3f>")]
		public void SetVector3(int nameID, Vector3 v)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VFXEventAttribute.SetVector3_Injected(intPtr, nameID, ref v);
		}

		[NativeName("SetValueFromScript<Vector4f>")]
		public void SetVector4(int nameID, Vector4 v)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VFXEventAttribute.SetVector4_Injected(intPtr, nameID, ref v);
		}

		[NativeName("SetValueFromScript<Matrix4x4f>")]
		public void SetMatrix4x4(int nameID, Matrix4x4 v)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VFXEventAttribute.SetMatrix4x4_Injected(intPtr, nameID, ref v);
		}

		[NativeName("GetValueFromScript<bool>")]
		public bool GetBool(int nameID)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VFXEventAttribute.GetBool_Injected(intPtr, nameID);
		}

		[NativeName("GetValueFromScript<int>")]
		public int GetInt(int nameID)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VFXEventAttribute.GetInt_Injected(intPtr, nameID);
		}

		[NativeName("GetValueFromScript<UInt32>")]
		public uint GetUint(int nameID)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VFXEventAttribute.GetUint_Injected(intPtr, nameID);
		}

		[NativeName("GetValueFromScript<float>")]
		public float GetFloat(int nameID)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VFXEventAttribute.GetFloat_Injected(intPtr, nameID);
		}

		[NativeName("GetValueFromScript<Vector2f>")]
		public Vector2 GetVector2(int nameID)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector;
			VFXEventAttribute.GetVector2_Injected(intPtr, nameID, out vector);
			return vector;
		}

		[NativeName("GetValueFromScript<Vector3f>")]
		public Vector3 GetVector3(int nameID)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			VFXEventAttribute.GetVector3_Injected(intPtr, nameID, out vector);
			return vector;
		}

		[NativeName("GetValueFromScript<Vector4f>")]
		public Vector4 GetVector4(int nameID)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector4 vector;
			VFXEventAttribute.GetVector4_Injected(intPtr, nameID, out vector);
			return vector;
		}

		[NativeName("GetValueFromScript<Matrix4x4f>")]
		public Matrix4x4 GetMatrix4x4(int nameID)
		{
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Matrix4x4 matrix4x;
			VFXEventAttribute.GetMatrix4x4_Injected(intPtr, nameID, out matrix4x);
			return matrix4x;
		}

		public bool HasBool(string name)
		{
			return this.HasBool(Shader.PropertyToID(name));
		}

		public bool HasInt(string name)
		{
			return this.HasInt(Shader.PropertyToID(name));
		}

		public bool HasUint(string name)
		{
			return this.HasUint(Shader.PropertyToID(name));
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

		public void SetBool(string name, bool b)
		{
			this.SetBool(Shader.PropertyToID(name), b);
		}

		public void SetInt(string name, int i)
		{
			this.SetInt(Shader.PropertyToID(name), i);
		}

		public void SetUint(string name, uint i)
		{
			this.SetUint(Shader.PropertyToID(name), i);
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

		public bool GetBool(string name)
		{
			return this.GetBool(Shader.PropertyToID(name));
		}

		public int GetInt(string name)
		{
			return this.GetInt(Shader.PropertyToID(name));
		}

		public uint GetUint(string name)
		{
			return this.GetUint(Shader.PropertyToID(name));
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

		public void CopyValuesFrom([NotNull] VFXEventAttribute eventAttibute)
		{
			if (eventAttibute == null)
			{
				ThrowHelper.ThrowArgumentNullException(eventAttibute, "eventAttibute");
			}
			IntPtr intPtr = VFXEventAttribute.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = VFXEventAttribute.BindingsMarshaller.ConvertToNative(eventAttibute);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(eventAttibute, "eventAttibute");
			}
			VFXEventAttribute.CopyValuesFrom_Injected(intPtr, intPtr2);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_InitFromAsset_Injected(IntPtr _unity_self, IntPtr vfxAsset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_InitFromEventAttribute_Injected(IntPtr _unity_self, IntPtr vfxEventAttribute);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasBool_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasInt_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasUint_Injected(IntPtr _unity_self, int nameID);

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
		private static extern void SetBool_Injected(IntPtr _unity_self, int nameID, bool b);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetInt_Injected(IntPtr _unity_self, int nameID, int i);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetUint_Injected(IntPtr _unity_self, int nameID, uint i);

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
		private static extern bool GetBool_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetInt_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetUint_Injected(IntPtr _unity_self, int nameID);

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
		private static extern void CopyValuesFrom_Injected(IntPtr _unity_self, IntPtr eventAttibute);

		private IntPtr m_Ptr;

		private bool m_Owner;

		private VisualEffectAsset m_VfxAsset;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(VFXEventAttribute eventAttibute)
			{
				return eventAttibute.m_Ptr;
			}

			public static VFXEventAttribute ConvertToManaged(IntPtr ptr)
			{
				return new VFXEventAttribute(ptr);
			}
		}
	}
}
