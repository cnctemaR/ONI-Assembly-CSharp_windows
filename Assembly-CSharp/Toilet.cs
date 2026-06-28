using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class Toilet : StateMachineComponent<Toilet.StatesInstance>, IUsable, IEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Component[] components = base.gameObject.GetComponents<Storage>();
		this.inStorage = (Storage)components[0];
		this.outStorage = (Storage)components[1];
		Components.Toilets.Add(this);
		base.smi.StartSM();
		ToiletWorkableUse component = base.GetComponent<ToiletWorkableUse>();
		component.onComplete = new Action<Worker>(this.Flush);
		component.onAbort = new Action<Worker>(this.Flush);
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, new string[] { "meter_target", "meter_arrow", "meter_scale" });
		this.meter.SetPositionPercent((float)base.smi.sm.flushes.Get(base.smi) / (float)base.smi.master.maxFlushes);
		this.Subscribe(493375141, new EventSystem.EventHandler(this.OnRefreshUserMenu));
		foreach (GameObject gameObject in this.inStorage.items)
		{
			if (gameObject != null)
			{
				this.PreventStoredSublimation(gameObject);
			}
		}
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
		Element element = ElementLoader.FindElementByHash(this.solidWaste.elementID);
		GameObject gameObject = element.substance.SpawnResource(this.transform.position, base.smi.MassPerFlush(), temperature, true, false);
		this.inStorage.Store(gameObject, false, false);
		base.smi.sm.flushes.Delta(1, base.smi);
		this.meter.SetPositionPercent((float)base.smi.sm.flushes.Get(base.smi) / (float)base.smi.master.maxFlushes);
	}

	private void OnEmitGas()
	{
		bool flag = this.outStorage != null && !this.outStorage.IsEmpty();
		if (flag)
		{
			int num = Grid.PosToCell(this);
			float temperature = base.GetComponent<PrimaryElement>().Temperature;
			SimMessages.AddRemoveSubstance(num, this.gasWaste.elementID, CellEventLogger.Instance.ToiletEmit, this.gasWaste.mass, temperature, -1);
		}
	}

	private void OnRefreshUserMenu(object data)
	{
		if (base.smi.GetCurrentState() == base.smi.sm.full || !base.smi.IsSoiled || base.smi.cleanChore != null)
		{
			return;
		}
		UserMenu userMenu = this.userMenu;
		string text = UI.USERMENUACTIONS.CLEANTOILET.TOOLTIP;
		userMenu.AddButton(new KIconButtonMenu.ButtonInfo("status_item_toilet_needs_emptying", UI.USERMENUACTIONS.CLEANTOILET.NAME, delegate
		{
			base.smi.GoTo(base.smi.sm.earlyclean);
		}, global::Action.NumActions, null, null, null, null, text));
	}

	private void SpawnMonster()
	{
		GameObject gameObject = (GameObject)global::UnityEngine.Object.Instantiate(Assets.GetPrefab(new Tag("Glom")), base.smi.transform.position, Quaternion.identity);
		gameObject.SetActive(true);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.GetComponent<Storage>().choreType = Db.Get().ChoreTypes.FetchCritical;
		this.Subscribe(-1697596308, new EventSystem.EventHandler(this.OnStorageChanged));
	}

	private void OnStorageChanged(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject == null)
		{
			return;
		}
		this.PreventStoredSublimation(gameObject);
	}

	private void PreventStoredSublimation(GameObject go)
	{
		Sublimates component = go.GetComponent<Sublimates>();
		if (component != null)
		{
			bool flag = this.inStorage.items.Contains(go);
			component.enabled = !flag;
		}
	}

	public List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		ManualDeliveryKG component = base.GetComponent<ManualDeliveryKG>();
		Tag requestedItemTag = component.requestedItemTag;
		string keywordStyle = GameUtil.GetKeywordStyle(requestedItemTag);
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, keywordStyle, requestedItemTag, GameUtil.GetFormattedMass(base.smi.MassPerFlush(), GameUtil.TimeSlice.None, true, "F1"))), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, keywordStyle, requestedItemTag, GameUtil.GetFormattedMass(base.smi.MassPerFlush(), GameUtil.TimeSlice.None, true, "F1")));
		list.Add(descriptor);
		return list;
	}

	public int DescriptionOrder { get; set; }

	public List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(this.solidWaste.elementID);
		string text = element.tag.ProperName();
		string keywordStyle = element.keywordStyle;
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTEDPERUSE, keywordStyle, text, GameUtil.GetFormattedMass(base.smi.MassPerFlush(), GameUtil.TimeSlice.None, true, "F1"))), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTEDPERUSE, keywordStyle, text, GameUtil.GetFormattedMass(base.smi.MassPerFlush(), GameUtil.TimeSlice.None, true, "F1")));
		list.Add(descriptor);
		return list;
	}

	[SerializeField]
	public Toilet.SpawnInfo solidWaste;

	[SerializeField]
	public Toilet.SpawnInfo gasWaste;

	[SerializeField]
	public int maxFlushes = 15;

	private MeterController meter;

	[MyCmpReq]
	private Storage inStorage;

	private Storage outStorage;

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

	public class StatesInstance : GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet>.GameInstance
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
			return !base.GetComponent<Storage>().IsEmpty();
		}

		public float MassPerFlush()
		{
			return base.master.solidWaste.mass / (float)base.master.maxFlushes;
		}

		public bool IsToxicSandRemoved()
		{
			Tag tag = TagManager.Create(base.master.solidWaste.elementID);
			return base.master.inStorage.Find(tag).Count == 0;
		}

		public void CreateCleanChore()
		{
			if (this.cleanChore != null)
			{
				this.cleanChore.Cancel("dupe");
			}
			ToiletWorkableClean component = base.master.GetComponent<ToiletWorkableClean>();
			Action<Chore> action = new Action<Chore>(this.OnCleanComplete);
			this.cleanChore = new WorkChore<ToiletWorkableClean>(Db.Get().ChoreTypes.CleanToilet, component, null, true, action, null, null, true, null, true, default(Tag), null, false, true);
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
			Tag tag = TagManager.Create(base.master.solidWaste.elementID);
			List<GameObject> list = base.master.inStorage.Find(tag);
			foreach (GameObject gameObject in list)
			{
				base.master.inStorage.Drop(gameObject);
			}
		}

		public Chore cleanChore;

		public float monsterSpawnTime = 1200f;
	}

	public class States : GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.needsdirt;
			base.serializable = true;
			this.root.PlayAnim("off", KAnim.PlayMode.Once, null).EventTransition(GameHashes.OnStorageChange, this.needsdirt, (Toilet.StatesInstance smi) => !smi.HasDirt()).EventTransition(GameHashes.OperationalChanged, this.notoperational, (Toilet.StatesInstance smi) => !smi.Get<Operational>().IsOperational);
			this.needsdirt.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable).EventTransition(GameHashes.OnStorageChange, this.ready, (Toilet.StatesInstance smi) => smi.HasDirt());
			this.ready.ParamTransition<int>(this.flushes, this.full, (Toilet.StatesInstance smi, int p) => smi.GetFlushesRemaining() <= 0).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Toilet).ToggleChore(new Func<Toilet.StatesInstance, Chore>(this.CreateUseChore), this.ready, true)
				.Tag(new Tag[] { GameTags.Usable });
			this.earlyclean.Enter(delegate(Toilet.StatesInstance smi)
			{
				smi.CreateCleanChore();
			}).Exit(delegate(Toilet.StatesInstance smi)
			{
				smi.CancelCleanChore();
			}).PlayAnim("full_pre", KAnim.PlayMode.Once, null)
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
			}).PlayAnim("full_pre", KAnim.PlayMode.Once, null)
				.QueueAnim("full", false, null)
				.ToggleStatusItem(Db.Get().BuildingStatusItems.ToiletNeedsEmptying, null)
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable)
				.ToggleSchedulePeriodic("toilet_emit_gas", (Toilet.StatesInstance smi) => smi.master.gasWaste.interval, delegate(Toilet.StatesInstance smi)
				{
					smi.master.OnEmitGas();
				})
				.EventTransition(GameHashes.OnStorageChange, this.empty, (Toilet.StatesInstance smi) => smi.IsToxicSandRemoved())
				.Enter(delegate(Toilet.StatesInstance smi)
				{
					smi.Schedule(smi.monsterSpawnTime, delegate
					{
						smi.master.SpawnMonster();
					}, null);
				});
			this.empty.PlayAnim("off", KAnim.PlayMode.Once, null).Enter("ClearFlushes", delegate(Toilet.StatesInstance smi)
			{
				this.flushes.Set(0, smi);
			}).Enter("ClearStorage", delegate(Toilet.StatesInstance smi)
			{
				smi.GetComponent<Storage>().ConsumeAll();
			})
				.GoTo(this.needsdirt);
			this.notoperational.EventTransition(GameHashes.OperationalChanged, this.needsdirt, (Toilet.StatesInstance smi) => smi.Get<Operational>().IsOperational).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable);
		}

		private Chore CreateUseChore(Toilet.StatesInstance smi)
		{
			return new WorkChore<ToiletWorkableUse>(Db.Get().ChoreTypes.Pee, smi.master, null, true, null, null, null, false, null, true, default(Tag), null, false, true);
		}

		public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet>.State needsdirt;

		public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet>.State empty;

		public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet>.State notoperational;

		public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet>.State ready;

		public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet>.State full;

		public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet>.State earlyclean;

		public StateMachine<Toilet.States, Toilet.StatesInstance, Toilet>.IntParameter flushes = new StateMachine<Toilet.States, Toilet.StatesInstance, Toilet>.IntParameter(0);
	}
}
