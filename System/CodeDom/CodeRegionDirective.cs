using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeRegionDirective : CodeDirective
	{
		public CodeRegionDirective()
		{
		}

		public CodeRegionDirective(CodeRegionMode regionMode, string regionText)
		{
			this.RegionText = regionText;
			this.RegionMode = regionMode;
		}

		public string RegionText
		{
			get
			{
				return this._regionText ?? string.Empty;
			}
			set
			{
				this._regionText = value;
			}
		}

		public CodeRegionMode RegionMode { get; set; }

		private string _regionText;
	}
}
