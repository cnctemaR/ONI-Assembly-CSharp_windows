using System;
using TUNING;
using UnityEngine;

public class MicrobeMusher : Fabricator
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreType = Db.Get().ChoreTypes.Cook;
		this.choreTags = GameTags.ChoreTypes.CookingChores;
		this.fetchChoreTypeIdHash = Db.Get().ChoreTypes.CookFetch.IdHash;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Mushing;
		this.attributeConverter = Db.Get().AttributeConverters.CookingSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, new string[] { "meter_target", "meter_ration" });
		this.meter.meterController.SetSymbolVisiblity(MicrobeMusher.canHash, false);
		this.meter.meterController.SetSymbolVisiblity(MicrobeMusher.meterRationHash, false);
		this.meter.meterController.GetComponent<KBatchedAnimTracker>().skipInitialDisable = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("WaterFetchingTutorial", 2f, delegate(object obj)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_FetchingWater);
		}, null, null);
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(Cook.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
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
		gameObject.transform.SetPosition(gameObject.transform.GetPosition() + this.mushbarSpawnOffset);
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
		this.visualizer.transform.SetLocalPosition(new Vector3(0f, 0f, 1f));
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
