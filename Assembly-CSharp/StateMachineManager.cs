using System;
using System.Collections.Generic;
using System.Reflection;

public class StateMachineManager : IScheduler
{
	public StateMachineManager()
	{
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (typeof(StateMachine).IsAssignableFrom(type) && !type.IsAbstract)
				{
					try
					{
						StateMachine stateMachine = (StateMachine)Activator.CreateInstance(type);
						stateMachine.InitializeStateMachine();
						this.stateMachines[type] = stateMachine;
					}
					catch
					{
					}
				}
			}
		}
	}

	public static StateMachineManager Instance
	{
		get
		{
			return Singleton<StateMachineManager>.Instance;
		}
	}

	public static void Destroy()
	{
		Singleton<StateMachineManager>.Destroy();
	}

	public void RegisterScheduler(Scheduler scheduler)
	{
		this.scheduler = scheduler;
	}

	public void Add(StateMachine.Instance state_machine_instance)
	{
		this.stateMachineInstances.Add(state_machine_instance);
	}

	public void Remove(StateMachine.Instance state_machine_instance)
	{
		this.stateMachineInstances.Remove(state_machine_instance);
	}

	public SchedulerHandle Schedule(string name, float time, Action<object> callback, object callback_data = null, SchedulerGroup group = null)
	{
		return this.scheduler.Schedule(name, time, callback, callback_data, group);
	}

	public SchedulerGroup CreateSchedulerGroup()
	{
		return new SchedulerGroup(this.scheduler);
	}

	public T CreateStateMachine<T>()
	{
		StateMachine stateMachine = null;
		Type typeFromHandle = typeof(T);
		if (!this.stateMachines.TryGetValue(typeFromHandle, out stateMachine))
		{
			stateMachine = (StateMachine)Activator.CreateInstance(typeFromHandle);
			stateMachine.InitializeStateMachine();
			this.stateMachines[typeFromHandle] = stateMachine;
		}
		return (T)((object)stateMachine);
	}

	private List<StateMachine.Instance> stateMachineInstances = new List<StateMachine.Instance>();

	private Scheduler scheduler;

	private float elapsedTime;

	private Dictionary<Type, StateMachine> stateMachines = new Dictionary<Type, StateMachine>();
}
