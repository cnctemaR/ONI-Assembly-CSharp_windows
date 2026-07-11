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
			if (obj == this)
			{
				return true;
			}
			DataObjectAttribute dataObjectAttribute = obj as DataObjectAttribute;
			return dataObjectAttribute != null && dataObjectAttribute.IsDataObject == this.IsDataObject;
		}

		public override int GetHashCode()
		{
			return this._isDataObject.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(DataObjectAttribute.Default);
		}

		public static readonly DataObjectAttribute DataObject = new DataObjectAttribute(true);

		public static readonly DataObjectAttribute NonDataObject = new DataObjectAttribute(false);

		public static readonly DataObjectAttribute Default = DataObjectAttribute.NonDataObject;

		private bool _isDataObject;
	}
}
