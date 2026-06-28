using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class StateMachineController : KMonoBehaviour, ISaveLoadableDetailJson
{
	public int Count
	{
		get
		{
			return this.stateMachines.Count;
		}
	}

	public IEnumerator<StateMachine.Instance> GetEnumerator()
	{
		return this.stateMachines.GetEnumerator();
	}

	public void AddStateMachineInstance(StateMachine.Instance state_machine)
	{
		if (!this.stateMachines.Contains(state_machine))
		{
			this.stateMachines.Add(state_machine);
		}
		LoggerFS loggerFS = state_machine.GetLog();
		LoggerFS loggerFS2 = loggerFS;
		loggerFS2.OnLog = (Action<LoggerFS.Entry>)Delegate.Combine(loggerFS2.OnLog, new Action<LoggerFS.Entry>(this.OnLog));
		this.logs.Remove(loggerFS);
		this.logs.Add(loggerFS);
		if (this.logs.Count > 25)
		{
			this.logs.RemoveAt(0);
		}
	}

	public void RemoveStateMachineInstance(StateMachine.Instance state_machine)
	{
		LoggerFS loggerFS = state_machine.GetLog();
		LoggerFS loggerFS2 = loggerFS;
		loggerFS2.OnLog = (Action<LoggerFS.Entry>)Delegate.Remove(loggerFS2.OnLog, new Action<LoggerFS.Entry>(this.OnLog));
		if (!state_machine.GetStateMachine().saveHistory && !state_machine.GetStateMachine().debugSettings.saveHistory)
		{
			this.stateMachines.Remove(state_machine);
		}
	}

	public bool HasStateMachineInstance(StateMachine.Instance state_machine)
	{
		return this.stateMachines.Contains(state_machine);
	}

	public List<LoggerFS> GetLogs()
	{
		return this.logs;
	}

	public LoggerFS GetLog()
	{
		return this.log;
	}

	private void OnLog(LoggerFS.Entry entry)
	{
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.log.SetName(base.name);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		while (this.stateMachines.Count > 0)
		{
			StateMachine.Instance instance = this.stateMachines[0];
			instance.StopSM("StateMachineController.OnCleanUp");
			this.stateMachines.Remove(instance);
		}
	}

	public void Serialize(BinaryWriter writer)
	{
		this.serializer.Serialize(this.stateMachines, writer);
	}

	public void Deserialize(IReader reader)
	{
		this.serializer.Deserialize(reader);
	}

	public bool Restore(StateMachine.Instance smi)
	{
		return this.serializer.Restore(smi);
	}

	public StateMachineInstanceType GetSMI<StateMachineInstanceType>() where StateMachineInstanceType : StateMachine.Instance
	{
		foreach (StateMachine.Instance instance in this.stateMachines)
		{
			if (typeof(StateMachineInstanceType).IsAssignableFrom(instance.GetType()))
			{
				return (StateMachineInstanceType)((object)instance);
			}
		}
		return (StateMachineInstanceType)((object)null);
	}

	private List<StateMachine.Instance> stateMachines = new List<StateMachine.Instance>();

	private List<LoggerFS> logs = new List<LoggerFS>();

	private LoggerFS log = new LoggerFS("StateMachineController");

	private StateMachineSerializer serializer = new StateMachineSerializer();
}
