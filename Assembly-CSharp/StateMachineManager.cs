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

	public StateMachine.Instance CreateSMIFromDef(IStateMachineTarget master, StateMachine.Instance.BaseDef def)
	{
		StateMachineManager.parameters[0] = master;
		StateMachineManager.parameters[1] = def;
		return (StateMachine.Instance)Activator.CreateInstance(def.GetType().DeclaringType, StateMachineManager.parameters);
	}

	private Scheduler scheduler;

	private float elapsedTime;

	private Dictionary<Type, StateMachine> stateMachines = new Dictionary<Type, StateMachine>();

	private static object[] parameters = new object[2];
}
