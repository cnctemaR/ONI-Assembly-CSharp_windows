using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class DataObjectFieldAttribute : Attribute
	{
		public DataObjectFieldAttribute(bool primaryKey)
			: this(primaryKey, false, false, -1)
		{
		}

		public DataObjectFieldAttribute(bool primaryKey, bool isIdentity)
			: this(primaryKey, isIdentity, false, -1)
		{
		}

		public DataObjectFieldAttribute(bool primaryKey, bool isIdentity, bool isNullable)
			: this(primaryKey, isIdentity, isNullable, -1)
		{
		}

		public DataObjectFieldAttribute(bool primaryKey, bool isIdentity, bool isNullable, int length)
		{
			this._primaryKey = primaryKey;
			this._isIdentity = isIdentity;
			this._isNullable = isNullable;
			this._length = length;
		}

		public bool IsIdentity
		{
			get
			{
				return this._isIdentity;
			}
		}

		public bool IsNullable
		{
			get
			{
				return this._isNullable;
			}
		}

		public int Length
		{
			get
			{
				return this._length;
			}
		}

		public bool PrimaryKey
		{
			get
			{
				return this._primaryKey;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DataObjectFieldAttribute dataObjectFieldAttribute = obj as DataObjectFieldAttribute;
			return dataObjectFieldAttribute != null && dataObjectFieldAttribute.IsIdentity == this.IsIdentity && dataObjectFieldAttribute.IsNullable == this.IsNullable && dataObjectFieldAttribute.Length == this.Length && dataObjectFieldAttribute.PrimaryKey == this.PrimaryKey;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private bool _primaryKey;

		private bool _isIdentity;

		private bool _isNullable;

		private int _length;
	}
}
