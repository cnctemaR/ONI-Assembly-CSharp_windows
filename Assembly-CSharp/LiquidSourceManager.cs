using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LiquidSourceManager")]
public class LiquidSourceManager : KMonoBehaviour, IChunkManager
{
	protected override void OnPrefabInit()
	{
		LiquidSourceManager.Instance = this;
	}

	public SubstanceChunk CreateChunk(SimHashes element_id, float mass, float temperature, byte diseaseIdx, int diseaseCount, Vector3 position)
	{
		return this.CreateChunk(ElementLoader.FindElementByHash(element_id), mass, temperature, diseaseIdx, diseaseCount, position);
	}

	public SubstanceChunk CreateChunk(Element element, float mass, float temperature, byte diseaseIdx, int diseaseCount, Vector3 position)
	{
		return GeneratedOre.CreateChunk(element, mass, temperature, diseaseIdx, diseaseCount, position);
	}

	public static LiquidSourceManager Instance;
}
