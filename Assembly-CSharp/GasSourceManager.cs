using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/GasSourceManager")]
public class GasSourceManager : KMonoBehaviour, IChunkManager
{
	protected override void OnPrefabInit()
	{
		GasSourceManager.Instance = this;
	}

	public SubstanceChunk CreateChunk(SimHashes element_id, float mass, float temperature, byte diseaseIdx, int diseaseCount, Vector3 position)
	{
		return this.CreateChunk(ElementLoader.FindElementByHash(element_id), mass, temperature, diseaseIdx, diseaseCount, position);
	}

	public SubstanceChunk CreateChunk(Element element, float mass, float temperature, byte diseaseIdx, int diseaseCount, Vector3 position)
	{
		return GeneratedOre.CreateChunk(element, mass, temperature, diseaseIdx, diseaseCount, position);
	}

	public static GasSourceManager Instance;
}
