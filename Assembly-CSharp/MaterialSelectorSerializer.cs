using System;
using System.Collections.Generic;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class MaterialSelectorSerializer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.previouslySelectedElements == null)
		{
			this.previouslySelectedElements = new List<Dictionary<Tag, Tag>>();
		}
	}

	public void SetSelectedElement(int selectorIndex, Tag recipe, Tag element)
	{
		while (this.previouslySelectedElements.Count <= selectorIndex)
		{
			this.previouslySelectedElements.Add(new Dictionary<Tag, Tag>());
		}
		this.previouslySelectedElements[selectorIndex][recipe] = element;
	}

	public Tag GetPreviousElement(int selectorIndex, Tag recipe)
	{
		Tag invalid = Tag.Invalid;
		if (this.previouslySelectedElements.Count <= selectorIndex)
		{
			return invalid;
		}
		this.previouslySelectedElements[selectorIndex].TryGetValue(recipe, out invalid);
		return invalid;
	}

	[Serialize]
	private List<Dictionary<Tag, Tag>> previouslySelectedElements;
}
