using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;
using UnityEngine;

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

	public void AddDef(StateMachine.BaseDef def)
	{
		this.defs.Add(def);
	}

	public List<LoggerFSSSS> GetLogs()
	{
		return null;
	}

	public LoggerFSSSS GetLog()
	{
		return null;
	}

	private void OnLog(LoggerFSSSS.Entry entry)
	{
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(1969584890, new Action<object>(this.OnTargetDestroyed));
		base.Subscribe(1502190696, new Action<object>(this.OnTargetDestroyed));
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

	protected override void OnLoadLevel()
	{
		while (this.stateMachines.Count > 0)
		{
			StateMachine.Instance instance = this.stateMachines[0];
			instance.FreeResources();
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
		GameObject prefab = Assets.GetPrefab(base.GetComponent<KPrefabID>().PrefabTag);
		if (prefab != null)
		{
			StateMachineController component = prefab.GetComponent<StateMachineController>();
			if (component != null)
			{
				for (int i = 0; i < component.defs.Count; i++)
				{
					StateMachine.BaseDef baseDef = component.defs[i];
					baseDef.CreateSMI(this);
					this.defs.Add(baseDef);
				}
			}
		}
	}

	public void StartSMIS()
	{
		foreach (StateMachine.BaseDef baseDef in this.defs)
		{
			StateMachine.Instance smi = this.GetSMI(StateMachineManager.Instance.CreateStateMachine(baseDef.GetStateMachineType()).GetStateMachineInstanceType());
			if (smi != null && !smi.IsRunning())
			{
				smi.StartSM();
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

	public DefType GetDef<DefType>() where DefType : StateMachine.BaseDef
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

	public List<DefType> GetDefs<DefType>() where DefType : StateMachine.BaseDef
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

	public StateMachine.Instance GetSMI(Type type)
	{
		for (int i = 0; i < this.stateMachines.Count; i++)
		{
			StateMachine.Instance instance = this.stateMachines[i];
			if (type.IsAssignableFrom(instance.GetType()))
			{
				return instance;
			}
		}
		return null;
	}

	public StateMachineInstanceType GetSMI<StateMachineInstanceType>() where StateMachineInstanceType : class
	{
		return this.GetSMI(typeof(StateMachineInstanceType)) as StateMachineInstanceType;
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

	private List<StateMachine.BaseDef> defs = new List<StateMachine.BaseDef>();

	private List<StateMachine.Instance> stateMachines = new List<StateMachine.Instance>();

	private StateMachineSerializer serializer = new StateMachineSerializer();
}
