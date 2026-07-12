using System;
using UnityEngine;

public class MultipleRenderTargetProxy : MonoBehaviour
{
	private void Start()
	{
		if (ScreenResize.Instance != null)
		{
			ScreenResize instance = ScreenResize.Instance;
			instance.OnResize = (global::System.Action)Delegate.Combine(instance.OnResize, new global::System.Action(this.OnResize));
		}
		this.CreateRenderTarget();
		ShaderReloader.Register(new global::System.Action(this.OnShadersReloaded));
	}

	public void ToggleColouredOverlayView(bool enabled)
	{
		this.colouredOverlayBufferEnabled = enabled;
		this.CreateRenderTarget();
	}

	private void CreateRenderTarget()
	{
		RenderBuffer[] array = new RenderBuffer[this.colouredOverlayBufferEnabled ? 3 : 2];
		this.Textures[0] = this.RecreateRT(this.Textures[0], 24, RenderTextureFormat.ARGB32);
		this.Textures[0].filterMode = FilterMode.Point;
		this.Textures[0].name = "MRT0";
		this.Textures[1] = this.RecreateRT(this.Textures[1], 0, RenderTextureFormat.R8);
		this.Textures[1].filterMode = FilterMode.Point;
		this.Textures[1].name = "MRT1";
		array[0] = this.Textures[0].colorBuffer;
		array[1] = this.Textures[1].colorBuffer;
		if (this.colouredOverlayBufferEnabled)
		{
			this.Textures[2] = this.RecreateRT(this.Textures[2], 0, RenderTextureFormat.ARGB32);
			this.Textures[2].filterMode = FilterMode.Bilinear;
			this.Textures[2].name = "MRT2";
			array[2] = this.Textures[2].colorBuffer;
		}
		base.GetComponent<Camera>().SetTargetBuffers(array, this.Textures[0].depthBuffer);
		this.OnShadersReloaded();
	}

	private RenderTexture RecreateRT(RenderTexture rt, int depth, RenderTextureFormat format)
	{
		RenderTexture renderTexture = rt;
		if (rt == null || rt.width != Screen.width || rt.height != Screen.height || rt.format != format)
		{
			if (rt != null)
			{
				rt.DestroyRenderTexture();
			}
			renderTexture = new RenderTexture(Screen.width, Screen.height, depth, format);
		}
		return renderTexture;
	}

	private void OnResize()
	{
		this.CreateRenderTarget();
	}

	private void Update()
	{
		if (!this.Textures[0].IsCreated())
		{
			this.CreateRenderTarget();
		}
	}

	private void OnShadersReloaded()
	{
		Shader.SetGlobalTexture("_MRT0", this.Textures[0]);
		Shader.SetGlobalTexture("_MRT1", this.Textures[1]);
		if (this.colouredOverlayBufferEnabled)
		{
			Shader.SetGlobalTexture("_MRT2", this.Textures[2]);
		}
	}

	public RenderTexture[] Textures = new RenderTexture[3];

	private bool colouredOverlayBufferEnabled;
}
