using System;
using STRINGS;
using UnityEngine;

public class DeathLoot : GameStateMachine<DeathLoot, DeathLoot.Instance, IStateMachineTarget, DeathLoot.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.root;
	}

	private StateMachine<DeathLoot, DeathLoot.Instance, IStateMachineTarget, DeathLoot.Def>.BoolParameter WasLoopDropped;

	public class Loot
	{
		public Tag Id { get; private set; } = Tag.Invalid;

		public bool IsElement { get; private set; }

		public Loot(Tag tag)
		{
			this.Id = tag;
			this.IsElement = false;
			this.Quantity = 1f;
		}

		public Loot(SimHashes element, float quantity)
		{
			this.Id = element.CreateTag();
			this.IsElement = true;
			this.Quantity = quantity;
		}

		public float Quantity;
	}

	public class Def : StateMachine.BaseDef
	{
		public DeathLoot.Loot[] loot;

		public CellOffset lootSpawnOffset;
	}

	public new class Instance : GameStateMachine<DeathLoot, DeathLoot.Instance, IStateMachineTarget, DeathLoot.Def>.GameInstance
	{
		public bool WasLoopDropped
		{
			get
			{
				return base.sm.WasLoopDropped.Get(base.smi);
			}
		}

		public Instance(IStateMachineTarget master, DeathLoot.Def def)
			: base(master, def)
		{
			base.Subscribe(1623392196, new Action<object>(this.OnDeath));
		}

		private void OnDeath(object obj)
		{
			if (!this.WasLoopDropped)
			{
				base.sm.WasLoopDropped.Set(true, this, false);
				this.CreateLoot();
			}
		}

		public GameObject[] CreateLoot()
		{
			if (base.def.loot == null)
			{
				return null;
			}
			GameObject[] array = new GameObject[base.def.loot.Length];
			for (int i = 0; i < base.def.loot.Length; i++)
			{
				DeathLoot.Loot loot = base.def.loot[i];
				if (!(loot.Id == Tag.Invalid))
				{
					GameObject gameObject = Scenario.SpawnPrefab(this.GetLootSpawnCell(), 0, 0, loot.Id.ToString(), Grid.SceneLayer.Ore);
					gameObject.SetActive(true);
					Edible component = gameObject.GetComponent<Edible>();
					if (component)
					{
						ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.BUTCHERED, "{0}", gameObject.GetProperName()), UI.ENDOFDAYREPORT.NOTES.BUTCHERED_CONTEXT);
					}
					if (loot.IsElement)
					{
						PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
						if (component2 != null)
						{
							component2.Mass = loot.Quantity;
						}
					}
					array[i] = gameObject;
				}
			}
			return array;
		}

		public int GetLootSpawnCell()
		{
			int num = Grid.PosToCell(base.gameObject);
			int num2 = Grid.OffsetCell(num, base.def.lootSpawnOffset);
			if (Grid.IsWorldValidCell(num2) && Grid.IsValidCellInWorld(num2, base.gameObject.GetMyWorldId()))
			{
				return num2;
			}
			return num;
		}

		protected override void OnCleanUp()
		{
			base.Unsubscribe(1623392196, new Action<object>(this.OnDeath));
		}
	}
}
