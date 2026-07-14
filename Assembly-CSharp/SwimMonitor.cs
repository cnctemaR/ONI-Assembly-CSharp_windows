using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class SwimMonitor : GameStateMachine<SwimMonitor, SwimMonitor.Instance, IStateMachineTarget>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.cannotSwim;
		this.root.EventHandler(GameHashes.RolesUpdated, (SwimMonitor.Instance smi) => Game.Instance, new StateMachine<SwimMonitor, SwimMonitor.Instance, IStateMachineTarget, object>.State.Callback(SwimMonitor.CheckSwimSkill)).EventHandler(GameHashes.PathAdvanced, delegate(SwimMonitor.Instance smi, object data)
		{
			this.OnPathAdvanced(smi, data);
		});
		this.cannotSwim.Enter(delegate(SwimMonitor.Instance smi)
		{
			if (smi.navigator.CurrentNavType == NavType.Swim)
			{
				smi.navigator.SetCurrentNavType(NavType.Floor);
				smi.navigator.Stop(false, true);
			}
		}).ParamTransition<bool>(this.hasSwimSkill, this.canSwim, GameStateMachine<SwimMonitor, SwimMonitor.Instance, IStateMachineTarget, object>.IsTrue);
		this.canSwim.ParamTransition<bool>(this.hasSwimSkill, this.cannotSwim, GameStateMachine<SwimMonitor, SwimMonitor.Instance, IStateMachineTarget, object>.IsFalse).Enter(delegate(SwimMonitor.Instance smi)
		{
			bool flag = smi.navigator.CurrentNavType == NavType.Swim;
			if (smi.resume.HasPerk(smi.swimStaminaPerk))
			{
				smi.ApplyAttributeModifier(SwimMonitor.swimmingStaminaModifier, flag);
			}
			if (smi.resume.HasPerk(smi.swimAthleticPerk))
			{
				smi.ApplyAttributeModifier(SwimMonitor.swimmingAthleticsModifier, flag);
			}
			smi.previousNavType = smi.navigator.CurrentNavType;
		}).ToggleAnims("anim_loco_swim_kanim", 0f)
			.Update(delegate(SwimMonitor.Instance smi, float dt)
			{
				SwimMonitor.UpdateSwimOffset(smi);
			}, UpdateRate.SIM_200ms, false);
	}

	private static void UpdateSwimOffset(SwimMonitor.Instance smi)
	{
		if (!smi.navigator.IsMoving() && smi.navigator.CurrentNavType == NavType.Swim)
		{
			SwimMonitor.SetSwimOffset(smi);
		}
	}

	private static void SetSwimOffset(SwimMonitor.Instance smi)
	{
		Vector3 offset = smi.animController.Offset;
		offset.y = SwimMonitor.ComputeSwimOffsetY(smi.navigator.cachedCell);
		if (MathF.Abs(offset.y - smi.animController.Offset.y) > SwimMonitor.OffsetEpsilon)
		{
			smi.animController.Offset = offset;
		}
	}

	public static void CheckSwimSkill(SwimMonitor.Instance smi)
	{
		smi.sm.hasSwimSkill.Set(smi.resume.HasPerk(Db.Get().SkillPerks.CanSwim), smi, false);
	}

	public static float ComputeSwimOffsetY(int cell)
	{
		return Mathf.Clamp((Grid.Mass[cell] / 1000f - 1f) * 0.5f, -0.6f, 0f);
	}

	public void OnPathAdvanced(SwimMonitor.Instance smi, object data)
	{
		bool flag = smi.sm.hasSwimSkill.Get(smi);
		bool flag2 = smi.navigator.CurrentNavType == NavType.Swim;
		if (flag)
		{
			bool flag3 = false;
			if (flag2)
			{
				flag3 = (smi.navigator.flags & PathFinder.PotentialPath.Flags.HasAtmoSuit) > PathFinder.PotentialPath.Flags.None || (smi.navigator.flags & PathFinder.PotentialPath.Flags.HasJetPack) > PathFinder.PotentialPath.Flags.None || (smi.navigator.flags & PathFinder.PotentialPath.Flags.HasLeadSuit) > PathFinder.PotentialPath.Flags.None;
			}
			bool flag4 = smi.previousNavType == NavType.Swim;
			if (flag2 != flag4)
			{
				if (smi.resume.HasPerk(smi.swimStaminaPerk))
				{
					smi.ApplyAttributeModifier(SwimMonitor.swimmingStaminaModifier, flag2);
				}
				if (smi.resume.HasPerk(smi.swimAthleticPerk))
				{
					smi.ApplyAttributeModifier(SwimMonitor.swimmingAthleticsModifier, flag2);
				}
			}
			smi.previousNavType = smi.navigator.CurrentNavType;
			smi.selectable.ToggleStatusItem(SwimMonitor.HasSuitSwimPenalty, flag3, null);
		}
		bool flag5 = flag2 || Grid.IsSubstantialLiquid(smi.navigator.cachedCell, 0.35f);
		if (flag5 != smi.wasInLiquid)
		{
			smi.ApplyAttributeModifier(SwimMonitor.inLiquidStaminaModifier, flag5);
			smi.wasInLiquid = flag5;
		}
	}

	public static StatusItem HasSuitSwimPenalty = new StatusItem("HasSuitSwimPenalty", DUPLICANTS.STATUSITEMS.HASSUITSWIMPENALTY.NAME, DUPLICANTS.STATUSITEMS.HASSUITSWIMPENALTY.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 129022, true, null);

	public static Dictionary<HashedString, HashedString> SurfaceSwimOverride = new Dictionary<HashedString, HashedString> { { "swim_swim_1_0_loop", "shallow_swim_1_0_loop" } };

	public static Dictionary<HashedString, Dictionary<HashedString, HashedString>> transitionAnims = new Dictionary<HashedString, Dictionary<HashedString, HashedString>>
	{
		{
			"treading_loop",
			new Dictionary<HashedString, HashedString>
			{
				{ "shallow_swim_1_0_loop", "treading_trans_shallow_swim_1_0" },
				{ "swim_swim_1_0_loop", "treading_trans_swim_1_0" },
				{ "swim_swim_1_-1_loop", "treading_trans_swim_1_-1" },
				{ "swim_swim_0_-1_loop", "treading_trans_swim_0_-1" }
			}
		},
		{
			"shallow_swim_1_0_loop",
			new Dictionary<HashedString, HashedString>
			{
				{ "swim_swim_1_0_loop", "shallow_trans_swim_1_0" },
				{ "swim_swim_1_-1_loop", "shallow_trans_swim_1_-1" },
				{ "swim_swim_0_-1_loop", "shallow_trans_swim_0_-1" }
			}
		},
		{
			"swim_swim_1_0_loop",
			new Dictionary<HashedString, HashedString>
			{
				{ "shallow_swim_1_0_loop", "horizontal_trans_shallow_swim_1_0" },
				{ "swim_swim_1_-1_loop", "horizontal_trans_swim_1_-1" },
				{ "swim_swim_0_-1_loop", "horizontal_trans_swim_0_-1" },
				{ "swim_swim_0_1_loop", "horizontal_trans_swim_0_1" },
				{ "swim_swim_1_1_loop", "horizontal_trans_swim_1_1" }
			}
		},
		{
			"swim_swim_0_1_loop",
			new Dictionary<HashedString, HashedString>
			{
				{ "swim_swim_1_-1_loop", "up_trans_swim_1_-1" },
				{ "swim_swim_0_-1_loop", "up_trans_swim_0_-1" },
				{ "swim_swim_0_1_loop", "up_trans_swim_1_0" },
				{ "swim_swim_1_1_loop", "up_trans_swim_1_1" }
			}
		},
		{
			"swim_swim_0_-1_loop",
			new Dictionary<HashedString, HashedString>
			{
				{ "swim_swim_1_0_loop", "down_trans_swim_1_0" },
				{ "swim_swim_0_1_loop", "down_trans_swim_0_1" },
				{ "swim_swim_1_-1_loop", "down_trans_swim_1_-1" },
				{ "swim_swim_1_1_loop", "down_trans_swim_1_1" }
			}
		},
		{
			"swim_swim_1_1_loop",
			new Dictionary<HashedString, HashedString>
			{
				{ "swim_swim_1_0_loop", "up_diagonal_trans_swim_1_0" },
				{ "swim_swim_0_1_loop", "up_diagonal_trans_swim_0_1" },
				{ "swim_swim_1_-1_loop", "up_diagonal_trans_swim_1_-1" },
				{ "swim_swim_0_-1_loop", "up_diagonal_trans_swim_0_-1" },
				{ "shallow_swim_1_0_loop", "up_diagonal_trans_shallow_swim_1_0" }
			}
		},
		{
			"swim_swim_1_-1_loop",
			new Dictionary<HashedString, HashedString>
			{
				{ "swim_swim_0_1_loop", "down_diagonal_trans_swim_0_1" },
				{ "swim_swim_1_0_loop", "down_diagonal_trans_swim_1_0" },
				{ "swim_swim_1_1_loop", "down_diagonal_trans_swim_1_1" },
				{ "swim_swim_0_-1_loop", "down_diagonal_trans_swim_0_-1" }
			}
		}
	};

	public static float OffsetEpsilon = 0.0001f;

	private static AttributeModifier swimmingStaminaModifier = new AttributeModifier(Db.Get().Amounts.Stamina.deltaAttribute.Id, 0.06666667f, DUPLICANTS.MODIFIERS.SWIMMINGSTAMINA.NAME, false, false, true);

	private static AttributeModifier swimmingAthleticsModifier = new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, 3f, DUPLICANTS.MODIFIERS.SWIMMINGATHLETICS.NAME, false, false, true);

	private static AttributeModifier inLiquidStaminaModifier = new AttributeModifier(Db.Get().Amounts.Stamina.deltaAttribute.Id, -0.06666667f, DUPLICANTS.MODIFIERS.INLIQUIDSTAMINA.NAME, false, false, true);

	public GameStateMachine<SwimMonitor, SwimMonitor.Instance, IStateMachineTarget, object>.State cannotSwim;

	public GameStateMachine<SwimMonitor, SwimMonitor.Instance, IStateMachineTarget, object>.State canSwim;

	public StateMachine<SwimMonitor, SwimMonitor.Instance, IStateMachineTarget, object>.BoolParameter hasSwimSkill;

	public StateMachine<SwimMonitor, SwimMonitor.Instance, IStateMachineTarget, object>.BoolParameter hasSwimSkill2;

	public new class Instance : GameStateMachine<SwimMonitor, SwimMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.navigator = base.GetComponent<Navigator>();
			this.animController = base.GetComponent<KBatchedAnimController>();
			this.selectable = base.GetComponent<KSelectable>();
			this.previousNavType = this.navigator.CurrentNavType;
			this.swimStaminaPerk = Db.Get().SkillPerks.IncreaseSwimmerStaminaInLiquid;
			this.swimAthleticPerk = Db.Get().SkillPerks.IncreaseSwimmerAthleticsInLiquid;
			this.wasInLiquid = Grid.IsSubstantialLiquid(this.navigator.cachedCell, 0.35f);
			if (this.wasInLiquid)
			{
				this.ApplyAttributeModifier(SwimMonitor.inLiquidStaminaModifier, true);
			}
			SwimMonitor.CheckSwimSkill(this);
		}

		public bool CanSwim()
		{
			return base.sm.hasSwimSkill.Get(this);
		}

		public void ApplyAttributeModifier(AttributeModifier modifier, bool add)
		{
			Klei.AI.Attributes attributes = base.gameObject.GetAttributes();
			if (add)
			{
				attributes.Add(modifier);
				return;
			}
			attributes.Remove(modifier);
		}

		[MyCmpReq]
		public MinionResume resume;

		public Navigator navigator;

		public KBatchedAnimController animController;

		public KSelectable selectable;

		public SkillPerk swimStaminaPerk;

		public SkillPerk swimAthleticPerk;

		public NavType previousNavType;

		public bool wasInLiquid;
	}
}
