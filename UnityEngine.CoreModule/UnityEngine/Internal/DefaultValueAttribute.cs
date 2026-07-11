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
			bool flag = defaultValueAttribute == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this.DefaultValue == null;
				if (flag3)
				{
					flag2 = defaultValueAttribute.Value == null;
				}
				else
				{
					flag2 = this.DefaultValue.Equals(defaultValueAttribute.Value);
				}
			}
			return flag2;
		}

		public override int GetHashCode()
		{
			bool flag = this.DefaultValue == null;
			int num;
			if (flag)
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
