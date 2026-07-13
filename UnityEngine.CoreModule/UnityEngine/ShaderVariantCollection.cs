using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine
{
	public sealed class ShaderVariantCollection : Object
	{
		public int shaderCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ShaderVariantCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ShaderVariantCollection.get_shaderCount_Injected(intPtr);
			}
		}

		public int variantCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ShaderVariantCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ShaderVariantCollection.get_variantCount_Injected(intPtr);
			}
		}

		public int warmedUpVariantCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ShaderVariantCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ShaderVariantCollection.get_warmedUpVariantCount_Injected(intPtr);
			}
		}

		public bool isWarmedUp
		{
			[NativeName("IsWarmedUp")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ShaderVariantCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ShaderVariantCollection.get_isWarmedUp_Injected(intPtr);
			}
		}

		private bool AddVariant(Shader shader, PassType passType, [UnityMarshalAs(NativeType.ScriptingObjectPtr)] string[] keywords)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ShaderVariantCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ShaderVariantCollection.AddVariant_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(shader), passType, keywords);
		}

		private bool RemoveVariant(Shader shader, PassType passType, [UnityMarshalAs(NativeType.ScriptingObjectPtr)] string[] keywords)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ShaderVariantCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ShaderVariantCollection.RemoveVariant_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(shader), passType, keywords);
		}

		private bool ContainsVariant(Shader shader, PassType passType, [UnityMarshalAs(NativeType.ScriptingObjectPtr)] string[] keywords)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ShaderVariantCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ShaderVariantCollection.ContainsVariant_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(shader), passType, keywords);
		}

		[NativeName("ClearVariants")]
		public void Clear()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ShaderVariantCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ShaderVariantCollection.Clear_Injected(intPtr);
		}

		[NativeName("WarmupShaders")]
		public void WarmUp()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ShaderVariantCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ShaderVariantCollection.WarmUp_Injected(intPtr);
		}

		[NativeName("WarmupShadersProgressively")]
		public bool WarmUpProgressively(int variantCount)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ShaderVariantCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ShaderVariantCollection.WarmUpProgressively_Injected(intPtr, variantCount);
		}

		[NativeName("CreateFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] ShaderVariantCollection svc);

		public ShaderVariantCollection()
		{
			ShaderVariantCollection.Internal_Create(this);
		}

		public bool Add(ShaderVariantCollection.ShaderVariant variant)
		{
			return this.AddVariant(variant.shader, variant.passType, variant.keywords);
		}

		public bool Remove(ShaderVariantCollection.ShaderVariant variant)
		{
			return this.RemoveVariant(variant.shader, variant.passType, variant.keywords);
		}

		public bool Contains(ShaderVariantCollection.ShaderVariant variant)
		{
			return this.ContainsVariant(variant.shader, variant.passType, variant.keywords);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_shaderCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_variantCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_warmedUpVariantCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isWarmedUp_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddVariant_Injected(IntPtr _unity_self, IntPtr shader, PassType passType, string[] keywords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemoveVariant_Injected(IntPtr _unity_self, IntPtr shader, PassType passType, string[] keywords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ContainsVariant_Injected(IntPtr _unity_self, IntPtr shader, PassType passType, string[] keywords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Clear_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WarmUp_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool WarmUpProgressively_Injected(IntPtr _unity_self, int variantCount);

		public struct ShaderVariant
		{
			[FreeFunction]
			[NativeConditional("UNITY_EDITOR")]
			private static string CheckShaderVariant(Shader shader, PassType passType, string[] keywords)
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					ShaderVariantCollection.ShaderVariant.CheckShaderVariant_Injected(Object.MarshalledUnityObject.Marshal<Shader>(shader), passType, keywords, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}

			public ShaderVariant(Shader shader, PassType passType, params string[] keywords)
			{
				this.shader = shader;
				this.passType = passType;
				this.keywords = keywords;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void CheckShaderVariant_Injected(IntPtr shader, PassType passType, string[] keywords, out ManagedSpanWrapper ret);

			public Shader shader;

			public PassType passType;

			public string[] keywords;
		}
	}
}
