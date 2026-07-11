using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	[AddComponentMenu("KMonoBehaviour/scripts/PrefabAttributeModifiers")]
	public class PrefabAttributeModifiers : KMonoBehaviour
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
		}

		public void AddAttributeDescriptor(AttributeModifier modifier)
		{
			this.descriptors.Add(modifier);
		}

		public void RemovePrefabAttribute(AttributeModifier modifier)
		{
			this.descriptors.Remove(modifier);
		}

		public List<AttributeModifier> descriptors = new List<AttributeModifier>();
	}
}
