using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class CropTendingStates : GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.findCrop;
		this.root.Exit(delegate(CropTendingStates.Instance smi)
		{
			this.UnreserveCrop(smi);
			if (!smi.tendedSucceeded)
			{
				this.RestoreSymbolsVisibility(smi);
			}
		});
		this.findCrop.Enter(delegate(CropTendingStates.Instance smi)
		{
			this.FindCrop(smi);
			if (smi.sm.targetCrop.Get(smi) == null)
			{
				smi.GoTo(this.behaviourcomplete);
				return;
			}
			this.ReserverCrop(smi);
			smi.GoTo(this.moveToCrop);
		});
		this.moveToCrop.ToggleStatusItem(CREATURES.STATUSITEMS.DIVERGENT_WILL_TEND.NAME, CREATURES.STATUSITEMS.DIVERGENT_WILL_TEND.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).MoveTo((CropTendingStates.Instance smi) => smi.moveCell, this.tendCrop, this.behaviourcomplete, false).ParamTransition<GameObject>(this.targetCrop, this.behaviourcomplete, (CropTendingStates.Instance smi, GameObject p) => this.targetCrop.Get(smi) == null);
		this.tendCrop.DefaultState(this.tendCrop.pre).ToggleStatusItem(CREATURES.STATUSITEMS.DIVERGENT_TENDING.NAME, CREATURES.STATUSITEMS.DIVERGENT_TENDING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).ParamTransition<GameObject>(this.targetCrop, this.behaviourcomplete, (CropTendingStates.Instance smi, GameObject p) => this.targetCrop.Get(smi) == null)
			.Enter(delegate(CropTendingStates.Instance smi)
			{
				smi.animSet = this.GetCropTendingAnimSet(smi);
				this.StoreSymbolsVisibility(smi);
			});
		this.tendCrop.pre.Face(this.targetCrop, 0f).PlayAnim((CropTendingStates.Instance smi) => smi.animSet.crop_tending_pre, KAnim.PlayMode.Once).OnAnimQueueComplete(this.tendCrop.tend);
		this.tendCrop.tend.Enter(delegate(CropTendingStates.Instance smi)
		{
			this.SetSymbolsVisibility(smi, false);
		}).QueueAnim((CropTendingStates.Instance smi) => smi.animSet.crop_tending, false, null).OnAnimQueueComplete(this.tendCrop.pst);
		this.tendCrop.pst.QueueAnim((CropTendingStates.Instance smi) => smi.animSet.crop_tending_pst, false, null).OnAnimQueueComplete(this.behaviourcomplete).Exit(delegate(CropTendingStates.Instance smi)
		{
			GameObject gameObject = smi.sm.targetCrop.Get(smi);
			if (gameObject != null)
			{
				if (smi.effect != null)
				{
					gameObject.GetComponent<Effects>().Add(smi.effect, true);
				}
				smi.tendedSucceeded = true;
				CropTendingStates.CropTendingEventData cropTendingEventData = new CropTendingStates.CropTendingEventData
				{
					source = smi.gameObject,
					cropId = smi.sm.targetCrop.Get(smi).PrefabID()
				};
				smi.sm.targetCrop.Get(smi).Trigger(90606262, cropTendingEventData);
				smi.Trigger(90606262, cropTendingEventData);
			}
		});
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToTendCrops, false);
	}

	private CropTendingStates.AnimSet GetCropTendingAnimSet(CropTendingStates.Instance smi)
	{
		CropTendingStates.AnimSet animSet;
		if (smi.def.animSetOverrides.TryGetValue(this.targetCrop.Get(smi).PrefabID(), out animSet))
		{
			return animSet;
		}
		return CropTendingStates.defaultAnimSet;
	}

	private void FindCrop(CropTendingStates.Instance smi)
	{
		Navigator component = smi.GetComponent<Navigator>();
		Crop crop = null;
		int num = Grid.InvalidCell;
		int num2 = 100;
		int num3 = -1;
		foreach (Crop crop2 in Components.Crops.GetWorldItems(smi.gameObject.GetMyWorldId(), false))
		{
			if (smi.effect != null)
			{
				Effects component2 = crop2.GetComponent<Effects>();
				if (component2 != null)
				{
					bool flag = false;
					foreach (string text in smi.def.ignoreEffectGroup)
					{
						if (component2.HasEffect(text))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						continue;
					}
				}
			}
			Growing component3 = crop2.GetComponent<Growing>();
			if ((!(component3 != null) || !component3.IsGrown()) && !crop2.HasTag(GameTags.Creatures.ReservedByCreature) && Vector2.SqrMagnitude(crop2.transform.position - smi.transform.position) <= 625f)
			{
				int num4;
				smi.def.interests.TryGetValue(crop2.PrefabID(), out num4);
				if (num4 >= num3)
				{
					bool flag2 = num4 > num3;
					int num5 = Grid.PosToCell(crop2);
					int[] array = new int[]
					{
						Grid.CellLeft(num5),
						Grid.CellRight(num5)
					};
					int num6 = 100;
					int num7 = Grid.InvalidCell;
					for (int j = 0; j < array.Length; j++)
					{
						if (Grid.IsValidCell(array[j]))
						{
							int navigationCost = component.GetNavigationCost(array[j]);
							if (navigationCost != -1 && navigationCost < num6)
							{
								num6 = navigationCost;
								num7 = array[j];
							}
						}
					}
					if (num6 != -1 && num7 != Grid.InvalidCell && (flag2 || num6 < num2))
					{
						num = num7;
						num2 = num6;
						num3 = num4;
						crop = crop2;
					}
				}
			}
		}
		GameObject gameObject = ((crop != null) ? crop.gameObject : null);
		smi.sm.targetCrop.Set(gameObject, smi, false);
		smi.moveCell = num;
	}

	private void ReserverCrop(CropTendingStates.Instance smi)
	{
		GameObject gameObject = smi.sm.targetCrop.Get(smi);
		if (gameObject != null)
		{
			DebugUtil.Assert(!gameObject.HasTag(GameTags.Creatures.ReservedByCreature));
			gameObject.AddTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private void UnreserveCrop(CropTendingStates.Instance smi)
	{
		GameObject gameObject = smi.sm.targetCrop.Get(smi);
		if (gameObject != null)
		{
			gameObject.RemoveTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private void SetSymbolsVisibility(CropTendingStates.Instance smi, bool isVisible)
	{
		if (this.targetCrop.Get(smi) != null)
		{
			string[] hide_symbols_after_pre = smi.animSet.hide_symbols_after_pre;
			if (hide_symbols_after_pre != null)
			{
				KAnimControllerBase component = this.targetCrop.Get(smi).GetComponent<KAnimControllerBase>();
				if (component != null)
				{
					foreach (string text in hide_symbols_after_pre)
					{
						component.SetSymbolVisiblity(text, isVisible);
					}
				}
			}
		}
	}

	private void StoreSymbolsVisibility(CropTendingStates.Instance smi)
	{
		if (this.targetCrop.Get(smi) != null)
		{
			string[] hide_symbols_after_pre = smi.animSet.hide_symbols_after_pre;
			if (hide_symbols_after_pre != null)
			{
				KAnimControllerBase component = this.targetCrop.Get(smi).GetComponent<KAnimControllerBase>();
				if (component != null)
				{
					smi.symbolStates = new bool[hide_symbols_after_pre.Length];
					for (int i = 0; i < hide_symbols_after_pre.Length; i++)
					{
						smi.symbolStates[i] = component.GetSymbolVisiblity(hide_symbols_after_pre[i]);
					}
				}
			}
		}
	}

	private void RestoreSymbolsVisibility(CropTendingStates.Instance smi)
	{
		if (this.targetCrop.Get(smi) != null && smi.symbolStates != null)
		{
			string[] hide_symbols_after_pre = smi.animSet.hide_symbols_after_pre;
			if (hide_symbols_after_pre != null)
			{
				KAnimControllerBase component = this.targetCrop.Get(smi).GetComponent<KAnimControllerBase>();
				if (component != null)
				{
					for (int i = 0; i < hide_symbols_after_pre.Length; i++)
					{
						component.SetSymbolVisiblity(hide_symbols_after_pre[i], smi.symbolStates[i]);
					}
				}
			}
		}
	}

	private const int MAX_NAVIGATE_DISTANCE = 100;

	private const int MAX_SQR_EUCLIDEAN_DISTANCE = 625;

	private static CropTendingStates.AnimSet defaultAnimSet = new CropTendingStates.AnimSet
	{
		crop_tending_pre = "crop_tending_pre",
		crop_tending = "crop_tending_loop",
		crop_tending_pst = "crop_tending_pst"
	};

	public StateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.TargetParameter targetCrop;

	private GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State findCrop;

	private GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State moveToCrop;

	private CropTendingStates.TendingStates tendCrop;

	private GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State behaviourcomplete;

	public class AnimSet
	{
		public string crop_tending_pre;

		public string crop_tending;

		public string crop_tending_pst;

		public string[] hide_symbols_after_pre;
	}

	public class CropTendingEventData
	{
		public GameObject source;

		public Tag cropId;
	}

	public class Def : StateMachine.BaseDef
	{
		public string effectId;

		public string[] ignoreEffectGroup;

		public Dictionary<Tag, int> interests = new Dictionary<Tag, int>();

		public Dictionary<Tag, CropTendingStates.AnimSet> animSetOverrides = new Dictionary<Tag, CropTendingStates.AnimSet>();
	}

	public new class Instance : GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.GameInstance
	{
		public Instance(Chore<CropTendingStates.Instance> chore, CropTendingStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToTendCrops);
			this.effect = Db.Get().effects.TryGet(base.smi.def.effectId);
		}

		public Effect effect;

		public int moveCell;

		public CropTendingStates.AnimSet animSet;

		public bool tendedSucceeded;

		public bool[] symbolStates;
	}

	public class TendingStates : GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State
	{
		public GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State pre;

		public GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State tend;

		public GameStateMachine<CropTendingStates, CropTendingStates.Instance, IStateMachineTarget, CropTendingStates.Def>.State pst;
	}
}
