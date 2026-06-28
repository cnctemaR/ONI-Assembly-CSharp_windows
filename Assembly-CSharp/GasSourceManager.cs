using System;
using System.Collections.Generic;
using UnityEngine;

public class GasSourceManager : KMonoBehaviour, IChunkManager
{
	protected override void OnPrefabInit()
	{
		GasSourceManager.Instance = this;
		EntityPrefabs.Instance.GasSource.gameObject.SetActive(false);
		EntityPrefabs.Instance.GasChunk.gameObject.SetActive(false);
		foreach (Element element in ElementLoader.elements)
		{
			if (element.IsGas)
			{
				GasSource component = GameUtil.KInstantiate(EntityPrefabs.Instance.GasSource, Grid.SceneLayer.Use, Folder.GasSourcePrefabs, null, 0).GetComponent<GasSource>();
				component.GetComponent<PrimaryElement>().SetElement(element.id);
				component.name = "GasSource " + element.id.ToString();
				this.sourcePrefabs[element.id] = component;
				SubstanceChunk component2 = GameUtil.KInstantiate(EntityPrefabs.Instance.GasChunk, Grid.SceneLayer.Use, Folder.GasChunkPrefabs, null, 0).GetComponent<SubstanceChunk>();
				component2.GetComponent<PrimaryElement>().SetElement(element.id);
				component2.GetComponent<KSelectable>().SetName("Bottled " + element.name);
				component2.name = element.id.ToString();
				Tag tag = TagManager.Create(element.id);
				KPrefabID component3 = component2.GetComponent<KPrefabID>();
				component3.PrefabTag = tag;
				component3.InitializeTags();
				this.chunkPrefabs[element.id] = component2;
				Assets.AddPrefab(component2.gameObject.GetComponent<KPrefabID>());
			}
		}
	}

	public GasSource CreateSource(Element element)
	{
		GasSource gasSource = this.sourcePrefabs[element.id];
		GasSource component = GameUtil.KInstantiate(gasSource.gameObject, Grid.SceneLayer.Use, Folder.GasSources, null, 0).GetComponent<GasSource>();
		component.gameObject.SetActive(true);
		return component;
	}

	public SubstanceChunk CreateChunk(Element element, float mass, float temperature, Vector3 position)
	{
		return this.CreateChunk(element.id, mass, temperature, position);
	}

	public SubstanceChunk CreateChunk(SimHashes element, float mass, float temperature, Vector3 position)
	{
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

	public static GasSourceManager Instance;

	private Dictionary<SimHashes, GasSource> sourcePrefabs = new Dictionary<SimHashes, GasSource>();

	private Dictionary<SimHashes, SubstanceChunk> chunkPrefabs = new Dictionary<SimHashes, SubstanceChunk>();
}
