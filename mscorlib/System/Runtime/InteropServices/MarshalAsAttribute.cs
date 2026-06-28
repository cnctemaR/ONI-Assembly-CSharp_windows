using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
	public sealed class MarshalAsAttribute : Attribute
	{
		public MarshalAsAttribute(short unmanagedType)
		{
			this.utype = (UnmanagedType)unmanagedType;
		}

		public MarshalAsAttribute(UnmanagedType unmanagedType)
		{
			this.utype = unmanagedType;
		}

		public UnmanagedType Value
		{
			get
			{
				return this.utype;
			}
		}

		private UnmanagedType utype;

		public UnmanagedType ArraySubType;

		public string MarshalCookie;

		[ComVisible(true)]
		public string MarshalType;

		[ComVisible(true)]
		public Type MarshalTypeRef;

		public VarEnum SafeArraySubType;

		public int SizeConst;

		public short SizeParamIndex;

		public Type SafeArrayUserDefinedSubType;

		public int IidParameterIndex;
	}
}
