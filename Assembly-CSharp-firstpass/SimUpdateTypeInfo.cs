using System;
using System.Reflection;

public class SimUpdateTypeInfo
{
	public SimUpdateTypeInfo(Type type)
	{
		int num = 3;
		if (SimUpdateTypeInfo.MethodNames == null)
		{
			SimUpdateTypeInfo.MethodNames = new string[num];
			for (int i = 0; i < num; i++)
			{
				string[] methodNames = SimUpdateTypeInfo.MethodNames;
				int num2 = i;
				UpdateManager.ListType listType = (UpdateManager.ListType)i;
				methodNames[num2] = listType.ToString();
			}
		}
		this.UpdateArrays = new SimUpdateArray[num];
		this.MethodInfos = new MethodInfo[num];
		FieldInfo field = type.GetField("SimUpdateSortKey", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		if (field != null)
		{
			this.SortKey = (int)field.GetRawConstantValue();
		}
		this.IsValid = false;
		for (int j = 0; j < num; j++)
		{
			this.MethodInfos[j] = type.GetMethod(SimUpdateTypeInfo.MethodNames[j], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (this.MethodInfos[j] != null)
			{
				this.IsValid = true;
				this.UpdateArrays[j] = new SimUpdateArray(this.SortKey, type.ToString());
			}
		}
	}

	public void Add(KMonoBehaviour behaviour)
	{
		for (int i = 0; i < this.UpdateArrays.Length; i++)
		{
			if (this.UpdateArrays[i] != null)
			{
				this.UpdateArrays[i].Add(behaviour, (SimUpdateFn)Delegate.CreateDelegate(typeof(SimUpdateFn), behaviour, this.MethodInfos[i]));
			}
		}
	}

	public void Remove(KMonoBehaviour behaviour)
	{
		for (int i = 0; i < this.UpdateArrays.Length; i++)
		{
			if (this.UpdateArrays[i] != null)
			{
				this.UpdateArrays[i].Remove(behaviour);
			}
		}
	}

	private static string[] MethodNames;

	public bool IsValid = false;

	public int SortKey = 1000;

	public MethodInfo[] MethodInfos;

	public SimUpdateArray[] UpdateArrays;
}
