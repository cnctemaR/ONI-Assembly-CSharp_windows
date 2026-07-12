using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CargoLander : GameStateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.init;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.root.InitializeOperationalFlag(RocketModule.landedFlag, false).Enter(delegate(CargoLander.StatesInstance smi)
		{
			smi.CheckIfLoaded();
		}).EventHandler(GameHashes.OnStorageChange, delegate(CargoLander.StatesInstance smi)
		{
			smi.CheckIfLoaded();
		});
		this.init.ParamTransition<bool>(this.isLanded, this.grounded, GameStateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>.IsTrue).GoTo(this.stored);
		this.stored.TagTransition(GameTags.Stored, this.landing, true).EventHandler(GameHashes.JettisonedLander, delegate(CargoLander.StatesInstance smi)
		{
			smi.OnJettisoned();
		});
		this.landing.PlayAnim("landing", KAnim.PlayMode.Loop).Enter(delegate(CargoLander.StatesInstance smi)
		{
			smi.ShowLandingPreview(true);
		}).Exit(delegate(CargoLander.StatesInstance smi)
		{
			smi.ShowLandingPreview(false);
		})
			.Enter(delegate(CargoLander.StatesInstance smi)
			{
				smi.ResetAnimPosition();
			})
			.Update(delegate(CargoLander.StatesInstance smi, float dt)
			{
				smi.LandingUpdate(dt);
			}, UpdateRate.SIM_EVERY_TICK, false)
			.Transition(this.land, (CargoLander.StatesInstance smi) => smi.flightAnimOffset <= 0f, UpdateRate.SIM_200ms);
		this.land.PlayAnim("grounded_pre").OnAnimQueueComplete(this.grounded);
		this.grounded.DefaultState(this.grounded.loaded).ToggleOperationalFlag(RocketModule.landedFlag).Enter(delegate(CargoLander.StatesInstance smi)
		{
			smi.CheckIfLoaded();
		})
			.Enter(delegate(CargoLander.StatesInstance smi)
			{
				smi.sm.isLanded.Set(true, smi);
			});
		this.grounded.loaded.PlayAnim("grounded").ParamTransition<bool>(this.hasCargo, this.grounded.empty, GameStateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>.IsFalse).OnSignal(this.emptyCargo, this.grounded.emptying)
			.Enter(delegate(CargoLander.StatesInstance smi)
			{
				smi.DoLand();
			});
		this.grounded.emptying.PlayAnim("deploying").TriggerOnEnter(GameHashes.JettisonCargo, null).OnAnimQueueComplete(this.grounded.empty);
		this.grounded.empty.PlayAnim("deployed").ParamTransition<bool>(this.hasCargo, this.grounded.loaded, GameStateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>.IsTrue);
	}

	public StateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>.BoolParameter hasCargo;

	public StateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>.Signal emptyCargo;

	public GameStateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>.State init;

	public GameStateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>.State stored;

	public GameStateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>.State landing;

	public GameStateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>.State land;

	public CargoLander.CrashedStates grounded;

	public StateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>.BoolParameter isLanded = new StateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>.BoolParameter(false);

	public class Def : StateMachine.BaseDef
	{
		public Tag previewTag;

		public bool deployOnLanding = true;
	}

	public class CrashedStates : GameStateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>.State
	{
		public GameStateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>.State loaded;

		public GameStateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>.State emptying;

		public GameStateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>.State empty;
	}

	public class StatesInstance : GameStateMachine<CargoLander, CargoLander.StatesInstance, IStateMachineTarget, CargoLander.Def>.GameInstance, ISidescreenButtonControl
	{
		public StatesInstance(IStateMachineTarget master, CargoLander.Def def)
			: base(master, def)
		{
		}

		public void ResetAnimPosition()
		{
			base.GetComponent<KBatchedAnimController>().Offset = Vector3.up * this.flightAnimOffset;
		}

		public void OnJettisoned()
		{
			this.flightAnimOffset = 50f;
		}

		public void ShowLandingPreview(bool show)
		{
			if (show)
			{
				this.landingPreview = Util.KInstantiate(Assets.GetPrefab(base.def.previewTag), base.transform.GetPosition(), Quaternion.identity, base.gameObject, null, true, 0);
				this.landingPreview.SetActive(true);
				return;
			}
			this.landingPreview.DeleteObject();
			this.landingPreview = null;
		}

		public void LandingUpdate(float dt)
		{
			this.flightAnimOffset = Mathf.Max(this.flightAnimOffset - dt * this.topSpeed, 0f);
			this.ResetAnimPosition();
			int num = Grid.PosToCell(base.gameObject.transform.GetPosition() + new Vector3(0f, this.flightAnimOffset, 0f));
			if (Grid.IsValidCell(num))
			{
				SimMessages.EmitMass(num, (byte)ElementLoader.GetElementIndex(this.exhaustElement), dt * this.exhaustEmitRate, this.exhaustTemperature, 0, 0, -1);
			}
		}

		public void DoLand()
		{
			base.smi.master.GetComponent<KBatchedAnimController>().Offset = Vector3.zero;
			OccupyArea component = base.smi.GetComponent<OccupyArea>();
			if (component != null)
			{
				component.ApplyToCells = true;
			}
			if (base.def.deployOnLanding && this.CheckIfLoaded())
			{
				base.sm.emptyCargo.Trigger(this);
			}
			base.smi.master.gameObject.Trigger(1591811118, this);
		}

		public bool CheckIfLoaded()
		{
			bool flag = false;
			MinionStorage component = base.GetComponent<MinionStorage>();
			if (component != null)
			{
				flag |= component.GetStoredMinionInfo().Count > 0;
			}
			Storage component2 = base.GetComponent<Storage>();
			if (component2 != null && !component2.IsEmpty())
			{
				flag = true;
			}
			if (flag != base.sm.hasCargo.Get(this))
			{
				base.sm.hasCargo.Set(flag, this);
			}
			return flag;
		}

		public string SidescreenButtonText
		{
			get
			{
				return "_Cargo Lander";
			}
		}

		public string SidescreenButtonTooltip
		{
			get
			{
				return "_Cargo Lander tooltip";
			}
		}

		public bool SidescreenEnabled()
		{
			return true;
		}

		public void OnSidescreenButtonPressed()
		{
			base.sm.emptyCargo.Trigger(this);
		}

		public bool SidescreenButtonInteractable()
		{
			return base.IsInsideState(base.sm.grounded.loaded);
		}

		public int ButtonSideScreenSortOrder()
		{
			return 20;
		}

		[Serialize]
		public float flightAnimOffset = 50f;

		public float exhaustEmitRate = 2f;

		public float exhaustTemperature = 1000f;

		public SimHashes exhaustElement = SimHashes.CarbonDioxide;

		public float topSpeed = 5f;

		private GameObject landingPreview;
	}
}
