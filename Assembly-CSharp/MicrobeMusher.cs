using System;
using Klei.AI;
using UnityEngine;

public class MicrobeMusher : Fabricator
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreType = Db.Get().ChoreTypes.Mush;
		this.inStorage.choreType = Db.Get().ChoreTypes.MushFetch;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Mushing;
		this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, new string[] { "meter_target", "meter_ration" });
		this.meter.meterController.HideSymbol(new KAnimHashedString("can"), true);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("WaterFetchingTutorial", 2f, delegate(object obj)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_FetchingWater);
		}, null, null);
	}

	protected override void OnBuildQueued()
	{
		base.OnBuildQueued();
		this.UpdateMeter();
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		this.UpdateMeter();
		return false;
	}

	private void UpdateMeter()
	{
		float workTime = this.GetWorkTime();
		float num = (workTime - this.workTimeRemaining) / workTime;
		this.meter.SetPositionPercent(num);
	}

	protected override void CompleteOrder(Fabricator.UserOrder completed_order)
	{
		this.workTimeRemaining = this.GetWorkTime();
		this.UpdateMeter();
		int num = Grid.PosToCell(this);
		for (int i = 0; i < 1; i++)
		{
			int num2 = Grid.OffsetCell(num, new CellOffset(0, i));
			GameObject gameObject = completed_order.recipe.Craft(this.buildStorage, completed_order.orderTags);
			gameObject.transform.SetPosition(Grid.CellToPosCCC(num2, Grid.SceneLayer.Move) + this.mushbarSpawnOffset);
			gameObject.SetActive(true);
			gameObject.GetComponent<KMonoBehaviour>().Trigger(748399584, null);
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		worker.GetComponent<Effects>().Add("DirtyHands", true);
	}

	[SerializeField]
	public Vector3 mushbarSpawnOffset = Vector3.right;

	private MeterController meter;
}
