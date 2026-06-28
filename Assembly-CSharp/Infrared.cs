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

	public bool IsOn()
	{
		return this.isOn;
	}

	public void Toggle(bool on)
	{
		Vector4 vector = new Vector4(0f, 0f, 0f, 0f);
		if (on)
		{
			vector.x = 1f;
		}
		this.isOn = on;
		Shader.SetGlobalVector("_InfraredParameters", vector);
		this.UpdateState();
	}

	private void UpdateState()
	{
		base.enabled = this.isOn;
		if (!this.isOn)
		{
			this.Update();
		}
	}

	private void Update()
	{
		if (this.IsOn())
		{
			GridArea visibleArea = GridVisibleArea.GetVisibleArea();
			this.cleared = false;
			foreach (InfraredVisualizer infraredVisualizer in Components.InfraredVisualizers)
			{
				Vector3 position = infraredVisualizer.transform.position;
				if (visibleArea.Min <= position && position <= visibleArea.Max)
				{
					infraredVisualizer.UpdateTemperature();
				}
			}
		}
		else if (!this.cleared)
		{
			this.cleared = true;
			foreach (InfraredVisualizer infraredVisualizer2 in Components.InfraredVisualizers)
			{
				infraredVisualizer2.Clear();
			}
		}
	}

	private RenderTexture minionTexture;

	private RenderTexture cameraTexture;

	private bool isOn;

	public static int temperatureParametersId;

	public static Infrared Instance;

	private bool cleared;
}
