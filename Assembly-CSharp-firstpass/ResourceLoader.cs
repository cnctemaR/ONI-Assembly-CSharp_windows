using System;
using System.Collections.Generic;
using Klei;
using UnityEngine;

public class ResourceLoader<T> where T : Resource, new()
{
	public ResourceLoader()
	{
	}

	public ResourceLoader(TextAsset file)
	{
		this.Load(file);
	}

	public ResourceLoader(string text, string name)
	{
		this.Load(text, name);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return this.resources.GetEnumerator();
	}

	public void Load(string text, string name)
	{
		string[,] array = CSVReader.SplitCsvGrid(text, name);
		int length = array.GetLength(1);
		for (int i = 1; i < length; i++)
		{
			if (!array[0, i].IsNullOrWhiteSpace())
			{
				T t = new T();
				CSVUtil.ParseData<T>(t, array, i);
				if (!t.Disabled)
				{
					this.resources.Add(t);
				}
			}
		}
	}

	public virtual void Load(TextAsset file)
	{
		if (file == null)
		{
			global::Debug.LogWarning("Missing resource file of type: " + typeof(T).Name);
			return;
		}
		this.Load(file.text, file.name);
	}

	public List<T> resources = new List<T>();
}
