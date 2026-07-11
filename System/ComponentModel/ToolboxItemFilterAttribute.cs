using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	[Serializable]
	public sealed class ToolboxItemFilterAttribute : Attribute
	{
		public ToolboxItemFilterAttribute(string filterString)
			: this(filterString, ToolboxItemFilterType.Allow)
		{
		}

		public ToolboxItemFilterAttribute(string filterString, ToolboxItemFilterType filterType)
		{
			if (filterString == null)
			{
				filterString = string.Empty;
			}
			this.filterString = filterString;
			this.filterType = filterType;
		}

		public string FilterString
		{
			get
			{
				return this.filterString;
			}
		}

		public ToolboxItemFilterType FilterType
		{
			get
			{
				return this.filterType;
			}
		}

		public override object TypeId
		{
			get
			{
				if (this.typeId == null)
				{
					this.typeId = base.GetType().FullName + this.filterString;
				}
				return this.typeId;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ToolboxItemFilterAttribute toolboxItemFilterAttribute = obj as ToolboxItemFilterAttribute;
			return toolboxItemFilterAttribute != null && toolboxItemFilterAttribute.FilterType.Equals(this.FilterType) && toolboxItemFilterAttribute.FilterString.Equals(this.FilterString);
		}

		public override int GetHashCode()
		{
			return this.filterString.GetHashCode();
		}

		public override bool Match(object obj)
		{
			ToolboxItemFilterAttribute toolboxItemFilterAttribute = obj as ToolboxItemFilterAttribute;
			return toolboxItemFilterAttribute != null && toolboxItemFilterAttribute.FilterString.Equals(this.FilterString);
		}

		public override string ToString()
		{
			return this.filterString + "," + Enum.GetName(typeof(ToolboxItemFilterType), this.filterType);
		}

		private ToolboxItemFilterType filterType;

		private string filterString;

		private string typeId;
	}
}
