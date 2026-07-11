using System;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
	public sealed class AssemblyMetadataAttribute : Attribute
	{
		public AssemblyMetadataAttribute(string key, string value)
		{
			this.m_key = key;
			this.m_value = value;
		}

		public string Key
		{
			get
			{
				return this.m_key;
			}
		}

		public string Value
		{
			get
			{
				return this.m_value;
			}
		}

		private string m_key;

		private string m_value;
	}
}
