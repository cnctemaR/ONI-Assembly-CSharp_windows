using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class MinorFossilDigSite : GameStateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.Idle;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.Idle.Enter(delegate(MinorFossilDigSite.Instance smi)
		{
			MinorFossilDigSite.SetEntombedStatusItemVisibility(smi, false);
		}).Enter(delegate(MinorFossilDigSite.Instance smi)
		{
			smi.SetDecorState(true);
		}).PlayAnim("object_dirty")
			.ParamTransition<bool>(this.IsQuestCompleted, this.Completed, GameStateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.IsTrue)
			.ParamTransition<bool>(this.IsRevealed, this.WaitingForQuestCompletion, GameStateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.IsTrue)
			.ParamTransition<bool>(this.MarkedForDig, this.Workable, GameStateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.IsTrue);
		this.Workable.PlayAnim("object_dirty").Toggle("Activate Entombed Status Item If Required", delegate(MinorFossilDigSite.Instance smi)
		{
			MinorFossilDigSite.SetEntombedStatusItemVisibility(smi, true);
		}, delegate(MinorFossilDigSite.Instance smi)
		{
			MinorFossilDigSite.SetEntombedStatusItemVisibility(smi, false);
		}).DefaultState(this.Workable.NonOperational)
			.ParamTransition<bool>(this.MarkedForDig, this.Idle, GameStateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.IsFalse);
		this.Workable.NonOperational.TagTransition(GameTags.Operational, this.Workable.Operational, false);
		this.Workable.Operational.Enter(new StateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.State.Callback(MinorFossilDigSite.StartWorkChore)).Exit(new StateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.State.Callback(MinorFossilDigSite.CancelWorkChore)).TagTransition(GameTags.Operational, this.Workable.NonOperational, true)
			.WorkableCompleteTransition((MinorFossilDigSite.Instance smi) => smi.GetWorkable(), this.WaitingForQuestCompletion);
		this.WaitingForQuestCompletion.Enter(delegate(MinorFossilDigSite.Instance smi)
		{
			smi.SetDecorState(false);
		}).Enter(delegate(MinorFossilDigSite.Instance smi)
		{
			MinorFossilDigSite.SetEntombedStatusItemVisibility(smi, true);
		}).Enter(new StateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.State.Callback(MinorFossilDigSite.Reveal))
			.Enter(new StateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.State.Callback(MinorFossilDigSite.ProgressStoryTrait))
			.Enter(new StateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.State.Callback(MinorFossilDigSite.DestroyUIExcavateButton))
			.Enter(new StateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.State.Callback(MinorFossilDigSite.MakeItDemolishable))
			.PlayAnim("object")
			.ParamTransition<bool>(this.IsQuestCompleted, this.Completed, GameStateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.IsTrue);
		this.Completed.Enter(delegate(MinorFossilDigSite.Instance smi)
		{
			smi.SetDecorState(false);
		}).Enter(delegate(MinorFossilDigSite.Instance smi)
		{
			MinorFossilDigSite.SetEntombedStatusItemVisibility(smi, true);
		}).PlayAnim("object")
			.Enter(new StateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.State.Callback(MinorFossilDigSite.DestroyUIExcavateButton))
			.Enter(new StateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.State.Callback(MinorFossilDigSite.MakeItDemolishable));
	}

	public static void MakeItDemolishable(MinorFossilDigSite.Instance smi)
	{
		smi.gameObject.GetComponent<Demolishable>().allowDemolition = true;
	}

	public static void DestroyUIExcavateButton(MinorFossilDigSite.Instance smi)
	{
		smi.DestroyExcavateButton();
	}

	public static void SetEntombedStatusItemVisibility(MinorFossilDigSite.Instance smi, bool val)
	{
		smi.SetEntombStatusItemVisibility(val);
	}

	public static void UnregisterFromComponents(MinorFossilDigSite.Instance smi)
	{
		Components.MinorFossilDigSites.Remove(smi);
	}

	public static void SelfDestroy(MinorFossilDigSite.Instance smi)
	{
		Util.KDestroyGameObject(smi.gameObject);
	}

	public static void StartWorkChore(MinorFossilDigSite.Instance smi)
	{
		smi.CreateWorkableChore();
	}

	public static void CancelWorkChore(MinorFossilDigSite.Instance smi)
	{
		smi.CancelWorkChore();
	}

	public static void Reveal(MinorFossilDigSite.Instance smi)
	{
		bool flag = !smi.sm.IsRevealed.Get(smi);
		smi.sm.IsRevealed.Set(true, smi, false);
		if (flag)
		{
			smi.ShowCompletionNotification();
			MinorFossilDigSite.DropLoot(smi);
		}
	}

	public static void DropLoot(MinorFossilDigSite.Instance smi)
	{
		PrimaryElement component = smi.GetComponent<PrimaryElement>();
		int num = Grid.PosToCell(smi.transform.GetPosition());
		Element element = ElementLoader.GetElement(component.Element.tag);
		if (element != null)
		{
			float num2 = component.Mass;
			int num3 = 0;
			while ((float)num3 < component.Mass / 400f)
			{
				float num4 = num2;
				if (num2 > 400f)
				{
					num4 = 400f;
					num2 -= 400f;
				}
				int num5 = (int)((float)component.DiseaseCount * (num4 / component.Mass));
				element.substance.SpawnResource(Grid.CellToPosCBC(num, Grid.SceneLayer.Ore), num4, component.Temperature, component.DiseaseIdx, num5, false, false, false);
				num3++;
			}
		}
	}

	public static void ProgressStoryTrait(MinorFossilDigSite.Instance smi)
	{
		MinorFossilDigSite.ProgressQuest(smi);
	}

	public static QuestInstance ProgressQuest(MinorFossilDigSite.Instance smi)
	{
		QuestInstance instance = QuestManager.GetInstance(FossilDigSiteConfig.hashID, Db.Get().Quests.FossilHuntQuest);
		if (instance != null)
		{
			Quest.ItemData itemData = new Quest.ItemData
			{
				CriteriaId = smi.def.fossilQuestCriteriaID,
				CurrentValue = 1f
			};
			bool flag;
			bool flag2;
			instance.TrackProgress(itemData, out flag, out flag2);
			return instance;
		}
		return null;
	}

	public GameStateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.State Idle;

	public GameStateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.State Completed;

	public GameStateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.State WaitingForQuestCompletion;

	public MinorFossilDigSite.ReadyToBeWorked Workable;

	public StateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.BoolParameter MarkedForDig;

	public StateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.BoolParameter IsRevealed;

	public StateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.BoolParameter IsQuestCompleted;

	private const string UNEXCAVATED_ANIM_NAME = "object_dirty";

	private const string EXCAVATED_ANIM_NAME = "object";

	public class Def : StateMachine.BaseDef
	{
		public HashedString fossilQuestCriteriaID;
	}

	public class ReadyToBeWorked : GameStateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.State
	{
		public GameStateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.State Operational;

		public GameStateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.State NonOperational;
	}

	public new class Instance : GameStateMachine<MinorFossilDigSite, MinorFossilDigSite.Instance, IStateMachineTarget, MinorFossilDigSite.Def>.GameInstance, ICheckboxListGroupControl
	{
		public Instance(IStateMachineTarget master, MinorFossilDigSite.Def def)
			: base(master, def)
		{
			Components.MinorFossilDigSites.Add(this);
			this.negativeDecorModifier = new AttributeModifier(Db.Get().Attributes.Decor.Id, -1f, CODEX.STORY_TRAITS.FOSSILHUNT.MISC.DECREASE_DECOR_ATTRIBUTE, true, false, true);
		}

		public void SetDecorState(bool isDusty)
		{
			if (isDusty)
			{
				base.gameObject.GetComponent<DecorProvider>().decor.Add(this.negativeDecorModifier);
				return;
			}
			base.gameObject.GetComponent<DecorProvider>().decor.Remove(this.negativeDecorModifier);
		}

		public override void StartSM()
		{
			StoryInstance storyInstance = StoryManager.Instance.GetStoryInstance(Db.Get().Stories.FossilHunt.HashId);
			if (storyInstance != null)
			{
				StoryInstance storyInstance2 = storyInstance;
				storyInstance2.StoryStateChanged = (Action<StoryInstance.State>)Delegate.Combine(storyInstance2.StoryStateChanged, new Action<StoryInstance.State>(this.OnStorytraitProgressChanged));
			}
			if (!base.sm.IsRevealed.Get(this))
			{
				this.CreateExcavateButton();
			}
			QuestInstance questInstance = QuestManager.InitializeQuest(FossilDigSiteConfig.hashID, Db.Get().Quests.FossilHuntQuest);
			questInstance.QuestProgressChanged = (Action<QuestInstance, Quest.State, float>)Delegate.Combine(questInstance.QuestProgressChanged, new Action<QuestInstance, Quest.State, float>(this.OnQuestProgressChanged));
			this.workable.SetShouldShowSkillPerkStatusItem(base.sm.MarkedForDig.Get(this));
			base.StartSM();
			this.RefreshUI();
		}

		private void OnQuestProgressChanged(QuestInstance quest, Quest.State previousState, float progressIncreased)
		{
			if (quest.CurrentState == Quest.State.Completed && base.sm.IsRevealed.Get(this))
			{
				this.OnQuestCompleted(quest);
			}
			this.RefreshUI();
		}

		public void OnQuestCompleted(QuestInstance quest)
		{
			base.sm.IsQuestCompleted.Set(true, this, false);
			quest.QuestProgressChanged = (Action<QuestInstance, Quest.State, float>)Delegate.Remove(quest.QuestProgressChanged, new Action<QuestInstance, Quest.State, float>(this.OnQuestProgressChanged));
		}

		protected override void OnCleanUp()
		{
			MinorFossilDigSite.ProgressQuest(base.smi);
			StoryInstance storyInstance = StoryManager.Instance.GetStoryInstance(Db.Get().Stories.FossilHunt.HashId);
			if (storyInstance != null)
			{
				StoryInstance storyInstance2 = storyInstance;
				storyInstance2.StoryStateChanged = (Action<StoryInstance.State>)Delegate.Remove(storyInstance2.StoryStateChanged, new Action<StoryInstance.State>(this.OnStorytraitProgressChanged));
			}
			QuestInstance instance = QuestManager.GetInstance(FossilDigSiteConfig.hashID, Db.Get().Quests.FossilHuntQuest);
			if (instance != null)
			{
				QuestInstance questInstance = instance;
				questInstance.QuestProgressChanged = (Action<QuestInstance, Quest.State, float>)Delegate.Remove(questInstance.QuestProgressChanged, new Action<QuestInstance, Quest.State, float>(this.OnQuestProgressChanged));
			}
			Components.MinorFossilDigSites.Remove(this);
			base.OnCleanUp();
		}

		public void OnStorytraitProgressChanged(StoryInstance.State state)
		{
			if (state != StoryInstance.State.IN_PROGRESS)
			{
				return;
			}
			this.RefreshUI();
		}

		public void RefreshUI()
		{
			base.Trigger(1980521255, null);
		}

		public Workable GetWorkable()
		{
			return this.workable;
		}

		public void CreateWorkableChore()
		{
			if (this.chore == null)
			{
				this.chore = new WorkChore<MinorDigSiteWorkable>(Db.Get().ChoreTypes.ExcavateFossil, this.workable, null, true, null, null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			}
		}

		public void CancelWorkChore()
		{
			if (this.chore != null)
			{
				this.chore.Cancel("MinorFossilDigsite.CancelChore");
				this.chore = null;
			}
		}

		public void SetEntombStatusItemVisibility(bool visible)
		{
			this.entombComponent.SetShowStatusItemOnEntombed(visible);
		}

		public void ShowCompletionNotification()
		{
			FossilHuntInitializer.Instance smi = base.gameObject.GetSMI<FossilHuntInitializer.Instance>();
			if (smi != null)
			{
				smi.ShowObjectiveCompletedNotification();
			}
		}

		public void OnExcavateButtonPressed()
		{
			base.sm.MarkedForDig.Set(!base.sm.MarkedForDig.Get(this), this, false);
			this.workable.SetShouldShowSkillPerkStatusItem(base.sm.MarkedForDig.Get(this));
		}

		public ExcavateButton CreateExcavateButton()
		{
			if (this.excavateButton == null)
			{
				this.excavateButton = base.gameObject.AddComponent<ExcavateButton>();
				ExcavateButton excavateButton = this.excavateButton;
				excavateButton.OnButtonPressed = (global::System.Action)Delegate.Combine(excavateButton.OnButtonPressed, new global::System.Action(this.OnExcavateButtonPressed));
				this.excavateButton.isMarkedForDig = () => base.sm.MarkedForDig.Get(this);
			}
			return this.excavateButton;
		}

		public void DestroyExcavateButton()
		{
			this.workable.SetShouldShowSkillPerkStatusItem(false);
			if (this.excavateButton != null)
			{
				global::UnityEngine.Object.DestroyImmediate(this.excavateButton);
				this.excavateButton = null;
			}
		}

		public string Title
		{
			get
			{
				return CODEX.STORY_TRAITS.FOSSILHUNT.NAME;
			}
		}

		public string Description
		{
			get
			{
				if (base.sm.IsRevealed.Get(this))
				{
					return CODEX.STORY_TRAITS.FOSSILHUNT.DESCRIPTION_REVEALED;
				}
				return CODEX.STORY_TRAITS.FOSSILHUNT.DESCRIPTION_BUILDINGMENU_COVERED;
			}
		}

		public bool SidescreenEnabled()
		{
			return !base.sm.IsQuestCompleted.Get(this);
		}

		public ICheckboxListGroupControl.ListGroup[] GetData()
		{
			return FossilHuntInitializer.GetFossilHuntQuestData();
		}

		public int CheckboxSideScreenSortOrder()
		{
			return 20;
		}

		[MyCmpGet]
		private MinorDigSiteWorkable workable;

		[MyCmpGet]
		private EntombVulnerable entombComponent;

		private ExcavateButton excavateButton;

		private Chore chore;

		private AttributeModifier negativeDecorModifier;
	}
}
