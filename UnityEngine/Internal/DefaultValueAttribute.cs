using System;

namespace UnityEngine.Internal
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
	[Serializable]
	public class DefaultValueAttribute : Attribute
	{
		public DefaultValueAttribute(string value)
		{
			this.DefaultValue = value;
		}

		public object Value
		{
			get
			{
				return this.DefaultValue;
			}
		}

		public override bool Equals(object obj)
		{
			DefaultValueAttribute defaultValueAttribute = obj as DefaultValueAttribute;
			bool flag;
			if (defaultValueAttribute == null)
			{
				flag = false;
			}
			else if (this.DefaultValue == null)
			{
				flag = defaultValueAttribute.Value == null;
			}
			else
			{
				flag = this.DefaultValue.Equals(defaultValueAttribute.Value);
			}
			return flag;
		}

		public override int GetHashCode()
		{
			int num;
			if (this.DefaultValue == null)
			{
				num = base.GetHashCode();
			}
			else
			{
				num = this.DefaultValue.GetHashCode();
			}
			return num;
		}

		private object DefaultValue;
	}
}
