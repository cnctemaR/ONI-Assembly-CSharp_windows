using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.Rendering
{
	[NativeHeader("Runtime/Graphics/ScriptableRenderLoop/ScriptableDrawRenderersUtility.h")]
	public struct DrawRendererSettings
	{
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
			this.useSRPBatcher = 0;
		}

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

		[FreeFunction("InitializeSortSettings")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InitializeSortSettings(Camera camera, out DrawRendererSortSettings sortSettings);

		private const int kMaxShaderPasses = 16;

		public static readonly int maxShaderPasses = 16;

		public DrawRendererSortSettings sorting;

		[FixedBuffer(typeof(int), 16)]
		internal DrawRendererSettings.<shaderPassNames>__FixedBuffer7 shaderPassNames;

		public RendererConfiguration rendererConfiguration;

		public DrawRendererFlags flags;

		private int m_OverrideMaterialInstanceId;

		private int m_OverrideMaterialPassIdx;

		private int useSRPBatcher;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 64)]
		public struct <shaderPassNames>__FixedBuffer7
		{
			public int FixedElementField;
		}
	}
}
