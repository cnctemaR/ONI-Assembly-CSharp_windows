using System;
using System.Collections.Generic;
using UnityEngine;

public class RefinementRecipe
{
	public RefinementRecipe()
	{
		this.results = new List<RefinementRecipe.Result>();
		RefineryRecipeManager.Get().Add(this);
	}

	public RefinementRecipe AddResult(Tag tag, float amount)
	{
		this.results.Add(new RefinementRecipe.Result(tag, amount));
		return this;
	}

	public float TotalResultMass()
	{
		float num = 0f;
		foreach (RefinementRecipe.Result result in this.results)
		{
			num += result.amount;
		}
		return num;
	}

	public Sprite GetUIIcon()
	{
		Sprite sprite = null;
		GameObject prefab = Assets.GetPrefab(this.material);
		KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			sprite = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], component.initialAnim);
		}
		return sprite;
	}

	public Color GetUIColor()
	{
		return Color.white;
	}

	public Tag material;

	public float amount;

	public List<RefinementRecipe.Result> results;

	public float time;

	public GameObject FabricationVisualizer;

	public string description;

	public List<Tag> fabricators;

	public int sortOrder = 0;

	public class Result
	{
		public Result(Tag tag, float amount)
		{
			this.tag = tag;
			this.amount = amount;
		}

		public Tag tag;

		public float amount;
	}
}
