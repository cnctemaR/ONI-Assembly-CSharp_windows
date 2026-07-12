using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class HugMonitor : GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.normal;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.root.Update(new Action<HugMonitor.Instance, float>(this.UpdateHugEggCooldownTimer), UpdateRate.SIM_1000ms, false).ToggleBehaviour(GameTags.Creatures.WantsToTendEgg, (HugMonitor.Instance smi) => smi.UpdateHasTarget(), delegate(HugMonitor.Instance smi)
		{
			smi.hugTarget = null;
		});
		this.normal.DefaultState(this.normal.idle).ParamTransition<float>(this.hugFrenzyTimer, this.hugFrenzy, GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.IsGTZero);
		this.normal.idle.ParamTransition<float>(this.wantsHugCooldownTimer, this.normal.hugReady.seekingHug, GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.IsLTEZero).Update(new Action<HugMonitor.Instance, float>(this.UpdateWantsHugCooldownTimer), UpdateRate.SIM_1000ms, false);
		this.normal.hugReady.ToggleReactable(new Func<HugMonitor.Instance, Reactable>(this.GetHugReactable));
		this.normal.hugReady.passiveHug.ParamTransition<float>(this.wantsHugCooldownTimer, this.normal.hugReady.seekingHug, GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.IsLTEZero).Update(new Action<HugMonitor.Instance, float>(this.UpdateWantsHugCooldownTimer), UpdateRate.SIM_1000ms, false).ToggleStatusItem(CREATURES.STATUSITEMS.HUGMINIONWAITING.NAME, CREATURES.STATUSITEMS.HUGMINIONWAITING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		this.normal.hugReady.seekingHug.ToggleBehaviour(GameTags.Creatures.WantsAHug, (HugMonitor.Instance smi) => true, delegate(HugMonitor.Instance smi)
		{
			this.wantsHugCooldownTimer.Set(smi.def.hugFrenzyCooldownFailed, smi);
			smi.GoTo(this.normal.hugReady.passiveHug);
		});
		this.hugFrenzy.ParamTransition<float>(this.hugFrenzyTimer, this.normal, (HugMonitor.Instance smi, float p) => p <= 0f && !smi.IsHugging()).Update(new Action<HugMonitor.Instance, float>(this.UpdateHugFrenzyTimer), UpdateRate.SIM_1000ms, false).ToggleEffect((HugMonitor.Instance smi) => smi.frenzyEffect)
			.ToggleLoopingSound(HugMonitor.soundPath, null, true, true, true)
			.Enter(delegate(HugMonitor.Instance smi)
			{
				smi.hugParticleFx = Util.KInstantiate(EffectPrefabs.Instance.HugFrenzyFX, smi.master.transform.GetPosition() + smi.hugParticleOffset);
				smi.hugParticleFx.transform.SetParent(smi.master.transform);
				smi.hugParticleFx.SetActive(true);
			})
			.Exit(delegate(HugMonitor.Instance smi)
			{
				Util.KDestroyGameObject(smi.hugParticleFx);
				this.wantsHugCooldownTimer.Set(smi.def.hugFrenzyCooldown, smi);
			});
	}

	private Reactable GetHugReactable(HugMonitor.Instance smi)
	{
		return new HugMinionReactable(smi.gameObject);
	}

	private void UpdateWantsHugCooldownTimer(HugMonitor.Instance smi, float dt)
	{
		this.wantsHugCooldownTimer.DeltaClamp(-dt, 0f, float.MaxValue, smi);
	}

	private void UpdateHugEggCooldownTimer(HugMonitor.Instance smi, float dt)
	{
		this.hugEggCooldownTimer.DeltaClamp(-dt, 0f, float.MaxValue, smi);
	}

	private void UpdateHugFrenzyTimer(HugMonitor.Instance smi, float dt)
	{
		this.hugFrenzyTimer.DeltaClamp(-dt, 0f, float.MaxValue, smi);
	}

	private static string soundPath = GlobalAssets.GetSound("Squirrel_hug_frenzyFX", false);

	private StateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.FloatParameter hugFrenzyTimer;

	private StateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.FloatParameter wantsHugCooldownTimer;

	private StateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.FloatParameter hugEggCooldownTimer;

	public HugMonitor.NormalStates normal;

	public GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.State hugFrenzy;

	public class HUGTUNING
	{
		public const float HUG_EGG_TIME = 15f;

		public const float HUG_DUPE_WAIT = 60f;

		public const float FRENZY_EGGS_PER_CYCLE = 6f;

		public const float FRENZY_EGG_TRAVEL_TIME_BUFFER = 5f;

		public const float HUG_FRENZY_DURATION = 120f;
	}

	public class Def : StateMachine.BaseDef
	{
		public float hugsPerCycle = 2f;

		public float scanningInterval = 30f;

		public float hugFrenzyDuration = 120f;

		public float hugFrenzyCooldown = 480f;

		public float hugFrenzyCooldownFailed = 120f;

		public float scanningIntervalFrenzy = 15f;
	}

	public class HugReadyStates : GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.State
	{
		public GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.State passiveHug;

		public GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.State seekingHug;
	}

	public class NormalStates : GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.State
	{
		public GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.State idle;

		public HugMonitor.HugReadyStates hugReady;
	}

	public new class Instance : GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, HugMonitor.Def def)
			: base(master, def)
		{
			this.frenzyEffect = Db.Get().effects.Get("HuggingFrenzy");
			this.RefreshSearchTime();
			base.smi.sm.wantsHugCooldownTimer.Set(global::UnityEngine.Random.Range(base.smi.def.hugFrenzyCooldownFailed, base.smi.def.hugFrenzyCooldown), base.smi);
		}

		private void RefreshSearchTime()
		{
			if (this.hugTarget == null)
			{
				base.smi.sm.hugEggCooldownTimer.Set(this.GetScanningInterval(), base.smi);
				return;
			}
			base.smi.sm.hugEggCooldownTimer.Set(this.GetHugInterval(), base.smi);
		}

		private float GetScanningInterval()
		{
			if (!this.IsHuggingFrenzy())
			{
				return base.def.scanningInterval;
			}
			return base.def.scanningIntervalFrenzy;
		}

		private float GetHugInterval()
		{
			if (this.IsHuggingFrenzy())
			{
				return 0f;
			}
			return 600f / base.def.hugsPerCycle;
		}

		public bool IsHuggingFrenzy()
		{
			return base.smi.GetCurrentState() == base.smi.sm.hugFrenzy;
		}

		public bool IsHugging()
		{
			return base.smi.GetSMI<AnimInterruptMonitor.Instance>().anims != null;
		}

		public bool UpdateHasTarget()
		{
			if (this.hugTarget == null)
			{
				if (base.smi.sm.hugEggCooldownTimer.Get(base.smi) > 0f)
				{
					return false;
				}
				this.FindEgg();
				this.RefreshSearchTime();
			}
			return this.hugTarget != null;
		}

		public void EnterHuggingFrenzy()
		{
			base.smi.sm.hugFrenzyTimer.Set(base.smi.def.hugFrenzyDuration, base.smi);
			base.smi.sm.hugEggCooldownTimer.Set(0f, base.smi);
		}

		private void FindEgg()
		{
			this.hugTarget = null;
			ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
			ListPool<KMonoBehaviour, SquirrelHugConfig>.PooledList pooledList2 = ListPool<KMonoBehaviour, SquirrelHugConfig>.Allocate();
			Vector3 position = base.master.transform.GetPosition();
			Extents extents = new Extents(Grid.PosToCell(position), 10);
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.completeBuildings, pooledList);
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
			Navigator component = base.GetComponent<Navigator>();
			foreach (ScenePartitionerEntry scenePartitionerEntry in pooledList)
			{
				KMonoBehaviour kmonoBehaviour = scenePartitionerEntry.obj as KMonoBehaviour;
				KPrefabID component2 = kmonoBehaviour.GetComponent<KPrefabID>();
				if (!component2.HasTag(GameTags.Creatures.ReservedByCreature))
				{
					int num = Grid.PosToCell(kmonoBehaviour);
					if (component.CanReach(num))
					{
						EggIncubator component3 = kmonoBehaviour.GetComponent<EggIncubator>();
						if (component3 != null)
						{
							if (component3.Occupant == null || component3.Occupant.HasTag(GameTags.Creatures.ReservedByCreature) || !component3.Occupant.HasTag(GameTags.Egg))
							{
								continue;
							}
							if (component3.Occupant.GetComponent<Effects>().HasEffect("EggHug"))
							{
								continue;
							}
						}
						else if (!component2.HasTag(GameTags.Egg) || kmonoBehaviour.GetComponent<Effects>().HasEffect("EggHug"))
						{
							continue;
						}
						pooledList2.Add(kmonoBehaviour);
					}
				}
			}
			if (pooledList2.Count > 0)
			{
				int num2 = global::UnityEngine.Random.Range(0, pooledList2.Count);
				KMonoBehaviour kmonoBehaviour2 = pooledList2[num2];
				this.hugTarget = kmonoBehaviour2.gameObject;
			}
			pooledList.Recycle();
			pooledList2.Recycle();
		}

		public GameObject hugParticleFx;

		public Vector3 hugParticleOffset;

		public GameObject hugTarget;

		[MyCmpGet]
		private Effects effects;

		public Effect frenzyEffect;
	}
}
