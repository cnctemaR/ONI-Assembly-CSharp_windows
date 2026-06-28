using System;
using System.Collections.Generic;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class GasFilterable : Filterable
{
	protected override IList<Tag> GetTagOptions()
	{
		List<Tag> list = new List<Tag>();
		foreach (Element element in ElementLoader.elements)
		{
			if (element.IsGas)
			{
				Tag tag = TagManager.Create(element.id);
				list.Add(tag);
			}
		}
		return list;
	}
}
