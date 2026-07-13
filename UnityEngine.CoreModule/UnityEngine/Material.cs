using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
	[NativeHeader("Runtime/Shaders/Material.h")]
	public class Material : Object
	{
		[Obsolete("Creating materials from shader source string will be removed in the future. Use Shader assets instead.", true)]
		public static Material Create(string scriptContents)
		{
			return new Material(scriptContents);
		}

		[FreeFunction("MaterialScripting::CreateWithShader")]
		private static void CreateWithShader([Writable] Material self, [NotNull] Shader shader)
		{
			if (shader == null)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Shader>(shader);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			Material.CreateWithShader_Injected(self, intPtr);
		}

		[FreeFunction("MaterialScripting::CreateWithMaterial")]
		private static void CreateWithMaterial([Writable] Material self, [NotNull] Material source)
		{
			if (source == null)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(source);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			Material.CreateWithMaterial_Injected(self, intPtr);
		}

		public Material(Shader shader)
		{
			Material.CreateWithShader(this, shader);
		}

		[RequiredByNativeCode]
		public Material(Material source)
		{
			Material.CreateWithMaterial(this, source);
		}

		[Obsolete("Creating materials from shader source string is no longer supported. Use Shader assets instead.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Material(string contents)
		{
		}

		internal static Material GetDefaultMaterial()
		{
			return Unmarshal.UnmarshalUnityObject<Material>(Material.GetDefaultMaterial_Injected());
		}

		internal static Material GetDefaultParticleMaterial()
		{
			return Unmarshal.UnmarshalUnityObject<Material>(Material.GetDefaultParticleMaterial_Injected());
		}

		internal static Material GetDefaultLineMaterial()
		{
			return Unmarshal.UnmarshalUnityObject<Material>(Material.GetDefaultLineMaterial_Injected());
		}

		public Shader shader
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Shader>(Material.get_shader_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Material.set_shader_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(value));
			}
		}

		public Color color
		{
			get
			{
				int firstPropertyNameIdByAttribute = this.GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags.MainColor);
				bool flag = firstPropertyNameIdByAttribute >= 0;
				Color color;
				if (flag)
				{
					color = this.GetColor(firstPropertyNameIdByAttribute);
				}
				else
				{
					color = this.GetColor(Material.k_ColorId);
				}
				return color;
			}
			set
			{
				int firstPropertyNameIdByAttribute = this.GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags.MainColor);
				bool flag = firstPropertyNameIdByAttribute >= 0;
				if (flag)
				{
					this.SetColor(firstPropertyNameIdByAttribute, value);
				}
				else
				{
					this.SetColor(Material.k_ColorId, value);
				}
			}
		}

		public Texture mainTexture
		{
			get
			{
				int firstPropertyNameIdByAttribute = this.GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags.MainTexture);
				bool flag = firstPropertyNameIdByAttribute >= 0;
				Texture texture;
				if (flag)
				{
					texture = this.GetTexture(firstPropertyNameIdByAttribute);
				}
				else
				{
					texture = this.GetTexture(Material.k_MainTexId);
				}
				return texture;
			}
			set
			{
				int firstPropertyNameIdByAttribute = this.GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags.MainTexture);
				bool flag = firstPropertyNameIdByAttribute >= 0;
				if (flag)
				{
					this.SetTexture(firstPropertyNameIdByAttribute, value);
				}
				else
				{
					this.SetTexture(Material.k_MainTexId, value);
				}
			}
		}

		public Vector2 mainTextureOffset
		{
			get
			{
				int firstPropertyNameIdByAttribute = this.GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags.MainTexture);
				bool flag = firstPropertyNameIdByAttribute >= 0;
				Vector2 vector;
				if (flag)
				{
					vector = this.GetTextureOffset(firstPropertyNameIdByAttribute);
				}
				else
				{
					vector = this.GetTextureOffset(Material.k_MainTexId);
				}
				return vector;
			}
			set
			{
				int firstPropertyNameIdByAttribute = this.GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags.MainTexture);
				bool flag = firstPropertyNameIdByAttribute >= 0;
				if (flag)
				{
					this.SetTextureOffset(firstPropertyNameIdByAttribute, value);
				}
				else
				{
					this.SetTextureOffset(Material.k_MainTexId, value);
				}
			}
		}

		public Vector2 mainTextureScale
		{
			get
			{
				int firstPropertyNameIdByAttribute = this.GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags.MainTexture);
				bool flag = firstPropertyNameIdByAttribute >= 0;
				Vector2 vector;
				if (flag)
				{
					vector = this.GetTextureScale(firstPropertyNameIdByAttribute);
				}
				else
				{
					vector = this.GetTextureScale(Material.k_MainTexId);
				}
				return vector;
			}
			set
			{
				int firstPropertyNameIdByAttribute = this.GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags.MainTexture);
				bool flag = firstPropertyNameIdByAttribute >= 0;
				if (flag)
				{
					this.SetTextureScale(firstPropertyNameIdByAttribute, value);
				}
				else
				{
					this.SetTextureScale(Material.k_MainTexId, value);
				}
			}
		}

		[NativeName("GetFirstPropertyNameIdByAttributeFromScript")]
		private int GetFirstPropertyNameIdByAttribute(ShaderPropertyFlags attributeFlag)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.GetFirstPropertyNameIdByAttribute_Injected(intPtr, attributeFlag);
		}

		[NativeName("HasPropertyFromScript")]
		public bool HasProperty(int nameID)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.HasProperty_Injected(intPtr, nameID);
		}

		public bool HasProperty(string name)
		{
			return this.HasProperty(Shader.PropertyToID(name));
		}

		[NativeName("HasFloatFromScript")]
		private bool HasFloatImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.HasFloatImpl_Injected(intPtr, name);
		}

		public bool HasFloat(string name)
		{
			return this.HasFloatImpl(Shader.PropertyToID(name));
		}

		public bool HasFloat(int nameID)
		{
			return this.HasFloatImpl(nameID);
		}

		public bool HasInt(string name)
		{
			return this.HasFloatImpl(Shader.PropertyToID(name));
		}

		public bool HasInt(int nameID)
		{
			return this.HasFloatImpl(nameID);
		}

		[NativeName("HasIntegerFromScript")]
		private bool HasIntImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.HasIntImpl_Injected(intPtr, name);
		}

		public bool HasInteger(string name)
		{
			return this.HasIntImpl(Shader.PropertyToID(name));
		}

		public bool HasInteger(int nameID)
		{
			return this.HasIntImpl(nameID);
		}

		[NativeName("HasTextureFromScript")]
		private bool HasTextureImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.HasTextureImpl_Injected(intPtr, name);
		}

		public bool HasTexture(string name)
		{
			return this.HasTextureImpl(Shader.PropertyToID(name));
		}

		public bool HasTexture(int nameID)
		{
			return this.HasTextureImpl(nameID);
		}

		[NativeName("HasMatrixFromScript")]
		private bool HasMatrixImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.HasMatrixImpl_Injected(intPtr, name);
		}

		public bool HasMatrix(string name)
		{
			return this.HasMatrixImpl(Shader.PropertyToID(name));
		}

		public bool HasMatrix(int nameID)
		{
			return this.HasMatrixImpl(nameID);
		}

		[NativeName("HasVectorFromScript")]
		private bool HasVectorImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.HasVectorImpl_Injected(intPtr, name);
		}

		public bool HasVector(string name)
		{
			return this.HasVectorImpl(Shader.PropertyToID(name));
		}

		public bool HasVector(int nameID)
		{
			return this.HasVectorImpl(nameID);
		}

		public bool HasColor(string name)
		{
			return this.HasVectorImpl(Shader.PropertyToID(name));
		}

		public bool HasColor(int nameID)
		{
			return this.HasVectorImpl(nameID);
		}

		[NativeName("HasBufferFromScript")]
		private bool HasBufferImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.HasBufferImpl_Injected(intPtr, name);
		}

		public bool HasBuffer(string name)
		{
			return this.HasBufferImpl(Shader.PropertyToID(name));
		}

		public bool HasBuffer(int nameID)
		{
			return this.HasBufferImpl(nameID);
		}

		[NativeName("HasConstantBufferFromScript")]
		private bool HasConstantBufferImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.HasConstantBufferImpl_Injected(intPtr, name);
		}

		public bool HasConstantBuffer(string name)
		{
			return this.HasConstantBufferImpl(Shader.PropertyToID(name));
		}

		public bool HasConstantBuffer(int nameID)
		{
			return this.HasConstantBufferImpl(nameID);
		}

		public int renderQueue
		{
			[NativeName("GetActualRenderQueue")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Material.get_renderQueue_Injected(intPtr);
			}
			[NativeName("SetCustomRenderQueue")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Material.set_renderQueue_Injected(intPtr, value);
			}
		}

		public int rawRenderQueue
		{
			[NativeName("GetCustomRenderQueue")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Material.get_rawRenderQueue_Injected(intPtr);
			}
		}

		public unsafe void EnableKeyword(string keyword)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = keyword.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Material.EnableKeyword_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public unsafe void DisableKeyword(string keyword)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = keyword.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Material.DisableKeyword_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public unsafe bool IsKeywordEnabled(string keyword)
		{
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = keyword.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = Material.IsKeywordEnabled_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[FreeFunction("MaterialScripting::EnableKeyword", HasExplicitThis = true)]
		private void EnableLocalKeyword(LocalKeyword keyword)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.EnableLocalKeyword_Injected(intPtr, ref keyword);
		}

		[FreeFunction("MaterialScripting::DisableKeyword", HasExplicitThis = true)]
		private void DisableLocalKeyword(LocalKeyword keyword)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.DisableLocalKeyword_Injected(intPtr, ref keyword);
		}

		[FreeFunction("MaterialScripting::SetKeyword", HasExplicitThis = true)]
		private void SetLocalKeyword(LocalKeyword keyword, bool value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.SetLocalKeyword_Injected(intPtr, ref keyword, value);
		}

		[FreeFunction("MaterialScripting::IsKeywordEnabled", HasExplicitThis = true)]
		private bool IsLocalKeywordEnabled(LocalKeyword keyword)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.IsLocalKeywordEnabled_Injected(intPtr, ref keyword);
		}

		public void EnableKeyword(in LocalKeyword keyword)
		{
			this.EnableLocalKeyword(keyword);
		}

		public void DisableKeyword(in LocalKeyword keyword)
		{
			this.DisableLocalKeyword(keyword);
		}

		public void SetKeyword(in LocalKeyword keyword, bool value)
		{
			this.SetLocalKeyword(keyword, value);
		}

		public bool IsKeywordEnabled(in LocalKeyword keyword)
		{
			return this.IsLocalKeywordEnabled(keyword);
		}

		[FreeFunction("MaterialScripting::GetEnabledKeywords", HasExplicitThis = true)]
		private LocalKeyword[] GetEnabledKeywords()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.GetEnabledKeywords_Injected(intPtr);
		}

		[FreeFunction("MaterialScripting::SetEnabledKeywords", HasExplicitThis = true)]
		private void SetEnabledKeywords(LocalKeyword[] keywords)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.SetEnabledKeywords_Injected(intPtr, keywords);
		}

		public LocalKeyword[] enabledKeywords
		{
			get
			{
				return this.GetEnabledKeywords();
			}
			set
			{
				this.SetEnabledKeywords(value);
			}
		}

		public MaterialGlobalIlluminationFlags globalIlluminationFlags
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Material.get_globalIlluminationFlags_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Material.set_globalIlluminationFlags_Injected(intPtr, value);
			}
		}

		public bool doubleSidedGI
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Material.get_doubleSidedGI_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Material.set_doubleSidedGI_Injected(intPtr, value);
			}
		}

		[NativeProperty("EnableInstancingVariants")]
		public bool enableInstancing
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Material.get_enableInstancing_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Material.set_enableInstancing_Injected(intPtr, value);
			}
		}

		public int passCount
		{
			[NativeName("GetShader()->GetPassCount")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Material.get_passCount_Injected(intPtr);
			}
		}

		[FreeFunction("MaterialScripting::SetShaderPassEnabled", HasExplicitThis = true)]
		public unsafe void SetShaderPassEnabled(string passName, bool enabled)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(passName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = passName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Material.SetShaderPassEnabled_Injected(intPtr, ref managedSpanWrapper, enabled);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("MaterialScripting::GetShaderPassEnabled", HasExplicitThis = true)]
		public unsafe bool GetShaderPassEnabled(string passName)
		{
			bool shaderPassEnabled_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(passName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = passName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				shaderPassEnabled_Injected = Material.GetShaderPassEnabled_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return shaderPassEnabled_Injected;
		}

		public string GetPassName(int pass)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				Material.GetPassName_Injected(intPtr, pass, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public unsafe int FindPass(string passName)
		{
			int num;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(passName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = passName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				num = Material.FindPass_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		public unsafe void SetOverrideTag(string tag, string val)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(tag, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = tag.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(val, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = val.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				Material.SetOverrideTag_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		[NativeName("GetTag")]
		private unsafe string GetTagImpl(string tag, bool currentSubShaderOnly, string defaultValue)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(tag, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = tag.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(defaultValue, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = defaultValue.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper3;
				Material.GetTagImpl_Injected(intPtr, ref managedSpanWrapper, currentSubShaderOnly, ref managedSpanWrapper2, out managedSpanWrapper3);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
				ManagedSpanWrapper managedSpanWrapper3;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper3);
			}
			return stringAndDispose;
		}

		public string GetTag(string tag, bool searchFallbacks, string defaultValue)
		{
			return this.GetTagImpl(tag, !searchFallbacks, defaultValue);
		}

		public string GetTag(string tag, bool searchFallbacks)
		{
			return this.GetTagImpl(tag, !searchFallbacks, "");
		}

		[NativeThrows]
		[FreeFunction("MaterialScripting::Lerp", HasExplicitThis = true)]
		public void Lerp(Material start, Material end, float t)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.Lerp_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(start), Object.MarshalledUnityObject.Marshal<Material>(end), t);
		}

		[FreeFunction("MaterialScripting::SetPass", HasExplicitThis = true)]
		public bool SetPass(int pass)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.SetPass_Injected(intPtr, pass);
		}

		[FreeFunction("MaterialScripting::CopyPropertiesFrom", HasExplicitThis = true)]
		public void CopyPropertiesFromMaterial(Material mat)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.CopyPropertiesFromMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(mat));
		}

		[FreeFunction("MaterialScripting::CopyMatchingPropertiesFrom", HasExplicitThis = true)]
		public void CopyMatchingPropertiesFromMaterial(Material mat)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.CopyMatchingPropertiesFromMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(mat));
		}

		[FreeFunction("MaterialScripting::GetShaderKeywords", HasExplicitThis = true)]
		private string[] GetShaderKeywords()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.GetShaderKeywords_Injected(intPtr);
		}

		[FreeFunction("MaterialScripting::SetShaderKeywords", HasExplicitThis = true)]
		private void SetShaderKeywords(string[] names)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.SetShaderKeywords_Injected(intPtr, names);
		}

		public string[] shaderKeywords
		{
			get
			{
				return this.GetShaderKeywords();
			}
			set
			{
				this.SetShaderKeywords(value);
			}
		}

		[FreeFunction("MaterialScripting::GetPropertyNames", HasExplicitThis = true)]
		private string[] GetPropertyNamesImpl(int propertyType)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.GetPropertyNamesImpl_Injected(intPtr, propertyType);
		}

		[FreeFunction("MaterialScripting::GetPropertyCount", HasExplicitThis = true)]
		internal int GetPropertyCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.GetPropertyCount_Injected(intPtr);
		}

		public int ComputeCRC()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.ComputeCRC_Injected(intPtr);
		}

		[FreeFunction("MaterialScripting::GetTexturePropertyNames", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public string[] GetTexturePropertyNames()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.GetTexturePropertyNames_Injected(intPtr);
		}

		[FreeFunction("MaterialScripting::GetTexturePropertyNameIDs", HasExplicitThis = true)]
		public int[] GetTexturePropertyNameIDs()
		{
			int[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				Material.GetTexturePropertyNameIDs_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				int[] array;
				blittableArrayWrapper.Unmarshal<int>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction("MaterialScripting::GetTexturePropertyNamesInternal", HasExplicitThis = true)]
		private void GetTexturePropertyNamesInternal(object outNames)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.GetTexturePropertyNamesInternal_Injected(intPtr, outNames);
		}

		[FreeFunction("MaterialScripting::GetTexturePropertyNameIDsInternal", HasExplicitThis = true)]
		private void GetTexturePropertyNameIDsInternal(object outNames)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.GetTexturePropertyNameIDsInternal_Injected(intPtr, outNames);
		}

		public void GetTexturePropertyNames(List<string> outNames)
		{
			bool flag = outNames == null;
			if (flag)
			{
				throw new ArgumentNullException("outNames");
			}
			this.GetTexturePropertyNamesInternal(outNames);
		}

		public void GetTexturePropertyNameIDs(List<int> outNames)
		{
			bool flag = outNames == null;
			if (flag)
			{
				throw new ArgumentNullException("outNames");
			}
			this.GetTexturePropertyNameIDsInternal(outNames);
		}

		[NativeName("SetIntFromScript")]
		private void SetIntImpl(int name, int value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.SetIntImpl_Injected(intPtr, name, value);
		}

		[NativeName("SetFloatFromScript")]
		private void SetFloatImpl(int name, float value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.SetFloatImpl_Injected(intPtr, name, value);
		}

		[NativeName("SetColorFromScript")]
		private void SetColorImpl(int name, Color value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.SetColorImpl_Injected(intPtr, name, ref value);
		}

		[NativeName("SetMatrixFromScript")]
		private void SetMatrixImpl(int name, Matrix4x4 value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.SetMatrixImpl_Injected(intPtr, name, ref value);
		}

		[NativeName("SetTextureFromScript")]
		private void SetTextureImpl(int name, Texture value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.SetTextureImpl_Injected(intPtr, name, Object.MarshalledUnityObject.Marshal<Texture>(value));
		}

		[NativeName("SetRenderTextureFromScript")]
		private void SetRenderTextureImpl(int name, RenderTexture value, RenderTextureSubElement element)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.SetRenderTextureImpl_Injected(intPtr, name, Object.MarshalledUnityObject.Marshal<RenderTexture>(value), element);
		}

		[NativeName("SetBufferFromScript")]
		private void SetBufferImpl(int name, ComputeBuffer value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.SetBufferImpl_Injected(intPtr, name, (value == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(value));
		}

		[NativeName("SetBufferFromScript")]
		private void SetGraphicsBufferImpl(int name, GraphicsBuffer value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.SetGraphicsBufferImpl_Injected(intPtr, name, (value == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(value));
		}

		[NativeName("SetConstantBufferFromScript")]
		private void SetConstantBufferImpl(int name, ComputeBuffer value, int offset, int size)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.SetConstantBufferImpl_Injected(intPtr, name, (value == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(value), offset, size);
		}

		[NativeName("SetConstantBufferFromScript")]
		private void SetConstantGraphicsBufferImpl(int name, GraphicsBuffer value, int offset, int size)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.SetConstantGraphicsBufferImpl_Injected(intPtr, name, (value == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(value), offset, size);
		}

		[NativeName("GetIntFromScript")]
		private int GetIntImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.GetIntImpl_Injected(intPtr, name);
		}

		[NativeName("GetFloatFromScript")]
		private float GetFloatImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.GetFloatImpl_Injected(intPtr, name);
		}

		[NativeName("GetColorFromScript")]
		private Color GetColorImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Color color;
			Material.GetColorImpl_Injected(intPtr, name, out color);
			return color;
		}

		[NativeName("GetMatrixFromScript")]
		private Matrix4x4 GetMatrixImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Matrix4x4 matrix4x;
			Material.GetMatrixImpl_Injected(intPtr, name, out matrix4x);
			return matrix4x;
		}

		[NativeName("GetTextureFromScript")]
		private Texture GetTextureImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture>(Material.GetTextureImpl_Injected(intPtr, name));
		}

		[NativeName("GetBufferFromScript")]
		private GraphicsBufferHandle GetBufferImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GraphicsBufferHandle graphicsBufferHandle;
			Material.GetBufferImpl_Injected(intPtr, name, out graphicsBufferHandle);
			return graphicsBufferHandle;
		}

		[NativeName("GetConstantBufferFromScript")]
		private GraphicsBufferHandle GetConstantBufferImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GraphicsBufferHandle graphicsBufferHandle;
			Material.GetConstantBufferImpl_Injected(intPtr, name, out graphicsBufferHandle);
			return graphicsBufferHandle;
		}

		[FreeFunction(Name = "MaterialScripting::SetFloatArray", HasExplicitThis = true)]
		private unsafe void SetFloatArrayImpl(int name, float[] values, int count)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<float> span = new Span<float>(values);
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Material.SetFloatArrayImpl_Injected(intPtr, name, ref managedSpanWrapper, count);
			}
		}

		[FreeFunction(Name = "MaterialScripting::SetVectorArray", HasExplicitThis = true)]
		private unsafe void SetVectorArrayImpl(int name, Vector4[] values, int count)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector4> span = new Span<Vector4>(values);
			fixed (Vector4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Material.SetVectorArrayImpl_Injected(intPtr, name, ref managedSpanWrapper, count);
			}
		}

		[FreeFunction(Name = "MaterialScripting::SetColorArray", HasExplicitThis = true)]
		private unsafe void SetColorArrayImpl(int name, Color[] values, int count)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Color> span = new Span<Color>(values);
			fixed (Color* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Material.SetColorArrayImpl_Injected(intPtr, name, ref managedSpanWrapper, count);
			}
		}

		[FreeFunction(Name = "MaterialScripting::SetMatrixArray", HasExplicitThis = true)]
		private unsafe void SetMatrixArrayImpl(int name, Matrix4x4[] values, int count)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Matrix4x4> span = new Span<Matrix4x4>(values);
			fixed (Matrix4x4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Material.SetMatrixArrayImpl_Injected(intPtr, name, ref managedSpanWrapper, count);
			}
		}

		[FreeFunction(Name = "MaterialScripting::GetFloatArray", HasExplicitThis = true)]
		private float[] GetFloatArrayImpl(int name)
		{
			float[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				Material.GetFloatArrayImpl_Injected(intPtr, name, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				float[] array;
				blittableArrayWrapper.Unmarshal<float>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction(Name = "MaterialScripting::GetVectorArray", HasExplicitThis = true)]
		private Vector4[] GetVectorArrayImpl(int name)
		{
			Vector4[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				Material.GetVectorArrayImpl_Injected(intPtr, name, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Vector4[] array;
				blittableArrayWrapper.Unmarshal<Vector4>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction(Name = "MaterialScripting::GetColorArray", HasExplicitThis = true)]
		private Color[] GetColorArrayImpl(int name)
		{
			Color[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				Material.GetColorArrayImpl_Injected(intPtr, name, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Color[] array;
				blittableArrayWrapper.Unmarshal<Color>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction(Name = "MaterialScripting::GetMatrixArray", HasExplicitThis = true)]
		private Matrix4x4[] GetMatrixArrayImpl(int name)
		{
			Matrix4x4[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				Material.GetMatrixArrayImpl_Injected(intPtr, name, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Matrix4x4[] array;
				blittableArrayWrapper.Unmarshal<Matrix4x4>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction(Name = "MaterialScripting::GetFloatArrayCount", HasExplicitThis = true)]
		private int GetFloatArrayCountImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.GetFloatArrayCountImpl_Injected(intPtr, name);
		}

		[FreeFunction(Name = "MaterialScripting::GetVectorArrayCount", HasExplicitThis = true)]
		private int GetVectorArrayCountImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.GetVectorArrayCountImpl_Injected(intPtr, name);
		}

		[FreeFunction(Name = "MaterialScripting::GetColorArrayCount", HasExplicitThis = true)]
		private int GetColorArrayCountImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.GetColorArrayCountImpl_Injected(intPtr, name);
		}

		[FreeFunction(Name = "MaterialScripting::GetMatrixArrayCount", HasExplicitThis = true)]
		private int GetMatrixArrayCountImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Material.GetMatrixArrayCountImpl_Injected(intPtr, name);
		}

		[FreeFunction(Name = "MaterialScripting::ExtractFloatArray", HasExplicitThis = true)]
		private unsafe void ExtractFloatArrayImpl(int name, [Out] float[] val)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (val != null)
				{
					fixed (float[] array = val)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				Material.ExtractFloatArrayImpl_Injected(intPtr, name, out blittableArrayWrapper);
			}
			finally
			{
				float[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<float>(ref array);
			}
		}

		[FreeFunction(Name = "MaterialScripting::ExtractVectorArray", HasExplicitThis = true)]
		private unsafe void ExtractVectorArrayImpl(int name, [Out] Vector4[] val)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (val != null)
				{
					fixed (Vector4[] array = val)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				Material.ExtractVectorArrayImpl_Injected(intPtr, name, out blittableArrayWrapper);
			}
			finally
			{
				Vector4[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<Vector4>(ref array);
			}
		}

		[FreeFunction(Name = "MaterialScripting::ExtractColorArray", HasExplicitThis = true)]
		private unsafe void ExtractColorArrayImpl(int name, [Out] Color[] val)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (val != null)
				{
					fixed (Color[] array = val)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				Material.ExtractColorArrayImpl_Injected(intPtr, name, out blittableArrayWrapper);
			}
			finally
			{
				Color[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<Color>(ref array);
			}
		}

		[FreeFunction(Name = "MaterialScripting::ExtractMatrixArray", HasExplicitThis = true)]
		private unsafe void ExtractMatrixArrayImpl(int name, [Out] Matrix4x4[] val)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (val != null)
				{
					fixed (Matrix4x4[] array = val)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				Material.ExtractMatrixArrayImpl_Injected(intPtr, name, out blittableArrayWrapper);
			}
			finally
			{
				Matrix4x4[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<Matrix4x4>(ref array);
			}
		}

		[NativeName("GetTextureScaleAndOffsetFromScript")]
		private Vector4 GetTextureScaleAndOffsetImpl(int name)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector4 vector;
			Material.GetTextureScaleAndOffsetImpl_Injected(intPtr, name, out vector);
			return vector;
		}

		[NativeName("SetTextureOffsetFromScript")]
		private void SetTextureOffsetImpl(int name, Vector2 offset)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.SetTextureOffsetImpl_Injected(intPtr, name, ref offset);
		}

		[NativeName("SetTextureScaleFromScript")]
		private void SetTextureScaleImpl(int name, Vector2 scale)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Material.SetTextureScaleImpl_Injected(intPtr, name, ref scale);
		}

		private void SetFloatArray(int name, float[] values, int count)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			bool flag2 = values.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			bool flag3 = values.Length < count;
			if (flag3)
			{
				throw new ArgumentException("array has less elements than passed count.");
			}
			this.SetFloatArrayImpl(name, values, count);
		}

		private void SetVectorArray(int name, Vector4[] values, int count)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			bool flag2 = values.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			bool flag3 = values.Length < count;
			if (flag3)
			{
				throw new ArgumentException("array has less elements than passed count.");
			}
			this.SetVectorArrayImpl(name, values, count);
		}

		private void SetColorArray(int name, Color[] values, int count)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			bool flag2 = values.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			bool flag3 = values.Length < count;
			if (flag3)
			{
				throw new ArgumentException("array has less elements than passed count.");
			}
			this.SetColorArrayImpl(name, values, count);
		}

		private void SetMatrixArray(int name, Matrix4x4[] values, int count)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			bool flag2 = values.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			bool flag3 = values.Length < count;
			if (flag3)
			{
				throw new ArgumentException("array has less elements than passed count.");
			}
			this.SetMatrixArrayImpl(name, values, count);
		}

		private void ExtractFloatArray(int name, List<float> values)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			values.Clear();
			int floatArrayCountImpl = this.GetFloatArrayCountImpl(name);
			bool flag2 = floatArrayCountImpl > 0;
			if (flag2)
			{
				NoAllocHelpers.EnsureListElemCount<float>(values, floatArrayCountImpl);
				this.ExtractFloatArrayImpl(name, NoAllocHelpers.ExtractArrayFromList<float>(values));
			}
		}

		private void ExtractVectorArray(int name, List<Vector4> values)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			values.Clear();
			int vectorArrayCountImpl = this.GetVectorArrayCountImpl(name);
			bool flag2 = vectorArrayCountImpl > 0;
			if (flag2)
			{
				NoAllocHelpers.EnsureListElemCount<Vector4>(values, vectorArrayCountImpl);
				this.ExtractVectorArrayImpl(name, NoAllocHelpers.ExtractArrayFromList<Vector4>(values));
			}
		}

		private void ExtractColorArray(int name, List<Color> values)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			values.Clear();
			int colorArrayCountImpl = this.GetColorArrayCountImpl(name);
			bool flag2 = colorArrayCountImpl > 0;
			if (flag2)
			{
				NoAllocHelpers.EnsureListElemCount<Color>(values, colorArrayCountImpl);
				this.ExtractColorArrayImpl(name, NoAllocHelpers.ExtractArrayFromList<Color>(values));
			}
		}

		private void ExtractMatrixArray(int name, List<Matrix4x4> values)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			values.Clear();
			int matrixArrayCountImpl = this.GetMatrixArrayCountImpl(name);
			bool flag2 = matrixArrayCountImpl > 0;
			if (flag2)
			{
				NoAllocHelpers.EnsureListElemCount<Matrix4x4>(values, matrixArrayCountImpl);
				this.ExtractMatrixArrayImpl(name, NoAllocHelpers.ExtractArrayFromList<Matrix4x4>(values));
			}
		}

		public void SetInt(string name, int value)
		{
			this.SetFloatImpl(Shader.PropertyToID(name), (float)value);
		}

		public void SetInt(int nameID, int value)
		{
			this.SetFloatImpl(nameID, (float)value);
		}

		public void SetFloat(string name, float value)
		{
			this.SetFloatImpl(Shader.PropertyToID(name), value);
		}

		public void SetFloat(int nameID, float value)
		{
			this.SetFloatImpl(nameID, value);
		}

		public void SetInteger(string name, int value)
		{
			this.SetIntImpl(Shader.PropertyToID(name), value);
		}

		public void SetInteger(int nameID, int value)
		{
			this.SetIntImpl(nameID, value);
		}

		public void SetColor(string name, Color value)
		{
			this.SetColorImpl(Shader.PropertyToID(name), value);
		}

		public void SetColor(int nameID, Color value)
		{
			this.SetColorImpl(nameID, value);
		}

		public void SetVector(string name, Vector4 value)
		{
			this.SetColorImpl(Shader.PropertyToID(name), value);
		}

		public void SetVector(int nameID, Vector4 value)
		{
			this.SetColorImpl(nameID, value);
		}

		public void SetMatrix(string name, Matrix4x4 value)
		{
			this.SetMatrixImpl(Shader.PropertyToID(name), value);
		}

		public void SetMatrix(int nameID, Matrix4x4 value)
		{
			this.SetMatrixImpl(nameID, value);
		}

		public void SetTexture(string name, Texture value)
		{
			this.SetTextureImpl(Shader.PropertyToID(name), value);
		}

		public void SetTexture(int nameID, Texture value)
		{
			this.SetTextureImpl(nameID, value);
		}

		public void SetTexture(string name, RenderTexture value, RenderTextureSubElement element)
		{
			this.SetRenderTextureImpl(Shader.PropertyToID(name), value, element);
		}

		public void SetTexture(int nameID, RenderTexture value, RenderTextureSubElement element)
		{
			this.SetRenderTextureImpl(nameID, value, element);
		}

		public void SetBuffer(string name, ComputeBuffer value)
		{
			this.SetBufferImpl(Shader.PropertyToID(name), value);
		}

		public void SetBuffer(int nameID, ComputeBuffer value)
		{
			this.SetBufferImpl(nameID, value);
		}

		public void SetBuffer(string name, GraphicsBuffer value)
		{
			this.SetGraphicsBufferImpl(Shader.PropertyToID(name), value);
		}

		public void SetBuffer(int nameID, GraphicsBuffer value)
		{
			this.SetGraphicsBufferImpl(nameID, value);
		}

		public void SetConstantBuffer(string name, ComputeBuffer value, int offset, int size)
		{
			this.SetConstantBufferImpl(Shader.PropertyToID(name), value, offset, size);
		}

		public void SetConstantBuffer(int nameID, ComputeBuffer value, int offset, int size)
		{
			this.SetConstantBufferImpl(nameID, value, offset, size);
		}

		public void SetConstantBuffer(string name, GraphicsBuffer value, int offset, int size)
		{
			this.SetConstantGraphicsBufferImpl(Shader.PropertyToID(name), value, offset, size);
		}

		public void SetConstantBuffer(int nameID, GraphicsBuffer value, int offset, int size)
		{
			this.SetConstantGraphicsBufferImpl(nameID, value, offset, size);
		}

		public void SetFloatArray(string name, List<float> values)
		{
			this.SetFloatArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromList<float>(values), values.Count);
		}

		public void SetFloatArray(int nameID, List<float> values)
		{
			this.SetFloatArray(nameID, NoAllocHelpers.ExtractArrayFromList<float>(values), values.Count);
		}

		public void SetFloatArray(string name, float[] values)
		{
			this.SetFloatArray(Shader.PropertyToID(name), values, values.Length);
		}

		public void SetFloatArray(int nameID, float[] values)
		{
			this.SetFloatArray(nameID, values, values.Length);
		}

		public void SetColorArray(string name, List<Color> values)
		{
			this.SetColorArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromList<Color>(values), values.Count);
		}

		public void SetColorArray(int nameID, List<Color> values)
		{
			this.SetColorArray(nameID, NoAllocHelpers.ExtractArrayFromList<Color>(values), values.Count);
		}

		public void SetColorArray(string name, Color[] values)
		{
			this.SetColorArray(Shader.PropertyToID(name), values, values.Length);
		}

		public void SetColorArray(int nameID, Color[] values)
		{
			this.SetColorArray(nameID, values, values.Length);
		}

		public void SetVectorArray(string name, List<Vector4> values)
		{
			this.SetVectorArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromList<Vector4>(values), values.Count);
		}

		public void SetVectorArray(int nameID, List<Vector4> values)
		{
			this.SetVectorArray(nameID, NoAllocHelpers.ExtractArrayFromList<Vector4>(values), values.Count);
		}

		public void SetVectorArray(string name, Vector4[] values)
		{
			this.SetVectorArray(Shader.PropertyToID(name), values, values.Length);
		}

		public void SetVectorArray(int nameID, Vector4[] values)
		{
			this.SetVectorArray(nameID, values, values.Length);
		}

		public void SetMatrixArray(string name, List<Matrix4x4> values)
		{
			this.SetMatrixArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromList<Matrix4x4>(values), values.Count);
		}

		public void SetMatrixArray(int nameID, List<Matrix4x4> values)
		{
			this.SetMatrixArray(nameID, NoAllocHelpers.ExtractArrayFromList<Matrix4x4>(values), values.Count);
		}

		public void SetMatrixArray(string name, Matrix4x4[] values)
		{
			this.SetMatrixArray(Shader.PropertyToID(name), values, values.Length);
		}

		public void SetMatrixArray(int nameID, Matrix4x4[] values)
		{
			this.SetMatrixArray(nameID, values, values.Length);
		}

		public int GetInt(string name)
		{
			return (int)this.GetFloatImpl(Shader.PropertyToID(name));
		}

		public int GetInt(int nameID)
		{
			return (int)this.GetFloatImpl(nameID);
		}

		public float GetFloat(string name)
		{
			return this.GetFloatImpl(Shader.PropertyToID(name));
		}

		public float GetFloat(int nameID)
		{
			return this.GetFloatImpl(nameID);
		}

		public int GetInteger(string name)
		{
			return this.GetIntImpl(Shader.PropertyToID(name));
		}

		public int GetInteger(int nameID)
		{
			return this.GetIntImpl(nameID);
		}

		public Color GetColor(string name)
		{
			return this.GetColorImpl(Shader.PropertyToID(name));
		}

		public Color GetColor(int nameID)
		{
			return this.GetColorImpl(nameID);
		}

		public Vector4 GetVector(string name)
		{
			return this.GetColorImpl(Shader.PropertyToID(name));
		}

		public Vector4 GetVector(int nameID)
		{
			return this.GetColorImpl(nameID);
		}

		public Matrix4x4 GetMatrix(string name)
		{
			return this.GetMatrixImpl(Shader.PropertyToID(name));
		}

		public Matrix4x4 GetMatrix(int nameID)
		{
			return this.GetMatrixImpl(nameID);
		}

		public Texture GetTexture(string name)
		{
			return this.GetTextureImpl(Shader.PropertyToID(name));
		}

		public Texture GetTexture(int nameID)
		{
			return this.GetTextureImpl(nameID);
		}

		public GraphicsBufferHandle GetBuffer(string name)
		{
			return this.GetBufferImpl(Shader.PropertyToID(name));
		}

		public GraphicsBufferHandle GetConstantBuffer(string name)
		{
			return this.GetConstantBufferImpl(Shader.PropertyToID(name));
		}

		public float[] GetFloatArray(string name)
		{
			return this.GetFloatArray(Shader.PropertyToID(name));
		}

		public float[] GetFloatArray(int nameID)
		{
			return (this.GetFloatArrayCountImpl(nameID) != 0) ? this.GetFloatArrayImpl(nameID) : null;
		}

		public Color[] GetColorArray(string name)
		{
			return this.GetColorArray(Shader.PropertyToID(name));
		}

		public Color[] GetColorArray(int nameID)
		{
			return (this.GetColorArrayCountImpl(nameID) != 0) ? this.GetColorArrayImpl(nameID) : null;
		}

		public Vector4[] GetVectorArray(string name)
		{
			return this.GetVectorArray(Shader.PropertyToID(name));
		}

		public Vector4[] GetVectorArray(int nameID)
		{
			return (this.GetVectorArrayCountImpl(nameID) != 0) ? this.GetVectorArrayImpl(nameID) : null;
		}

		public Matrix4x4[] GetMatrixArray(string name)
		{
			return this.GetMatrixArray(Shader.PropertyToID(name));
		}

		public Matrix4x4[] GetMatrixArray(int nameID)
		{
			return (this.GetMatrixArrayCountImpl(nameID) != 0) ? this.GetMatrixArrayImpl(nameID) : null;
		}

		public void GetFloatArray(string name, List<float> values)
		{
			this.ExtractFloatArray(Shader.PropertyToID(name), values);
		}

		public void GetFloatArray(int nameID, List<float> values)
		{
			this.ExtractFloatArray(nameID, values);
		}

		public void GetColorArray(string name, List<Color> values)
		{
			this.ExtractColorArray(Shader.PropertyToID(name), values);
		}

		public void GetColorArray(int nameID, List<Color> values)
		{
			this.ExtractColorArray(nameID, values);
		}

		public void GetVectorArray(string name, List<Vector4> values)
		{
			this.ExtractVectorArray(Shader.PropertyToID(name), values);
		}

		public void GetVectorArray(int nameID, List<Vector4> values)
		{
			this.ExtractVectorArray(nameID, values);
		}

		public void GetMatrixArray(string name, List<Matrix4x4> values)
		{
			this.ExtractMatrixArray(Shader.PropertyToID(name), values);
		}

		public void GetMatrixArray(int nameID, List<Matrix4x4> values)
		{
			this.ExtractMatrixArray(nameID, values);
		}

		public void SetTextureOffset(string name, Vector2 value)
		{
			this.SetTextureOffsetImpl(Shader.PropertyToID(name), value);
		}

		public void SetTextureOffset(int nameID, Vector2 value)
		{
			this.SetTextureOffsetImpl(nameID, value);
		}

		public void SetTextureScale(string name, Vector2 value)
		{
			this.SetTextureScaleImpl(Shader.PropertyToID(name), value);
		}

		public void SetTextureScale(int nameID, Vector2 value)
		{
			this.SetTextureScaleImpl(nameID, value);
		}

		public Vector2 GetTextureOffset(string name)
		{
			return this.GetTextureOffset(Shader.PropertyToID(name));
		}

		public Vector2 GetTextureOffset(int nameID)
		{
			Vector4 textureScaleAndOffsetImpl = this.GetTextureScaleAndOffsetImpl(nameID);
			return new Vector2(textureScaleAndOffsetImpl.z, textureScaleAndOffsetImpl.w);
		}

		public Vector2 GetTextureScale(string name)
		{
			return this.GetTextureScale(Shader.PropertyToID(name));
		}

		public Vector2 GetTextureScale(int nameID)
		{
			Vector4 textureScaleAndOffsetImpl = this.GetTextureScaleAndOffsetImpl(nameID);
			return new Vector2(textureScaleAndOffsetImpl.x, textureScaleAndOffsetImpl.y);
		}

		public string[] GetPropertyNames(MaterialPropertyType type)
		{
			return this.GetPropertyNamesImpl((int)type);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateWithShader_Injected([Writable] Material self, IntPtr shader);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateWithMaterial_Injected([Writable] Material self, IntPtr source);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetDefaultMaterial_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetDefaultParticleMaterial_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetDefaultLineMaterial_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_shader_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_shader_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetFirstPropertyNameIdByAttribute_Injected(IntPtr _unity_self, ShaderPropertyFlags attributeFlag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasProperty_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasFloatImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasIntImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasTextureImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasMatrixImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasVectorImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasBufferImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasConstantBufferImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_renderQueue_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_renderQueue_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_rawRenderQueue_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableKeyword_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableKeyword_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsKeywordEnabled_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableLocalKeyword_Injected(IntPtr _unity_self, [In] ref LocalKeyword keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableLocalKeyword_Injected(IntPtr _unity_self, [In] ref LocalKeyword keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLocalKeyword_Injected(IntPtr _unity_self, [In] ref LocalKeyword keyword, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsLocalKeywordEnabled_Injected(IntPtr _unity_self, [In] ref LocalKeyword keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LocalKeyword[] GetEnabledKeywords_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetEnabledKeywords_Injected(IntPtr _unity_self, LocalKeyword[] keywords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern MaterialGlobalIlluminationFlags get_globalIlluminationFlags_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_globalIlluminationFlags_Injected(IntPtr _unity_self, MaterialGlobalIlluminationFlags value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_doubleSidedGI_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_doubleSidedGI_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enableInstancing_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enableInstancing_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_passCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetShaderPassEnabled_Injected(IntPtr _unity_self, ref ManagedSpanWrapper passName, bool enabled);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetShaderPassEnabled_Injected(IntPtr _unity_self, ref ManagedSpanWrapper passName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPassName_Injected(IntPtr _unity_self, int pass, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int FindPass_Injected(IntPtr _unity_self, ref ManagedSpanWrapper passName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetOverrideTag_Injected(IntPtr _unity_self, ref ManagedSpanWrapper tag, ref ManagedSpanWrapper val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTagImpl_Injected(IntPtr _unity_self, ref ManagedSpanWrapper tag, bool currentSubShaderOnly, ref ManagedSpanWrapper defaultValue, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Lerp_Injected(IntPtr _unity_self, IntPtr start, IntPtr end, float t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetPass_Injected(IntPtr _unity_self, int pass);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyPropertiesFromMaterial_Injected(IntPtr _unity_self, IntPtr mat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyMatchingPropertiesFromMaterial_Injected(IntPtr _unity_self, IntPtr mat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetShaderKeywords_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetShaderKeywords_Injected(IntPtr _unity_self, string[] names);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetPropertyNamesImpl_Injected(IntPtr _unity_self, int propertyType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPropertyCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ComputeCRC_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetTexturePropertyNames_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTexturePropertyNameIDs_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTexturePropertyNamesInternal_Injected(IntPtr _unity_self, object outNames);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTexturePropertyNameIDsInternal_Injected(IntPtr _unity_self, object outNames);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIntImpl_Injected(IntPtr _unity_self, int name, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatImpl_Injected(IntPtr _unity_self, int name, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetColorImpl_Injected(IntPtr _unity_self, int name, [In] ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMatrixImpl_Injected(IntPtr _unity_self, int name, [In] ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTextureImpl_Injected(IntPtr _unity_self, int name, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRenderTextureImpl_Injected(IntPtr _unity_self, int name, IntPtr value, RenderTextureSubElement element);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBufferImpl_Injected(IntPtr _unity_self, int name, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGraphicsBufferImpl_Injected(IntPtr _unity_self, int name, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetConstantBufferImpl_Injected(IntPtr _unity_self, int name, IntPtr value, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetConstantGraphicsBufferImpl_Injected(IntPtr _unity_self, int name, IntPtr value, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIntImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFloatImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetColorImpl_Injected(IntPtr _unity_self, int name, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMatrixImpl_Injected(IntPtr _unity_self, int name, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetTextureImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBufferImpl_Injected(IntPtr _unity_self, int name, out GraphicsBufferHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetConstantBufferImpl_Injected(IntPtr _unity_self, int name, out GraphicsBufferHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatArrayImpl_Injected(IntPtr _unity_self, int name, ref ManagedSpanWrapper values, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVectorArrayImpl_Injected(IntPtr _unity_self, int name, ref ManagedSpanWrapper values, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetColorArrayImpl_Injected(IntPtr _unity_self, int name, ref ManagedSpanWrapper values, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMatrixArrayImpl_Injected(IntPtr _unity_self, int name, ref ManagedSpanWrapper values, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetFloatArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVectorArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetColorArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMatrixArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetFloatArrayCountImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetVectorArrayCountImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetColorArrayCountImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMatrixArrayCountImpl_Injected(IntPtr _unity_self, int name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExtractFloatArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExtractVectorArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExtractColorArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExtractMatrixArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTextureScaleAndOffsetImpl_Injected(IntPtr _unity_self, int name, out Vector4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTextureOffsetImpl_Injected(IntPtr _unity_self, int name, [In] ref Vector2 offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTextureScaleImpl_Injected(IntPtr _unity_self, int name, [In] ref Vector2 scale);

		private static readonly int k_ColorId = Shader.PropertyToID("_Color");

		private static readonly int k_MainTexId = Shader.PropertyToID("_MainTex");
	}
}
