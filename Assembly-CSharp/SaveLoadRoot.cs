using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;
using UnityEngine;

public class SaveLoadRoot : KMonoBehaviour
{
	protected override void OnCleanUp()
	{
		if (SaveLoader.Instance != null && SaveLoader.Instance.saveManager != null)
		{
			SaveLoader.Instance.saveManager.Unregister(this);
		}
	}

	public void Save(BinaryWriter writer)
	{
		Transform transform = this.transform;
		writer.Write(transform.position);
		writer.Write(transform.rotation);
		writer.Write(transform.localScale);
		writer.Write((byte)this.folder);
		KMonoBehaviour[] components = base.GetComponents<KMonoBehaviour>();
		if (components == null)
		{
			return;
		}
		int num = 0;
		foreach (KMonoBehaviour kmonoBehaviour in components)
		{
			if (kmonoBehaviour is ISaveLoadableDetailJson || kmonoBehaviour is ISaveLoadableJson)
			{
				if (!kmonoBehaviour.GetType().IsDefined(typeof(SkipSerialization), false))
				{
					num++;
				}
			}
		}
		writer.Write(num);
		foreach (KMonoBehaviour kmonoBehaviour2 in components)
		{
			if (kmonoBehaviour2 is ISaveLoadableDetailJson || kmonoBehaviour2 is ISaveLoadableJson)
			{
				if (!kmonoBehaviour2.GetType().IsDefined(typeof(SkipSerialization), false))
				{
					writer.WriteKleiString(kmonoBehaviour2.GetType().ToString());
					long position = writer.BaseStream.Position;
					writer.Write(0);
					long position2 = writer.BaseStream.Position;
					if (kmonoBehaviour2 is ISaveLoadableDetailJson)
					{
						ISaveLoadableDetailJson saveLoadableDetailJson = (ISaveLoadableDetailJson)kmonoBehaviour2;
						Serializer.SerializeTypeless(kmonoBehaviour2, writer);
						saveLoadableDetailJson.Serialize(writer);
					}
					else if (kmonoBehaviour2 is ISaveLoadableJson)
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
	}

	public static SaveLoadRoot Load(Tag tag, IReader reader)
	{
		GameObject prefab = SaveLoader.Instance.saveManager.GetPrefab(tag);
		if (prefab == null)
		{
			Output.Log(new object[] { "Couldn't find prefab for tag [" + tag.Name + "]" });
			return null;
		}
		return SaveLoadRoot.Load(prefab, reader);
	}

	public static SaveLoadRoot Load(GameObject prefab, IReader reader)
	{
		Vector3 vector = reader.ReadVector3();
		Quaternion quaternion = reader.ReadQuaternion();
		Vector3 vector2 = reader.ReadVector3();
		Folder folder = (Folder)reader.ReadByte();
		GameObject gameObject = SceneOrganizer.Instance.GetFolder(folder);
		GameObject gameObject2 = Util.KInstantiate(prefab, vector, quaternion, gameObject, null, true, 0);
		gameObject2.transform.localScale = vector2;
		gameObject2.SetActive(true);
		SaveLoadRoot component = gameObject2.GetComponent<SaveLoadRoot>();
		if (component != null)
		{
			if (gameObject2.GetComponent<SavedObject>() == null)
			{
				gameObject2.AddComponent<SavedObject>();
			}
			component.folder = folder;
			try
			{
				component.LoadInternal(reader);
			}
			catch (ArgumentException ex)
			{
				Output.LogErrorWithObj(gameObject2, new object[] { "Failed to load SaveLoadRoot ", ex.Message, "\n", ex.StackTrace });
			}
		}
		else
		{
			Output.LogWithObj(gameObject2, new object[] { "missing SaveLoadRoot" });
		}
		return component;
	}

	protected void LoadInternal(IReader reader)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		KMonoBehaviour[] components = base.GetComponents<KMonoBehaviour>();
		if (components == null)
		{
			return;
		}
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			string text = reader.ReadKleiString();
			int num2 = reader.ReadInt32();
			int position = reader.Position;
			int num3 = 0;
			dictionary.TryGetValue(text, out num3);
			KMonoBehaviour kmonoBehaviour = null;
			int num4 = 0;
			for (int j = 0; j < components.Length; j++)
			{
				if (components[j].GetType().ToString() == text)
				{
					if (num4 == num3)
					{
						kmonoBehaviour = components[j];
						break;
					}
					num4++;
				}
			}
			if (kmonoBehaviour == null)
			{
				Output.LogWarningWithObj(base.gameObject, new object[] { string.Concat(new string[]
				{
					"GameObject ",
					base.gameObject.name,
					" is missing component \"",
					text,
					"\""
				}) });
				reader.SkipBytes(num2);
			}
			else if (!(kmonoBehaviour is ISaveLoadableJson) && !(kmonoBehaviour is ISaveLoadableDetailJson))
			{
				Output.LogError(new object[] { "Component", text, "is not ISaveLoadable" });
				reader.SkipBytes(num2);
			}
			else
			{
				dictionary[text] = num4 + 1;
				if (kmonoBehaviour is ISaveLoadableDetailJson)
				{
					ISaveLoadableDetailJson saveLoadableDetailJson = (ISaveLoadableDetailJson)kmonoBehaviour;
					Deserializer.DeserializeTypeless(kmonoBehaviour, reader);
					saveLoadableDetailJson.Deserialize(reader);
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

	public Folder folder = Folder.Misc;

	public class ComponentSaveData
	{
		public string tag;

		public object json;
	}
}
