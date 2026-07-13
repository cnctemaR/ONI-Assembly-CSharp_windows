using System;
using UnityEngine;

public class PlantFiberProducer : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(1272413801, new Action<object>(this.OnHarvest));
	}

	protected override void OnCleanUp()
	{
		base.Unsubscribe(1272413801, new Action<object>(this.OnHarvest));
	}

	private void OnHarvest(object obj)
	{
		Harvestable harvestable = (Harvestable)obj;
		if (harvestable != null && harvestable.completed_by != null && harvestable.completed_by.GetComponent<MinionResume>().HasPerk(Db.Get().SkillPerks.CanSalvagePlantFiber))
		{
			this.SpawnPlantFiber();
		}
	}

	private GameObject SpawnPlantFiber()
	{
		Vector3 vector = base.gameObject.transform.GetPosition() + new Vector3(0f, 0.5f, 0f);
		GameObject prefab = Assets.GetPrefab(new Tag("PlantFiber"));
		GameObject gameObject = GameUtil.KInstantiate(prefab, vector, Grid.SceneLayer.Ore, null, 0);
		PrimaryElement component = base.gameObject.GetComponent<PrimaryElement>();
		PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
		component2.Temperature = component.Temperature;
		component2.Mass = this.amount;
		gameObject.SetActive(true);
		string properName = gameObject.GetProperName();
		PopFXManager.Instance.SpawnFX(Def.GetUISprite(prefab, "ui", false).first, PopFXManager.Instance.sprite_Plus, properName, gameObject.transform, Vector3.zero, 1.5f, true, false, false);
		return gameObject;
	}

	public float amount;
}
