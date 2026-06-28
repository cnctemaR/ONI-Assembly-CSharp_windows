using System;
using UnityEngine;

public class MicrobeMusher : Fabricator
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreType = Db.Get().ChoreTypes.Mush;
		this.inStorage.choreType = Db.Get().ChoreTypes.MushFetch;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Mushing;
		this.attributeConverter = Db.Get().AttributeConverters.CookingSpeed;
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, new string[] { "meter_target", "meter_ration" });
		this.meter.meterController.HideSymbol(MicrobeMusher.canHash, true);
		this.meter.meterController.HideSymbol(MicrobeMusher.meterRationHash, true);
		this.meter.meterController.GetComponent<KBatchedAnimTracker>().skipInitialDisable = true;
	}

	protected override void OnBuildQueued(Fabricator.MachineOrder order)
	{
		base.OnBuildQueued(order);
		this.InstantiateVisualizer(order);
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

	protected override GameObject CompleteOrder(Fabricator.UserOrder completed_order)
	{
		GameObject gameObject = base.CompleteOrder(completed_order);
		gameObject.transform.Translate(this.mushbarSpawnOffset);
		gameObject.SetActive(true);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		if (component != null && component.DiseaseCount > 0)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_DiseaseCooking);
		}
		this.workTimeRemaining = this.GetWorkTime();
		this.UpdateMeter();
		return gameObject;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.DestroyVisualizer();
		base.OnCompleteWork(worker);
	}

	public override void CancelOrder(int idx)
	{
		if (idx == 0)
		{
			this.DestroyVisualizer();
		}
		base.CancelOrder(idx);
		this.UpdateMeter();
	}

	private void InstantiateVisualizer(Fabricator.MachineOrder order)
	{
		if (this.visualizer != null)
		{
			this.DestroyVisualizer();
		}
		this.visualizer = Util.KInstantiate(order.parentOrder.recipe.FabricationVisualizer, null, null);
		this.visualizer.transform.parent = this.meter.meterController.transform;
		this.visualizer.transform.localPosition = new Vector3(0f, 0f, 1f);
		this.visualizer.SetActive(true);
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		KBatchedAnimController component2 = this.visualizer.GetComponent<KBatchedAnimController>();
		if (this.visualizerLink != null)
		{
			this.visualizerLink.Unregister();
			this.visualizerLink = null;
		}
		this.visualizerLink = new KAnimLink(component, component2);
	}

	private void DestroyVisualizer()
	{
		if (this.visualizer != null)
		{
			if (this.visualizerLink != null)
			{
				this.visualizerLink.Unregister();
				this.visualizerLink = null;
			}
			Util.KDestroyGameObject(this.visualizer);
			this.visualizer = null;
		}
	}

	[SerializeField]
	public Vector3 mushbarSpawnOffset = Vector3.right;

	private MeterController meter;

	private GameObject visualizer;

	private KAnimLink visualizerLink;

	private static readonly KAnimHashedString meterRationHash = new KAnimHashedString("meter_ration");

	private static readonly KAnimHashedString canHash = new KAnimHashedString("can");
}
