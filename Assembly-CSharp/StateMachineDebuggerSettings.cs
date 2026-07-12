using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineDebuggerSettings : ScriptableObject
{
	public IEnumerator<StateMachineDebuggerSettings.Entry> GetEnumerator()
	{
		return this.entries.GetEnumerator();
	}

	public static StateMachineDebuggerSettings Get()
	{
		if (StateMachineDebuggerSettings._Instance == null)
		{
			StateMachineDebuggerSettings._Instance = Resources.Load<StateMachineDebuggerSettings>("StateMachineDebuggerSettings");
			StateMachineDebuggerSettings._Instance.Initialize();
		}
		return StateMachineDebuggerSettings._Instance;
	}

	private void Initialize()
	{
		foreach (Type type in App.GetCurrentDomainTypes())
		{
			if (typeof(StateMachine).IsAssignableFrom(type))
			{
				this.CreateEntry(type);
			}
		}
		this.entries.RemoveAll((StateMachineDebuggerSettings.Entry x) => x.type == null);
	}

	public StateMachineDebuggerSettings.Entry CreateEntry(Type type)
	{
		foreach (StateMachineDebuggerSettings.Entry entry in this.entries)
		{
			if (type.FullName == entry.typeName)
			{
				entry.type = type;
				return entry;
			}
		}
		StateMachineDebuggerSettings.Entry entry2 = new StateMachineDebuggerSettings.Entry(type);
		this.entries.Add(entry2);
		return entry2;
	}

	public void Clear()
	{
		this.entries.Clear();
		this.Initialize();
	}

	public List<StateMachineDebuggerSettings.Entry> entries = new List<StateMachineDebuggerSettings.Entry>();

	private static StateMachineDebuggerSettings _Instance;

	[Serializable]
	public class Entry
	{
		public Entry(Type type)
		{
			this.typeName = type.FullName;
			this.type = type;
		}

		public Type type;

		public string typeName;

		public bool breakOnGoTo;

		public bool enableConsoleLogging;

		public bool saveHistory;
	}
}
