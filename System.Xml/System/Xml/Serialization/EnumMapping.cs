using System;

namespace System.Xml.Serialization
{
	internal class EnumMapping : PrimitiveMapping
	{
		internal bool IsFlags
		{
			get
			{
				return this.isFlags;
			}
			set
			{
				this.isFlags = value;
			}
		}

		internal ConstantMapping[] Constants
		{
			get
			{
				return this.constants;
			}
			set
			{
				this.constants = value;
			}
		}

		private ConstantMapping[] constants;

		private bool isFlags;
	}
}
