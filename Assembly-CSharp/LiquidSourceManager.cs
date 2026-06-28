using System;
using System.Collections.Generic;
using UnityEngine;

public class LiquidSourceManager : KMonoBehaviour, IChunkManager
{
	protected override void OnPrefabInit()
	{
		LiquidSourceManager.Instance = this;
		EntityPrefabs.Instance.LiquidSource.gameObject.SetActive(false);
		EntityPrefabs.Instance.LiquidChunk.gameObject.SetActive(false);
		foreach (Element element in ElementLoader.elements)
		{
			if (element.IsLiquid)
			{
				LiquidSource component = GameUtil.KInstantiate(EntityPrefabs.Instance.LiquidSource, Grid.SceneLayer.Use, Folder.LiquidSourcePrefabs, null, 0).GetComponent<LiquidSource>();
				component.GetComponent<PrimaryElement>().SetElement(element.id);
				component.name = "LiquidSource " + element.id.ToString();
				this.sourcePrefabs[element.id] = component;
				SubstanceChunk component2 = GameUtil.KInstantiate(EntityPrefabs.Instance.LiquidChunk, Grid.SceneLayer.Use, Folder.LiquidChunkPrefabs, null, 0).GetComponent<SubstanceChunk>();
				PrimaryElement component3 = component2.GetComponent<PrimaryElement>();
				component2.GetComponent<KSelectable>().SetName("Bottled " + element.name);
				component2.name = element.id.ToString();
				component3.SetElement(element.id);
				component3.InternalTemperature = element.defaultValues.temperature;
				Tag tag = TagManager.Create(element.id);
				KPrefabID component4 = component2.GetComponent<KPrefabID>();
				component4.PrefabTag = tag;
				component4.InitializeTags();
				this.chunkPrefabs[element.id] = component2;
				Assets.AddPrefab(component2.gameObject.GetComponent<KPrefabID>());
			}
		}
	}

	public LiquidSource CreateSource(Element element)
	{
		DebugUtil.Assert(element.IsLiquid, "Assert!");
		LiquidSource liquidSource = this.sourcePrefabs[element.id];
		LiquidSource component = GameUtil.KInstantiate(liquidSource.gameObject, Grid.SceneLayer.Use, Folder.LiquidSources, null, 0).GetComponent<LiquidSource>();
		component.GetComponent<PrimaryElement>().Temperature = element.defaultValues.temperature;
		component.GetComponent<KSelectable>().SetName(element.name);
		component.gameObject.SetActive(true);
		return component;
	}

	public SubstanceChunk CreateChunk(Element element, float mass, float temperature, Vector3 position)
	{
		return this.CreateChunk(element.id, mass, temperature, position);
	}

	public SubstanceChunk CreateChunk(SimHashes element, float mass, float temperature, Vector3 position)
	{
		if (temperature <= 0f)
		{
			Output.LogWarning(new object[] { "LiquidSourceManager.CreateChunk tried to create a chunk with a temperature <= 0" });
		}
		SubstanceChunk substanceChunk = this.chunkPrefabs[element];
		SubstanceChunk component = GameUtil.KInstantiate(substanceChunk.gameObject, Grid.SceneLayer.Use, Folder.LiquidSources, null, 0).GetComponent<SubstanceChunk>();
		component.transform.SetPosition(position);
		component.gameObject.SetActive(true);
		PrimaryElement component2 = component.GetComponent<PrimaryElement>();
		component2.Mass = mass;
		component2.Temperature = temperature;
		component2.SetElement(element);
		KPrefabID component3 = component.GetComponent<KPrefabID>();
		component3.InitializeTags();
		return component;
	}

	public static LiquidSourceManager Instance;

	private Dictionary<SimHashes, LiquidSource> sourcePrefabs = new Dictionary<SimHashes, LiquidSource>();

	private Dictionary<SimHashes, SubstanceChunk> chunkPrefabs = new Dictionary<SimHashes, SubstanceChunk>();
}
