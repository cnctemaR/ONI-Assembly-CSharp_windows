using System;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Maps a RenderType to a specific render state override.</para>
	/// </summary>
	public struct RenderStateMapping
	{
		/// <summary>
		///   <para>Creates a new render state mapping with the specified values.</para>
		/// </summary>
		/// <param name="renderType">Specifices the RenderType to override the render state for.</param>
		/// <param name="stateBlock">Specifies the values to override the render state with.</param>
		public RenderStateMapping(string renderType, RenderStateBlock stateBlock)
		{
			this.m_RenderTypeID = Shader.TagToID(renderType);
			this.m_StateBlock = stateBlock;
		}

		/// <summary>
		///   <para>Creates a new render state mapping with the specified values.</para>
		/// </summary>
		/// <param name="renderType">Specifices the RenderType to override the render state for.</param>
		/// <param name="stateBlock">Specifies the values to override the render state with.</param>
		public RenderStateMapping(RenderStateBlock stateBlock)
		{
			this = new RenderStateMapping(null, stateBlock);
		}

		/// <summary>
		///   <para>Specifices the RenderType to override the render state for.</para>
		/// </summary>
		public string renderType
		{
			get
			{
				return Shader.IDToTag(this.m_RenderTypeID);
			}
			set
			{
				this.m_RenderTypeID = Shader.TagToID(value);
			}
		}

		/// <summary>
		///   <para>Specifies the values to override the render state with.</para>
		/// </summary>
		public RenderStateBlock stateBlock
		{
			get
			{
				return this.m_StateBlock;
			}
			set
			{
				this.m_StateBlock = value;
			}
		}

		private int m_RenderTypeID;

		private RenderStateBlock m_StateBlock;
	}
}
