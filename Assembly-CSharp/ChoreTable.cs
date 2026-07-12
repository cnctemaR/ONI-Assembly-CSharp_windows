using System;
using System.Collections.Generic;

public class ChoreTable
{
	public ChoreTable(ChoreTable.Entry[] entries)
	{
		this.entries = entries;
	}

	public ref ChoreTable.Entry GetEntry<T>()
	{
		ref ChoreTable.Entry ptr = ref ChoreTable.InvalidEntry;
		for (int i = 0; i < this.entries.Length; i++)
		{
			if (this.entries[i].stateMachineDef is T)
			{
				ptr = ref this.entries[i];
				break;
			}
		}
		return ref ptr;
	}

	public int GetChorePriority<StateMachineType>(ChoreConsumer chore_consumer)
	{
		for (int i = 0; i < this.entries.Length; i++)
		{
			ChoreTable.Entry entry = this.entries[i];
			if (entry.stateMachineDef.GetStateMachineType() == typeof(StateMachineType))
			{
				return entry.choreType.priority;
			}
		}
		Debug.LogError(chore_consumer.name + "'s chore table does not have an entry for: " + typeof(StateMachineType).Name);
		return -1;
	}

	private ChoreTable.Entry[] entries;

	public static ChoreTable.Entry InvalidEntry;

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

		public ChoreTable.Builder Add(StateMachine.BaseDef def, bool condition = true, int forcePriority = -1)
		{
			if (condition)
			{
				ChoreTable.Builder.Info info = new ChoreTable.Builder.Info
				{
					interruptGroupId = this.interruptGroupId,
					forcePriority = forcePriority,
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
			int num = 10000;
			for (int i = 0; i < this.infos.Count; i++)
			{
				int num2 = ((this.infos[i].forcePriority != -1) ? this.infos[i].forcePriority : (num - 100));
				num = num2;
				int num3 = 10000 - i * 100;
				int num4 = this.infos[i].interruptGroupId;
				if (num4 != 0)
				{
					if (stack.Count != num4)
					{
						stack.Push(num3);
					}
					else
					{
						num3 = stack.Peek();
					}
				}
				else if (stack.Count > 0)
				{
					stack.Pop();
				}
				array[i] = new ChoreTable.Entry(this.infos[i].def, num2, num3);
			}
			return new ChoreTable(array);
		}

		private int interruptGroupId;

		private List<ChoreTable.Builder.Info> infos = new List<ChoreTable.Builder.Info>();

		private const int INVALID_PRIORITY = -1;

		private struct Info
		{
			public int interruptGroupId;

			public int forcePriority;

			public StateMachine.BaseDef def;
		}
	}

	public class ChoreTableChore<StateMachineType, StateMachineInstanceType> : Chore<StateMachineInstanceType> where StateMachineInstanceType : StateMachine.Instance
	{
		public ChoreTableChore(StateMachine.BaseDef state_machine_def, ChoreType chore_type, KPrefabID prefab_id)
			: base(chore_type, prefab_id, prefab_id.GetComponent<ChoreProvider>(), true, null, null, null, PriorityScreen.PriorityClass.basic, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
		{
			this.showAvailabilityInHoverText = false;
			base.smi = state_machine_def.CreateSMI(this) as StateMachineInstanceType;
		}
	}

	public struct Entry
	{
		public Entry(StateMachine.BaseDef state_machine_def, int priority, int interrupt_priority)
		{
			Type stateMachineInstanceType = Singleton<StateMachineManager>.Instance.CreateStateMachine(state_machine_def.GetStateMachineType()).GetStateMachineInstanceType();
			Type[] array = new Type[]
			{
				state_machine_def.GetStateMachineType(),
				stateMachineInstanceType
			};
			this.choreClassType = typeof(ChoreTable.ChoreTableChore<, >).MakeGenericType(array);
			this.choreType = new ChoreType(state_machine_def.ToString(), null, new string[0], "", "", "", "", new Tag[0], priority, priority);
			this.choreType.interruptPriority = interrupt_priority;
			this.stateMachineDef = state_machine_def;
		}

		public Type choreClassType;

		public ChoreType choreType;

		public StateMachine.BaseDef stateMachineDef;
	}

	public class Instance
	{
		public static void ResetParameters()
		{
			for (int i = 0; i < ChoreTable.Instance.parameters.Length; i++)
			{
				ChoreTable.Instance.parameters[i] = null;
			}
		}

		public Instance(ChoreTable chore_table, KPrefabID prefab_id)
		{
			this.prefabId = prefab_id;
			this.entries = ListPool<ChoreTable.Instance.Entry, ChoreTable.Instance>.Allocate();
			for (int i = 0; i < chore_table.entries.Length; i++)
			{
				this.entries.Add(new ChoreTable.Instance.Entry(chore_table.entries[i], prefab_id));
			}
		}

		~Instance()
		{
			this.OnCleanUp(this.prefabId);
		}

		public void OnCleanUp(KPrefabID prefab_id)
		{
			if (this.entries == null)
			{
				return;
			}
			for (int i = 0; i < this.entries.Count; i++)
			{
				this.entries[i].OnCleanUp(prefab_id);
			}
			this.entries.Recycle();
			this.entries = null;
		}

		private static object[] parameters = new object[3];

		private KPrefabID prefabId;

		private ListPool<ChoreTable.Instance.Entry, ChoreTable.Instance>.PooledList entries;

		private struct Entry
		{
			public Entry(ChoreTable.Entry chore_table_entry, KPrefabID prefab_id)
			{
				ChoreTable.Instance.parameters[0] = chore_table_entry.stateMachineDef;
				ChoreTable.Instance.parameters[1] = chore_table_entry.choreType;
				ChoreTable.Instance.parameters[2] = prefab_id;
				this.chore = (Chore)Activator.CreateInstance(chore_table_entry.choreClassType, ChoreTable.Instance.parameters);
				ChoreTable.Instance.parameters[0] = null;
				ChoreTable.Instance.parameters[1] = null;
				ChoreTable.Instance.parameters[2] = null;
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
