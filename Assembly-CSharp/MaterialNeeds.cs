using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MaterialNeeds")]
public class MaterialNeeds : KMonoBehaviour
{
	public static MaterialNeeds Instance { get; private set; }

	public static void DestroyInstance()
	{
		MaterialNeeds.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		MaterialNeeds.Instance = this;
	}

	public void UpdateNeed(Tag tag, float amount)
	{
		float num = 0f;
		if (!this.Needs.TryGetValue(tag, out num))
		{
			this.Needs[tag] = 0f;
		}
		this.Needs[tag] = num + amount;
	}

	public float GetAmount(Tag tag)
	{
		float num = 0f;
		this.Needs.TryGetValue(tag, out num);
		return num;
	}

	public Dictionary<Tag, float> GetNeeds()
	{
		return this.Needs;
	}

	private Dictionary<Tag, float> Needs = new Dictionary<Tag, float>();

	public global::System.Action OnDirty;
}
