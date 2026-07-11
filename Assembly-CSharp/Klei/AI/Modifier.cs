using System;
using System.Collections.Generic;

namespace Klei.AI
{
	public class Modifier : Resource
	{
		public Modifier(string id, string name, string description)
			: base(id, name)
		{
			this.description = description;
		}

		public void Add(AttributeModifier modifier)
		{
			if (modifier.AttributeId != string.Empty)
			{
				this.SelfModifiers.Add(modifier);
			}
		}

		public virtual void AddTo(Attributes attributes)
		{
			foreach (AttributeModifier attributeModifier in this.SelfModifiers)
			{
				attributes.Add(attributeModifier);
			}
		}

		public virtual void RemoveFrom(Attributes attributes)
		{
			foreach (AttributeModifier attributeModifier in this.SelfModifiers)
			{
				attributes.Remove(attributeModifier);
			}
		}

		public string description;

		public List<AttributeModifier> SelfModifiers = new List<AttributeModifier>();
	}
}
