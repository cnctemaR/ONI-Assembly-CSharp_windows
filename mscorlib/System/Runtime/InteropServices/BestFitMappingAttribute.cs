using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
	public sealed class BestFitMappingAttribute : Attribute
	{
		public BestFitMappingAttribute(bool BestFitMapping)
		{
			this.bfm = BestFitMapping;
		}

		public bool BestFitMapping
		{
			get
			{
				return this.bfm;
			}
		}

		private bool bfm;

		public bool ThrowOnUnmappableChar;
	}
}
