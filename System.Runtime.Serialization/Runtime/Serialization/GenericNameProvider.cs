using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class GenericNameProvider : IGenericNameProvider
	{
		internal GenericNameProvider(Type type)
			: this(DataContract.GetClrTypeFullName(type.GetGenericTypeDefinition()), type.GetGenericArguments())
		{
		}

		internal GenericNameProvider(string genericTypeName, object[] genericParams)
		{
			this.genericTypeName = genericTypeName;
			this.genericParams = new object[genericParams.Length];
			genericParams.CopyTo(this.genericParams, 0);
			string text;
			string text2;
			DataContract.GetClrNameAndNamespace(genericTypeName, out text, out text2);
			this.nestedParamCounts = DataContract.GetDataContractNameForGenericName(text, null);
		}

		public int GetParameterCount()
		{
			return this.genericParams.Length;
		}

		public IList<int> GetNestedParameterCounts()
		{
			return this.nestedParamCounts;
		}

		public string GetParameterName(int paramIndex)
		{
			return this.GetStableName(paramIndex).Name;
		}

		public string GetNamespaces()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.GetParameterCount(); i++)
			{
				stringBuilder.Append(" ").Append(this.GetStableName(i).Namespace);
			}
			return stringBuilder.ToString();
		}

		public string GetGenericTypeName()
		{
			return this.genericTypeName;
		}

		public bool ParametersFromBuiltInNamespaces
		{
			get
			{
				bool flag = true;
				int num = 0;
				while (num < this.GetParameterCount() && flag)
				{
					flag = DataContract.IsBuiltInNamespace(this.GetStableName(num).Namespace);
					num++;
				}
				return flag;
			}
		}

		private XmlQualifiedName GetStableName(int i)
		{
			object obj = this.genericParams[i];
			XmlQualifiedName xmlQualifiedName = obj as XmlQualifiedName;
			if (xmlQualifiedName == null)
			{
				Type type = obj as Type;
				if (type != null)
				{
					xmlQualifiedName = (this.genericParams[i] = DataContract.GetStableName(type));
				}
				else
				{
					xmlQualifiedName = (this.genericParams[i] = ((DataContract)obj).StableName);
				}
			}
			return xmlQualifiedName;
		}

		private string genericTypeName;

		private object[] genericParams;

		private IList<int> nestedParamCounts;
	}
}
