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
		this.elementConsumer.EnableConsumption(false);
		base.smi.animController.randomiseLoopedOffset = true;
		base.smi.StartSM();
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

	private const float EXHALE_PERIOD = 1f;

	public float deltaEmitTemperature = -5f;

	public Vector3 emitOffsetCell = new Vector3(0f, 0f);

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

	public class StatesInstance : GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.GameInstance
	{
		public StatesInstance(ColdBreather master)
			: base(master)
		{
		}

		public void Exhale()
		{
			this.gases.Clear();
			base.master.storage.Find(GameTags.Gas, this.gases);
			for (int i = 0; i < this.gases.Count; i++)
			{
				PrimaryElement component = this.gases[i].GetComponent<PrimaryElement>();
				if (component != null && component.Mass > 0f)
				{
					float num = Mathf.Max(component.Element.lowTemp + 5f, component.Temperature + base.master.deltaEmitTemperature);
					int num2 = Grid.PosToCell(base.transform.position + base.master.emitOffsetCell);
					SimMessages.AddRemoveSubstance(num2, component.Element.id, CellEventLogger.Instance.ElementEmitted, component.Mass, num, component.DiseaseIdx, component.DiseaseCount, -1);
					base.master.storage.ConsumeIgnoringDisease(this.gases[i]);
				}
			}
		}

		private List<GameObject> gases = new List<GameObject>();
	}

	public class States : GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = true;
			default_state = this.grow;
			this.statusItemCooling = new StatusItem("cooling", CREATURES.STATUSITEMS.COOLING.NAME, CREATURES.STATUSITEMS.COOLING.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 14334);
			this.dead.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead).Enter(delegate(ColdBreather.StatesInstance smi)
			{
				GameUtil.KInstantiate(EffectPrefabs.Instance.PlantDeath, smi.master.transform.position, Grid.SceneLayer.FXFront, SceneOrganizer.Instance.GetFolder(Folder.FX), null, 0);
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
			}).PlayAnim("grow_seed", KAnim.PlayMode.Once, null).EventTransition(GameHashes.AnimQueueComplete, this.alive, null);
			this.alive.InitializeStates(this.masterTarget, this.dead).DefaultState(this.alive.mature).EventHandler(GameHashes.OnStorageChange, delegate(ColdBreather.StatesInstance smi)
			{
				smi.Exhale();
			});
			this.alive.mature.EventTransition(GameHashes.Wilt, this.alive.wilting, (ColdBreather.StatesInstance smi) => smi.master.wiltCondition.IsWilting()).PlayAnim("idle", KAnim.PlayMode.Loop, null).ToggleMainStatusItem(this.statusItemCooling)
				.Enter(delegate(ColdBreather.StatesInstance smi)
				{
					smi.master.elementConsumer.EnableConsumption(true);
				})
				.Exit(delegate(ColdBreather.StatesInstance smi)
				{
					smi.master.elementConsumer.EnableConsumption(false);
				});
			this.alive.wilting.PlayAnim("wilt1", KAnim.PlayMode.Once, null).EventTransition(GameHashes.WiltRecover, this.alive.mature, (ColdBreather.StatesInstance smi) => !smi.master.wiltCondition.IsWilting());
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
