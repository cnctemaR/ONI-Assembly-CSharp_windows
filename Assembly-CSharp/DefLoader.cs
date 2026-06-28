using System;
using Klei;
using UnityEngine;

public class DefLoader : KMonoBehaviour
{
	public Def GetDefinition(Def[] defs, string prefab_id)
	{
		int num = defs.Length;
		for (int i = 0; i < num; i++)
		{
			if (defs[i].PrefabID == prefab_id)
			{
				return defs[i];
			}
		}
		return null;
	}

	public static KPrefabID AddID(GameObject go, string str)
	{
		KPrefabID kprefabID = go.GetComponent<KPrefabID>();
		if (kprefabID == null)
		{
			kprefabID = go.AddComponent<KPrefabID>();
		}
		kprefabID.PrefabTag = new Tag(str);
		kprefabID.SaveLoadTag = kprefabID.PrefabTag;
		return kprefabID;
	}

	public static string GetPathForGenerated<T>() where T : Def
	{
		return "Assets/Defs/" + typeof(T).ToString() + "s/Generated/";
	}

	public void ParseData<T>(Def[] defs)
	{
		this.ParseData<T>(defs, CSVReader.SplitCsvGrid(this.file.text, typeof(T).Name), this.file.name);
	}

	public void ParseData<T>(Def[] defs, string[,] grid, string filename)
	{
		int num = 0;
		int length = grid.GetLength(1);
		for (int i = 1; i < length; i++)
		{
			if (DefLoader.IsValidRow(grid, i))
			{
				num++;
			}
		}
		for (int j = 1; j < length; j++)
		{
			try
			{
				if (DefLoader.IsValidRow(grid, j))
				{
					string prefabID = DefLoader.GetPrefabID(grid, j);
					Def definition = this.GetDefinition(defs, prefabID);
					if (definition == null)
					{
						throw new InvalidOperationException(string.Format("ERROR Parsing [{0}]: Def [{1}] does not exist.", filename, prefabID));
					}
					CSVUtil.ParseData<T>(definition, grid, j);
				}
			}
			catch (Exception ex)
			{
			}
		}
	}

	public static string GetPrefabID(string[,] grid, int row)
	{
		return grid[0, row];
	}

	public static string GetName(string[,] grid, int row)
	{
		return grid[1, row];
	}

	public static bool IsValidRow(string[,] grid, int row)
	{
		return grid[0, row] != null && grid[0, row] != string.Empty;
	}

	public TextAsset file;
}
