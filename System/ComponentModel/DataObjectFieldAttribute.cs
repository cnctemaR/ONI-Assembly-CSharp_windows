using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class DataObjectFieldAttribute : Attribute
	{
		public DataObjectFieldAttribute(bool primaryKey)
		{
			this.primary_key = primaryKey;
		}

		public DataObjectFieldAttribute(bool primaryKey, bool isIdentity)
		{
			this.primary_key = primaryKey;
			this.is_identity = isIdentity;
		}

		public DataObjectFieldAttribute(bool primaryKey, bool isIdentity, bool isNullable)
		{
			this.primary_key = primaryKey;
			this.is_identity = isIdentity;
			this.is_nullable = isNullable;
		}

		public DataObjectFieldAttribute(bool primaryKey, bool isIdentity, bool isNullable, int length)
		{
			this.primary_key = primaryKey;
			this.is_identity = isIdentity;
			this.is_nullable = isNullable;
			this.length = length;
		}

		public bool IsIdentity
		{
			get
			{
				return this.is_identity;
			}
		}

		public bool IsNullable
		{
			get
			{
				return this.is_nullable;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}

		public bool PrimaryKey
		{
			get
			{
				return this.primary_key;
			}
		}

		public override bool Equals(object obj)
		{
			DataObjectFieldAttribute dataObjectFieldAttribute = obj as DataObjectFieldAttribute;
			return dataObjectFieldAttribute != null && (dataObjectFieldAttribute.primary_key == this.primary_key && dataObjectFieldAttribute.is_identity == this.is_identity && dataObjectFieldAttribute.is_nullable == this.is_nullable) && dataObjectFieldAttribute.length == this.length;
		}

		public override int GetHashCode()
		{
			return (((!this.primary_key) ? 0 : 1) | ((!this.is_identity) ? 0 : 2) | ((!this.is_nullable) ? 0 : 4)) ^ this.length;
		}

		private bool primary_key;

		private bool is_identity;

		private bool is_nullable;

		private int length = -1;
	}
}
