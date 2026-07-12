using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class BuildingInternalConstructor : GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.inoperational;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.inoperational.EventTransition(GameHashes.OperationalChanged, this.operational, (BuildingInternalConstructor.Instance smi) => smi.GetComponent<Operational>().IsOperational).Enter(delegate(BuildingInternalConstructor.Instance smi)
		{
			smi.ShowConstructionSymbol(false);
		});
		this.operational.DefaultState(this.operational.constructionRequired).EventTransition(GameHashes.OperationalChanged, this.inoperational, (BuildingInternalConstructor.Instance smi) => !smi.GetComponent<Operational>().IsOperational);
		this.operational.constructionRequired.EventTransition(GameHashes.OnStorageChange, this.operational.constructionHappening, (BuildingInternalConstructor.Instance smi) => smi.GetMassForConstruction() != null).EventTransition(GameHashes.OnStorageChange, this.operational.constructionSatisfied, (BuildingInternalConstructor.Instance smi) => smi.HasOutputInStorage()).ToggleFetch((BuildingInternalConstructor.Instance smi) => smi.CreateFetchList(), this.operational.constructionHappening)
			.ParamTransition<bool>(this.constructionRequested, this.operational.constructionSatisfied, GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.IsFalse)
			.Enter(delegate(BuildingInternalConstructor.Instance smi)
			{
				smi.ShowConstructionSymbol(true);
			})
			.Exit(delegate(BuildingInternalConstructor.Instance smi)
			{
				smi.ShowConstructionSymbol(false);
			});
		this.operational.constructionHappening.EventTransition(GameHashes.OnStorageChange, this.operational.constructionSatisfied, (BuildingInternalConstructor.Instance smi) => smi.HasOutputInStorage()).EventTransition(GameHashes.OnStorageChange, this.operational.constructionRequired, (BuildingInternalConstructor.Instance smi) => smi.GetMassForConstruction() == null).ToggleChore((BuildingInternalConstructor.Instance smi) => smi.CreateWorkChore(), this.operational.constructionHappening, this.operational.constructionHappening)
			.ParamTransition<bool>(this.constructionRequested, this.operational.constructionSatisfied, GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.IsFalse)
			.Enter(delegate(BuildingInternalConstructor.Instance smi)
			{
				smi.ShowConstructionSymbol(true);
			})
			.Exit(delegate(BuildingInternalConstructor.Instance smi)
			{
				smi.ShowConstructionSymbol(false);
			});
		this.operational.constructionSatisfied.EventTransition(GameHashes.OnStorageChange, this.operational.constructionRequired, (BuildingInternalConstructor.Instance smi) => !smi.HasOutputInStorage() && this.constructionRequested.Get(smi)).ParamTransition<bool>(this.constructionRequested, this.operational.constructionRequired, (BuildingInternalConstructor.Instance smi, bool p) => p && !smi.HasOutputInStorage());
	}

	public GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.State inoperational;

	public BuildingInternalConstructor.OperationalStates operational;

	public StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.BoolParameter constructionRequested = new StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.BoolParameter(true);

	public class Def : StateMachine.BaseDef
	{
		public DefComponent<Storage> storage;

		public float constructionMass;

		public List<string> outputIDs;

		public bool spawnIntoStorage;

		public string constructionSymbol;
	}

	public class OperationalStates : GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.State
	{
		public GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.State constructionRequired;

		public GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.State constructionHappening;

		public GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.State constructionSatisfied;
	}

	public new class Instance : GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.GameInstance, ISidescreenButtonControl
	{
		public Instance(IStateMachineTarget master, BuildingInternalConstructor.Def def)
			: base(master, def)
		{
			this.storage = def.storage.Get(this);
			base.GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new InternalConstructionCompleteCondition(this));
		}

		protected override void OnCleanUp()
		{
			Element element = null;
			float num = 0f;
			float num2 = 0f;
			byte maxValue = byte.MaxValue;
			int num3 = 0;
			foreach (string text in base.def.outputIDs)
			{
				GameObject gameObject = this.storage.FindFirst(text);
				if (gameObject != null)
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					global::Debug.Assert(element == null || element == component.Element);
					element = component.Element;
					num2 = GameUtil.GetFinalTemperature(num, num2, component.Mass, component.Temperature);
					num += component.Mass;
					gameObject.DeleteObject();
				}
			}
			if (element != null)
			{
				element.substance.SpawnResource(base.transform.GetPosition(), num, num2, maxValue, num3, false, false, false);
			}
			base.OnCleanUp();
		}

		public FetchList2 CreateFetchList()
		{
			FetchList2 fetchList = new FetchList2(this.storage, Db.Get().ChoreTypes.Fetch);
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			fetchList.Add(component.Element.tag, null, base.def.constructionMass, Operational.State.None);
			return fetchList;
		}

		public PrimaryElement GetMassForConstruction()
		{
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			return this.storage.FindFirstWithMass(component.Element.tag, base.def.constructionMass);
		}

		public bool HasOutputInStorage()
		{
			return this.storage.FindFirst(base.def.outputIDs[0].ToTag());
		}

		public bool IsRequestingConstruction()
		{
			base.sm.constructionRequested.Get(this);
			return base.smi.sm.constructionRequested.Get(base.smi);
		}

		public void ConstructionComplete(bool force = false)
		{
			SimHashes simHashes;
			if (!force)
			{
				PrimaryElement massForConstruction = this.GetMassForConstruction();
				simHashes = massForConstruction.ElementID;
				float mass = massForConstruction.Mass;
				float num = massForConstruction.Temperature * massForConstruction.Mass;
				massForConstruction.Mass -= base.def.constructionMass;
				Mathf.Clamp(num / mass, 288.15f, 318.15f);
			}
			else
			{
				simHashes = SimHashes.Cuprite;
				float temperature = base.GetComponent<PrimaryElement>().Temperature;
			}
			foreach (string text in base.def.outputIDs)
			{
				GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(text), base.transform.GetPosition(), Grid.SceneLayer.Ore, null, 0);
				gameObject.GetComponent<PrimaryElement>().SetElement(simHashes, false);
				gameObject.SetActive(true);
				if (base.def.spawnIntoStorage)
				{
					this.storage.Store(gameObject, false, false, true, false);
				}
			}
		}

		public WorkChore<BuildingInternalConstructorWorkable> CreateWorkChore()
		{
			return new WorkChore<BuildingInternalConstructorWorkable>(Db.Get().ChoreTypes.Build, base.master, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		}

		public void ShowConstructionSymbol(bool show)
		{
			KBatchedAnimController component = base.master.GetComponent<KBatchedAnimController>();
			if (component != null)
			{
				component.SetSymbolVisiblity(base.def.constructionSymbol, show);
			}
		}

		public string SidescreenButtonText
		{
			get
			{
				if (!base.smi.sm.constructionRequested.Get(base.smi))
				{
					return string.Format(UI.UISIDESCREENS.BUTTONMENUSIDESCREEN.ALLOW_INTERNAL_CONSTRUCTOR.text, Assets.GetPrefab(base.def.outputIDs[0]).GetProperName());
				}
				return string.Format(UI.UISIDESCREENS.BUTTONMENUSIDESCREEN.DISALLOW_INTERNAL_CONSTRUCTOR.text, Assets.GetPrefab(base.def.outputIDs[0]).GetProperName());
			}
		}

		public string SidescreenButtonTooltip
		{
			get
			{
				if (!base.smi.sm.constructionRequested.Get(base.smi))
				{
					return string.Format(UI.UISIDESCREENS.BUTTONMENUSIDESCREEN.ALLOW_INTERNAL_CONSTRUCTOR_TOOLTIP.text, Assets.GetPrefab(base.def.outputIDs[0]).GetProperName());
				}
				return string.Format(UI.UISIDESCREENS.BUTTONMENUSIDESCREEN.DISALLOW_INTERNAL_CONSTRUCTOR_TOOLTIP.text, Assets.GetPrefab(base.def.outputIDs[0]).GetProperName());
			}
		}

		public void OnSidescreenButtonPressed()
		{
			base.smi.sm.constructionRequested.Set(!base.smi.sm.constructionRequested.Get(base.smi), base.smi, false);
			if (DebugHandler.InstantBuildMode && base.smi.sm.constructionRequested.Get(base.smi) && !this.HasOutputInStorage())
			{
				this.ConstructionComplete(true);
			}
		}

		public void SetButtonTextOverride(ButtonMenuTextOverride text)
		{
			throw new NotImplementedException();
		}

		public bool SidescreenEnabled()
		{
			return true;
		}

		public bool SidescreenButtonInteractable()
		{
			return true;
		}

		public int ButtonSideScreenSortOrder()
		{
			return 20;
		}

		public int HorizontalGroupID()
		{
			return -1;
		}

		private Storage storage;

		[Serialize]
		private float constructionElapsed;

		private ProgressBar progressBar;
	}
}
