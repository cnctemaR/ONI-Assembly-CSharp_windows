using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeRegionDirective : CodeDirective
	{
		public CodeRegionDirective()
		{
		}

		public CodeRegionDirective(CodeRegionMode regionMode, string regionText)
		{
			this.regionMode = regionMode;
			this.regionText = regionText;
		}

		public CodeRegionMode RegionMode
		{
			get
			{
				return this.regionMode;
			}
			set
			{
				this.regionMode = value;
			}
		}

		public string RegionText
		{
			get
			{
				if (this.regionText == null)
				{
					return string.Empty;
				}
				return this.regionText;
			}
			set
			{
				this.regionText = value;
			}
		}

		private CodeRegionMode regionMode;

		private string regionText;
	}
}
