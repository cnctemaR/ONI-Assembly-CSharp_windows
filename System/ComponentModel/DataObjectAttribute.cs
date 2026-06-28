using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DataObjectAttribute : Attribute
	{
		public DataObjectAttribute()
			: this(true)
		{
		}

		public DataObjectAttribute(bool isDataObject)
		{
			this._isDataObject = isDataObject;
		}

		public bool IsDataObject
		{
			get
			{
				return this._isDataObject;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is DataObjectAttribute && ((DataObjectAttribute)obj).IsDataObject == this.IsDataObject;
		}

		public override int GetHashCode()
		{
			return this.IsDataObject.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return DataObjectAttribute.Default.Equals(this);
		}

		public static readonly DataObjectAttribute DataObject = new DataObjectAttribute(true);

		public static readonly DataObjectAttribute Default = DataObjectAttribute.NonDataObject;

		public static readonly DataObjectAttribute NonDataObject = new DataObjectAttribute(false);

		private readonly bool _isDataObject;
	}
}
