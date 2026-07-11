using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
	public sealed class BestFitMappingAttribute : Attribute
	{
		public BestFitMappingAttribute(bool BestFitMapping)
		{
			this._bestFitMapping = BestFitMapping;
		}

		public bool BestFitMapping
		{
			get
			{
				return this._bestFitMapping;
			}
		}

		internal bool _bestFitMapping;

		public bool ThrowOnUnmappableChar;
	}
}
