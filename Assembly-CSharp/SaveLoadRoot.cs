using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using KSerialization;
using UnityEngine;

[SkipSaveFileSerialization]
public class SaveLoadRoot : KMonoBehaviour
{
	public static void DestroyStatics()
	{
		SaveLoadRoot.serializableComponentManagers = null;
	}

	protected override void OnPrefabInit()
	{
		if (SaveLoadRoot.serializableComponentManagers == null)
		{
			SaveLoadRoot.serializableComponentManagers = new Dictionary<string, ISerializableComponentManager>();
			FieldInfo[] fields = typeof(GameComps).GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				IComponentManager componentManager = (IComponentManager)fieldInfo.GetValue(null);
				if (typeof(ISerializableComponentManager).IsAssignableFrom(componentManager.GetType()))
				{
					Type type = componentManager.GetType();
					SaveLoadRoot.serializableComponentManagers[type.ToString()] = (ISerializableComponentManager)componentManager;
				}
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.registered)
		{
			SaveLoader.Instance.saveManager.Register(this);
		}
		this.hasOnSpawnRun = true;
	}

	public void SetRegistered(bool registered)
	{
		if (this.registered != registered)
		{
			this.registered = registered;
			if (this.hasOnSpawnRun)
			{
				if (registered)
				{
					SaveLoader.Instance.saveManager.Register(this);
				}
				else
				{
					SaveLoader.Instance.saveManager.Unregister(this);
				}
			}
		}
	}

	protected override void OnCleanUp()
	{
		if (SaveLoader.Instance != null && SaveLoader.Instance.saveManager != null)
		{
			SaveLoader.Instance.saveManager.Unregister(this);
		}
		if (GameComps.WhiteBoards.Has(base.gameObject))
		{
			GameComps.WhiteBoards.Remove(base.gameObject);
		}
	}

	public void Save(BinaryWriter writer)
	{
		Transform transform = base.transform;
		writer.Write(transform.GetPosition());
		writer.Write(transform.rotation);
		writer.Write(transform.localScale);
		byte b = 0;
		writer.Write(b);
		this.SaveWithoutTransform(writer);
	}

	public void SaveWithoutTransform(BinaryWriter writer)
	{
		KMonoBehaviour[] components = base.GetComponents<KMonoBehaviour>();
		if (components == null)
		{
			return;
		}
		int num = 0;
		foreach (KMonoBehaviour kmonoBehaviour in components)
		{
			if (kmonoBehaviour is ISaveLoadableDetails || kmonoBehaviour != null)
			{
				if (!kmonoBehaviour.GetType().IsDefined(typeof(SkipSaveFileSerialization), false))
				{
					num++;
				}
			}
		}
		foreach (KeyValuePair<string, ISerializableComponentManager> keyValuePair in SaveLoadRoot.serializableComponentManagers)
		{
			ISerializableComponentManager value = keyValuePair.Value;
			if (value.Has(base.gameObject))
			{
				num++;
			}
		}
		writer.Write(num);
		foreach (KMonoBehaviour kmonoBehaviour2 in components)
		{
			if (kmonoBehaviour2 is ISaveLoadableDetails || kmonoBehaviour2 != null)
			{
				if (!kmonoBehaviour2.GetType().IsDefined(typeof(SkipSaveFileSerialization), false))
				{
					writer.WriteKleiString(kmonoBehaviour2.GetType().ToString());
					long position = writer.BaseStream.Position;
					writer.Write(0);
					long position2 = writer.BaseStream.Position;
					if (kmonoBehaviour2 is ISaveLoadableDetails)
					{
						ISaveLoadableDetails saveLoadableDetails = (ISaveLoadableDetails)kmonoBehaviour2;
						Serializer.SerializeTypeless(kmonoBehaviour2, writer);
						saveLoadableDetails.Serialize(writer);
					}
					else if (kmonoBehaviour2 != null)
					{
						Serializer.SerializeTypeless(kmonoBehaviour2, writer);
					}
					long position3 = writer.BaseStream.Position;
					long num2 = position3 - position2;
					writer.BaseStream.Position = position;
					writer.Write((int)num2);
					writer.BaseStream.Position = position3;
				}
			}
		}
		foreach (KeyValuePair<string, ISerializableComponentManager> keyValuePair2 in SaveLoadRoot.serializableComponentManagers)
		{
			ISerializableComponentManager value2 = keyValuePair2.Value;
			if (value2.Has(base.gameObject))
			{
				string key = keyValuePair2.Key;
				writer.WriteKleiString(key);
				value2.Serialize(base.gameObject, writer);
			}
		}
	}

	public static SaveLoadRoot Load(Tag tag, IReader reader)
	{
		GameObject prefab = SaveLoader.Instance.saveManager.GetPrefab(tag);
		return SaveLoadRoot.Load(prefab, reader);
	}

	public static SaveLoadRoot Load(GameObject prefab, IReader reader)
	{
		Vector3 vector = reader.ReadVector3();
		Quaternion quaternion = reader.ReadQuaternion();
		Vector3 vector2 = reader.ReadVector3();
		reader.ReadByte();
		return SaveLoadRoot.Load(prefab, vector, quaternion, vector2, reader);
	}

	public static SaveLoadRoot Load(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, IReader reader)
	{
		SaveLoadRoot saveLoadRoot = null;
		if (prefab != null)
		{
			GameObject gameObject = Util.KInstantiate(prefab, position, rotation, null, null, true, 0);
			gameObject.transform.localScale = scale;
			gameObject.SetActive(true);
			saveLoadRoot = gameObject.GetComponent<SaveLoadRoot>();
			if (saveLoadRoot != null)
			{
				try
				{
					SaveLoadRoot.LoadInternal(gameObject, reader);
				}
				catch (ArgumentException ex)
				{
					Output.LogErrorWithObj(gameObject, new object[] { "Failed to load SaveLoadRoot ", ex.Message, "\n", ex.StackTrace });
				}
			}
			else
			{
				Output.LogWithObj(gameObject, new object[] { "missing SaveLoadRoot" });
			}
		}
		else
		{
			SaveLoadRoot.LoadInternal(null, reader);
		}
		return saveLoadRoot;
	}

	private static void LoadInternal(GameObject gameObject, IReader reader)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		KMonoBehaviour[] array = ((!(gameObject != null)) ? null : gameObject.GetComponents<KMonoBehaviour>());
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			string text = reader.ReadKleiString();
			int num2 = reader.ReadInt32();
			int position = reader.Position;
			ISerializableComponentManager serializableComponentManager;
			if (SaveLoadRoot.serializableComponentManagers.TryGetValue(text, out serializableComponentManager))
			{
				serializableComponentManager.Deserialize(gameObject, reader);
			}
			else
			{
				int num3 = 0;
				dictionary.TryGetValue(text, out num3);
				KMonoBehaviour kmonoBehaviour = null;
				int num4 = 0;
				if (array != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j].GetType().ToString() == text)
						{
							if (num4 == num3)
							{
								kmonoBehaviour = array[j];
								break;
							}
							num4++;
						}
					}
				}
				if (kmonoBehaviour == null)
				{
					reader.SkipBytes(num2);
				}
				else if (kmonoBehaviour == null && !(kmonoBehaviour is ISaveLoadableDetails))
				{
					Output.LogError(new object[] { "Component", text, "is not ISaveLoadable" });
					reader.SkipBytes(num2);
				}
				else
				{
					dictionary[text] = num4 + 1;
					if (kmonoBehaviour is ISaveLoadableDetails)
					{
						ISaveLoadableDetails saveLoadableDetails = (ISaveLoadableDetails)kmonoBehaviour;
						Deserializer.DeserializeTypeless(kmonoBehaviour, reader);
						saveLoadableDetails.Deserialize(reader);
					}
					else
					{
						Deserializer.DeserializeTypeless(kmonoBehaviour, reader);
					}
					if (reader.Position != position + num2)
					{
						Output.LogWarning(new object[]
						{
							"Expected to be at offset",
							position + num2,
							"but was only at offset",
							reader.Position,
							". Skipping to catch up."
						});
						reader.SkipBytes(position + num2 - reader.Position);
					}
				}
			}
		}
	}

	private bool hasOnSpawnRun;

	private bool registered = true;

	private static Dictionary<string, ISerializableComponentManager> serializableComponentManagers;
}
