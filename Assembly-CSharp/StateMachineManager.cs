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
				if (!type.IsAbstract && !type.IsGenericTypeDefinition && typeof(StateMachine).IsAssignableFrom(type))
				{
					StateMachine stateMachine = (StateMachine)Activator.CreateInstance(type);
					stateMachine.InitializeStateMachine();
					this.stateMachines[type] = stateMachine;
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

	public SchedulerHandle Schedule(string name, float time, Action<object> callback, object callback_data = null, SchedulerGroup group = null)
	{
		return this.scheduler.Schedule(name, time, callback, callback_data, group);
	}

	public SchedulerGroup CreateSchedulerGroup()
	{
		return new SchedulerGroup(this.scheduler);
	}

	public StateMachine CreateStateMachine(Type type)
	{
		StateMachine stateMachine = null;
		if (!this.stateMachines.TryGetValue(type, out stateMachine))
		{
			stateMachine = (StateMachine)Activator.CreateInstance(type);
			stateMachine.InitializeStateMachine();
			this.stateMachines[type] = stateMachine;
		}
		return stateMachine;
	}

	public T CreateStateMachine<T>()
	{
		return (T)((object)this.CreateStateMachine(typeof(T)));
	}

	public StateMachine.Instance CreateSMIFromDef(IStateMachineTarget master, StateMachine.BaseDef def)
	{
		StateMachineManager.parameters[0] = master;
		StateMachineManager.parameters[1] = def;
		StateMachine stateMachine = StateMachineManager.Instance.CreateStateMachine(def.GetStateMachineType());
		Type stateMachineInstanceType = stateMachine.GetStateMachineInstanceType();
		return (StateMachine.Instance)Activator.CreateInstance(stateMachineInstanceType, StateMachineManager.parameters);
	}

	private Scheduler scheduler;

	private float elapsedTime;

	private Dictionary<Type, StateMachine> stateMachines = new Dictionary<Type, StateMachine>();

	private static object[] parameters = new object[2];
}
