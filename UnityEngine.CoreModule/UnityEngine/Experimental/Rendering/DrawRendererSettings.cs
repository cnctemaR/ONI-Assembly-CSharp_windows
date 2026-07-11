using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Settings for ScriptableRenderContext.DrawRenderers.</para>
	/// </summary>
	public struct DrawRendererSettings
	{
		/// <summary>
		///   <para>Create a draw settings struct.</para>
		/// </summary>
		/// <param name="camera">Camera to use. Camera's transparency sort mode is used to determine whether to use orthographic or distance based sorting.</param>
		/// <param name="shaderPassName">Shader pass to use.</param>
		public unsafe DrawRendererSettings(Camera camera, ShaderPassName shaderPassName)
		{
			this.rendererConfiguration = RendererConfiguration.None;
			this.flags = DrawRendererFlags.EnableInstancing;
			this.m_OverrideMaterialInstanceId = 0;
			this.m_OverrideMaterialPassIdx = 0;
			fixed (int* ptr = &this.shaderPassNames.FixedElementField)
			{
				for (int i = 0; i < DrawRendererSettings.maxShaderPasses; i++)
				{
					ptr[(IntPtr)i * 4] = -1;
				}
			}
			fixed (int* ptr2 = &this.shaderPassNames.FixedElementField)
			{
				*ptr2 = shaderPassName.nameIndex;
			}
			this.rendererConfiguration = RendererConfiguration.None;
			this.flags = DrawRendererFlags.EnableInstancing;
			DrawRendererSettings.InitializeSortSettings(camera, out this.sorting);
		}

		/// <summary>
		///   <para>Set the Material to use for all drawers that would render in this group.</para>
		/// </summary>
		/// <param name="mat">Override material.</param>
		/// <param name="passIndex">Pass to use in the material.</param>
		public void SetOverrideMaterial(Material mat, int passIndex)
		{
			if (mat == null)
			{
				this.m_OverrideMaterialInstanceId = 0;
			}
			else
			{
				this.m_OverrideMaterialInstanceId = mat.GetInstanceID();
			}
			this.m_OverrideMaterialPassIdx = passIndex;
		}

		/// <summary>
		///   <para>Set the shader passes that this draw call can render.</para>
		/// </summary>
		/// <param name="index">Index of the shader pass to use.</param>
		/// <param name="shaderPassName">Name of the shader pass.</param>
		public unsafe void SetShaderPassName(int index, ShaderPassName shaderPassName)
		{
			if (index >= DrawRendererSettings.maxShaderPasses || index < 0)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Index should range from 0 - DrawRendererSettings.maxShaderPasses ({0}), was {1}", DrawRendererSettings.maxShaderPasses, index));
			}
			fixed (int* ptr = &this.shaderPassNames.FixedElementField)
			{
				ptr[(IntPtr)index * 4] = shaderPassName.nameIndex;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InitializeSortSettings(Camera camera, out DrawRendererSortSettings sortSettings);

		private const int kMaxShaderPasses = 16;

		/// <summary>
		///   <para>The maxiumum number of passes that can be rendered in 1 DrawRenderers call.</para>
		/// </summary>
		public static readonly int maxShaderPasses = 16;

		/// <summary>
		///   <para>How to sort objects during rendering.</para>
		/// </summary>
		public DrawRendererSortSettings sorting;

		private DrawRendererSettings.<shaderPassNames>__FixedBuffer0 shaderPassNames;

		/// <summary>
		///   <para>What kind of per-object data to setup during rendering.</para>
		/// </summary>
		public RendererConfiguration rendererConfiguration;

		/// <summary>
		///   <para>Other flags controlling object rendering.</para>
		/// </summary>
		public DrawRendererFlags flags;

		private int m_OverrideMaterialInstanceId;

		private int m_OverrideMaterialPassIdx;

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 64)]
		public struct <shaderPassNames>__FixedBuffer0
		{
			public int FixedElementField;
		}
	}
}
