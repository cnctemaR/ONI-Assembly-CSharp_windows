using System;
using System.Collections.Generic;
using FileHelpers;
using UnityEngine;

public class TagModifierLoader : KMonoBehaviour
{
	public static TagModifierLoader Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		TagModifierLoader.Instance = this;
		FileHelperEngine fileHelperEngine = new FileHelperEngine(typeof(TagModifierLoader.TagModifierConfig));
		TagModifierLoader.TagModifierConfig[] array = (TagModifierLoader.TagModifierConfig[])fileHelperEngine.ReadString(this.tagModifiersAsset.text);
		this.durablityModifiers = new Dictionary<int, float>();
		this.beautyModifiers = new Dictionary<int, float>();
		for (int i = 0; i < array.Length; i++)
		{
			Tag tag = TagManager.Create(array[i].tag, null);
			this.durablityModifiers.Add(tag.GetHashCode(), array[i].durablityModifier);
			this.beautyModifiers.Add(tag.GetHashCode(), array[i].beautyModifier);
		}
	}

	private float GetModifier(List<Tag> tags, Dictionary<int, float> modifiers)
	{
		float num = 0f;
		for (int i = 0; i < tags.Count; i++)
		{
			float num2 = 0f;
			modifiers.TryGetValue(tags[i].GetHashCode(), out num2);
			num += num2;
		}
		return num;
	}

	public float GetDurabilityModifier(List<Tag> tags)
	{
		float num;
		if (tags == null || tags.Count == 0)
		{
			num = 0f;
		}
		else
		{
			num = this.GetModifier(tags, this.durablityModifiers);
		}
		return num;
	}

	public float GetBeautyModifier(List<Tag> tags)
	{
		float num;
		if (tags == null || tags.Count == 0)
		{
			num = 0f;
		}
		else
		{
			num = this.GetModifier(tags, this.beautyModifiers);
		}
		return num;
	}

	[SerializeField]
	private TextAsset tagModifiersAsset;

	private Dictionary<int, float> durablityModifiers;

	private Dictionary<int, float> beautyModifiers;

	[DelimitedRecord(",")]
	[IgnoreFirst(1)]
	[IgnoreEmptyLines]
	private class TagModifierConfig
	{
		[FieldOrder(1)]
		public string tag;

		[FieldOrder(2)]
		public float durablityModifier;

		[FieldOrder(3)]
		public float beautyModifier;
	}
}
