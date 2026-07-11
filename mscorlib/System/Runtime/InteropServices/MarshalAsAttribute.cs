using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
	[StructLayout(LayoutKind.Sequential)]
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

		internal MarshalAsAttribute Copy()
		{
			return (MarshalAsAttribute)base.MemberwiseClone();
		}

		public string MarshalCookie;

		[ComVisible(true)]
		public string MarshalType;

		[ComVisible(true)]
		public Type MarshalTypeRef;

		public Type SafeArrayUserDefinedSubType;

		private UnmanagedType utype;

		public UnmanagedType ArraySubType;

		public VarEnum SafeArraySubType;

		public int SizeConst;

		public int IidParameterIndex;

		public short SizeParamIndex;
	}
}
