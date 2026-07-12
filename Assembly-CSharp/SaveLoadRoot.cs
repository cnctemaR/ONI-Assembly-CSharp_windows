using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using KSerialization;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/SaveLoadRoot")]
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
			for (int i = 0; i < fields.Length; i++)
			{
				IComponentManager componentManager = (IComponentManager)fields[i].GetValue(null);
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

	public void DeclareOptionalComponent<T>() where T : KMonoBehaviour
	{
		this.m_optionalComponentTypeNames.Add(typeof(T).ToString());
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
					return;
				}
				SaveLoader.Instance.saveManager.Unregister(this);
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
			if ((kmonoBehaviour is ISaveLoadableDetails || kmonoBehaviour != null) && !kmonoBehaviour.GetType().IsDefined(typeof(SkipSaveFileSerialization), false))
			{
				num++;
			}
		}
		foreach (KeyValuePair<string, ISerializableComponentManager> keyValuePair in SaveLoadRoot.serializableComponentManagers)
		{
			if (keyValuePair.Value.Has(base.gameObject))
			{
				num++;
			}
		}
		writer.Write(num);
		foreach (KMonoBehaviour kmonoBehaviour2 in components)
		{
			if ((kmonoBehaviour2 is ISaveLoadableDetails || kmonoBehaviour2 != null) && !kmonoBehaviour2.GetType().IsDefined(typeof(SkipSaveFileSerialization), false))
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
		foreach (KeyValuePair<string, ISerializableComponentManager> keyValuePair2 in SaveLoadRoot.serializableComponentManagers)
		{
			ISerializableComponentManager value = keyValuePair2.Value;
			if (value.Has(base.gameObject))
			{
				string key = keyValuePair2.Key;
				writer.WriteKleiString(key);
				value.Serialize(base.gameObject, writer);
			}
		}
	}

	public static SaveLoadRoot Load(Tag tag, IReader reader)
	{
		return SaveLoadRoot.Load(SaveLoader.Instance.saveManager.GetPrefab(tag), reader);
	}

	public static SaveLoadRoot Load(GameObject prefab, IReader reader)
	{
		Vector3 vector = reader.ReadVector3();
		Quaternion quaternion = reader.ReadQuaternion();
		Vector3 vector2 = reader.ReadVector3();
		reader.ReadByte();
		if (SaveManager.DEBUG_OnlyLoadThisCellsObjects > -1)
		{
			Vector3 vector3 = Grid.CellToPos(SaveManager.DEBUG_OnlyLoadThisCellsObjects);
			if ((vector.x < vector3.x || vector.x >= vector3.x + 1f || vector.y < vector3.y || vector.y >= vector3.y + 1f) && prefab.name != "SaveGame")
			{
				prefab = null;
			}
			else
			{
				global::Debug.Log("Keeping " + prefab.name);
			}
		}
		return SaveLoadRoot.Load(prefab, vector, quaternion, vector2, reader);
	}

	public static SaveLoadRoot Load(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, IReader reader)
	{
		SaveLoadRoot saveLoadRoot = null;
		if (prefab != null)
		{
			GameObject gameObject = Util.KInstantiate(prefab, position, rotation, null, null, false, 0);
			gameObject.transform.localScale = scale;
			gameObject.SetActive(true);
			saveLoadRoot = gameObject.GetComponent<SaveLoadRoot>();
			if (saveLoadRoot != null)
			{
				try
				{
					SaveLoadRoot.LoadInternal(gameObject, reader);
					return saveLoadRoot;
				}
				catch (ArgumentException ex)
				{
					DebugUtil.LogErrorArgs(gameObject, new object[] { "Failed to load SaveLoadRoot ", ex.Message, "\n", ex.StackTrace });
					return saveLoadRoot;
				}
			}
			global::Debug.Log("missing SaveLoadRoot", gameObject);
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
		KMonoBehaviour[] array = ((gameObject != null) ? gameObject.GetComponents<KMonoBehaviour>() : null);
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
						Type type = array[j].GetType();
						string text2;
						if (!SaveLoadRoot.sTypeToString.TryGetValue(type, out text2))
						{
							text2 = type.ToString();
							SaveLoadRoot.sTypeToString[type] = text2;
						}
						if (text2 == text)
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
				if (kmonoBehaviour == null && gameObject != null)
				{
					SaveLoadRoot component = gameObject.GetComponent<SaveLoadRoot>();
					int num5;
					if (component != null && (num5 = component.m_optionalComponentTypeNames.IndexOf(text)) != -1)
					{
						DebugUtil.DevAssert(num3 == 0 && num4 == 0, string.Format("Implementation does not support multiple components with optional components, type {0}, {1}, {2}. Using only the first one and skipping the rest.", text, num3, num4), null);
						Type type2 = Type.GetType(component.m_optionalComponentTypeNames[num5]);
						if (gameObject.GetComponent(type2) == null)
						{
							kmonoBehaviour = (KMonoBehaviour)gameObject.AddComponent(type2);
						}
					}
				}
				if (kmonoBehaviour == null)
				{
					reader.SkipBytes(num2);
				}
				else if (kmonoBehaviour == null && !(kmonoBehaviour is ISaveLoadableDetails))
				{
					DebugUtil.LogErrorArgs(new object[] { "Component", text, "is not ISaveLoadable" });
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
						DebugUtil.LogWarningArgs(new object[]
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

	[SerializeField]
	private List<string> m_optionalComponentTypeNames = new List<string>();

	private static Dictionary<string, ISerializableComponentManager> serializableComponentManagers;

	private static Dictionary<Type, string> sTypeToString = new Dictionary<Type, string>();
}
