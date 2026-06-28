using System;
using UnityEngine;

public interface IChunkManager
{
	SubstanceChunk CreateChunk(Element element, float mass, float temperature, Vector3 position);
}
