using System;
using System.Reflection;

namespace System.Xml.Serialization
{
	internal class ConstantModel
	{
		internal ConstantModel(FieldInfo fieldInfo, long value)
		{
			this.fieldInfo = fieldInfo;
			this.value = value;
		}

		internal string Name
		{
			get
			{
				return this.fieldInfo.Name;
			}
		}

		internal long Value
		{
			get
			{
				return this.value;
			}
		}

		internal FieldInfo FieldInfo
		{
			get
			{
				return this.fieldInfo;
			}
		}

		private FieldInfo fieldInfo;

		private long value;
	}
}
