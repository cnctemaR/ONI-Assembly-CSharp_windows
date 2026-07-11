using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Values for the depth state.</para>
	/// </summary>
	public struct DepthState
	{
		/// <summary>
		///   <para>Creates a new depth state with the given values.</para>
		/// </summary>
		/// <param name="writeEnabled">Controls whether pixels from this object are written to the depth buffer.</param>
		/// <param name="compareFunction">How should depth testing be performed.</param>
		public DepthState(bool writeEnabled = true, CompareFunction compareFunction = CompareFunction.Less)
		{
			this.m_WriteEnabled = Convert.ToByte(writeEnabled);
			this.m_CompareFunction = (sbyte)compareFunction;
		}

		/// <summary>
		///   <para>Default values for the depth state.</para>
		/// </summary>
		public static DepthState Default
		{
			get
			{
				return new DepthState(true, CompareFunction.Less);
			}
		}

		/// <summary>
		///   <para>Controls whether pixels from this object are written to the depth buffer.</para>
		/// </summary>
		public bool writeEnabled
		{
			get
			{
				return Convert.ToBoolean(this.m_WriteEnabled);
			}
			set
			{
				this.m_WriteEnabled = Convert.ToByte(value);
			}
		}

		/// <summary>
		///   <para>How should depth testing be performed.</para>
		/// </summary>
		public CompareFunction compareFunction
		{
			get
			{
				return (CompareFunction)this.m_CompareFunction;
			}
			set
			{
				this.m_CompareFunction = (sbyte)value;
			}
		}

		private byte m_WriteEnabled;

		private sbyte m_CompareFunction;
	}
}
