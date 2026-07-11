using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine
{
	/// <summary>
	///   <para>ShaderVariantCollection records which shader variants are actually used in each shader.</para>
	/// </summary>
	public sealed class ShaderVariantCollection : Object
	{
		/// <summary>
		///   <para>Create a new empty shader variant collection.</para>
		/// </summary>
		public ShaderVariantCollection()
		{
			ShaderVariantCollection.Internal_Create(this);
		}

		/// <summary>
		///   <para>Number of shaders in this collection (Read Only).</para>
		/// </summary>
		public extern int shaderCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Number of total varians in this collection (Read Only).</para>
		/// </summary>
		public extern int variantCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Is this ShaderVariantCollection already warmed up? (Read Only)</para>
		/// </summary>
		public extern bool isWarmedUp
		{
			[NativeName("IsWarmedUp")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool AddVariant(Shader shader, PassType passType, string[] keywords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool RemoveVariant(Shader shader, PassType passType, string[] keywords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool ContainsVariant(Shader shader, PassType passType, string[] keywords);

		/// <summary>
		///   <para>Remove all shader variants from the collection.</para>
		/// </summary>
		[NativeName("ClearVariants")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Clear();

		/// <summary>
		///   <para>Fully load shaders in ShaderVariantCollection.</para>
		/// </summary>
		[NativeName("WarmupShaders")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void WarmUp();

		[NativeName("CreateFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] ShaderVariantCollection svc);

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

		/// <summary>
		///   <para>Identifies a specific variant of a shader.</para>
		/// </summary>
		public struct ShaderVariant
		{
			/// <summary>
			///   <para>Creates a ShaderVariant structure.</para>
			/// </summary>
			/// <param name="shader"></param>
			/// <param name="passType"></param>
			/// <param name="keywords"></param>
			public ShaderVariant(Shader shader, PassType passType, params string[] keywords)
			{
				this.shader = shader;
				this.passType = passType;
				this.keywords = keywords;
			}

			[FreeFunction]
			[NativeConditional("UNITY_EDITOR")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern string CheckShaderVariant(Shader shader, PassType passType, string[] keywords);

			/// <summary>
			///   <para>Shader to use in this variant.</para>
			/// </summary>
			public Shader shader;

			/// <summary>
			///   <para>Pass type to use in this variant.</para>
			/// </summary>
			public PassType passType;

			/// <summary>
			///   <para>Array of shader keywords to use in this variant.</para>
			/// </summary>
			public string[] keywords;
		}
	}
}
