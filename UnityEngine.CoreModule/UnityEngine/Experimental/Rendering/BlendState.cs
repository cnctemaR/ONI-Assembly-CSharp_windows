using System;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Values for the blend state.</para>
	/// </summary>
	public struct BlendState
	{
		/// <summary>
		///   <para>Creates a new blend state with the specified values.</para>
		/// </summary>
		/// <param name="separateMRTBlend">Determines whether each render target uses a separate blend state.</param>
		/// <param name="alphaToMask">Turns on alpha-to-coverage.</param>
		public BlendState(bool separateMRTBlend = false, bool alphaToMask = false)
		{
			this.m_BlendState0 = RenderTargetBlendState.Default;
			this.m_BlendState1 = RenderTargetBlendState.Default;
			this.m_BlendState2 = RenderTargetBlendState.Default;
			this.m_BlendState3 = RenderTargetBlendState.Default;
			this.m_BlendState4 = RenderTargetBlendState.Default;
			this.m_BlendState5 = RenderTargetBlendState.Default;
			this.m_BlendState6 = RenderTargetBlendState.Default;
			this.m_BlendState7 = RenderTargetBlendState.Default;
			this.m_SeparateMRTBlendStates = Convert.ToByte(separateMRTBlend);
			this.m_AlphaToMask = Convert.ToByte(alphaToMask);
			this.m_Padding = 0;
		}

		/// <summary>
		///   <para>Default values for the blend state.</para>
		/// </summary>
		public static BlendState Default
		{
			get
			{
				return new BlendState(false, false);
			}
		}

		/// <summary>
		///   <para>Determines whether each render target uses a separate blend state.</para>
		/// </summary>
		public bool separateMRTBlendStates
		{
			get
			{
				return Convert.ToBoolean(this.m_SeparateMRTBlendStates);
			}
			set
			{
				this.m_SeparateMRTBlendStates = Convert.ToByte(value);
			}
		}

		/// <summary>
		///   <para>Turns on alpha-to-coverage.</para>
		/// </summary>
		public bool alphaToMask
		{
			get
			{
				return Convert.ToBoolean(this.m_AlphaToMask);
			}
			set
			{
				this.m_AlphaToMask = Convert.ToByte(value);
			}
		}

		/// <summary>
		///   <para>Blend state for render target 0.</para>
		/// </summary>
		public RenderTargetBlendState blendState0
		{
			get
			{
				return this.m_BlendState0;
			}
			set
			{
				this.m_BlendState0 = value;
			}
		}

		/// <summary>
		///   <para>Blend state for render target 1.</para>
		/// </summary>
		public RenderTargetBlendState blendState1
		{
			get
			{
				return this.m_BlendState1;
			}
			set
			{
				this.m_BlendState1 = value;
			}
		}

		/// <summary>
		///   <para>Blend state for render target 2.</para>
		/// </summary>
		public RenderTargetBlendState blendState2
		{
			get
			{
				return this.m_BlendState2;
			}
			set
			{
				this.m_BlendState2 = value;
			}
		}

		/// <summary>
		///   <para>Blend state for render target 3.</para>
		/// </summary>
		public RenderTargetBlendState blendState3
		{
			get
			{
				return this.m_BlendState3;
			}
			set
			{
				this.m_BlendState3 = value;
			}
		}

		/// <summary>
		///   <para>Blend state for render target 4.</para>
		/// </summary>
		public RenderTargetBlendState blendState4
		{
			get
			{
				return this.m_BlendState4;
			}
			set
			{
				this.m_BlendState4 = value;
			}
		}

		/// <summary>
		///   <para>Blend state for render target 5.</para>
		/// </summary>
		public RenderTargetBlendState blendState5
		{
			get
			{
				return this.m_BlendState5;
			}
			set
			{
				this.m_BlendState5 = value;
			}
		}

		/// <summary>
		///   <para>Blend state for render target 6.</para>
		/// </summary>
		public RenderTargetBlendState blendState6
		{
			get
			{
				return this.m_BlendState6;
			}
			set
			{
				this.m_BlendState6 = value;
			}
		}

		/// <summary>
		///   <para>Blend state for render target 7.</para>
		/// </summary>
		public RenderTargetBlendState blendState7
		{
			get
			{
				return this.m_BlendState7;
			}
			set
			{
				this.m_BlendState7 = value;
			}
		}

		private RenderTargetBlendState m_BlendState0;

		private RenderTargetBlendState m_BlendState1;

		private RenderTargetBlendState m_BlendState2;

		private RenderTargetBlendState m_BlendState3;

		private RenderTargetBlendState m_BlendState4;

		private RenderTargetBlendState m_BlendState5;

		private RenderTargetBlendState m_BlendState6;

		private RenderTargetBlendState m_BlendState7;

		private byte m_SeparateMRTBlendStates;

		private byte m_AlphaToMask;

		private short m_Padding;
	}
}
