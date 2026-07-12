using System;

namespace Klei.Actions
{
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public class ActionTypeAttribute : Attribute
	{
		public ActionTypeAttribute(string groupName, string typeName, bool generateConfig = true)
		{
			this.TypeName = typeName;
			this.GroupName = groupName;
			this.GenerateConfig = generateConfig;
		}

		public static bool operator ==(ActionTypeAttribute lhs, ActionTypeAttribute rhs)
		{
			bool flag = object.Equals(lhs, null);
			bool flag2 = object.Equals(rhs, null);
			if (flag || flag2)
			{
				return flag == flag2;
			}
			return lhs.TypeName == rhs.TypeName && lhs.GroupName == rhs.GroupName;
		}

		public static bool operator !=(ActionTypeAttribute lhs, ActionTypeAttribute rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public readonly string TypeName;

		public readonly string GroupName;

		public readonly bool GenerateConfig;
	}
}
