using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class FlushToilet : StateMachineComponent<FlushToilet.SMInstance>, IUsable, IGameObjectEffectDescriptor, IBasicBuilding
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = base.GetComponent<Building>();
		this.inputCell = component.GetUtilityInputCell();
		this.outputCell = component.GetUtilityOutputCell();
		ConduitFlow liquidConduitFlow = Game.Instance.liquidConduitFlow;
		liquidConduitFlow.onConduitsRebuilt += this.OnConduitsRebuilt;
		liquidConduitFlow.AddConduitUpdater(new Action<float>(this.OnConduitUpdate), ConduitFlowPriority.Default);
		KBatchedAnimController component2 = base.GetComponent<KBatchedAnimController>();
		this.fillMeter = new MeterController(component2, "meter_target", "meter", this.meterOffset, Grid.SceneLayer.NoLayer, new Vector3(0.4f, 3.2f, 0.1f), Array.Empty<string>());
		this.contaminationMeter = new MeterController(component2, "meter_target", "meter_dirty", this.meterOffset, Grid.SceneLayer.NoLayer, new Vector3(0.4f, 3.2f, 0.1f), Array.Empty<string>());
		this.gunkMeter = new MeterController(component2, "meter_target", "meter_gunky", this.meterOffset, Grid.SceneLayer.NoLayer, new Vector3(0.4f, 3.2f, 0.1f), Array.Empty<string>());
		Components.Toilets.Add(this);
		Components.BasicBuildings.Add(this);
		base.smi.StartSM();
		base.smi.ShowFillMeter();
	}

	protected override void OnCleanUp()
	{
		Game.Instance.liquidConduitFlow.onConduitsRebuilt -= this.OnConduitsRebuilt;
		Components.BasicBuildings.Remove(this);
		Components.Toilets.Remove(this);
		base.OnCleanUp();
	}

	private void OnConduitsRebuilt()
	{
		base.Trigger(-2094018600, null);
	}

	public bool IsUsable()
	{
		return base.smi.HasTag(GameTags.Usable);
	}

	private void Flush(WorkerBase worker)
	{
		ToiletWorkableUse component = base.GetComponent<ToiletWorkableUse>();
		ListPool<GameObject, Storage>.PooledList pooledList = ListPool<GameObject, Storage>.Allocate();
		this.storage.Find(FlushToilet.WaterTag, pooledList);
		float num = 0f;
		float num2 = this.massConsumedPerUse;
		foreach (GameObject gameObject in pooledList)
		{
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			float num3 = Mathf.Min(component2.Mass, num2);
			component2.Mass -= num3;
			num2 -= num3;
			num += num3 * component2.Temperature;
		}
		pooledList.Recycle();
		float lastAmountOfWasteMassRemovedFromDupe = component.lastAmountOfWasteMassRemovedFromDupe;
		num += lastAmountOfWasteMassRemovedFromDupe * this.newPeeTemperature;
		float num4 = this.massConsumedPerUse + lastAmountOfWasteMassRemovedFromDupe;
		float num5 = num / num4;
		byte index = Db.Get().Diseases.GetIndex(this.diseaseId);
		this.storage.AddLiquid(component.lastElementRemovedFromDupe, num4, num5, index, this.diseasePerFlush, false, true);
		if (worker != null)
		{
			worker.GetComponent<PrimaryElement>().AddDisease(index, this.diseaseOnDupePerFlush, "FlushToilet.Flush");
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, string.Format(DUPLICANTS.DISEASES.ADDED_POPFX, Db.Get().Diseases[(int)index].Name, this.diseasePerFlush + this.diseaseOnDupePerFlush), base.transform, Vector3.up, 1.5f, false, false);
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_LotsOfGerms, true);
			return;
		}
		DebugUtil.LogWarningArgs(new object[] { "Tried to add disease on toilet use but worker was null" });
	}

	public List<Descriptor> RequirementDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		string text = ElementLoader.FindElementByHash(SimHashes.Water).tag.ProperName();
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, text, GameUtil.GetFormattedMass(this.massConsumedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, text, GameUtil.GetFormattedMass(this.massConsumedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Requirement, false));
		return list;
	}

	public List<Descriptor> EffectDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		string text = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag.ProperName();
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED_TOILET, text, GameUtil.GetFormattedMass(this.massEmittedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}"), GameUtil.GetFormattedTemperature(this.newPeeTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_TOILET, text, GameUtil.GetFormattedMass(this.massEmittedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}"), GameUtil.GetFormattedTemperature(this.newPeeTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), Descriptor.DescriptorType.Effect, false));
		Disease disease = Db.Get().Diseases.Get(this.diseaseId);
		int num = this.diseasePerFlush + this.diseaseOnDupePerFlush;
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.DISEASEEMITTEDPERUSE, disease.Name, GameUtil.GetFormattedDiseaseAmount(num, GameUtil.TimeSlice.None)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.DISEASEEMITTEDPERUSE, disease.Name, GameUtil.GetFormattedDiseaseAmount(num, GameUtil.TimeSlice.None)), Descriptor.DescriptorType.DiseaseSource, false));
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.AddRange(this.RequirementDescriptors());
		list.AddRange(this.EffectDescriptors());
		return list;
	}

	private void OnConduitUpdate(float dt)
	{
		if (this.GetSMI() == null)
		{
			return;
		}
		ConduitFlow liquidConduitFlow = Game.Instance.liquidConduitFlow;
		bool flag = base.smi.master.requireOutput && liquidConduitFlow.GetContents(this.outputCell).mass > 0f && base.smi.HasContaminatedMass();
		base.smi.sm.outputBlocked.Set(flag, base.smi, false);
	}

	private static readonly HashedString[] CLOGGED_ANIMS = new HashedString[] { "full_gunk_pre", "full_gunk" };

	private const string UNCLOG_ANIM = "full_gunk_pst";

	private MeterController fillMeter;

	private MeterController contaminationMeter;

	private MeterController gunkMeter;

	public Meter.Offset meterOffset = Meter.Offset.Behind;

	[SerializeField]
	public float massConsumedPerUse = 5f;

	[SerializeField]
	public float massEmittedPerUse = 5f;

	[SerializeField]
	public float newPeeTemperature;

	[SerializeField]
	public string diseaseId;

	[SerializeField]
	public int diseasePerFlush;

	[SerializeField]
	public int diseaseOnDupePerFlush;

	[SerializeField]
	public bool requireOutput = true;

	[MyCmpGet]
	private ConduitConsumer conduitConsumer;

	[MyCmpGet]
	private Storage storage;

	public static readonly Tag WaterTag = GameTagExtensions.Create(SimHashes.Water);

	private int inputCell;

	private int outputCell;

	public class SMInstance : GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.GameInstance
	{
		public bool IsClogged
		{
			get
			{
				return base.sm.isClogged.Get(this);
			}
		}

		public SMInstance(FlushToilet master)
			: base(master)
		{
			this.activeUseChores = new List<Chore>();
			this.UpdateFullnessState();
			this.UpdateDirtyState();
		}

		public void CreateCleanChore()
		{
			if (this.cleanChore != null)
			{
				this.cleanChore.Cancel("dupe");
			}
			ToiletWorkableClean component = base.GetComponent<ToiletWorkableClean>();
			component.SetIsCloggedByGunk(this.IsClogged);
			this.cleanChore = new WorkChore<ToiletWorkableClean>(Db.Get().ChoreTypes.CleanToilet, component, null, true, new Action<Chore>(this.OnCleanComplete), null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, true, true);
		}

		public void CancelCleanChore()
		{
			if (this.cleanChore != null)
			{
				this.cleanChore.Cancel("Cancelled");
				this.cleanChore = null;
			}
		}

		private void OnCleanComplete(object o)
		{
			base.sm.isClogged.Set(false, this, false);
		}

		public bool HasValidConnections()
		{
			return Game.Instance.liquidConduitFlow.HasConduit(base.master.inputCell) && (!base.master.requireOutput || Game.Instance.liquidConduitFlow.HasConduit(base.master.outputCell));
		}

		public bool UpdateFullnessState()
		{
			float num = 0f;
			ListPool<GameObject, FlushToilet>.PooledList pooledList = ListPool<GameObject, FlushToilet>.Allocate();
			base.master.storage.Find(FlushToilet.WaterTag, pooledList);
			foreach (GameObject gameObject in pooledList)
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				num += component.Mass;
			}
			pooledList.Recycle();
			bool flag = num >= base.master.massConsumedPerUse;
			base.master.conduitConsumer.enabled = !flag;
			float num2 = Mathf.Clamp01(num / base.master.massConsumedPerUse);
			base.master.fillMeter.SetPositionPercent(num2);
			return flag;
		}

		public void SetDirtyStatesForClogged()
		{
			bool flag = base.GetComponent<ToiletWorkableUse>().last_user_id == BionicMinionConfig.ID;
			this.SetDirtyStateMeterPercentage((float)(flag ? 0 : 1), (float)(flag ? 1 : 0));
		}

		public void SetDirtyStateMeterPercentage(float contaminationPercentage, float gunkPercentage)
		{
			base.master.contaminationMeter.SetPositionPercent(contaminationPercentage);
			base.master.gunkMeter.SetPositionPercent(gunkPercentage);
		}

		public void UpdateDirtyState()
		{
			ToiletWorkableUse component = base.GetComponent<ToiletWorkableUse>();
			float percentComplete = component.GetPercentComplete();
			bool flag = component.last_user_id == BionicMinionConfig.ID;
			this.SetDirtyStateMeterPercentage(flag ? 0f : percentComplete, flag ? percentComplete : 0f);
		}

		public void Flush()
		{
			bool flag = base.GetComponent<ToiletWorkableUse>().last_user_id == BionicMinionConfig.ID;
			base.master.fillMeter.SetPositionPercent(0f);
			base.master.contaminationMeter.SetPositionPercent(flag ? 0f : 1f);
			base.master.gunkMeter.SetPositionPercent(flag ? 1f : 0f);
			base.smi.ShowFillMeter();
			WorkerBase worker = base.master.GetComponent<ToiletWorkableUse>().worker;
			base.master.Flush(worker);
		}

		public void ShowFillMeter()
		{
			base.master.fillMeter.gameObject.SetActive(true);
			base.master.contaminationMeter.gameObject.SetActive(false);
			base.master.gunkMeter.gameObject.SetActive(false);
		}

		public bool HasContaminatedMass()
		{
			foreach (GameObject gameObject in base.GetComponent<Storage>().items)
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (!(component == null) && (component.ElementID == SimHashes.DirtyWater || component.ElementID == GunkMonitor.GunkElement) && component.Mass > 0f)
				{
					return true;
				}
			}
			return false;
		}

		public void ShowContaminatedMeter()
		{
			bool flag = base.GetComponent<ToiletWorkableUse>().last_user_id == BionicMinionConfig.ID;
			base.master.fillMeter.gameObject.SetActive(false);
			base.master.contaminationMeter.gameObject.SetActive(!flag);
			base.master.gunkMeter.gameObject.SetActive(flag);
		}

		public List<Chore> activeUseChores;

		private Chore cleanChore;
	}

	public class States : GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.disconnected;
			base.serializable = StateMachine.SerializeType.ParamsOnly;
			this.disconnected.PlayAnim("off").EventTransition(GameHashes.ConduitConnectionChanged, this.backedup, (FlushToilet.SMInstance smi) => smi.HasValidConnections()).Enter(delegate(FlushToilet.SMInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(false, false);
			});
			this.backedup.PlayAnim("off").ToggleStatusItem(Db.Get().BuildingStatusItems.OutputPipeFull, null).EventTransition(GameHashes.ConduitConnectionChanged, this.disconnected, (FlushToilet.SMInstance smi) => !smi.HasValidConnections())
				.ParamTransition<bool>(this.outputBlocked, this.fillingInactive, GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.IsFalse)
				.Enter(delegate(FlushToilet.SMInstance smi)
				{
					smi.GetComponent<Operational>().SetActive(false, false);
				});
			this.filling.PlayAnim("on").Enter(delegate(FlushToilet.SMInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(true, false);
			}).EventTransition(GameHashes.ConduitConnectionChanged, this.disconnected, (FlushToilet.SMInstance smi) => !smi.HasValidConnections())
				.ParamTransition<bool>(this.outputBlocked, this.backedup, GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.IsTrue)
				.EventTransition(GameHashes.OnStorageChange, this.ready, (FlushToilet.SMInstance smi) => smi.UpdateFullnessState())
				.EventTransition(GameHashes.OperationalChanged, this.fillingInactive, (FlushToilet.SMInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			this.fillingInactive.PlayAnim("on").Enter(delegate(FlushToilet.SMInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(false, false);
			}).EventTransition(GameHashes.OperationalChanged, this.filling, (FlushToilet.SMInstance smi) => smi.GetComponent<Operational>().IsOperational)
				.ParamTransition<bool>(this.outputBlocked, this.backedup, GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.IsTrue);
			this.ready.DefaultState(this.ready.idle).ToggleTag(GameTags.Usable).Enter(delegate(FlushToilet.SMInstance smi)
			{
				smi.master.fillMeter.SetPositionPercent(1f);
				smi.master.contaminationMeter.SetPositionPercent(0f);
				smi.master.gunkMeter.SetPositionPercent(0f);
			})
				.PlayAnim("on")
				.EventHandler(GameHashes.FlushGunk, new GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.GameEvent.Callback(this.OnFlushedGunk))
				.EventTransition(GameHashes.ConduitConnectionChanged, this.disconnected, (FlushToilet.SMInstance smi) => !smi.HasValidConnections())
				.ParamTransition<bool>(this.outputBlocked, this.backedup, GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.IsTrue)
				.ToggleChore(new Func<FlushToilet.SMInstance, Chore>(this.CreateUrgentUseChore), this.ready.completed)
				.ToggleChore(new Func<FlushToilet.SMInstance, Chore>(this.CreateBreakUseChore), this.ready.completed);
			this.ready.idle.ParamTransition<bool>(this.isClogged, this.clogged, GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.IsTrue).Enter(delegate(FlushToilet.SMInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(false, false);
			}).ToggleMainStatusItem(Db.Get().BuildingStatusItems.FlushToilet, null)
				.WorkableStartTransition((FlushToilet.SMInstance smi) => smi.master.GetComponent<ToiletWorkableUse>(), this.ready.inuse);
			this.ready.inuse.Enter(delegate(FlushToilet.SMInstance smi)
			{
				smi.ShowContaminatedMeter();
			}).ToggleMainStatusItem(Db.Get().BuildingStatusItems.FlushToiletInUse, null).Update(delegate(FlushToilet.SMInstance smi, float dt)
			{
				smi.UpdateDirtyState();
			}, UpdateRate.SIM_200ms, false)
				.WorkableCompleteTransition((FlushToilet.SMInstance smi) => smi.master.GetComponent<ToiletWorkableUse>(), this.ready.completed)
				.WorkableStopTransition((FlushToilet.SMInstance smi) => smi.master.GetComponent<ToiletWorkableUse>(), this.flushed);
			this.ready.completed.ParamTransition<bool>(this.isClogged, this.clogged, GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.IsTrue).GoTo(this.flushing);
			this.clogged.PlayAnims((FlushToilet.SMInstance smi) => FlushToilet.CLOGGED_ANIMS, KAnim.PlayMode.Once).Enter(delegate(FlushToilet.SMInstance smi)
			{
				smi.ShowContaminatedMeter();
			}).Enter(new StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State.Callback(this.SetDirtyStatesForClogged))
				.Enter(new StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State.Callback(this.CreateCleanChore))
				.Exit(new StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State.Callback(this.CancelCleanChore))
				.ParamTransition<bool>(this.isClogged, this.unclogged, GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.IsFalse);
			this.unclogged.PlayAnim("full_gunk_pst").OnAnimQueueComplete(this.flushing);
			this.flushing.Enter(delegate(FlushToilet.SMInstance smi)
			{
				smi.Flush();
			}).PlayAnim("flush").OnAnimQueueComplete(this.flushed);
			this.flushed.EventTransition(GameHashes.OnStorageChange, this.fillingInactive, (FlushToilet.SMInstance smi) => !smi.HasContaminatedMass()).ParamTransition<bool>(this.outputBlocked, this.backedup, GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.IsTrue).PlayAnim("on");
		}

		public void OnFlushedGunk(FlushToilet.SMInstance smi, object o)
		{
			smi.sm.isClogged.Set(true, smi, false);
		}

		public void SetDirtyStatesForClogged(FlushToilet.SMInstance smi)
		{
			smi.SetDirtyStatesForClogged();
		}

		public void CreateCleanChore(FlushToilet.SMInstance smi)
		{
			smi.CreateCleanChore();
		}

		public void CancelCleanChore(FlushToilet.SMInstance smi)
		{
			smi.CancelCleanChore();
		}

		private Chore CreateUrgentUseChore(FlushToilet.SMInstance smi)
		{
			Chore chore = this.CreateUseChore(smi, Db.Get().ChoreTypes.Pee);
			chore.AddPrecondition(ChorePreconditions.instance.IsBladderFull, null);
			chore.AddPrecondition(ChorePreconditions.instance.NotCurrentlyPeeing, null);
			return chore;
		}

		private Chore CreateBreakUseChore(FlushToilet.SMInstance smi)
		{
			Chore chore = this.CreateUseChore(smi, Db.Get().ChoreTypes.BreakPee);
			chore.AddPrecondition(ChorePreconditions.instance.IsBladderNotFull, null);
			return chore;
		}

		private Chore CreateUseChore(FlushToilet.SMInstance smi, ChoreType choreType)
		{
			WorkChore<ToiletWorkableUse> workChore = new WorkChore<ToiletWorkableUse>(choreType, smi.master, null, true, null, null, null, false, null, true, true, null, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, false);
			smi.activeUseChores.Add(workChore);
			WorkChore<ToiletWorkableUse> workChore2 = workChore;
			workChore2.onExit = (Action<Chore>)Delegate.Combine(workChore2.onExit, new Action<Chore>(delegate(Chore exiting_chore)
			{
				smi.activeUseChores.Remove(exiting_chore);
			}));
			workChore.AddPrecondition(ChorePreconditions.instance.IsPreferredAssignableOrUrgentBladder, smi.master.GetComponent<Assignable>());
			workChore.AddPrecondition(ChorePreconditions.instance.IsExclusivelyAvailableWithOtherChores, smi.activeUseChores);
			return workChore;
		}

		public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State disconnected;

		public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State backedup;

		public FlushToilet.States.ReadyStates ready;

		public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State fillingInactive;

		public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State filling;

		public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State clogged;

		public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State unclogged;

		public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State flushing;

		public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State flushed;

		public StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.BoolParameter outputBlocked;

		public StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.BoolParameter isClogged;

		public class ReadyStates : GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State
		{
			public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State idle;

			public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State inuse;

			public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State completed;
		}
	}
}
