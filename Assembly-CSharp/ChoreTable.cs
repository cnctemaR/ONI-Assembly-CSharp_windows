using System;
using System.Collections.Generic;

public class ChoreTable
{
	public ChoreTable(ChoreTable.Entry[] entries)
	{
		this.entries = entries;
	}

	public int GetChorePriority<StateMachineType>(ChoreConsumer chore_consumer)
	{
		foreach (ChoreTable.Entry entry in this.entries)
		{
			if (entry.stateMachineDef.GetStateMachineType() == typeof(StateMachineType))
			{
				return entry.choreType.priority;
			}
		}
		Debug.LogError(chore_consumer.name + "'s chore table does not have an entry for: " + typeof(StateMachineType).Name);
		return -1;
	}

	private ChoreTable.Entry[] entries;

	public class Builder
	{
		public ChoreTable.Builder PushInterruptGroup()
		{
			this.interruptGroupId++;
			return this;
		}

		public ChoreTable.Builder PopInterruptGroup()
		{
			DebugUtil.Assert(this.interruptGroupId > 0);
			this.interruptGroupId--;
			return this;
		}

		public ChoreTable.Builder Add(StateMachine.BaseDef def, bool condition = true)
		{
			if (condition)
			{
				ChoreTable.Builder.Info info = new ChoreTable.Builder.Info
				{
					interruptGroupId = this.interruptGroupId,
					def = def
				};
				this.infos.Add(info);
			}
			return this;
		}

		public ChoreTable CreateTable()
		{
			DebugUtil.Assert(this.interruptGroupId == 0);
			ChoreTable.Entry[] array = new ChoreTable.Entry[this.infos.Count];
			Stack<int> stack = new Stack<int>();
			for (int i = 0; i < this.infos.Count; i++)
			{
				int num = 10000 - i * 100;
				int num2 = 10000 - i * 100;
				int num3 = this.infos[i].interruptGroupId;
				if (num3 != 0)
				{
					if (stack.Count != num3)
					{
						stack.Push(num2);
					}
					else
					{
						num2 = stack.Peek();
					}
				}
				else if (stack.Count > 0)
				{
					stack.Pop();
				}
				array[i] = new ChoreTable.Entry(this.infos[i].def, num, num2);
			}
			return new ChoreTable(array);
		}

		private int interruptGroupId;

		private List<ChoreTable.Builder.Info> infos = new List<ChoreTable.Builder.Info>();

		private struct Info
		{
			public int interruptGroupId;

			public StateMachine.BaseDef def;
		}
	}

	public class ChoreTableChore<StateMachineType, StateMachineInstanceType> : Chore<StateMachineInstanceType> where StateMachineInstanceType : StateMachine.Instance
	{
		public ChoreTableChore(StateMachine.BaseDef state_machine_def, ChoreType chore_type, KPrefabID prefab_id)
			: base(chore_type, prefab_id, prefab_id.GetComponent<ChoreProvider>(), true, null, null, null, PriorityScreen.PriorityClass.basic, 5, false, true, 0, null, false, ReportManager.ReportType.WorkTime)
		{
			this.showAvailabilityInHoverText = false;
			base.smi = state_machine_def.CreateSMI(this) as StateMachineInstanceType;
		}
	}

	public struct Entry
	{
		public Entry(StateMachine.BaseDef state_machine_def, int priority, int interrupt_priority)
		{
			StateMachine stateMachine = Singleton<StateMachineManager>.Instance.CreateStateMachine(state_machine_def.GetStateMachineType());
			Type stateMachineInstanceType = stateMachine.GetStateMachineInstanceType();
			Type[] array = new Type[]
			{
				state_machine_def.GetStateMachineType(),
				stateMachineInstanceType
			};
			this.choreClassType = typeof(ChoreTable.ChoreTableChore<, >).MakeGenericType(array);
			this.choreType = new ChoreType(state_machine_def.ToString(), null, new string[0], string.Empty, string.Empty, string.Empty, string.Empty, new Tag[0], priority, priority);
			this.choreType.interruptPriority = interrupt_priority;
			this.stateMachineDef = state_machine_def;
		}

		public Type choreClassType;

		public ChoreType choreType;

		public StateMachine.BaseDef stateMachineDef;
	}

	public class Instance
	{
		public Instance(ChoreTable chore_table, KPrefabID prefab_id)
		{
			this.entries = ListPool<ChoreTable.Instance.Entry, ChoreTable.Instance>.Allocate();
			for (int i = 0; i < chore_table.entries.Length; i++)
			{
				this.entries.Add(new ChoreTable.Instance.Entry(chore_table.entries[i], prefab_id));
			}
		}

		public static void ResetParameters()
		{
			for (int i = 0; i < ChoreTable.Instance.parameters.Length; i++)
			{
				ChoreTable.Instance.parameters[i] = null;
			}
		}

		public void OnCleanUp(KPrefabID prefab_id)
		{
			for (int i = 0; i < this.entries.Count; i++)
			{
				this.entries[i].OnCleanUp(prefab_id);
			}
			this.entries.Recycle();
			this.entries = null;
		}

		private static object[] parameters = new object[3];

		private ListPool<ChoreTable.Instance.Entry, ChoreTable.Instance>.PooledList entries;

		private struct Entry
		{
			public Entry(ChoreTable.Entry chore_table_entry, KPrefabID prefab_id)
			{
				ChoreTable.Instance.parameters[0] = chore_table_entry.stateMachineDef;
				ChoreTable.Instance.parameters[1] = chore_table_entry.choreType;
				ChoreTable.Instance.parameters[2] = prefab_id;
				this.chore = (Chore)Activator.CreateInstance(chore_table_entry.choreClassType, ChoreTable.Instance.parameters);
			}

			public void OnCleanUp(KPrefabID prefab_id)
			{
				if (this.chore != null)
				{
					this.chore.Cancel("ChoreTable.Instance.OnCleanUp");
					this.chore = null;
				}
			}

			public Chore chore;
		}
	}
}
