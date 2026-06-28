using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class ColdBreather : StateMachineComponent<ColdBreather.StatesInstance>, IGameObjectEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.simEmitCBHandle = Game.Instance.complexCallbackManager.Add(new Game.ComplexCallbackInfo(new Action<object>(this.OnSimEmitted)));
		this.elementConsumer.EnableConsumption(false);
		base.smi.animController.randomiseLoopedOffset = true;
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		Game.Instance.complexCallbackManager.Release(this.simEmitCBHandle);
		if (this.storage)
		{
			this.storage.DropAll(true);
		}
		base.OnCleanUp();
	}

	protected void DestroySelf(object callbackParam)
	{
		CreatureHelpers.DeselectCreature(base.gameObject);
		Util.KDestroyGameObject(base.gameObject);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(UI.GAMEOBJECTEFFECTS.COLDBREATHER, UI.GAMEOBJECTEFFECTS.TOOLTIPS.COLDBREATHER, Descriptor.DescriptorType.Effect, false)
		};
	}

	public void Exhale()
	{
		if (this.lastEmitTag != Tag.Invalid)
		{
			return;
		}
		this.gases.Clear();
		this.storage.Find(GameTags.Gas, this.gases);
		if (this.nextGasEmitIndex >= this.gases.Count)
		{
			this.nextGasEmitIndex = 0;
		}
		while (this.nextGasEmitIndex < this.gases.Count)
		{
			int num = this.nextGasEmitIndex++;
			PrimaryElement component = this.gases[num].GetComponent<PrimaryElement>();
			if (component != null && component.Mass > 0f)
			{
				float num2 = Mathf.Max(component.Element.lowTemp + 5f, component.Temperature + this.deltaEmitTemperature);
				int num3 = Grid.PosToCell(base.transform.GetPosition() + this.emitOffsetCell);
				byte b = (byte)ElementLoader.elements.IndexOf(component.Element);
				SimMessages.EmitMass(num3, b, component.Mass, num2, component.DiseaseIdx, component.DiseaseCount, this.simEmitCBHandle.index);
				this.lastEmitTag = component.Element.tag;
				break;
			}
		}
	}

	private void OnSimEmitted(object data)
	{
		Sim.MassEmittedCallback massEmittedCallback = (Sim.MassEmittedCallback)data;
		if (massEmittedCallback.suceeded == 1 && this.storage)
		{
			this.storage.ConsumeIgnoringDisease(this.lastEmitTag, massEmittedCallback.mass);
		}
		this.lastEmitTag = Tag.Invalid;
	}

	[MyCmpReq]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private KAnimControllerBase animController;

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private ElementConsumer elementConsumer;

	[MyCmpReq]
	private ReceptacleMonitor receptacleMonitor;

	private const float EXHALE_PERIOD = 1f;

	public float deltaEmitTemperature = -5f;

	public Vector3 emitOffsetCell = new Vector3(0f, 0f);

	private List<GameObject> gases = new List<GameObject>();

	private Tag lastEmitTag;

	private int nextGasEmitIndex;

	private HandleVector<Game.ComplexCallbackInfo>.Handle simEmitCBHandle = HandleVector<Game.ComplexCallbackInfo>.InvalidHandle;

	public class StatesInstance : GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.GameInstance
	{
		public StatesInstance(ColdBreather master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = true;
			default_state = this.grow;
			this.statusItemCooling = new StatusItem("cooling", CREATURES.STATUSITEMS.COOLING.NAME, CREATURES.STATUSITEMS.COOLING.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 63486);
			GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State state = this.dead;
			string text = CREATURES.STATUSITEMS.DEAD.NAME;
			string text2 = CREATURES.STATUSITEMS.DEAD.TOOLTIP;
			StatusItemCategory main = Db.Get().StatusItemCategories.Main;
			state.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, main).Enter(delegate(ColdBreather.StatesInstance smi)
			{
				GameUtil.KInstantiate(EffectPrefabs.Instance.PlantDeath, smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront, SceneOrganizer.Instance.GetFolder(Folder.FX), null, 0);
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				global::UnityEngine.Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, new Action<object>(smi.master.DestroySelf), null);
			});
			this.blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked, null).EventTransition(GameHashes.EntombedChanged, this.alive, (ColdBreather.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject)).EventTransition(GameHashes.TooColdWarning, this.alive, (ColdBreather.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.TooHotWarning, this.alive, (ColdBreather.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.Uprooted, this.dead, (ColdBreather.StatesInstance smi) => UprootedMonitor.IsObjectUprooted(smi.master.gameObject));
			this.grow.Enter(delegate(ColdBreather.StatesInstance smi)
			{
				if (smi.master.receptacleMonitor.HasReceptacle() && !this.alive.ForceUpdateStatus(smi.master.gameObject))
				{
					smi.GoTo(this.blocked_from_growing);
				}
			}).PlayAnim("grow_seed", KAnim.PlayMode.Once).EventTransition(GameHashes.AnimQueueComplete, this.alive, null);
			this.alive.InitializeStates(this.masterTarget, this.dead).DefaultState(this.alive.mature).Update(delegate(ColdBreather.StatesInstance smi, float dt)
			{
				smi.master.Exhale();
			});
			this.alive.mature.EventTransition(GameHashes.Wilt, this.alive.wilting, (ColdBreather.StatesInstance smi) => smi.master.wiltCondition.IsWilting()).PlayAnim("idle", KAnim.PlayMode.Loop).ToggleMainStatusItem(this.statusItemCooling)
				.Enter(delegate(ColdBreather.StatesInstance smi)
				{
					smi.master.elementConsumer.EnableConsumption(true);
				})
				.Exit(delegate(ColdBreather.StatesInstance smi)
				{
					smi.master.elementConsumer.EnableConsumption(false);
				});
			this.alive.wilting.PlayAnim("wilt1").EventTransition(GameHashes.WiltRecover, this.alive.mature, (ColdBreather.StatesInstance smi) => !smi.master.wiltCondition.IsWilting());
		}

		public GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State grow;

		public GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State blocked_from_growing;

		public ColdBreather.States.AliveStates alive;

		public GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State dead;

		private StatusItem statusItemCooling;

		public class AliveStates : GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.PlantAliveSubState
		{
			public GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State mature;

			public GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State wilting;
		}
	}
}
