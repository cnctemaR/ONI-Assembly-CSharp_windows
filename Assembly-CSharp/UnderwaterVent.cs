using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class UnderwaterVent : GameStateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.off;
		this.off.EventTransition(GameHashes.EntombedChanged, this.on, GameStateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.Not(new StateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.Transition.ConditionCallback(UnderwaterVent.ShouldBeOff))).EventTransition(GameHashes.SubmergedStateChanged, this.on, GameStateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.Not(new StateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.Transition.ConditionCallback(UnderwaterVent.ShouldBeOff))).PlayAnim("off");
		this.on.EventTransition(GameHashes.EntombedChanged, this.off, new StateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.Transition.ConditionCallback(UnderwaterVent.ShouldBeOff)).EventTransition(GameHashes.SubmergedStateChanged, this.off, new StateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.Transition.ConditionCallback(UnderwaterVent.ShouldBeOff)).DefaultState(this.on.erupting);
		this.on.erupting.ParamTransition<float>(this.BuildUp, this.on.blocked, GameStateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.IsGTEOne).PlayAnim("erupting", KAnim.PlayMode.Loop).ToggleStatusItem(Db.Get().MiscStatusItems.UnderwaterVentEmiting, null)
			.ToggleStatusItem(Db.Get().MiscStatusItems.UnderwaterVentBuildUpProgress, null)
			.Enter(new StateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.State.Callback(UnderwaterVent.RefreshBuildUpMeter))
			.Update(new Action<UnderwaterVent.Instance, float>(UnderwaterVent.EruptionUpdate), UpdateRate.SIM_1000ms, false);
		this.on.blocked.TriggerOnEnter(GameHashes.VentBlocked, null).DefaultState(this.on.blocked.idle);
		this.on.blocked.idle.ParamTransition<float>(this.BuildUp, this.on.blocked.unblock, GameStateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.IsZero).PlayAnim("blocked", KAnim.PlayMode.Once).ToggleStatusItem(Db.Get().MiscStatusItems.UnderwaterVentBlocked, null);
		this.on.blocked.unblock.Target(this.MeterController).PlayAnim("collapsed", KAnim.PlayMode.Once).OnAnimQueueComplete(this.on.erupting)
			.Target(this.masterTarget)
			.Exit(new StateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.State.Callback(UnderwaterVent.SpawnSolidDebri));
	}

	private static bool ShouldBeOff(UnderwaterVent.Instance smi)
	{
		return UnderwaterVent.IsEntombed(smi) || !UnderwaterVent.IsSubmerged(smi);
	}

	private static bool IsEntombed(UnderwaterVent.Instance smi)
	{
		return smi.IsEntombed;
	}

	private static bool IsSubmerged(UnderwaterVent.Instance smi)
	{
		return smi.IsSubmerged;
	}

	private static void RefreshBuildUpMeter(UnderwaterVent.Instance smi)
	{
		smi.RefreshBuildUpMeter();
	}

	private static void SpawnSolidDebri(UnderwaterVent.Instance smi)
	{
		smi.SpawnSolidDebri();
	}

	private static void EruptionUpdate(UnderwaterVent.Instance smi, float dt)
	{
		smi.EruptionUpdate(dt);
	}

	private const string IDLE_ANIM_NAME = "off";

	private const string ERUPTING_ANIM_NAME = "erupting";

	private const string BLOCKED_ANIM_NAME = "blocked";

	private const string METER_TARGET_NAME = "target_meter";

	private const string METER_ANIM_NAME = "meter";

	private const string METER_ANIM_COLLAPSE_NAME = "collapsed";

	private static readonly string[] ROCK_SYMBOLS_NAME = new string[] { "rock_1", "rock_2", "rock_3", "rock_4" };

	public GameStateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.State off;

	public UnderwaterVent.OnStates on;

	public StateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.FloatParameter BuildUp;

	public StateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.TargetParameter MeterController;

	public struct Data
	{
		public Data(Vector3 bubbleSpawnOffset, Vector3 solidSpawnOffset, SimHashes bubbleElement, float bubbleTemp, float bubbleMassPerSecond, SimHashes solidElement, float solidMass, float solidTemp, float buildUpDuration)
		{
			this.BubbleSpawnOffset = bubbleSpawnOffset;
			this.SolidSpawnOffset = solidSpawnOffset;
			this.BubbleElement = bubbleElement;
			this.BubbleTemp = bubbleTemp;
			this.BubbleMassRate = bubbleMassPerSecond;
			this.SolidElement = solidElement;
			this.SolidMass = solidMass;
			this.SolidTemp = solidTemp;
			this.BuildUpDuration = buildUpDuration;
		}

		public Vector3 BubbleSpawnOffset;

		public Vector3 SolidSpawnOffset;

		public SimHashes BubbleElement;

		public float BubbleTemp;

		public float BubbleMassRate;

		public SimHashes SolidElement;

		public float SolidMass;

		public float SolidTemp;

		public float BuildUpDuration;
	}

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			if (this.cachedDescriptor == null)
			{
				this.cachedDescriptor = new List<Descriptor>();
				this.cachedDescriptor.Add(new Descriptor(GameUtil.SafeStringFormat(UI.BUILDINGEFFECTS.UNDERWATERVENT_SHEARING, new object[] { GameUtil.GetFormattedMass(this.data.SolidMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}") }), GameUtil.SafeStringFormat(UI.BUILDINGEFFECTS.TOOLTIPS.UNDERWATERVENT_SHEARING, new object[] { this.data.SolidElement.CreateTag().ProperName() }), Descriptor.DescriptorType.Effect, false));
			}
			return this.cachedDescriptor;
		}

		public UnderwaterVent.Data data;

		private List<Descriptor> cachedDescriptor;
	}

	public class OnStates : GameStateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.State
	{
		public GameStateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.State erupting;

		public UnderwaterVent.BlockStates blocked;
	}

	public class BlockStates : GameStateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.State
	{
		public GameStateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.State idle;

		public GameStateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.State unblock;
	}

	public new class Instance : GameStateMachine<UnderwaterVent, UnderwaterVent.Instance, IStateMachineTarget, UnderwaterVent.Def>.GameInstance
	{
		public float BuildUpProgress
		{
			get
			{
				return base.sm.BuildUp.Get(this);
			}
		}

		public bool IsBlocked
		{
			get
			{
				return this.BuildUpProgress >= 1f;
			}
		}

		public bool IsSubmerged
		{
			get
			{
				return this.submergable.IsSubmerged;
			}
		}

		public bool IsEntombed
		{
			get
			{
				return this.entombVulnerable.GetEntombed;
			}
		}

		public Instance(IStateMachineTarget master, UnderwaterVent.Def def)
			: base(master, def)
		{
			this.entombVulnerable = base.GetComponent<EntombVulnerable>();
			this.submergable = base.GetComponent<Submergable>();
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			this.buildUpMeter = new MeterController(component, "target_meter", "meter", Meter.Offset.Infront, Grid.SceneLayer.BuildingBack, Array.Empty<string>());
			base.sm.MeterController.Set(this.buildUpMeter.meterController.gameObject, this, false);
		}

		public override void StartSM()
		{
			base.StartSM();
			this.RefreshBuildUpMeter();
		}

		public void EruptionUpdate(float dt)
		{
			float num = dt / base.def.data.BuildUpDuration;
			float num2 = base.def.data.BubbleMassRate * dt;
			if (num2 >= 1E-09f)
			{
				float bubbleTemp = base.def.data.BubbleTemp;
				Vector3 vector = Grid.CellToPos(Grid.PosToCell(base.gameObject)) + base.def.data.BubbleSpawnOffset;
				SimHashes bubbleElement = base.def.data.BubbleElement;
				BubbleManager.instance.SpawnBubble(bubbleElement, vector, num2, bubbleTemp, BubbleManager.Disease.None, null);
			}
			base.sm.BuildUp.Set(this.BuildUpProgress + num, this, false);
			this.RefreshBuildUpMeter();
		}

		public void RefreshBuildUpMeter()
		{
			if (this.buildUpMeter.meterController.currentAnim != "meter")
			{
				this.buildUpMeter.meterController.Play("meter", KAnim.PlayMode.Paused, 1f, 0f);
			}
			this.buildUpMeter.SetPositionPercent(this.BuildUpProgress);
		}

		public void SpawnSolidDebri()
		{
			KBatchedAnimController meterController = this.buildUpMeter.meterController;
			List<Vector3> list = new List<Vector3>(UnderwaterVent.ROCK_SYMBOLS_NAME.Length);
			float layerZ = Grid.GetLayerZ(Grid.SceneLayer.Ore);
			for (int i = 0; i < UnderwaterVent.ROCK_SYMBOLS_NAME.Length; i++)
			{
				string text = UnderwaterVent.ROCK_SYMBOLS_NAME[i];
				bool flag;
				Matrix4x4 symbolTransform = meterController.GetSymbolTransform(text, out flag);
				if (flag)
				{
					Vector3 vector = symbolTransform.GetColumn(3);
					vector.z = layerZ;
					list.Add(vector);
				}
			}
			if (list.Count == 0)
			{
				Vector3 vector2 = Grid.CellToPos(Grid.PosToCell(base.gameObject)) + base.def.data.SolidSpawnOffset;
				vector2.z = layerZ;
				list.Add(vector2);
			}
			float num = base.def.data.SolidMass / (float)list.Count;
			for (int j = 0; j < list.Count; j++)
			{
				Vector3 vector3 = list[j];
				this.SpawnRockDebri(vector3, num);
			}
		}

		private void SpawnRockDebri(Vector3 spawnPos, float massPerRock)
		{
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(base.def.data.SolidElement.CreateTag()), Grid.SceneLayer.Ore, null, 0);
			gameObject.transform.position = spawnPos;
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			component.Mass = massPerRock;
			component.Temperature = base.def.data.SolidTemp;
			gameObject.gameObject.SetActive(true);
		}

		public void Unblock()
		{
			base.sm.BuildUp.Set(0f, this, false);
		}

		private EntombVulnerable entombVulnerable;

		private Submergable submergable;

		private MeterController buildUpMeter;
	}
}
