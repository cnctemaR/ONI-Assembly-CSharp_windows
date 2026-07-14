using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class OxyCoral : GameStateMachine<OxyCoral, OxyCoral.Instance, IStateMachineTarget, OxyCoral.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.grow;
		this.grow.ParamTransition<bool>(this.HasPlayedGrowAnim, this.noProducing, GameStateMachine<OxyCoral, OxyCoral.Instance, IStateMachineTarget, OxyCoral.Def>.IsTrue).PlayAnim("grow", KAnim.PlayMode.Once).OnAnimQueueComplete(this.noProducing)
			.Exit(delegate(OxyCoral.Instance smi)
			{
				smi.sm.HasPlayedGrowAnim.Set(true, smi, false);
			});
		this.noProducing.DefaultState(this.noProducing.noLight);
		this.noProducing.noLight.EventTransition(GameHashes.Uprooted, this.noProducing.dead, null).EventTransition(GameHashes.Wilt, this.noProducing.wilted, new StateMachine<OxyCoral, OxyCoral.Instance, IStateMachineTarget, OxyCoral.Def>.Transition.ConditionCallback(OxyCoral.IsWilted)).PlayAnim("idle_loop", KAnim.PlayMode.Loop)
			.UpdateTransition(this.producing, new Func<OxyCoral.Instance, float, bool>(OxyCoral.IsThereEnoughtLight), UpdateRate.SIM_200ms, false);
		this.noProducing.wilted.TriggerOnEnter(GameHashes.PoopStationUpdate, null).TriggerOnExit(GameHashes.PoopStationUpdate, null).EventTransition(GameHashes.Uprooted, this.noProducing.dead, null)
			.EventTransition(GameHashes.WiltRecover, this.noProducing.noLight, GameStateMachine<OxyCoral, OxyCoral.Instance, IStateMachineTarget, OxyCoral.Def>.Not(new StateMachine<OxyCoral, OxyCoral.Instance, IStateMachineTarget, OxyCoral.Def>.Transition.ConditionCallback(OxyCoral.IsWilted)))
			.PlayAnim("wilt");
		this.noProducing.dead.Enter(delegate(OxyCoral.Instance smi)
		{
			GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
			smi.Trigger(1623392196, null);
			smi.DestroySelf(null);
		});
		this.producing.ToggleStatusItem(Db.Get().CreatureStatusItems.BubbleGasProduction, (OxyCoral.Instance smi) => new global::Tuple<SimHashes, float>(SimHashes.Oxygen, smi.IsWild ? (smi.def.OxygenProductionRate * 0.25f) : smi.def.OxygenProductionRate)).EventTransition(GameHashes.Uprooted, this.noProducing.dead, null).EventTransition(GameHashes.Wilt, this.noProducing.wilted, null)
			.UpdateTransition(this.noProducing.noLight, new Func<OxyCoral.Instance, float, bool>(OxyCoral.LightLostUpdate), UpdateRate.SIM_200ms, false)
			.PlayAnim("oxygen_idle_loop", KAnim.PlayMode.Loop)
			.Update(new Action<OxyCoral.Instance, float>(OxyCoral.ProduceOxygenUpdate), UpdateRate.SIM_1000ms, false);
	}

	private static bool IsWilted(OxyCoral.Instance smi)
	{
		return smi.IsWilted;
	}

	private static bool IsThereEnoughtLight(OxyCoral.Instance smi, float dt)
	{
		return smi.IsThereEnoughLight();
	}

	private static bool LightLostUpdate(OxyCoral.Instance smi, float dt)
	{
		return !smi.IsThereEnoughLight();
	}

	private static void ProduceOxygenUpdate(OxyCoral.Instance smi, float dt)
	{
		smi.ProduceOxygenUpdate(dt);
	}

	private const float WILD_PLANTED_RATE_MODIFIER = 0.25f;

	private const string ANIM_NAME_GROW = "grow";

	private const string ANIM_NAME_IDLE = "idle_loop";

	private const string ANIM_NAME_PRODUCING_OXYGEN = "oxygen_idle_loop";

	private const string ANIM_NAME_WILTED = "wilt";

	public OxyCoral.NoProducing noProducing;

	public GameStateMachine<OxyCoral, OxyCoral.Instance, IStateMachineTarget, OxyCoral.Def>.State producing;

	public GameStateMachine<OxyCoral, OxyCoral.Instance, IStateMachineTarget, OxyCoral.Def>.State grow;

	private StateMachine<OxyCoral, OxyCoral.Instance, IStateMachineTarget, OxyCoral.Def>.BoolParameter HasPlayedGrowAnim;

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			return new List<Descriptor>
			{
				new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED_ENTITYTEMP, ElementLoader.FindElementByHash(SimHashes.Oxygen).name, GameUtil.GetFormattedMass(this.OxygenProductionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_ENTITYTEMP, ElementLoader.FindElementByHash(SimHashes.Oxygen).name, GameUtil.GetFormattedMass(this.OxygenProductionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Effect, false),
				new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_LIGHT.Replace("{Lux}", GameUtil.GetFormattedLux(this.MinLuxRequired)), UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_LIGHT.Replace("{Lux}", GameUtil.GetFormattedLux(this.MinLuxRequired)), Descriptor.DescriptorType.Requirement, false)
			};
		}

		public float OxygenProductionRate;

		public int MinLuxRequired;

		public CellOffset[] OutputBubbleCells;
	}

	public class NoProducing : GameStateMachine<OxyCoral, OxyCoral.Instance, IStateMachineTarget, OxyCoral.Def>.State
	{
		public GameStateMachine<OxyCoral, OxyCoral.Instance, IStateMachineTarget, OxyCoral.Def>.State noLight;

		public GameStateMachine<OxyCoral, OxyCoral.Instance, IStateMachineTarget, OxyCoral.Def>.State wilted;

		public GameStateMachine<OxyCoral, OxyCoral.Instance, IStateMachineTarget, OxyCoral.Def>.State dead;
	}

	public new class Instance : GameStateMachine<OxyCoral, OxyCoral.Instance, IStateMachineTarget, OxyCoral.Def>.GameInstance, IPoopStation
	{
		public bool IsWild
		{
			get
			{
				return !this.receptacleMonitor.Replanted;
			}
		}

		public bool IsWilted
		{
			get
			{
				return this.wiltCondition == null || this.wiltCondition.IsWilting();
			}
		}

		public Instance(IStateMachineTarget master, OxyCoral.Def def)
			: base(master, def)
		{
			this.receptacleMonitor = base.GetComponent<ReceptacleMonitor>();
			this.primaryElement = base.GetComponent<PrimaryElement>();
			this.wiltCondition = base.GetComponent<WiltCondition>();
			this.cachedName = master.gameObject.GetProperName();
		}

		public override void StartSM()
		{
			this.RegisterPoopStation();
			if (!this.IsWild)
			{
				Tutorial.Instance.oxygenGenerators.Add(base.gameObject);
			}
			base.StartSM();
		}

		public bool IsThereEnoughLight()
		{
			int num = 0;
			int num2 = Grid.PosToCell(this);
			for (int i = 0; i < base.def.OutputBubbleCells.Length; i++)
			{
				int num3 = Grid.OffsetCell(num2, base.def.OutputBubbleCells[i]);
				float num4 = (float)Grid.LightIntensity[num3];
				num = ((num4 > (float)num) ? ((int)num4) : num);
			}
			return num >= base.def.MinLuxRequired;
		}

		public void ProduceOxygenUpdate(float dt)
		{
			int num = Grid.PosToCell(this);
			int num2 = global::UnityEngine.Random.Range(0, base.def.OutputBubbleCells.Length);
			int num3 = Grid.OffsetCell(num, base.def.OutputBubbleCells[num2]);
			float num4 = base.def.OxygenProductionRate;
			if (this.IsWild)
			{
				num4 *= 0.25f;
			}
			float num5 = num4 * dt;
			if (num5 >= 1E-09f)
			{
				this.CreateOxygenBubble(num3, num5);
			}
		}

		private void CreateOxygenBubble(int gameCell, float mass)
		{
			Vector3 vector = Grid.CellToPosCCC(gameCell, Grid.SceneLayer.BuildingFront);
			BubbleManager.instance.SpawnBubble(SimHashes.Oxygen, vector, mass, this.primaryElement.Temperature, BubbleManager.Disease.None, null);
			ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, mass, this.cachedName, null);
		}

		protected override void OnCleanUp()
		{
			Tutorial.Instance.oxygenGenerators.Remove(base.gameObject);
			this.UnregisterPoopStation();
			base.OnCleanUp();
		}

		public bool IsUserCompatibleWithPoopStation(KPrefabID userPrefabID)
		{
			return userPrefabID.HasTag("ParrotFish");
		}

		public GameObject GetPoopStationObject()
		{
			return base.gameObject;
		}

		public GameObject GetCurrentPoopStationUser()
		{
			return this.poopUser;
		}

		public float GetAvailablePoopCapacity()
		{
			if (this.IsWild)
			{
				return 0f;
			}
			Storage component = this.receptacleMonitor.smi.ReceptacleObject.GetComponent<Storage>();
			return component.RemainingCapacity() / component.capacityKg;
		}

		public bool IsPoopStationOperational()
		{
			return !base.smi.IsInsideState(base.smi.sm.noProducing.dead);
		}

		public string[] GetPoopingAnimNames()
		{
			return null;
		}

		public void RegisterPoopStation()
		{
			Components.PoopStations.Add(base.gameObject.GetMyWorldId(), this);
		}

		public void UnregisterPoopStation()
		{
			Components.PoopStations.Remove(base.gameObject.GetMyWorldId(), this);
		}

		public PoopData GetPoopData()
		{
			if (!this.IsWild)
			{
				return new PoopData(false, this.receptacleMonitor.smi.ReceptacleObject.GetComponent<Storage>(), CREATURES.POOP.PLANT_POOP_STATION_WILD, global::Def.GetUISprite(base.gameObject, "ui", false).first);
			}
			return new PoopData(true, null, CREATURES.POOP.PLANT_POOP_STATION_WILD, global::Def.GetUISprite(base.gameObject, "ui", false).first);
		}

		public void PlayPoopStationAnim(string animName, KAnim.PlayMode playMode)
		{
		}

		public void ClearPoopStationUser(GameObject userRequestingClearing)
		{
			if (this.poopUser == userRequestingClearing)
			{
				this.poopUser = null;
				base.Trigger(-984476291, null);
			}
		}

		public bool AttemptToReservePoopStation(GameObject userRequestingReserve)
		{
			if (this.poopUser != null && this.poopUser != userRequestingReserve)
			{
				return false;
			}
			this.poopUser = userRequestingReserve;
			return true;
		}

		public void DestroySelf(object o)
		{
			CreatureHelpers.DeselectCreature(base.gameObject);
			Util.KDestroyGameObject(base.gameObject);
		}

		private PrimaryElement primaryElement;

		private WiltCondition wiltCondition;

		private ReceptacleMonitor receptacleMonitor;

		private string cachedName;

		private GameObject poopUser;
	}
}
