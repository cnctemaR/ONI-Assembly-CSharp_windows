using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Values for the stencil state.</para>
	/// </summary>
	public struct StencilState
	{
		/// <summary>
		///   <para>Creates a new stencil state with the given values.</para>
		/// </summary>
		/// <param name="readMask">An 8 bit mask as an 0–255 integer, used when comparing the reference value with the contents of the buffer.</param>
		/// <param name="writeMask">An 8 bit mask as an 0–255 integer, used when writing to the buffer.</param>
		/// <param name="enabled">Controls whether the stencil buffer is enabled.</param>
		/// <param name="compareFunctionFront">The function used to compare the reference value to the current contents of the buffer for front-facing geometry.</param>
		/// <param name="passOperationFront">What to do with the contents of the buffer if the stencil test (and the depth test) passes for front-facing geometry.</param>
		/// <param name="failOperationFront">What to do with the contents of the buffer if the stencil test fails for front-facing geometry.</param>
		/// <param name="zFailOperationFront">What to do with the contents of the buffer if the stencil test passes, but the depth test fails for front-facing geometry.</param>
		/// <param name="compareFunctionBack">The function used to compare the reference value to the current contents of the buffer for back-facing geometry.</param>
		/// <param name="passOperationBack">What to do with the contents of the buffer if the stencil test (and the depth test) passes for back-facing geometry.</param>
		/// <param name="failOperationBack">What to do with the contents of the buffer if the stencil test fails for back-facing geometry.</param>
		/// <param name="zFailOperationBack">What to do with the contents of the buffer if the stencil test passes, but the depth test fails for back-facing geometry.</param>
		/// <param name="compareFunction">The function used to compare the reference value to the current contents of the buffer.</param>
		/// <param name="passOperation">What to do with the contents of the buffer if the stencil test (and the depth test) passes.</param>
		/// <param name="failOperation">What to do with the contents of the buffer if the stencil test fails.</param>
		/// <param name="zFailOperation">What to do with the contents of the buffer if the stencil test passes, but the depth test.</param>
		public StencilState(bool enabled = false, byte readMask = 255, byte writeMask = 255, CompareFunction compareFunction = CompareFunction.Always, StencilOp passOperation = StencilOp.Keep, StencilOp failOperation = StencilOp.Keep, StencilOp zFailOperation = StencilOp.Keep)
		{
			this = new StencilState(enabled, readMask, writeMask, compareFunction, passOperation, failOperation, zFailOperation, compareFunction, passOperation, failOperation, zFailOperation);
		}

		/// <summary>
		///   <para>Creates a new stencil state with the given values.</para>
		/// </summary>
		/// <param name="readMask">An 8 bit mask as an 0–255 integer, used when comparing the reference value with the contents of the buffer.</param>
		/// <param name="writeMask">An 8 bit mask as an 0–255 integer, used when writing to the buffer.</param>
		/// <param name="enabled">Controls whether the stencil buffer is enabled.</param>
		/// <param name="compareFunctionFront">The function used to compare the reference value to the current contents of the buffer for front-facing geometry.</param>
		/// <param name="passOperationFront">What to do with the contents of the buffer if the stencil test (and the depth test) passes for front-facing geometry.</param>
		/// <param name="failOperationFront">What to do with the contents of the buffer if the stencil test fails for front-facing geometry.</param>
		/// <param name="zFailOperationFront">What to do with the contents of the buffer if the stencil test passes, but the depth test fails for front-facing geometry.</param>
		/// <param name="compareFunctionBack">The function used to compare the reference value to the current contents of the buffer for back-facing geometry.</param>
		/// <param name="passOperationBack">What to do with the contents of the buffer if the stencil test (and the depth test) passes for back-facing geometry.</param>
		/// <param name="failOperationBack">What to do with the contents of the buffer if the stencil test fails for back-facing geometry.</param>
		/// <param name="zFailOperationBack">What to do with the contents of the buffer if the stencil test passes, but the depth test fails for back-facing geometry.</param>
		/// <param name="compareFunction">The function used to compare the reference value to the current contents of the buffer.</param>
		/// <param name="passOperation">What to do with the contents of the buffer if the stencil test (and the depth test) passes.</param>
		/// <param name="failOperation">What to do with the contents of the buffer if the stencil test fails.</param>
		/// <param name="zFailOperation">What to do with the contents of the buffer if the stencil test passes, but the depth test.</param>
		public StencilState(bool enabled, byte readMask, byte writeMask, CompareFunction compareFunctionFront, StencilOp passOperationFront, StencilOp failOperationFront, StencilOp zFailOperationFront, CompareFunction compareFunctionBack, StencilOp passOperationBack, StencilOp failOperationBack, StencilOp zFailOperationBack)
		{
			this.m_Enabled = Convert.ToByte(enabled);
			this.m_ReadMask = readMask;
			this.m_WriteMask = writeMask;
			this.m_Padding = 0;
			this.m_CompareFunctionFront = (byte)compareFunctionFront;
			this.m_PassOperationFront = (byte)passOperationFront;
			this.m_FailOperationFront = (byte)failOperationFront;
			this.m_ZFailOperationFront = (byte)zFailOperationFront;
			this.m_CompareFunctionBack = (byte)compareFunctionBack;
			this.m_PassOperationBack = (byte)passOperationBack;
			this.m_FailOperationBack = (byte)failOperationBack;
			this.m_ZFailOperationBack = (byte)zFailOperationBack;
		}

		/// <summary>
		///   <para>Default values for the stencil state.</para>
		/// </summary>
		public static StencilState Default
		{
			get
			{
				return new StencilState(false, byte.MaxValue, byte.MaxValue, CompareFunction.Always, StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
			}
		}

		/// <summary>
		///   <para>Controls whether the stencil buffer is enabled.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return Convert.ToBoolean(this.m_Enabled);
			}
			set
			{
				this.m_Enabled = Convert.ToByte(value);
			}
		}

		/// <summary>
		///   <para>An 8 bit mask as an 0–255 integer, used when comparing the reference value with the contents of the buffer.</para>
		/// </summary>
		public byte readMask
		{
			get
			{
				return this.m_ReadMask;
			}
			set
			{
				this.m_ReadMask = value;
			}
		}

		/// <summary>
		///   <para>An 8 bit mask as an 0–255 integer, used when writing to the buffer.</para>
		/// </summary>
		public byte writeMask
		{
			get
			{
				return this.m_WriteMask;
			}
			set
			{
				this.m_WriteMask = value;
			}
		}

		/// <summary>
		///   <para>The function used to compare the reference value to the current contents of the buffer.</para>
		/// </summary>
		public CompareFunction compareFunction
		{
			set
			{
				this.compareFunctionFront = value;
				this.compareFunctionBack = value;
			}
		}

		/// <summary>
		///   <para>What to do with the contents of the buffer if the stencil test (and the depth test) passes.</para>
		/// </summary>
		public StencilOp passOperation
		{
			set
			{
				this.passOperationFront = value;
				this.passOperationBack = value;
			}
		}

		/// <summary>
		///   <para>What to do with the contents of the buffer if the stencil test fails.</para>
		/// </summary>
		public StencilOp failOperation
		{
			set
			{
				this.failOperationFront = value;
				this.failOperationBack = value;
			}
		}

		/// <summary>
		///   <para>What to do with the contents of the buffer if the stencil test passes, but the depth test fails.</para>
		/// </summary>
		public StencilOp zFailOperation
		{
			set
			{
				this.zFailOperationFront = value;
				this.zFailOperationBack = value;
			}
		}

		/// <summary>
		///   <para>The function used to compare the reference value to the current contents of the buffer for front-facing geometry.</para>
		/// </summary>
		public CompareFunction compareFunctionFront
		{
			get
			{
				return (CompareFunction)this.m_CompareFunctionFront;
			}
			set
			{
				this.m_CompareFunctionFront = (byte)value;
			}
		}

		/// <summary>
		///   <para>What to do with the contents of the buffer if the stencil test (and the depth test) passes for front-facing geometry.</para>
		/// </summary>
		public StencilOp passOperationFront
		{
			get
			{
				return (StencilOp)this.m_PassOperationFront;
			}
			set
			{
				this.m_PassOperationFront = (byte)value;
			}
		}

		/// <summary>
		///   <para>What to do with the contents of the buffer if the stencil test fails for front-facing geometry.</para>
		/// </summary>
		public StencilOp failOperationFront
		{
			get
			{
				return (StencilOp)this.m_FailOperationFront;
			}
			set
			{
				this.m_FailOperationFront = (byte)value;
			}
		}

		/// <summary>
		///   <para>What to do with the contents of the buffer if the stencil test passes, but the depth test fails for front-facing geometry.</para>
		/// </summary>
		public StencilOp zFailOperationFront
		{
			get
			{
				return (StencilOp)this.m_ZFailOperationFront;
			}
			set
			{
				this.m_ZFailOperationFront = (byte)value;
			}
		}

		/// <summary>
		///   <para>The function used to compare the reference value to the current contents of the buffer for back-facing geometry.</para>
		/// </summary>
		public CompareFunction compareFunctionBack
		{
			get
			{
				return (CompareFunction)this.m_CompareFunctionBack;
			}
			set
			{
				this.m_CompareFunctionBack = (byte)value;
			}
		}

		/// <summary>
		///   <para>What to do with the contents of the buffer if the stencil test (and the depth test) passes for back-facing geometry.</para>
		/// </summary>
		public StencilOp passOperationBack
		{
			get
			{
				return (StencilOp)this.m_PassOperationBack;
			}
			set
			{
				this.m_PassOperationBack = (byte)value;
			}
		}

		/// <summary>
		///   <para>What to do with the contents of the buffer if the stencil test fails for back-facing geometry.</para>
		/// </summary>
		public StencilOp failOperationBack
		{
			get
			{
				return (StencilOp)this.m_FailOperationBack;
			}
			set
			{
				this.m_FailOperationBack = (byte)value;
			}
		}

		/// <summary>
		///   <para>What to do with the contents of the buffer if the stencil test passes, but the depth test fails for back-facing geometry.</para>
		/// </summary>
		public StencilOp zFailOperationBack
		{
			get
			{
				return (StencilOp)this.m_ZFailOperationBack;
			}
			set
			{
				this.m_ZFailOperationBack = (byte)value;
			}
		}

		private byte m_Enabled;

		private byte m_ReadMask;

		private byte m_WriteMask;

		private byte m_Padding;

		private byte m_CompareFunctionFront;

		private byte m_PassOperationFront;

		private byte m_FailOperationFront;

		private byte m_ZFailOperationFront;

		private byte m_CompareFunctionBack;

		private byte m_PassOperationBack;

		private byte m_FailOperationBack;

		private byte m_ZFailOperationBack;
	}
}
