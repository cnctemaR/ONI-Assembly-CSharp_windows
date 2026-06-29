using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class LureableMonitor : GameStateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.cooldown;
		this.cooldown.ScheduleGoTo(10f, this.nolure);
		this.nolure.Update("FindLure", delegate(LureableMonitor.Instance smi, float dt)
		{
			smi.FindLure();
		}, UpdateRate.SIM_1000ms, false).ParamTransition<GameObject>(this.targetLure, this.haslure, (LureableMonitor.Instance smi, GameObject p) => p != null);
		this.haslure.ParamTransition<GameObject>(this.targetLure, this.haslure, (LureableMonitor.Instance smi, GameObject p) => p == null).Update("FindLure", delegate(LureableMonitor.Instance smi, float dt)
		{
			smi.FindLure();
		}, UpdateRate.SIM_1000ms, false).ToggleBehaviour(GameTags.Creatures.MoveToLure, (LureableMonitor.Instance smi) => smi.HasLure(), delegate(LureableMonitor.Instance smi)
		{
			smi.GoTo(this.cooldown);
		});
	}

	public StateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.TargetParameter targetLure;

	public GameStateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.State nolure;

	public GameStateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.State haslure;

	public GameStateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.State cooldown;

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public Tag ActiveBaitTag
		{
			get
			{
				return this.activeBaitTag;
			}
			set
			{
				this.activeBaitTag = TagManager.Create(CreatureLure.BAIT_TAG_PREFIX + value.Name, null);
			}
		}

		public List<Descriptor> GetDescriptors(GameObject go)
		{
			return new List<Descriptor>
			{
				new Descriptor(UI.BUILDINGEFFECTS.CAPTURE_METHOD_LURE, UI.BUILDINGEFFECTS.TOOLTIPS.CAPTURE_METHOD_LURE, Descriptor.DescriptorType.Effect, false)
			};
		}

		private Tag activeBaitTag;
	}

	public new class Instance : GameStateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, LureableMonitor.Def def)
			: base(master, def)
		{
		}

		public void FindLure()
		{
			Navigator component = base.GetComponent<Navigator>();
			LureableMonitor.Instance.TagIterator tagIterator = new LureableMonitor.Instance.TagIterator(component, LureableMonitor.Instance.offsets);
			LureableMonitor.Instance.TagIterator tagIterator2 = tagIterator.AddTag("AirborneCreatureLure").AddTag(GameTags.Operational).AddTag(base.def.ActiveBaitTag);
			foreach (CreatureLure creatureLure in Components.Lures)
			{
				tagIterator2.Iterate(creatureLure);
			}
			tagIterator2.Cleanup();
			base.sm.targetLure.Set(tagIterator2.result, this);
		}

		public bool HasLure()
		{
			return base.sm.targetLure.Get(this) != null;
		}

		public GameObject GetTargetLure()
		{
			return base.sm.targetLure.Get(this);
		}

		private static CellOffset[] offsets = new CellOffset[]
		{
			new CellOffset(0, 3)
		};

		private struct TagIterator : GameScenePartitioner.Iterator
		{
			public TagIterator(Navigator navigator, CellOffset[] offsets)
			{
				this.requiredTags = ListPool<Tag, LureableMonitor.Instance.TagIterator>.Allocate();
				this.navigator = navigator;
				this.cost = PathProber.InvalidCost;
				this.result = null;
				this.offsets = offsets;
			}

			public int cost { get; private set; }

			public GameObject result { get; private set; }

			public CellOffset[] offsets { get; private set; }

			public void Iterate(object target_obj)
			{
				KMonoBehaviour kmonoBehaviour = target_obj as KMonoBehaviour;
				if (kmonoBehaviour == null)
				{
					return;
				}
				KPrefabID component = kmonoBehaviour.GetComponent<KPrefabID>();
				foreach (Tag tag in this.requiredTags)
				{
					if (!component.HasTag(tag))
					{
						return;
					}
				}
				int navigationCost = this.navigator.GetNavigationCost(Grid.PosToCell(kmonoBehaviour), this.offsets);
				if (this.cost != PathProber.InvalidCost && navigationCost > this.cost)
				{
					return;
				}
				this.cost = navigationCost;
				this.result = component.gameObject;
			}

			public LureableMonitor.Instance.TagIterator AddTag(Tag tag)
			{
				this.requiredTags.Add(tag);
				return this;
			}

			public void Cleanup()
			{
				ListPool<Tag, LureableMonitor.Instance.TagIterator>.Free(this.requiredTags);
			}

			private Navigator navigator;

			private List<Tag> requiredTags;
		}
	}
}
