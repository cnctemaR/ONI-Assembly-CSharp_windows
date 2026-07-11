using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Plugins/RenderTextureDestroyer")]
public class RenderTextureDestroyer : KMonoBehaviour
{
	public static void DestroyInstance()
	{
		RenderTextureDestroyer.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		RenderTextureDestroyer.Instance = this;
	}

	public void Add(RenderTexture render_texture)
	{
		this.queued.Add(render_texture);
	}

	private void LateUpdate()
	{
		foreach (RenderTexture renderTexture in this.finished)
		{
			global::UnityEngine.Object.Destroy(renderTexture);
		}
		this.finished.Clear();
		this.finished.AddRange(this.queued);
		this.queued.Clear();
	}

	public static RenderTextureDestroyer Instance;

	public List<RenderTexture> queued = new List<RenderTexture>();

	public List<RenderTexture> finished = new List<RenderTexture>();
}
