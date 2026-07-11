using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Klei;
using Newtonsoft.Json;
using UnityEngine;

public class TuningSystem
{
	static TuningSystem()
	{
		TuningSystem.InitializeTuning();
		TuningSystem.ListenForFileChanges();
		TuningSystem.Load();
	}

	public static void Init()
	{
	}

	private static void ListenForFileChanges()
	{
		string directoryName = Path.GetDirectoryName(TuningSystem._TuningPath);
		try
		{
			FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
			fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
			fileSystemWatcher.Changed += TuningSystem.OnFileChanged;
			fileSystemWatcher.Path = directoryName;
			fileSystemWatcher.Filter = Path.GetFileName(TuningSystem._TuningPath);
			fileSystemWatcher.EnableRaisingEvents = true;
		}
		catch (Exception ex)
		{
			global::Debug.LogWarning("Error when attempting to monitor path: " + directoryName + "\n" + ex.ToString(), null);
		}
	}

	private static void OnFileChanged(object source, FileSystemEventArgs e)
	{
		TuningSystem.Load();
	}

	private static void InitializeTuning()
	{
		TuningSystem._TuningValues = new Dictionary<Type, object>();
		foreach (Type type in App.GetCurrentDomainTypes())
		{
			Type baseType = type.BaseType;
			if (baseType != null && baseType.IsGenericType)
			{
				if (baseType.GetGenericTypeDefinition() == typeof(TuningData<>))
				{
					Type type2 = baseType.GetGenericArguments()[0];
					object obj = Activator.CreateInstance(type2);
					TuningSystem._TuningValues[obj.GetType()] = obj;
					FieldInfo field = baseType.GetField("_TuningData", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
					field.SetValue(null, obj);
				}
			}
		}
	}

	private static void Save()
	{
		if (TuningSystem._TuningValues == null)
		{
			return;
		}
		JsonSerializer jsonSerializer = JsonSerializer.Create(TuningSystem._SerializationSettings);
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<Type, object> keyValuePair in TuningSystem._TuningValues)
		{
			dictionary[keyValuePair.Value.GetType().FullName] = keyValuePair.Value;
		}
		using (StreamWriter streamWriter = File.CreateText(TuningSystem._TuningPath))
		{
			jsonSerializer.Serialize(streamWriter, dictionary);
		}
	}

	private static void Load()
	{
		string[] array = new string[]
		{
			TuningSystem._TuningPath,
			Path.Combine(Application.dataPath, "Tuning.json")
		};
		foreach (string text in array)
		{
			bool flag = ((LayeredFileSystem.instance != null) ? LayeredFileSystem.instance.Exists(text) : File.Exists(text));
			if (flag)
			{
				string text2 = ((LayeredFileSystem.instance != null) ? LayeredFileSystem.instance.ReadText(text) : File.ReadAllText(text));
				Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(text2);
				foreach (KeyValuePair<string, object> keyValuePair in dictionary)
				{
					Type type = null;
					foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
					{
						type = assembly.GetType(keyValuePair.Key);
						if (type != null)
						{
							break;
						}
					}
					if (type != null)
					{
						if (TuningSystem._TuningValues.ContainsKey(type))
						{
							JsonConvert.PopulateObject(keyValuePair.Value.ToString(), TuningSystem._TuningValues[type]);
						}
						else
						{
							object obj = JsonConvert.DeserializeObject(keyValuePair.Value.ToString(), type);
							TuningSystem._TuningValues[type] = obj;
						}
					}
				}
			}
		}
	}

	private static bool IsLoaded()
	{
		return TuningSystem._TuningValues != null;
	}

	public static Dictionary<Type, object> GetAllTuningValues()
	{
		return TuningSystem._TuningValues;
	}

	private static JsonSerializerSettings _SerializationSettings = new JsonSerializerSettings
	{
		Formatting = Formatting.Indented
	};

	private static Dictionary<Type, object> _TuningValues;

	private static string _TuningPath = Path.Combine(Application.streamingAssetsPath, "Tuning.json");
}
