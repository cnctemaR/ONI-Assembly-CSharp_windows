using System;

namespace System.Runtime.Serialization
{
	internal class ElementData
	{
		public void AddAttribute(string prefix, string ns, string name, string value)
		{
			this.GrowAttributesIfNeeded();
			AttributeData attributeData = this.attributes[this.attributeCount];
			if (attributeData == null)
			{
				attributeData = (this.attributes[this.attributeCount] = new AttributeData());
			}
			attributeData.prefix = prefix;
			attributeData.ns = ns;
			attributeData.localName = name;
			attributeData.value = value;
			this.attributeCount++;
		}

		private void GrowAttributesIfNeeded()
		{
			if (this.attributes == null)
			{
				this.attributes = new AttributeData[4];
				return;
			}
			if (this.attributes.Length == this.attributeCount)
			{
				AttributeData[] array = new AttributeData[this.attributes.Length * 2];
				Array.Copy(this.attributes, 0, array, 0, this.attributes.Length);
				this.attributes = array;
			}
		}

		public string localName;

		public string ns;

		public string prefix;

		public int attributeCount;

		public AttributeData[] attributes;

		public IDataNode dataNode;

		public int childElementIndex;
	}
}
