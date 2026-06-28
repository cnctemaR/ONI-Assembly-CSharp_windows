using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class PlantState : KMonoBehaviour, ISaveLoadableJson
{
	protected override void OnSpawn()
	{
		this.Refresh();
		this.Subscribe(1272413801, new EventSystem.EventHandler(this.OnHarvest));
	}

	private void Refresh()
	{
		int num = Math.Min((int)(this.growth * (float)this.stageCount), this.stageCount);
		this.plantRenderer.SetStage(num);
	}

	private void Update()
	{
		this.growth += Time.deltaTime * this.growthRate;
		this.growth = Mathf.Min(this.growth, 1f);
		this.Refresh();
	}

	private void OnHarvest(object data)
	{
		if (this.seedPrefab != null)
		{
			GameObject gameObject = Util.KInstantiate(this.seedPrefab, null, null);
			gameObject.SetActive(true);
			gameObject.transform.SetPosition(this.transform.position);
		}
		this.DeleteObject();
	}

	public bool IsDead()
	{
		return this.isDead;
	}

	public int stageCount;

	public float growthRate;

	public GameObject seedPrefab;

	[MyCmpReq]
	private PlantRenderer plantRenderer;

	[Serialize]
	private bool isDead;

	[Serialize]
	private float growth;
}
