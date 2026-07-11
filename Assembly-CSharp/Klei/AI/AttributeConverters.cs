using System;
using System.Collections.Generic;

namespace Klei.AI
{
	public class AttributeConverters : KMonoBehaviour
	{
		public int Count
		{
			get
			{
				return this.converters.Count;
			}
		}

		protected override void OnPrefabInit()
		{
			foreach (AttributeInstance attributeInstance in this.GetAttributes())
			{
				foreach (AttributeConverter attributeConverter in attributeInstance.Attribute.converters)
				{
					AttributeConverterInstance attributeConverterInstance = new AttributeConverterInstance(base.gameObject, attributeConverter, attributeInstance);
					this.converters.Add(attributeConverterInstance);
				}
			}
		}

		public AttributeConverterInstance Get(AttributeConverter converter)
		{
			foreach (AttributeConverterInstance attributeConverterInstance in this.converters)
			{
				if (attributeConverterInstance.converter == converter)
				{
					return attributeConverterInstance;
				}
			}
			return null;
		}

		public AttributeConverterInstance GetConverter(string id)
		{
			foreach (AttributeConverterInstance attributeConverterInstance in this.converters)
			{
				if (attributeConverterInstance.converter.Id == id)
				{
					return attributeConverterInstance;
				}
			}
			return null;
		}

		public List<AttributeConverterInstance> converters = new List<AttributeConverterInstance>();
	}
}
