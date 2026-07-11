using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	[Serializable]
	public sealed class ToolboxItemFilterAttribute : Attribute
	{
		public ToolboxItemFilterAttribute(string filterString)
		{
			this.Filter = filterString;
			this.ItemFilterType = ToolboxItemFilterType.Allow;
		}

		public ToolboxItemFilterAttribute(string filterString, ToolboxItemFilterType filterType)
		{
			this.Filter = filterString;
			this.ItemFilterType = filterType;
		}

		public string FilterString
		{
			get
			{
				return this.Filter;
			}
		}

		public ToolboxItemFilterType FilterType
		{
			get
			{
				return this.ItemFilterType;
			}
		}

		public override object TypeId
		{
			get
			{
				return base.TypeId + this.Filter;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is ToolboxItemFilterAttribute && (obj == this || (((ToolboxItemFilterAttribute)obj).FilterString == this.Filter && ((ToolboxItemFilterAttribute)obj).FilterType == this.ItemFilterType));
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override bool Match(object obj)
		{
			return obj is ToolboxItemFilterAttribute && ((ToolboxItemFilterAttribute)obj).FilterString == this.Filter;
		}

		public override string ToString()
		{
			return string.Format("{0},{1}", this.Filter, this.ItemFilterType);
		}

		private string Filter;

		private ToolboxItemFilterType ItemFilterType;
	}
}
