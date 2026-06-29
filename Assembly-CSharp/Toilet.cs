using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class Toilet : StateMachineComponent<Toilet.StatesInstance>, IUsable, IEffectDescriptor, IGameObjectEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.Toilets.Add(this);
		base.smi.StartSM();
		ToiletWorkableUse component = base.GetComponent<ToiletWorkableUse>();
		component.trackUses = true;
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, new string[] { "meter_target", "meter_arrow", "meter_scale" });
		this.meter.SetPositionPercent((float)base.smi.sm.flushes.Get(base.smi) / (float)base.smi.master.maxFlushes);
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Toilets.Remove(this);
	}

	public bool IsUsable()
	{
		return base.smi.HasTag(GameTags.Usable);
	}

	public void Flush(Worker worker)
	{
		float temperature = base.GetComponent<PrimaryElement>().Temperature;
		Element element = ElementLoader.FindElementByHash(this.solidWastePerUse.elementID);
		byte index = Db.Get().Diseases.GetIndex(this.diseaseId);
		GameObject gameObject = element.substance.SpawnResource(base.transform.GetPosition(), base.smi.MassPerFlush(), temperature, index, this.diseasePerFlush, true, false);
		this.storage.Store(gameObject, false, false, true, false);
		PrimaryElement component = worker.GetComponent<PrimaryElement>();
		component.AddDisease(index, this.diseaseOnDupePerFlush, "Toilet.Flush");
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, string.Format(DUPLICANTS.DISEASES.ADDED_POPFX, Db.Get().Diseases[(int)index].Name, this.diseasePerFlush + this.diseaseOnDupePerFlush), base.transform, Vector3.up, 1.5f, false, false);
		base.smi.sm.flushes.Delta(1, base.smi);
		this.meter.SetPositionPercent((float)base.smi.sm.flushes.Get(base.smi) / (float)base.smi.master.maxFlushes);
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_LotsOfGerms);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (base.smi.GetCurrentState() == base.smi.sm.full || !base.smi.IsSoiled || base.smi.cleanChore != null)
		{
			return;
		}
		UserMenu userMenu = this.userMenu;
		string text = "status_item_toilet_needs_emptying";
		string text2 = UI.USERMENUACTIONS.CLEANTOILET.NAME;
		global::System.Action action = delegate
		{
			base.smi.GoTo(base.smi.sm.earlyclean);
		};
		string text3 = UI.USERMENUACTIONS.CLEANTOILET.TOOLTIP;
		userMenu.AddButton(new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true), 1f);
	}

	private void SpawnMonster()
	{
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(new Tag("Glom")), base.smi.transform.GetPosition(), Grid.SceneLayer.Creatures, SceneOrganizer.Instance.GetFolder(Folder.Creatures), null, 0);
		gameObject.SetActive(true);
	}

	public List<Descriptor> RequirementDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		ManualDeliveryKG component = base.GetComponent<ManualDeliveryKG>();
		Tag requestedItemTag = component.requestedItemTag;
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, requestedItemTag, GameUtil.GetFormattedMass(base.smi.MassPerFlush(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, requestedItemTag, GameUtil.GetFormattedMass(base.smi.MassPerFlush(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
		list.Add(descriptor);
		return list;
	}

	public List<Descriptor> EffectDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(this.solidWastePerUse.elementID);
		string text = element.tag.ProperName();
		string keywordStyle = GameUtil.GetKeywordStyle(element);
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTEDPERUSE, keywordStyle, text, GameUtil.GetFormattedMass(base.smi.MassPerFlush(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTEDPERUSE, keywordStyle, text, GameUtil.GetFormattedMass(base.smi.MassPerFlush(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Effect, false));
		Disease disease = Db.Get().Diseases.Get(this.diseaseId);
		int num = this.diseasePerFlush + this.diseaseOnDupePerFlush;
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.DISEASEEMITTEDPERUSE, disease.Name, GameUtil.GetFormattedDiseaseAmount(num)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.DISEASEEMITTEDPERUSE, disease.Name, GameUtil.GetFormattedDiseaseAmount(num)), Descriptor.DescriptorType.DiseaseSource, false));
		return list;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.AddRange(this.RequirementDescriptors());
		list.AddRange(this.EffectDescriptors());
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in this.RequirementDescriptors())
		{
			list.Add(descriptor);
		}
		foreach (Descriptor descriptor2 in this.EffectDescriptors())
		{
			list.Add(descriptor2);
		}
		return list;
	}

	Transform IUsable.get_transform()
	{
		return base.transform;
	}

	[SerializeField]
	public Toilet.SpawnInfo solidWastePerUse;

	[SerializeField]
	public Toilet.SpawnInfo gasWasteWhenFull;

	[SerializeField]
	public int maxFlushes = 15;

	[SerializeField]
	public string diseaseId;

	[SerializeField]
	public int diseasePerFlush;

	[SerializeField]
	public int diseaseOnDupePerFlush;

	private MeterController meter;

	[MyCmpReq]
	private Storage storage;

	[MyCmpAdd]
	private UserMenu userMenu;

	[Serializable]
	public struct SpawnInfo
	{
		public SpawnInfo(SimHashes element_id, float mass, float interval)
		{
			this.elementID = element_id;
			this.mass = mass;
			this.interval = interval;
		}

		[HashedEnum]
		public SimHashes elementID;

		public float mass;

		public float interval;
	}

	public class StatesInstance : GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.GameInstance
	{
		public StatesInstance(Toilet master)
			: base(master)
		{
		}

		public bool IsSoiled
		{
			get
			{
				return base.sm.flushes.Get(base.smi) > 0;
			}
		}

		public int GetFlushesRemaining()
		{
			return base.master.maxFlushes - base.sm.flushes.Get(base.smi);
		}

		public bool HasDirt()
		{
			return !base.master.storage.IsEmpty() && base.master.storage.Has(ElementLoader.FindElementByHash(SimHashes.Dirt).tag);
		}

		public float MassPerFlush()
		{
			return base.master.solidWastePerUse.mass;
		}

		public bool IsToxicSandRemoved()
		{
			Tag tag = GameTagExtensions.Create(base.master.solidWastePerUse.elementID);
			return base.master.storage.Find(tag).Count == 0;
		}

		public void CreateCleanChore()
		{
			if (this.cleanChore != null)
			{
				this.cleanChore.Cancel("dupe");
			}
			ToiletWorkableClean component = base.master.GetComponent<ToiletWorkableClean>();
			this.cleanChore = new WorkChore<ToiletWorkableClean>(Db.Get().ChoreTypes.CleanToilet, component, null, null, true, new Action<Chore>(this.OnCleanComplete), null, null, true, null, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, true);
		}

		public void CancelCleanChore()
		{
			if (this.cleanChore != null)
			{
				this.cleanChore.Cancel("Cancelled");
				this.cleanChore = null;
			}
		}

		private void OnCleanComplete(Chore chore)
		{
			this.cleanChore = null;
			Tag tag = GameTagExtensions.Create(base.master.solidWastePerUse.elementID);
			List<GameObject> list = base.master.storage.Find(tag);
			foreach (GameObject gameObject in list)
			{
				base.master.storage.Drop(gameObject);
			}
		}

		public void Flush()
		{
			Worker worker = base.master.GetComponent<ToiletWorkableUse>().worker;
			base.master.Flush(worker);
		}

		public Chore cleanChore;

		public float monsterSpawnTime = 1200f;
	}

	public class States : GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.needsdirt;
			this.root.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, this.needsdirt, (Toilet.StatesInstance smi) => !smi.HasDirt()).EventTransition(GameHashes.OperationalChanged, this.notoperational, (Toilet.StatesInstance smi) => !smi.Get<Operational>().IsOperational);
			this.needsdirt.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable).EventTransition(GameHashes.OnStorageChange, this.ready, (Toilet.StatesInstance smi) => smi.HasDirt());
			this.ready.DefaultState(this.ready.idle).ParamTransition<int>(this.flushes, this.full, (Toilet.StatesInstance smi, int p) => smi.GetFlushesRemaining() <= 0).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Toilet)
				.ToggleRecurringChore(new Func<Toilet.StatesInstance, Chore>(this.CreateUseChore), null)
				.ToggleTag(GameTags.Usable);
			this.ready.idle.WorkableStartTransition((Toilet.StatesInstance smi) => smi.master.GetComponent<ToiletWorkableUse>(), this.ready.inuse);
			this.ready.inuse.WorkableCompleteTransition((Toilet.StatesInstance smi) => smi.master.GetComponent<ToiletWorkableUse>(), this.ready.flush).WorkableStopTransition((Toilet.StatesInstance smi) => smi.master.GetComponent<ToiletWorkableUse>(), this.ready.idle);
			this.ready.flush.Enter(delegate(Toilet.StatesInstance smi)
			{
				smi.Flush();
			}).GoTo(this.ready.idle);
			this.earlyclean.Enter(delegate(Toilet.StatesInstance smi)
			{
				smi.CreateCleanChore();
			}).Exit(delegate(Toilet.StatesInstance smi)
			{
				smi.CancelCleanChore();
			}).PlayAnim("full_pre")
				.QueueAnim("full", false, null)
				.ToggleStatusItem(Db.Get().BuildingStatusItems.ToiletNeedsEmptying, null)
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable)
				.EventTransition(GameHashes.OnStorageChange, this.empty, (Toilet.StatesInstance smi) => smi.IsToxicSandRemoved());
			this.full.Enter(delegate(Toilet.StatesInstance smi)
			{
				smi.CreateCleanChore();
			}).Exit(delegate(Toilet.StatesInstance smi)
			{
				smi.CancelCleanChore();
			}).PlayAnim("full_pre")
				.QueueAnim("full", false, null)
				.ToggleStatusItem(Db.Get().BuildingStatusItems.ToiletNeedsEmptying, null)
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable)
				.EventTransition(GameHashes.OnStorageChange, this.empty, (Toilet.StatesInstance smi) => smi.IsToxicSandRemoved())
				.Enter(delegate(Toilet.StatesInstance smi)
				{
					smi.Schedule(smi.monsterSpawnTime, delegate
					{
						smi.master.SpawnMonster();
					}, null);
				});
			this.empty.PlayAnim("off").Enter("ClearFlushes", delegate(Toilet.StatesInstance smi)
			{
				this.flushes.Set(0, smi);
			}).Enter("ClearDirt", delegate(Toilet.StatesInstance smi)
			{
				smi.master.storage.ConsumeAllIgnoringDisease();
			})
				.GoTo(this.needsdirt);
			this.notoperational.EventTransition(GameHashes.OperationalChanged, this.needsdirt, (Toilet.StatesInstance smi) => smi.Get<Operational>().IsOperational).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable);
		}

		private Chore CreateUseChore(Toilet.StatesInstance smi)
		{
			WorkChore<ToiletWorkableUse> workChore = new WorkChore<ToiletWorkableUse>(Db.Get().ChoreTypes.Pee, smi.master, null, null, true, null, null, null, false, null, true, null, false, true, false, PriorityScreen.PriorityClass.emergency, 0, false);
			workChore.AddPrecondition(ChorePreconditions.instance.IsOperational, smi.gameObject.GetComponent<Operational>());
			workChore.AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, smi.gameObject.GetComponent<Assignable>());
			workChore.AddPrecondition(ChorePreconditions.instance.IsPreferredAssignableOrUrgentBladder, smi.master.GetComponent<Assignable>());
			return workChore;
		}

		public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State needsdirt;

		public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State empty;

		public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State notoperational;

		public Toilet.States.ReadyStates ready;

		public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State full;

		public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State earlyclean;

		public StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.IntParameter flushes = new StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.IntParameter(0);

		public class ReadyStates : GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State
		{
			public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State idle;

			public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State inuse;

			public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State flush;
		}
	}
}
