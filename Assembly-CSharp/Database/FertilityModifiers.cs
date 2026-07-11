using System;
using System.Collections.Generic;
using Klei.AI;

namespace Database
{
	public class FertilityModifiers : ResourceSet<FertilityModifier>
	{
		public List<FertilityModifier> GetForTag(Tag searchTag)
		{
			List<FertilityModifier> list = new List<FertilityModifier>();
			foreach (FertilityModifier fertilityModifier in this.resources)
			{
				if (fertilityModifier.TargetTag == searchTag)
				{
					list.Add(fertilityModifier);
				}
			}
			return list;
		}
	}
}
