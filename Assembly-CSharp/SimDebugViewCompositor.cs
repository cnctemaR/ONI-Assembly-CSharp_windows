using System;
using UnityEngine;

public class SimDebugViewCompositor : MonoBehaviour
{
	private void Awake()
	{
		SimDebugViewCompositor.Instance = this;
	}

	private void OnDestroy()
	{
		SimDebugViewCompositor.Instance = null;
	}

	private void Start()
	{
		this.material = new Material(Shader.Find("Klei/PostFX/SimDebugViewCompositor"));
		this.Toggle(false);
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, this.material);
		if (OverlayScreen.Instance != null)
		{
			OverlayScreen.Instance.RunPostProcessEffects(src, dest);
		}
	}

	public void Toggle(bool is_on)
	{
		base.enabled = is_on;
	}

	public Material material;

	public static SimDebugViewCompositor Instance;
}
