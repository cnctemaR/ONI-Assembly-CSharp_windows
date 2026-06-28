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

	public IEnumerator<T> GetEnumerator()
	{
		return this.resources.GetEnumerator();
	}

	public virtual void Load(TextAsset file)
	{
		if (file == null)
		{
			Debug.LogWarning("Missing resource file of type: " + typeof(T).Name);
			return;
		}
		string[,] array = CSVReader.SplitCsvGrid(file.text, file.name);
		int length = array.GetLength(1);
		for (int i = 1; i < length; i++)
		{
			if (array[0, i] != null && !(array[0, i] == string.Empty))
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

	public List<T> resources = new List<T>();
}
