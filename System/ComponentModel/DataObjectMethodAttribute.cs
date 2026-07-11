using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class DataObjectMethodAttribute : Attribute
	{
		public DataObjectMethodAttribute(DataObjectMethodType methodType)
			: this(methodType, false)
		{
		}

		public DataObjectMethodAttribute(DataObjectMethodType methodType, bool isDefault)
		{
			this._methodType = methodType;
			this._isDefault = isDefault;
		}

		public bool IsDefault
		{
			get
			{
				return this._isDefault;
			}
		}

		public DataObjectMethodType MethodType
		{
			get
			{
				return this._methodType;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DataObjectMethodAttribute dataObjectMethodAttribute = obj as DataObjectMethodAttribute;
			return dataObjectMethodAttribute != null && dataObjectMethodAttribute.MethodType == this.MethodType && dataObjectMethodAttribute.IsDefault == this.IsDefault;
		}

		public override int GetHashCode()
		{
			int methodType = (int)this._methodType;
			return methodType.GetHashCode() ^ this._isDefault.GetHashCode();
		}

		public override bool Match(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DataObjectMethodAttribute dataObjectMethodAttribute = obj as DataObjectMethodAttribute;
			return dataObjectMethodAttribute != null && dataObjectMethodAttribute.MethodType == this.MethodType;
		}

		private bool _isDefault;

		private DataObjectMethodType _methodType;
	}
}
