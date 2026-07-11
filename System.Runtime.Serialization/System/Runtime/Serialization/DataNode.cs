using System;
using System.Globalization;

namespace System.Runtime.Serialization
{
	internal class DataNode<T> : IDataNode
	{
		internal DataNode()
		{
			this.dataType = typeof(T);
			this.isFinalValue = true;
		}

		internal DataNode(T value)
			: this()
		{
			this.value = value;
		}

		public Type DataType
		{
			get
			{
				return this.dataType;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = (T)((object)value);
			}
		}

		bool IDataNode.IsFinalValue
		{
			get
			{
				return this.isFinalValue;
			}
			set
			{
				this.isFinalValue = value;
			}
		}

		public T GetValue()
		{
			return this.value;
		}

		public string DataContractName
		{
			get
			{
				return this.dataContractName;
			}
			set
			{
				this.dataContractName = value;
			}
		}

		public string DataContractNamespace
		{
			get
			{
				return this.dataContractNamespace;
			}
			set
			{
				this.dataContractNamespace = value;
			}
		}

		public string ClrTypeName
		{
			get
			{
				return this.clrTypeName;
			}
			set
			{
				this.clrTypeName = value;
			}
		}

		public string ClrAssemblyName
		{
			get
			{
				return this.clrAssemblyName;
			}
			set
			{
				this.clrAssemblyName = value;
			}
		}

		public bool PreservesReferences
		{
			get
			{
				return this.Id != Globals.NewObjectId;
			}
		}

		public string Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		public virtual void GetData(ElementData element)
		{
			element.dataNode = this;
			element.attributeCount = 0;
			element.childElementIndex = 0;
			if (this.DataContractName != null)
			{
				this.AddQualifiedNameAttribute(element, "i", "type", "http://www.w3.org/2001/XMLSchema-instance", this.DataContractName, this.DataContractNamespace);
			}
			if (this.ClrTypeName != null)
			{
				element.AddAttribute("z", "http://schemas.microsoft.com/2003/10/Serialization/", "Type", this.ClrTypeName);
			}
			if (this.ClrAssemblyName != null)
			{
				element.AddAttribute("z", "http://schemas.microsoft.com/2003/10/Serialization/", "Assembly", this.ClrAssemblyName);
			}
		}

		public virtual void Clear()
		{
			this.clrTypeName = (this.clrAssemblyName = null);
		}

		internal void AddQualifiedNameAttribute(ElementData element, string elementPrefix, string elementName, string elementNs, string valueName, string valueNs)
		{
			string prefix = ExtensionDataReader.GetPrefix(valueNs);
			element.AddAttribute(elementPrefix, elementNs, elementName, string.Format(CultureInfo.InvariantCulture, "{0}:{1}", prefix, valueName));
			bool flag = false;
			if (element.attributes != null)
			{
				for (int i = 0; i < element.attributes.Length; i++)
				{
					AttributeData attributeData = element.attributes[i];
					if (attributeData != null && attributeData.prefix == "xmlns" && attributeData.localName == prefix)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				element.AddAttribute("xmlns", "http://www.w3.org/2000/xmlns/", prefix, valueNs);
			}
		}

		protected Type dataType;

		private T value;

		private string dataContractName;

		private string dataContractNamespace;

		private string clrTypeName;

		private string clrAssemblyName;

		private string id = Globals.NewObjectId;

		private bool isFinalValue;
	}
}
