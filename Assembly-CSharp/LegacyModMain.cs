using System;
using System.Collections.Generic;
using System.Reflection;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class LegacyModMain
{
	public static void Load()
	{
		LegacyModMain.LoadOre();
		LegacyModMain.LoadBuildings();
		LegacyModMain.ConfigElements();
		LegacyModMain.LoadEntities();
		LegacyModMain.LoadEquipment();
	}

	private static void Test()
	{
		Dictionary<Type, int> dictionary = new Dictionary<Type, int>();
		foreach (Component component in Resources.FindObjectsOfTypeAll(typeof(Component)))
		{
			Type type = component.GetType();
			int num = 0;
			dictionary.TryGetValue(type, out num);
			dictionary[type] = num + 1;
		}
		List<LegacyModMain.Entry> list = new List<LegacyModMain.Entry>();
		foreach (KeyValuePair<Type, int> keyValuePair in dictionary)
		{
			if (keyValuePair.Key.GetMethod("Update", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy) != null)
			{
				list.Add(new LegacyModMain.Entry
				{
					count = keyValuePair.Value,
					type = keyValuePair.Key
				});
			}
		}
		list.Sort((LegacyModMain.Entry x, LegacyModMain.Entry y) => y.count.CompareTo(x.count));
		string text = string.Empty;
		foreach (LegacyModMain.Entry entry in list)
		{
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				entry.type.Name,
				": ",
				entry.count,
				"\n"
			});
		}
		Debug.Log(text);
	}

	private static void ListUnusedTypes()
	{
		HashSet<Type> hashSet = new HashSet<Type>();
		foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
		{
			foreach (Component component in gameObject.GetComponents<Component>())
			{
				if (!(component == null))
				{
					for (Type type = component.GetType(); type != typeof(Component); type = type.BaseType)
					{
						hashSet.Add(type);
					}
				}
			}
		}
		HashSet<Type> hashSet2 = new HashSet<Type>();
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			foreach (Type type2 in assembly.GetTypes())
			{
				if (typeof(MonoBehaviour).IsAssignableFrom(type2) && !hashSet.Contains(type2))
				{
					hashSet2.Add(type2);
				}
			}
		}
		List<Type> list = new List<Type>(hashSet2);
		list.Sort((Type x, Type y) => x.FullName.CompareTo(y.FullName));
		string text = "Unused types:";
		foreach (Type type3 in list)
		{
			text = text + "\n" + type3.FullName;
		}
		Debug.Log(text);
	}

	private static void DebugSelected()
	{
	}

	private static void DebugSelected(GameObject go)
	{
		Constructable component = go.GetComponent<Constructable>();
		int num = 0;
		num++;
		Debug.Log(component);
	}

	private static void LoadOre()
	{
		GeneratedOre.LoadGeneratedOre();
	}

	private static void LoadBuildings()
	{
		LocString.CreateLocStringKeys(typeof(BUILDINGS.PREFABS), "STRINGS.BUILDINGS.");
		GeneratedBuildings.LoadGeneratedBuildings();
	}

	private static void LoadEntities()
	{
		GeneratedEntities.LoadGeneratedEntities();
	}

	private static void LoadEquipment()
	{
		LocString.CreateLocStringKeys(typeof(EQUIPMENT.PREFABS), "STRINGS.EQUIPMENT.");
		GeneratedEquipment.LoadGeneratedEquipment();
	}

	private static void ConfigElements()
	{
		LegacyModMain.ElementInfo[] array = new LegacyModMain.ElementInfo[]
		{
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.Cuprite,
				decor = 0.1f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.Copper,
				decor = 0.2f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.Gold,
				decor = 0.3f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.Granite,
				decor = 0.2f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.SandStone,
				decor = 0.1f
			}
		};
		foreach (LegacyModMain.ElementInfo elementInfo in array)
		{
			Element element = ElementLoader.FindElementByHash(elementInfo.id);
			if (elementInfo.decor != 0f)
			{
				AttributeModifier attributeModifier = new AttributeModifier("Decor", elementInfo.decor, element.name, true);
				element.attributeModifiers.Add(attributeModifier);
			}
		}
	}

	private struct Entry
	{
		public int count;

		public Type type;
	}

	private struct ElementInfo
	{
		public SimHashes id;

		public float decor;
	}
}
