using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.Runtime.Serialization
{
	internal class CollectionDataNode : DataNode<Array>
	{
		internal CollectionDataNode()
		{
			this.dataType = Globals.TypeOfCollectionDataNode;
		}

		internal IList<IDataNode> Items
		{
			get
			{
				return this.items;
			}
			set
			{
				this.items = value;
			}
		}

		internal string ItemName
		{
			get
			{
				return this.itemName;
			}
			set
			{
				this.itemName = value;
			}
		}

		internal string ItemNamespace
		{
			get
			{
				return this.itemNamespace;
			}
			set
			{
				this.itemNamespace = value;
			}
		}

		internal int Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		public override void GetData(ElementData element)
		{
			base.GetData(element);
			element.AddAttribute("z", "http://schemas.microsoft.com/2003/10/Serialization/", "Size", this.Size.ToString(NumberFormatInfo.InvariantInfo));
		}

		public override void Clear()
		{
			base.Clear();
			this.items = null;
			this.size = -1;
		}

		private IList<IDataNode> items;

		private string itemName;

		private string itemNamespace;

		private int size = -1;
	}
}
