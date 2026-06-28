using System;
using UnityEngine;

public class CameraRenderTexture : MonoBehaviour
{
	private void Awake()
	{
		this.material = new Material(Shader.Find("Klei/PostFX/CameraRenderTexture"));
	}

	private void Start()
	{
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (global::System.Action)Delegate.Combine(instance.OnResize, new global::System.Action(this.OnResize));
		this.OnResize();
	}

	private void OnResize()
	{
		if (this.resultTexture != null)
		{
			this.resultTexture.DestroyRenderTexture();
		}
		this.resultTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
		this.resultTexture.name = base.name;
		if (this.TextureName != string.Empty)
		{
			Shader.SetGlobalTexture(this.TextureName, this.resultTexture);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		Graphics.Blit(source, this.resultTexture, this.material);
	}

	public string TextureName;

	public RenderTexture resultTexture;

	private Material material;
}
