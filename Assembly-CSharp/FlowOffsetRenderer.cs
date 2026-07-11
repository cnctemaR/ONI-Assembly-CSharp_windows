using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FlowOffsetRenderer")]
public class FlowOffsetRenderer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		this.FlowMaterial = new Material(Shader.Find("Klei/Flow"));
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (global::System.Action)Delegate.Combine(instance.OnResize, new global::System.Action(this.OnResize));
		this.OnResize();
		this.DoUpdate(0.1f);
	}

	private void OnResize()
	{
		for (int i = 0; i < this.OffsetTextures.Length; i++)
		{
			if (this.OffsetTextures[i] != null)
			{
				this.OffsetTextures[i].DestroyRenderTexture();
			}
			this.OffsetTextures[i] = new RenderTexture(Screen.width / 2, Screen.height / 2, 0, RenderTextureFormat.ARGBHalf);
			this.OffsetTextures[i].filterMode = FilterMode.Bilinear;
			this.OffsetTextures[i].name = "FlowOffsetTexture";
		}
	}

	private void LateUpdate()
	{
		if ((Time.deltaTime > 0f && Time.timeScale > 0f) || this.forceUpdate)
		{
			float num = Time.deltaTime / Time.timeScale;
			this.DoUpdate(num * Time.timeScale / 4f + num * 0.5f);
		}
	}

	private void DoUpdate(float dt)
	{
		this.CurrentTime += dt;
		float num = this.CurrentTime * this.PhaseMultiplier;
		num -= (float)((int)num);
		float num2 = num - (float)((int)num);
		float num3 = 1f;
		if (num2 <= this.GasPhase0)
		{
			num3 = 0f;
		}
		this.GasPhase0 = num2;
		float num4 = 1f;
		float num5 = num + 0.5f - (float)((int)(num + 0.5f));
		if (num5 <= this.GasPhase1)
		{
			num4 = 0f;
		}
		this.GasPhase1 = num5;
		Shader.SetGlobalVector(this.ParametersName, new Vector4(this.GasPhase0, 0f, 0f, 0f));
		Shader.SetGlobalVector("_NoiseParameters", new Vector4(this.NoiseInfluence, this.NoiseScale, 0f, 0f));
		RenderTexture renderTexture = this.OffsetTextures[this.OffsetIdx];
		this.OffsetIdx = (this.OffsetIdx + 1) % 2;
		RenderTexture renderTexture2 = this.OffsetTextures[this.OffsetIdx];
		Material flowMaterial = this.FlowMaterial;
		flowMaterial.SetTexture("_PreviousOffsetTex", renderTexture);
		flowMaterial.SetVector("_FlowParameters", new Vector4(Time.deltaTime * this.OffsetSpeed, num3, num4, 0f));
		flowMaterial.SetVector("_MinFlow", new Vector4(this.MinFlow0.x, this.MinFlow0.y, this.MinFlow1.x, this.MinFlow1.y));
		flowMaterial.SetVector("_VisibleArea", new Vector4(0f, 0f, (float)Grid.WidthInCells, (float)Grid.HeightInCells));
		flowMaterial.SetVector("_LiquidGasMask", new Vector4(this.LiquidGasMask.x, this.LiquidGasMask.y, 0f, 0f));
		Graphics.Blit(renderTexture, renderTexture2, flowMaterial);
		Shader.SetGlobalTexture(this.OffsetTextureName, renderTexture2);
	}

	private float GasPhase0;

	private float GasPhase1;

	public float PhaseMultiplier;

	public float NoiseInfluence;

	public float NoiseScale;

	public float OffsetSpeed;

	public string OffsetTextureName;

	public string ParametersName;

	public Vector2 MinFlow0;

	public Vector2 MinFlow1;

	public Vector2 LiquidGasMask;

	[SerializeField]
	private Material FlowMaterial;

	[SerializeField]
	private bool forceUpdate;

	private TextureLerper FlowLerper;

	public RenderTexture[] OffsetTextures = new RenderTexture[2];

	private int OffsetIdx;

	private float CurrentTime;
}
