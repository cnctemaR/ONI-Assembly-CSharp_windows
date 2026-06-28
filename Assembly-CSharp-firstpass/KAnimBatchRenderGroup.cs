using System;
using UnityEngine;

public class KAnimBatchRenderGroup
{
	public KAnimBatchRenderGroup(BatchGroupKey groupkey)
	{
		this.key = groupkey;
		this.ResetMaterial(this.materialType);
	}

	public BatchGroupKey key { get; private set; }

	public KAnimBatchRenderGroup.MaterialType materialType { get; private set; }

	public Material material { get; private set; }

	public int batchCount { get; private set; }

	private void ResetMaterial(KAnimBatchRenderGroup.MaterialType matType)
	{
		switch (this.materialType)
		{
		case KAnimBatchRenderGroup.MaterialType.Simple:
		case KAnimBatchRenderGroup.MaterialType.Overlay:
			this.material = new Material(Shader.Find("Klei/AnimationSimple"));
			return;
		case KAnimBatchRenderGroup.MaterialType.UI:
			this.material = new Material(Shader.Find("Klei/BatchedAnimationUI"));
			return;
		}
		this.material = new Material(Shader.Find("Klei/BatchedAnimation"));
	}

	public enum MaterialType
	{
		Default,
		Simple,
		UI,
		Overlay
	}
}
