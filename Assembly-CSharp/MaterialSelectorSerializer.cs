using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/MaterialSelectorSerializer")]
public class MaterialSelectorSerializer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.previouslySelectedElementsPerWorld == null)
		{
			this.previouslySelectedElementsPerWorld = new List<Dictionary<Tag, Tag>>[255];
			if (this.previouslySelectedElements != null)
			{
				foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
				{
					List<Dictionary<Tag, Tag>> list = this.previouslySelectedElements.ConvertAll<Dictionary<Tag, Tag>>((Dictionary<Tag, Tag> input) => new Dictionary<Tag, Tag>(input));
					this.previouslySelectedElementsPerWorld[worldContainer.id] = list;
				}
				this.previouslySelectedElements = null;
			}
		}
	}

	public void WipeWorldSelectionData(int worldID)
	{
		this.previouslySelectedElementsPerWorld[worldID] = null;
	}

	public void SetSelectedElement(int worldID, int selectorIndex, Tag recipe, Tag element)
	{
		if (this.previouslySelectedElementsPerWorld[worldID] == null)
		{
			this.previouslySelectedElementsPerWorld[worldID] = new List<Dictionary<Tag, Tag>>();
		}
		List<Dictionary<Tag, Tag>> list = this.previouslySelectedElementsPerWorld[worldID];
		while (list.Count <= selectorIndex)
		{
			list.Add(new Dictionary<Tag, Tag>());
		}
		list[selectorIndex][recipe] = element;
	}

	public Tag GetPreviousElement(int worldID, int selectorIndex, Tag recipe)
	{
		Tag invalid = Tag.Invalid;
		if (this.previouslySelectedElementsPerWorld[worldID] == null)
		{
			return invalid;
		}
		List<Dictionary<Tag, Tag>> list = this.previouslySelectedElementsPerWorld[worldID];
		if (list.Count <= selectorIndex)
		{
			return invalid;
		}
		list[selectorIndex].TryGetValue(recipe, out invalid);
		return invalid;
	}

	[Serialize]
	private List<Dictionary<Tag, Tag>> previouslySelectedElements;

	[Serialize]
	private List<Dictionary<Tag, Tag>>[] previouslySelectedElementsPerWorld;
}
