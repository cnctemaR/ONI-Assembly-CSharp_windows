using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class FlushToilet : StateMachineComponent<FlushToilet.SMInstance>, IUsable, IEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = base.GetComponent<Building>();
		this.inputCell = component.GetUtilityInputCell();
		this.outputCell = component.GetUtilityOutputCell();
		ConduitFlow liquidConduitFlow = Game.Instance.liquidConduitFlow;
		liquidConduitFlow.onConduitsRebuilt += this.OnConduitsRebuilt;
		liquidConduitFlow.RegisterContentListener(this.inputCell, base.gameObject);
		liquidConduitFlow.RegisterContentListener(this.outputCell, base.gameObject);
		ToiletWorkableUse component2 = base.GetComponent<ToiletWorkableUse>();
		component2.onComplete = new Action<Worker>(this.Flush);
		KBatchedAnimController component3 = base.GetComponent<KBatchedAnimController>();
		this.fillMeter = new MeterController(component3, "meter_target", "meter", Meter.Offset.Behind, new Vector3(0.4f, 3.2f, 0.1f), Vector3.zero, new string[0]);
		this.contaminationMeter = new MeterController(component3, "meter_target", "meter_dirty", Meter.Offset.Behind, new Vector3(0.4f, 3.2f, 0.1f), Vector3.zero, new string[0]);
		Components.Toilets.Add(this);
		base.smi.StartSM();
		base.smi.ShowFillMeter();
	}

	protected override void OnCleanUp()
	{
		Building component = base.GetComponent<Building>();
		ConduitFlow liquidConduitFlow = Game.Instance.liquidConduitFlow;
		liquidConduitFlow.UnregisterContentListener(component.GetUtilityInputCell(), base.gameObject);
		liquidConduitFlow.UnregisterContentListener(component.GetUtilityOutputCell(), base.gameObject);
		liquidConduitFlow.onConduitsRebuilt -= this.OnConduitsRebuilt;
		Components.Toilets.Remove(this);
		base.OnCleanUp();
	}

	private void OnConduitsRebuilt()
	{
		this.Trigger(-2094018600, null);
	}

	public bool IsUsable()
	{
		return base.smi.HasTag(GameTags.Usable);
	}

	private void Flush(Worker worker)
	{
		List<GameObject> list = this.storage.Find(FlushToilet.WaterTag);
		float num = 0f;
		float num2 = this.massConsumedPerUse;
		foreach (GameObject gameObject in list)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			float num3 = Mathf.Min(component.Mass, num2);
			component.Mass -= num3;
			num2 -= num3;
			num += num3 * component.Temperature;
		}
		float num4 = num / this.massConsumedPerUse;
		this.storage.AddLiquid(SimHashes.DirtyWater, this.massEmittedPerUse, num4, false);
	}

	public List<Descriptor> RequirementDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(SimHashes.Water);
		string text = element.tag.ProperName();
		string keywordStyle = GameUtil.GetKeywordStyle(element);
		Descriptor descriptor = new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, keywordStyle, text, GameUtil.GetFormattedMass(this.massConsumedPerUse, GameUtil.TimeSlice.None, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, keywordStyle, text, GameUtil.GetFormattedMass(this.massConsumedPerUse, GameUtil.TimeSlice.None, true, "{0:0.##}")), Descriptor.DescriptorType.Requirement, false);
		list.Add(descriptor);
		return list;
	}

	public List<Descriptor> EffectDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(SimHashes.DirtyWater);
		string text = element.tag.ProperName();
		string keywordStyle = GameUtil.GetKeywordStyle(element);
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTEDPERUSE, keywordStyle, text, GameUtil.GetFormattedMass(this.massEmittedPerUse, GameUtil.TimeSlice.None, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTEDPERUSE, keywordStyle, text, GameUtil.GetFormattedMass(this.massEmittedPerUse, GameUtil.TimeSlice.None, true, "{0:0.##}")), Descriptor.DescriptorType.Effect);
		list.Add(descriptor);
		return list;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in this.RequirementDescriptors(def))
		{
			list.Add(descriptor);
		}
		foreach (Descriptor descriptor2 in this.EffectDescriptors(def))
		{
			list.Add(descriptor2);
		}
		return list;
	}

	private MeterController fillMeter;

	private MeterController contaminationMeter;

	[SerializeField]
	public float massConsumedPerUse = 5f;

	[SerializeField]
	public float massEmittedPerUse = 5f;

	[MyCmpGet]
	private ConduitConsumer conduitConsumer;

	[MyCmpGet]
	private Storage storage;

	public static readonly Tag WaterTag = TagManager.Create(SimHashes.Water);

	private int inputCell;

	private int outputCell;

	public class SMInstance : GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.GameInstance
	{
		public SMInstance(FlushToilet master)
			: base(master)
		{
			this.UpdateFullnessState();
			this.UpdateDirtyState();
		}

		public bool OutputConduitBlocked()
		{
			return Game.Instance.liquidConduitFlow.GetContents(base.master.outputCell).mass > 0f;
		}

		public bool HasValidConnections()
		{
			return Game.Instance.liquidConduitFlow.GetConduit(base.master.inputCell) != null && Game.Instance.liquidConduitFlow.GetConduit(base.master.outputCell) != null;
		}

		public bool UpdateFullnessState()
		{
			float num = 0f;
			List<GameObject> list = base.master.storage.Find(FlushToilet.WaterTag);
			foreach (GameObject gameObject in list)
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				num += component.Mass;
			}
			bool flag = num >= base.master.massConsumedPerUse;
			base.master.conduitConsumer.enabled = !flag;
			float num2 = Mathf.Clamp01(num / base.master.massConsumedPerUse);
			base.master.fillMeter.SetPositionPercent(num2);
			return flag;
		}

		public void UpdateDirtyState()
		{
			ToiletWorkableUse component = base.GetComponent<ToiletWorkableUse>();
			float percentComplete = component.GetPercentComplete();
			base.master.contaminationMeter.SetPositionPercent(percentComplete);
		}

		public void StartFlush()
		{
			base.master.fillMeter.SetPositionPercent(0f);
			base.master.contaminationMeter.SetPositionPercent(1f);
			base.smi.ShowFillMeter();
		}

		public void ShowFillMeter()
		{
			base.master.fillMeter.gameObject.SetActive(true);
			base.master.contaminationMeter.gameObject.SetActive(false);
		}

		public void ShowContaminatedMeter()
		{
			base.master.fillMeter.gameObject.SetActive(false);
			base.master.contaminationMeter.gameObject.SetActive(true);
		}
	}

	public class States : GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.disconnected;
			base.serializable = true;
			this.disconnected.PlayAnim("off", KAnim.PlayMode.Once, null).EventTransition(GameHashes.ConduitConnectionChanged, this.backedup, (FlushToilet.SMInstance smi) => smi.HasValidConnections()).Enter(delegate(FlushToilet.SMInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(false, false);
			});
			this.backedup.PlayAnim("off", KAnim.PlayMode.Once, null).ToggleStatusItem(Db.Get().BuildingStatusItems.ConduitBlocked, null).EventTransition(GameHashes.ConduitConnectionChanged, this.disconnected, (FlushToilet.SMInstance smi) => !smi.HasValidConnections())
				.EventTransition(GameHashes.ConduitContentsChanged, this.fillingInactive, (FlushToilet.SMInstance smi) => !smi.OutputConduitBlocked())
				.Enter(delegate(FlushToilet.SMInstance smi)
				{
					smi.GetComponent<Operational>().SetActive(false, false);
				});
			this.filling.PlayAnim("off", KAnim.PlayMode.Once, null).Enter(delegate(FlushToilet.SMInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(true, false);
			}).EventTransition(GameHashes.ConduitConnectionChanged, this.disconnected, (FlushToilet.SMInstance smi) => !smi.HasValidConnections())
				.EventTransition(GameHashes.ConduitContentsChanged, this.backedup, (FlushToilet.SMInstance smi) => smi.OutputConduitBlocked())
				.EventTransition(GameHashes.OnStorageChange, this.ready, (FlushToilet.SMInstance smi) => smi.UpdateFullnessState())
				.EventTransition(GameHashes.OperationalChanged, this.fillingInactive, (FlushToilet.SMInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			this.fillingInactive.PlayAnim("off", KAnim.PlayMode.Once, null).Enter(delegate(FlushToilet.SMInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(false, false);
			}).EventTransition(GameHashes.OperationalChanged, this.filling, (FlushToilet.SMInstance smi) => smi.GetComponent<Operational>().IsOperational)
				.EventTransition(GameHashes.ConduitContentsChanged, this.backedup, (FlushToilet.SMInstance smi) => smi.OutputConduitBlocked());
			this.ready.DefaultState(this.ready.idle).Enter(delegate(FlushToilet.SMInstance smi)
			{
				smi.master.fillMeter.SetPositionPercent(1f);
				smi.master.contaminationMeter.SetPositionPercent(0f);
			}).PlayAnim("off", KAnim.PlayMode.Once, null)
				.EventTransition(GameHashes.ConduitConnectionChanged, this.disconnected, (FlushToilet.SMInstance smi) => !smi.HasValidConnections())
				.EventTransition(GameHashes.ConduitConnectionChanged, this.backedup, (FlushToilet.SMInstance smi) => smi.OutputConduitBlocked())
				.ToggleChore(new Func<FlushToilet.SMInstance, Chore>(this.CreateUseChore), this.flushing, false);
			this.ready.idle.Enter(delegate(FlushToilet.SMInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(false, false);
			}).ToggleMainStatusItem(Db.Get().BuildingStatusItems.FlushToilet).EventTransition(GameHashes.WorkStarted, this.ready.inuse, null);
			this.ready.inuse.Enter(delegate(FlushToilet.SMInstance smi)
			{
				smi.ShowContaminatedMeter();
			}).ToggleMainStatusItem(Db.Get().BuildingStatusItems.FlushToiletInUse).Update(delegate(FlushToilet.SMInstance smi)
			{
				smi.UpdateDirtyState();
			})
				.EventTransition(GameHashes.WorkStopped, this.flushing, null);
			this.flushing.Enter(delegate(FlushToilet.SMInstance smi)
			{
				smi.StartFlush();
			}).PlayAnim("working_pst", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.fillingInactive);
		}

		private Chore CreateUseChore(FlushToilet.SMInstance smi)
		{
			ScheduleBlockType hygiene = Db.Get().ScheduleBlockTypes.Hygiene;
			return new WorkChore<ToiletWorkableUse>(Db.Get().ChoreTypes.Pee, smi.master, null, true, null, null, null, false, hygiene, true, default(Tag), null, false, true);
		}

		public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State disconnected;

		public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State backedup;

		public FlushToilet.States.ReadyStates ready;

		public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State fillingInactive;

		public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State filling;

		public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State flushing;

		public class ReadyStates : GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State
		{
			public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State idle;

			public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State inuse;
		}
	}
}
