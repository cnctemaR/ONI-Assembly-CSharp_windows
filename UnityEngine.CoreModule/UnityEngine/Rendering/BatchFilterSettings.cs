using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering
{
	public struct BatchFilterSettings
	{
		public byte batchLayer
		{
			get
			{
				return this.m_batchLayer;
			}
			set
			{
				this.m_batchLayer = value;
			}
		}

		public MotionVectorGenerationMode motionMode
		{
			get
			{
				return (MotionVectorGenerationMode)this.m_motionMode;
			}
			set
			{
				this.m_motionMode = (byte)value;
			}
		}

		public ShadowCastingMode shadowCastingMode
		{
			get
			{
				return (ShadowCastingMode)this.m_shadowMode;
			}
			set
			{
				this.m_shadowMode = (byte)value;
			}
		}

		public bool receiveShadows
		{
			get
			{
				return this.m_receiveShadows > 0;
			}
			set
			{
				this.m_receiveShadows = (value ? 1 : 0);
			}
		}

		public bool staticShadowCaster
		{
			get
			{
				return this.m_staticShadowCaster > 0;
			}
			set
			{
				this.m_staticShadowCaster = (value ? 1 : 0);
			}
		}

		public bool allDepthSorted
		{
			get
			{
				return this.m_allDepthSorted > 0;
			}
			set
			{
				this.m_allDepthSorted = (value ? 1 : 0);
			}
		}

		[FreeFunction("BatchFilterSettings::DefaultCullingMask", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ulong DefaultCullingMask();

		public ulong sceneCullingMask
		{
			get
			{
				return (this.m_isSceneCullingMaskSet != 0) ? this.m_sceneCullingMask : BatchFilterSettings.DefaultCullingMask();
			}
			set
			{
				this.m_isSceneCullingMaskSet = 1;
				this.m_sceneCullingMask = value;
			}
		}

		public uint renderingLayerMask;

		public int rendererPriority;

		private ulong m_sceneCullingMask;

		public byte layer;

		private byte m_batchLayer;

		private byte m_motionMode;

		private byte m_shadowMode;

		private byte m_receiveShadows;

		private byte m_staticShadowCaster;

		private byte m_allDepthSorted;

		private byte m_isSceneCullingMaskSet;
	}
}
