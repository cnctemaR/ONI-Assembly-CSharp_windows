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
		List<Type> list = new List<Type>();
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			Type[] types = assembly.GetTypes();
			if (types != null)
			{
				list.AddRange(types);
			}
		}
		EntityTemplates.CreateTemplates();
		EntityTemplates.CreateBaseOreTemplates();
		LegacyModMain.LoadOre(list);
		LegacyModMain.LoadBuildings(list);
		LegacyModMain.ConfigElements();
		LegacyModMain.LoadEntities(list);
		LegacyModMain.LoadEquipment();
		EntityTemplates.DestroyBaseOreTemplates();
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
		global::Debug.Log(text, null);
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
		foreach (Type type2 in App.GetCurrentDomainTypes())
		{
			if (typeof(MonoBehaviour).IsAssignableFrom(type2) && !hashSet.Contains(type2))
			{
				hashSet2.Add(type2);
			}
		}
		List<Type> list = new List<Type>(hashSet2);
		list.Sort((Type x, Type y) => x.FullName.CompareTo(y.FullName));
		string text = "Unused types:";
		foreach (Type type3 in list)
		{
			text = text + "\n" + type3.FullName;
		}
		global::Debug.Log(text, null);
	}

	private static void DebugSelected()
	{
	}

	private static void DebugSelected(GameObject go)
	{
		Constructable component = go.GetComponent<Constructable>();
		int num = 0;
		num++;
		global::Debug.Log(component, null);
	}

	private static void LoadOre(List<Type> types)
	{
		GeneratedOre.LoadGeneratedOre(types);
	}

	private static void LoadBuildings(List<Type> types)
	{
		LocString.CreateLocStringKeys(typeof(BUILDINGS.PREFABS), "STRINGS.BUILDINGS.");
		LocString.CreateLocStringKeys(typeof(BUILDINGS.DAMAGESOURCES), "STRINGS.BUILDINGS.DAMAGESOURCES");
		LocString.CreateLocStringKeys(typeof(BUILDINGS.REPAIRABLE), "STRINGS.BUILDINGS.REPAIRABLE");
		LocString.CreateLocStringKeys(typeof(BUILDINGS.DISINFECTABLE), "STRINGS.BUILDINGS.DISINFECTABLE");
		GeneratedBuildings.LoadGeneratedBuildings(types);
	}

	private static void LoadEntities(List<Type> types)
	{
		EntityConfigManager.Instance.LoadGeneratedEntities(types);
		BuildingConfigManager.Instance.ConfigurePost();
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
				id = SimHashes.Katairite,
				overheatMod = 200f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.Cuprite,
				decor = 0.1f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.Copper,
				decor = 0.2f,
				overheatMod = 50f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.Gold,
				decor = 0.5f,
				overheatMod = 50f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.Granite,
				decor = 0.2f,
				overheatMod = 15f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.SandStone,
				decor = 0.1f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.ToxicSand,
				overheatMod = -10f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.Dirt,
				overheatMod = -10f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.IgneousRock,
				overheatMod = 15f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.Obsidian,
				overheatMod = 15f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.Ceramic,
				overheatMod = 200f,
				decor = 0.2f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.Iron,
				overheatMod = 50f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.Tungsten,
				overheatMod = 50f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.Steel,
				overheatMod = 200f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.GoldAmalgam,
				overheatMod = 50f,
				decor = 0.1f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.Diamond,
				overheatMod = 200f,
				decor = 1f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.Niobium,
				decor = 0.5f,
				overheatMod = 500f
			},
			new LegacyModMain.ElementInfo
			{
				id = SimHashes.TempConductorSolid,
				overheatMod = 900f
			}
		};
		foreach (LegacyModMain.ElementInfo elementInfo in array)
		{
			Element element = ElementLoader.FindElementByHash(elementInfo.id);
			if (elementInfo.decor != 0f)
			{
				AttributeModifier attributeModifier = new AttributeModifier("Decor", elementInfo.decor, element.name, true, false, true);
				element.attributeModifiers.Add(attributeModifier);
			}
			if (elementInfo.overheatMod != 0f)
			{
				AttributeModifier attributeModifier2 = new AttributeModifier(Db.Get().BuildingAttributes.OverheatTemperature.Id, elementInfo.overheatMod, element.name, false, false, true);
				element.attributeModifiers.Add(attributeModifier2);
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

		public float overheatMod;
	}
}
