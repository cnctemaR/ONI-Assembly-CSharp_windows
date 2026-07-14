using System;
using UnityEngine;

public class DissolvingElementDiseaseEmitter : DiseaseEmitter
{
	public SimHashes DissolveTargetElement { get; private set; }

	public float CurrentAverageDissolveRate { get; private set; }

	public PrimaryElement PrimaryElement { get; private set; }

	public DissolvingElementDiseaseEmitter(SimHashes dissolveTargetElement)
	{
		this.DissolveTargetElement = dissolveTargetElement;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Init();
		this.UpdateStatusItem();
	}

	private void Init()
	{
		this.PrimaryElement = base.GetComponent<PrimaryElement>();
	}

	private void Update()
	{
		this.EvaluateEmissionCondition();
		this.MakeDiseaseAndBubbles();
	}

	protected virtual void EvaluateEmissionCondition()
	{
	}

	protected void UpdateStatusItem()
	{
		if (this.enableEmitter)
		{
			this.statusItemGUID = base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.DissolvingElementDissolving, this);
			return;
		}
		this.statusItemGUID = base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.DissolvingElementDormant, this);
	}

	protected void MakeDiseaseAndBubbles()
	{
		if (Time.deltaTime == 0f)
		{
			return;
		}
		if (this.enableEmitter)
		{
			this.CurrentAverageDissolveRate = this.massDecayScale * Mathf.Clamp(this.PrimaryElement.Mass, 200f, 1000f);
			float num = this.CurrentAverageDissolveRate * Time.deltaTime;
			if (this.PrimaryElement.Mass > num)
			{
				this.massDecayAccumulation += num;
				if (this.massDecayAccumulation >= this.minimumBubbleSize)
				{
					float num2 = Mathf.Min(this.PrimaryElement.Mass, this.massDecayAccumulation);
					this.PrimaryElement.Mass -= num2;
					BubbleManager.instance.SpawnBubble(this.DissolveTargetElement, base.transform.position, num2 * this.massConversionRatio, this.PrimaryElement.Temperature, BubbleManager.Disease.None, null);
					this.massDecayAccumulation = 0f;
					this.SpawnVisualFX();
					return;
				}
			}
			else
			{
				this.SpawnVisualFX();
				global::UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	protected void SpawnVisualFX()
	{
		if (this.spawnFXHash != SpawnFXHashes.None)
		{
			base.transform.GetPosition().z = Grid.GetLayerZ(Grid.SceneLayer.Front);
			Game.Instance.SpawnFX(this.spawnFXHash, base.transform.GetPosition(), 0f);
		}
	}

	protected SpawnFXHashes spawnFXHash = SpawnFXHashes.OxygenEmissionBubbles;

	protected float massDecayScale = 0.0001f;

	protected float massConversionRatio = 0.5f;

	protected const float massForMinEmission = 200f;

	protected const float massForMaxEmission = 1000f;

	private float minimumBubbleSize = 0.1f;

	private Guid statusItemGUID;

	protected float massDecayAccumulation;
}
