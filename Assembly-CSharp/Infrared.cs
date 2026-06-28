using System;
using UnityEngine;

public class Infrared : MonoBehaviour
{
	private void Awake()
	{
		Infrared.temperatureParametersId = Shader.PropertyToID("_TemperatureParameters");
		Infrared.Instance = this;
		this.OnResize();
		this.UpdateState();
	}

	private void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		Graphics.Blit(source, this.minionTexture);
		Graphics.Blit(source, dest);
	}

	private void OnResize()
	{
		if (this.minionTexture != null)
		{
			this.minionTexture.DestroyRenderTexture();
		}
		if (this.cameraTexture != null)
		{
			this.cameraTexture.DestroyRenderTexture();
		}
		int num = 2;
		this.minionTexture = new RenderTexture(Screen.width / num, Screen.height / num, 0, RenderTextureFormat.ARGB32);
		this.cameraTexture = new RenderTexture(Screen.width / num, Screen.height / num, 0, RenderTextureFormat.ARGB32);
		base.GetComponent<Camera>().targetTexture = this.cameraTexture;
	}

	public void SetMode(Infrared.Mode mode)
	{
		Vector4 zero;
		if (mode != Infrared.Mode.Disabled)
		{
			zero = new Vector4(1f, 0f, 0f, 0f);
		}
		else
		{
			zero = Vector4.zero;
		}
		Shader.SetGlobalVector("_ColouredOverlayParameters", zero);
		this.mode = mode;
		this.UpdateState();
	}

	private void UpdateState()
	{
		base.enabled = this.mode != Infrared.Mode.Disabled;
		if (base.enabled)
		{
			this.Update();
		}
	}

	private void Update()
	{
		switch (this.mode)
		{
		case Infrared.Mode.Infrared:
			GameComps.InfraredVisualizers.UpdateTemperature();
			break;
		case Infrared.Mode.Disease:
			GameComps.InfraredVisualizers.ClearOverlayColour();
			GameComps.DiseaseContainers.UpdateOverlayColours();
			break;
		}
	}

	private RenderTexture minionTexture;

	private RenderTexture cameraTexture;

	private Infrared.Mode mode;

	public static int temperatureParametersId;

	public static Infrared Instance;

	public enum Mode
	{
		Disabled,
		Infrared,
		Disease
	}
}
