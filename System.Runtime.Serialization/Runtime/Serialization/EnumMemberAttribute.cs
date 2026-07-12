using System;

namespace System.Runtime.Serialization
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class EnumMemberAttribute : Attribute
	{
		public string Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
				this.isValueSetExplicitly = true;
			}
		}

		public bool IsValueSetExplicitly
		{
			get
			{
				return this.isValueSetExplicitly;
			}
		}

		private string value;

		private bool isValueSetExplicitly;
	}
}
