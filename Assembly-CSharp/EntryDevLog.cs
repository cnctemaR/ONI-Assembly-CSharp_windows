using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

public class EntryDevLog
{
	[Conditional("UNITY_EDITOR")]
	public void AddModificationRecord(EntryDevLog.ModificationRecord.ActionType actionType, string target, object newValue)
	{
		string text = this.TrimAuthor();
		this.modificationRecords.Add(new EntryDevLog.ModificationRecord(actionType, target, newValue, text));
	}

	[Conditional("UNITY_EDITOR")]
	public void InsertModificationRecord(int index, EntryDevLog.ModificationRecord.ActionType actionType, string target, object newValue)
	{
		string text = this.TrimAuthor();
		this.modificationRecords.Insert(index, new EntryDevLog.ModificationRecord(actionType, target, newValue, text));
	}

	private string TrimAuthor()
	{
		string text = "";
		string[] array = new string[] { "Invoke", "CreateInstance", "AwakeInternal", "Internal", "<>", "YamlDotNet", "Deserialize" };
		string[] array2 = new string[]
		{
			".ctor", "Trigger", "AddContentContainerRange", "AddContentContainer", "InsertContentContainer", "KInstantiateUI", "Start", "InitializeComponentAwake", "TrimAuthor", "InsertModificationRecord",
			"AddModificationRecord", "SetValue", "Write"
		};
		StackTrace stackTrace = new StackTrace();
		int i = 0;
		int num = 0;
		int num2 = 3;
		while (i < num2)
		{
			num++;
			if (stackTrace.FrameCount <= num)
			{
				break;
			}
			MethodBase method = stackTrace.GetFrame(num).GetMethod();
			bool flag = false;
			for (int j = 0; j < array.Length; j++)
			{
				flag = flag || method.Name.Contains(array[j]);
			}
			for (int k = 0; k < array2.Length; k++)
			{
				flag = flag || method.Name.Contains(array2[k]);
			}
			if (!flag && !stackTrace.GetFrame(num).GetMethod().Name.StartsWith("set_") && !stackTrace.GetFrame(num).GetMethod().Name.StartsWith("Instantiate"))
			{
				if (i != 0)
				{
					text += " < ";
				}
				i++;
				text += stackTrace.GetFrame(num).GetMethod().Name;
			}
		}
		return text;
	}

	[SerializeField]
	public List<EntryDevLog.ModificationRecord> modificationRecords = new List<EntryDevLog.ModificationRecord>();

	public class ModificationRecord
	{
		public EntryDevLog.ModificationRecord.ActionType actionType { get; private set; }

		public string target { get; private set; }

		public object newValue { get; private set; }

		public string author { get; private set; }

		public ModificationRecord(EntryDevLog.ModificationRecord.ActionType actionType, string target, object newValue, string author)
		{
			this.target = target;
			this.newValue = newValue;
			this.author = author;
			this.actionType = actionType;
		}

		public enum ActionType
		{
			Created,
			ChangeSubEntry,
			ChangeContent,
			ValueChange,
			YAMLData
		}
	}
}
