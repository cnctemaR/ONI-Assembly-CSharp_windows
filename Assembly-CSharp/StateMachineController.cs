using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class StateMachineController : KMonoBehaviour, ISaveLoadableDetails, IStateMachineControllerHack
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
	}

	public void RemoveStateMachineInstance(StateMachine.Instance state_machine)
	{
		if (!state_machine.GetStateMachine().saveHistory && !state_machine.GetStateMachine().debugSettings.saveHistory)
		{
			this.stateMachines.Remove(state_machine);
		}
	}

	public bool HasStateMachineInstance(StateMachine.Instance state_machine)
	{
		return this.stateMachines.Contains(state_machine);
	}

	public void AddDef(StateMachine.Instance.BaseDef def)
	{
		this.defs.Add(def);
	}

	public List<LoggerFS> GetLogs()
	{
		return null;
	}

	public LoggerFS GetLog()
	{
		return null;
	}

	private void OnLog(LoggerFS.Entry entry)
	{
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(1969584890, new Action<object>(this.OnTargetDestroyed));
	}

	private void OnTargetDestroyed(object data)
	{
		while (this.stateMachines.Count > 0)
		{
			StateMachine.Instance instance = this.stateMachines[0];
			instance.StopSM("StateMachineController.OnCleanUp");
			this.stateMachines.Remove(instance);
		}
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

	public void CreateSMIS()
	{
		KPrefabID originalPrefab = base.GetComponent<KPrefabID>().GetOriginalPrefab();
		if (originalPrefab != null)
		{
			StateMachineController component = originalPrefab.GetComponent<StateMachineController>();
			if (component != null)
			{
				for (int i = 0; i < component.defs.Count; i++)
				{
					StateMachine.Instance.BaseDef baseDef = component.defs[i];
					this.defs.Add(baseDef);
					StateMachine.Instance instance = baseDef.CreateSMI(this);
					instance.StartSM();
				}
			}
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

	public DefType GetDef<DefType>() where DefType : StateMachine.Instance.BaseDef
	{
		for (int i = 0; i < this.defs.Count; i++)
		{
			DefType defType = this.defs[i] as DefType;
			if (defType != null)
			{
				return defType;
			}
		}
		return (DefType)((object)null);
	}

	public List<DefType> GetDefs<DefType>() where DefType : StateMachine.Instance.BaseDef
	{
		List<DefType> list = new List<DefType>();
		for (int i = 0; i < this.defs.Count; i++)
		{
			DefType defType = this.defs[i] as DefType;
			if (defType != null)
			{
				list.Add(defType);
			}
		}
		return list;
	}

	public StateMachineInstanceType GetSMI<StateMachineInstanceType>() where StateMachineInstanceType : class
	{
		for (int i = 0; i < this.stateMachines.Count; i++)
		{
			StateMachine.Instance instance = this.stateMachines[i];
			StateMachineInstanceType stateMachineInstanceType = instance as StateMachineInstanceType;
			if (stateMachineInstanceType != null)
			{
				return stateMachineInstanceType;
			}
		}
		return (StateMachineInstanceType)((object)null);
	}

	public List<StateMachineInstanceType> GetAllSMI<StateMachineInstanceType>() where StateMachineInstanceType : class
	{
		List<StateMachineInstanceType> list = new List<StateMachineInstanceType>();
		foreach (StateMachine.Instance instance in this.stateMachines)
		{
			StateMachineInstanceType stateMachineInstanceType = instance as StateMachineInstanceType;
			if (stateMachineInstanceType != null)
			{
				list.Add(stateMachineInstanceType);
			}
		}
		return list;
	}

	public List<IGameObjectEffectDescriptor> GetDescriptors()
	{
		List<IGameObjectEffectDescriptor> list = new List<IGameObjectEffectDescriptor>();
		for (int i = 0; i < this.defs.Count; i++)
		{
			if (this.defs[i] is IGameObjectEffectDescriptor)
			{
				list.Add(this.defs[i] as IGameObjectEffectDescriptor);
			}
		}
		return list;
	}

	private List<StateMachine.Instance.BaseDef> defs = new List<StateMachine.Instance.BaseDef>();

	private List<StateMachine.Instance> stateMachines = new List<StateMachine.Instance>();

	private StateMachineSerializer serializer = new StateMachineSerializer();
}
