using System;
using UnityEngine;

public class GridCompositor : MonoBehaviour
{
	private void Awake()
	{
		GridCompositor.Instance = this;
		base.enabled = false;
	}

	private void Start()
	{
		this.material = new Material(Shader.Find("Klei/PostFX/GridCompositor"));
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, this.material);
	}

	public void ToggleMajor(bool on)
	{
		this.onMajor = on;
		this.Refresh();
	}

	public void ToggleMinor(bool on)
	{
		this.onMinor = on;
		this.Refresh();
	}

	private void Refresh()
	{
		base.enabled = this.onMinor || this.onMajor;
	}

	public Material material;

	public static GridCompositor Instance;

	private bool onMajor;

	private bool onMinor;
}
