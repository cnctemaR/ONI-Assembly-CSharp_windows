using System;
using UnityEngine;

public abstract class VisualizerEffect : MonoBehaviour
{
	protected abstract void SetupMaterial();

	protected abstract void SetupOcclusionTex();

	protected abstract void OnPostRender();

	protected virtual void Start()
	{
		this.SetupMaterial();
		this.SetupOcclusionTex();
		this.myCamera = base.GetComponent<Camera>();
	}

	protected Material material;

	protected Camera myCamera;

	protected Texture2D OcclusionTex;
}
