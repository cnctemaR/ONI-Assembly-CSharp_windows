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

		public DataObjectMethodType MethodType
		{
			get
			{
				return this._methodType;
			}
		}

		public bool IsDefault
		{
			get
			{
				return this._isDefault;
			}
		}

		public override bool Match(object obj)
		{
			return obj is DataObjectMethodAttribute && ((DataObjectMethodAttribute)obj).MethodType == this.MethodType;
		}

		public override bool Equals(object obj)
		{
			return this.Match(obj) && ((DataObjectMethodAttribute)obj).IsDefault == this.IsDefault;
		}

		public override int GetHashCode()
		{
			return this.MethodType.GetHashCode() ^ this.IsDefault.GetHashCode();
		}

		private readonly DataObjectMethodType _methodType;

		private readonly bool _isDefault;
	}
}
