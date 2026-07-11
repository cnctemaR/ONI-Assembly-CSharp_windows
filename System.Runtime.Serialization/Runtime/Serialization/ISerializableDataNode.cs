using System;
using System.Collections.Generic;

namespace System.Runtime.Serialization
{
	internal class ISerializableDataNode : DataNode<object>
	{
		internal ISerializableDataNode()
		{
			this.dataType = Globals.TypeOfISerializableDataNode;
		}

		internal string FactoryTypeName
		{
			get
			{
				return this.factoryTypeName;
			}
			set
			{
				this.factoryTypeName = value;
			}
		}

		internal string FactoryTypeNamespace
		{
			get
			{
				return this.factoryTypeNamespace;
			}
			set
			{
				this.factoryTypeNamespace = value;
			}
		}

		internal IList<ISerializableDataMember> Members
		{
			get
			{
				return this.members;
			}
			set
			{
				this.members = value;
			}
		}

		public override void GetData(ElementData element)
		{
			base.GetData(element);
			if (this.FactoryTypeName != null)
			{
				base.AddQualifiedNameAttribute(element, "z", "FactoryType", "http://schemas.microsoft.com/2003/10/Serialization/", this.FactoryTypeName, this.FactoryTypeNamespace);
			}
		}

		public override void Clear()
		{
			base.Clear();
			this.members = null;
			this.factoryTypeName = (this.factoryTypeNamespace = null);
		}

		private string factoryTypeName;

		private string factoryTypeNamespace;

		private IList<ISerializableDataMember> members;
	}
}
