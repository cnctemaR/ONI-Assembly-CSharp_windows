using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public sealed class CustomAttributeData
	{
		internal CustomAttributeData(ConstructorInfo ctorInfo, object[] ctorArgs, object[] namedArgs)
		{
			this.ctorInfo = ctorInfo;
			this.ctorArgs = Array.AsReadOnly<CustomAttributeTypedArgument>((ctorArgs == null) ? new CustomAttributeTypedArgument[0] : CustomAttributeData.UnboxValues<CustomAttributeTypedArgument>(ctorArgs));
			this.namedArgs = Array.AsReadOnly<CustomAttributeNamedArgument>((namedArgs == null) ? new CustomAttributeNamedArgument[0] : CustomAttributeData.UnboxValues<CustomAttributeNamedArgument>(namedArgs));
		}

		[ComVisible(true)]
		public ConstructorInfo Constructor
		{
			get
			{
				return this.ctorInfo;
			}
		}

		[ComVisible(true)]
		public IList<CustomAttributeTypedArgument> ConstructorArguments
		{
			get
			{
				return this.ctorArgs;
			}
		}

		public IList<CustomAttributeNamedArgument> NamedArguments
		{
			get
			{
				return this.namedArgs;
			}
		}

		public static IList<CustomAttributeData> GetCustomAttributes(Assembly target)
		{
			return MonoCustomAttrs.GetCustomAttributesData(target);
		}

		public static IList<CustomAttributeData> GetCustomAttributes(MemberInfo target)
		{
			return MonoCustomAttrs.GetCustomAttributesData(target);
		}

		public static IList<CustomAttributeData> GetCustomAttributes(Module target)
		{
			return MonoCustomAttrs.GetCustomAttributesData(target);
		}

		public static IList<CustomAttributeData> GetCustomAttributes(ParameterInfo target)
		{
			return MonoCustomAttrs.GetCustomAttributesData(target);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[" + this.ctorInfo.DeclaringType.FullName + "(");
			for (int i = 0; i < this.ctorArgs.Count; i++)
			{
				stringBuilder.Append(this.ctorArgs[i].ToString());
				if (i + 1 < this.ctorArgs.Count)
				{
					stringBuilder.Append(", ");
				}
			}
			if (this.namedArgs.Count > 0)
			{
				stringBuilder.Append(", ");
			}
			for (int j = 0; j < this.namedArgs.Count; j++)
			{
				stringBuilder.Append(this.namedArgs[j].ToString());
				if (j + 1 < this.namedArgs.Count)
				{
					stringBuilder.Append(", ");
				}
			}
			stringBuilder.AppendFormat(")]", new object[0]);
			return stringBuilder.ToString();
		}

		private static T[] UnboxValues<T>(object[] values)
		{
			T[] array = new T[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				array[i] = (T)((object)values[i]);
			}
			return array;
		}

		public override bool Equals(object obj)
		{
			CustomAttributeData customAttributeData = obj as CustomAttributeData;
			if (customAttributeData == null || customAttributeData.ctorInfo != this.ctorInfo || customAttributeData.ctorArgs.Count != this.ctorArgs.Count || customAttributeData.namedArgs.Count != this.namedArgs.Count)
			{
				return false;
			}
			for (int i = 0; i < this.ctorArgs.Count; i++)
			{
				if (this.ctorArgs[i].Equals(customAttributeData.ctorArgs[i]))
				{
					return false;
				}
			}
			for (int j = 0; j < this.namedArgs.Count; j++)
			{
				bool flag = false;
				for (int k = 0; k < customAttributeData.namedArgs.Count; k++)
				{
					if (this.namedArgs[j].Equals(customAttributeData.namedArgs[k]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = this.ctorInfo.GetHashCode() << 16;
			for (int i = 0; i < this.ctorArgs.Count; i++)
			{
				num += num ^ (7 + this.ctorArgs[i].GetHashCode() << i * 4);
			}
			for (int j = 0; j < this.namedArgs.Count; j++)
			{
				num += this.namedArgs[j].GetHashCode() << 5;
			}
			return num;
		}

		private ConstructorInfo ctorInfo;

		private IList<CustomAttributeTypedArgument> ctorArgs;

		private IList<CustomAttributeNamedArgument> namedArgs;
	}
}
