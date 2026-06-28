using System;
using System.Collections.Generic;
using UnityEngine;

public class PropertyTextures : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		PropertyTextures.instance = this;
		base.OnPrefabInit();
		ShaderReloader.Register(new global::System.Action(this.OnShadersReloaded));
	}

	public void SetFilterMode(PropertyTextures.Property property, FilterMode mode)
	{
		this.textureProperties[(int)property].filterMode = mode;
	}

	public Texture GetTexture(PropertyTextures.Property property)
	{
		return this.textureBuffers[(int)property].texture;
	}

	private string GetShaderPropertyName(PropertyTextures.Property property)
	{
		return "_" + property.ToString() + "Tex";
	}

	protected override void OnSpawn()
	{
		this.WorldSizeID = Shader.PropertyToID("_WorldSizeInfo");
		this.FogOfWarScaleID = Shader.PropertyToID("_FogOfWarScale");
		this.PropTexWsToCsID = Shader.PropertyToID("_PropTexWsToCs");
		this.PropTexCsToWsID = Shader.PropertyToID("_PropTexCsToWs");
	}

	public void OnReset(object data = null)
	{
		this.lerpers = new TextureLerper[11];
		this.texturePagePool = new TexturePagePool();
		this.textureBuffers = new TextureBuffer[11];
		this.externallyUpdatedTextures = new Texture2D[11];
		for (int i = 0; i < 11; i++)
		{
			PropertyTextures.TextureProperties textureProperties = new PropertyTextures.TextureProperties
			{
				textureFormat = TextureFormat.Alpha8,
				filterMode = FilterMode.Bilinear,
				blend = false,
				blendSpeed = 1f
			};
			for (int j = 0; j < this.textureProperties.Length; j++)
			{
				if (i == (int)this.textureProperties[j].simProperty)
				{
					textureProperties = this.textureProperties[j];
				}
			}
			if (this.externallyUpdatedTextures[i] != null)
			{
				global::UnityEngine.Object.Destroy(this.externallyUpdatedTextures[i]);
				this.externallyUpdatedTextures[i] = null;
			}
			Texture texture;
			if (textureProperties.updatedExternally)
			{
				this.externallyUpdatedTextures[i] = new Texture2D(Grid.WidthInCells, Grid.HeightInCells, textureProperties.textureFormat, false);
				texture = this.externallyUpdatedTextures[i];
				texture.name = "LerpTexture" + i.ToString();
			}
			else
			{
				this.textureBuffers[i] = new TextureBuffer(((PropertyTextures.Property)i).ToString(), Grid.WidthInCells, Grid.HeightInCells, textureProperties.textureFormat, textureProperties.filterMode, this.texturePagePool);
				texture = this.textureBuffers[i].texture;
			}
			if (textureProperties.blend)
			{
				this.lerpers[i] = new TextureLerper(texture, ((PropertyTextures.Property)i).ToString(), texture.filterMode, textureProperties.textureFormat);
				this.lerpers[i].Speed = textureProperties.blendSpeed;
			}
			string shaderPropertyName = this.GetShaderPropertyName((PropertyTextures.Property)i);
			textureProperties.texturePropertyName = shaderPropertyName;
			Shader.SetGlobalTexture(shaderPropertyName, texture);
			this.allTextureProperties.Add(textureProperties);
		}
	}

	private void OnShadersReloaded()
	{
		for (int i = 0; i < 11; i++)
		{
			TextureLerper textureLerper = this.lerpers[i];
			if (textureLerper != null)
			{
				Shader.SetGlobalTexture(this.allTextureProperties[i].texturePropertyName, textureLerper.Update());
			}
		}
	}

	private void SimUpdate(float dt)
	{
		if (this.lerpers == null || this.lerpers.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this.lerpers.Length; i++)
		{
			TextureLerper textureLerper = this.lerpers[i];
			if (textureLerper != null)
			{
				textureLerper.LongUpdate(dt);
			}
		}
	}

	private void UpdateProperty(ref PropertyTextures.TextureProperties p, int x0, int y0, int x1, int y1)
	{
		if (Game.Instance.IsLoading())
		{
			return;
		}
		int simProperty = (int)p.simProperty;
		if (!p.updatedExternally)
		{
			TextureRegion textureRegion = this.textureBuffers[simProperty].Lock(x0, y0, x1 - x0 + 1, y1 - y0 + 1);
			switch (p.simProperty)
			{
			case PropertyTextures.Property.StateChange:
				this.UpdateStateChange(textureRegion, x0, y0, x1, y1);
				break;
			case PropertyTextures.Property.GasPressure:
				this.UpdatePressure(textureRegion, x0, y0, x1, y1);
				break;
			case PropertyTextures.Property.GasColour:
				this.UpdateGasColour(textureRegion, x0, y0, x1, y1);
				break;
			case PropertyTextures.Property.GasDanger:
				this.UpdateDanger(textureRegion, x0, y0, x1, y1);
				break;
			case PropertyTextures.Property.FogOfWar:
				this.UpdateFogOfWar(textureRegion, x0, y0, x1, y1);
				break;
			case PropertyTextures.Property.SolidDigAmount:
				this.UpdateSolidDigAmount(textureRegion, x0, y0, x1, y1);
				break;
			case PropertyTextures.Property.SolidLiquidGasMass:
				this.UpdateSolidLiquidGasMass(textureRegion, x0, y0, x1, y1);
				break;
			case PropertyTextures.Property.WorldLight:
				this.UpdateWorldLight(textureRegion, x0, y0, x1, y1);
				break;
			case PropertyTextures.Property.Temperature:
				this.UpdateTemperature(textureRegion, x0, y0, x1, y1);
				break;
			}
			textureRegion.Unlock();
		}
		else
		{
			PropertyTextures.Property simProperty2 = p.simProperty;
			if (simProperty2 != PropertyTextures.Property.Flow)
			{
				if (simProperty2 == PropertyTextures.Property.Liquid)
				{
					this.externallyUpdatedTextures[simProperty].LoadRawTextureData(PropertyTextures.externalLiquidTex, 4 * Grid.WidthInCells * Grid.HeightInCells);
				}
			}
			else
			{
				this.externallyUpdatedTextures[simProperty].LoadRawTextureData(PropertyTextures.externalFlowTex, 8 * Grid.WidthInCells * Grid.HeightInCells);
			}
			this.externallyUpdatedTextures[simProperty].Apply();
		}
	}

	private void LateUpdate()
	{
		if (!Grid.IsInitialized())
		{
			return;
		}
		Shader.SetGlobalVector(this.WorldSizeID, new Vector4((float)Grid.WidthInCells, (float)Grid.HeightInCells, 1f / (float)Grid.WidthInCells, 1f / (float)Grid.HeightInCells));
		Shader.SetGlobalVector(this.PropTexWsToCsID, new Vector4(0f, 0f, 1f, 1f));
		Shader.SetGlobalVector(this.PropTexCsToWsID, new Vector4(0f, 0f, 1f, 1f));
		Vector3 vector = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.transform.position.z));
		Vector3 vector2 = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.transform.position.z));
		Shader.SetGlobalVector("_CamPosInfo", new Vector4(vector.x, vector.y, vector2.x, vector2.y));
		int num;
		int num2;
		int num3;
		int num4;
		this.GetVisibleCellRange(out num, out num2, out num3, out num4);
		Shader.SetGlobalFloat(this.FogOfWarScaleID, PropertyTextures.FogOfWarScale);
		int num5 = this.NextPropertyIdx++ % this.allTextureProperties.Count;
		PropertyTextures.TextureProperties textureProperties = this.allTextureProperties[num5];
		while (textureProperties.updateEveryFrame)
		{
			num5 = this.NextPropertyIdx++ % this.allTextureProperties.Count;
			textureProperties = this.allTextureProperties[num5];
		}
		for (int i = 0; i < this.allTextureProperties.Count; i++)
		{
			PropertyTextures.TextureProperties textureProperties2 = this.allTextureProperties[i];
			if (num5 == i || textureProperties2.updateEveryFrame)
			{
				this.UpdateProperty(ref textureProperties2, num, num2, num3, num4);
			}
		}
		for (int j = 0; j < 11; j++)
		{
			TextureLerper textureLerper = this.lerpers[j];
			if (textureLerper != null)
			{
				if (Time.timeScale == 0f)
				{
					textureLerper.LongUpdate(Time.unscaledDeltaTime);
				}
				Shader.SetGlobalTexture(this.allTextureProperties[j].texturePropertyName, textureLerper.Update());
			}
		}
	}

	private void GetVisibleCellRange(out int x0, out int y0, out int x1, out int y1)
	{
		int num = 16;
		Grid.GetVisibleExtents(out x0, out y0, out x1, out y1);
		x0 = Math.Max(0, x0 - num);
		y0 = Math.Max(0, y0 - num);
		x0 = Mathf.Min(x0, Grid.WidthInCells - 1);
		y0 = Mathf.Min(y0, Grid.HeightInCells - 1);
		x1 = Mathf.CeilToInt((float)(x1 + num));
		y1 = Mathf.CeilToInt((float)(y1 + num));
		x1 = Mathf.Max(x1, 0);
		y1 = Mathf.Max(y1, 0);
		x1 = Mathf.Min(x1, Grid.WidthInCells - 1);
		y1 = Mathf.Min(y1, Grid.HeightInCells - 1);
	}

	private void UpdateFogOfWar(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		byte[] visible = Grid.Visible;
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				region.SetBytes(j, i, visible[num]);
			}
		}
	}

	private void UpdatePressure(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		float num = this.PressureRange.y - this.PressureRange.x;
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num2 = Grid.XYToCell(j, i);
				float num3 = 0f;
				Element element = Grid.Element[num2];
				if (element.IsGas)
				{
					num3 = Mathf.Clamp01((Grid.Pressure[num2] - this.PressureRange.x) / num);
				}
				else if (element.IsLiquid)
				{
					int num4 = Grid.CellAbove(num2);
					if (Grid.IsValidCell(num4))
					{
						num3 = Mathf.Clamp01((Grid.Pressure[num4] - this.PressureRange.x) / num);
					}
				}
				region.SetBytes(j, i, (byte)(num3 * 255f));
			}
		}
	}

	private void UpdateDanger(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				Element element = Grid.Element[num];
				byte b = ((element.id != SimHashes.Oxygen) ? byte.MaxValue : 0);
				region.SetBytes(j, i, b);
			}
		}
	}

	private void UpdateStateChange(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				float num2 = 0f;
				Element element = Grid.Element[num];
				if (!element.IsVacuum)
				{
					float num3 = Grid.Temperature[num];
					float num4 = element.lowTemp * this.TemperatureStateChangeRange;
					float num5 = Mathf.Abs(num3 - element.lowTemp);
					float num6 = num5 / num4;
					float num7 = element.highTemp * this.TemperatureStateChangeRange;
					float num8 = Mathf.Abs(num3 - element.highTemp);
					float num9 = num8 / num7;
					num2 = Mathf.Max(num2, 1f - Mathf.Min(num6, num9));
				}
				region.SetBytes(j, i, (byte)(num2 * 255f));
			}
		}
	}

	private void UpdateGasColour(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				Element element = Grid.Element[num];
				if (element.IsGas)
				{
					region.SetBytes(j, i, byte.MaxValue, element.substance.colour.r, element.substance.colour.g, element.substance.colour.b);
				}
				else if (element.IsLiquid)
				{
					int num2 = Grid.CellAbove(num);
					if (Grid.IsValidCell(num2))
					{
						region.SetBytes(j, i, byte.MaxValue, element.substance.colour.r, element.substance.colour.g, element.substance.colour.b);
					}
					else
					{
						region.SetBytes(j, i, 0, 0, 0, 0);
					}
				}
				else
				{
					region.SetBytes(j, i, 0, 0, 0, 0);
				}
			}
		}
	}

	private void UpdateLiquid(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		for (int i = x0; i <= x1; i++)
		{
			int num = Grid.XYToCell(i, y1);
			Element element = Grid.Element[num];
			for (int j = y1; j >= y0; j--)
			{
				int num2 = Grid.XYToCell(i, j);
				Element element2 = Grid.Element[num2];
				if (element2.IsLiquid)
				{
					Color32 colour = element2.substance.colour;
					float liquidMaxMass = Lighting.Instance.Settings.LiquidMaxMass;
					float liquidAmountOffset = Lighting.Instance.Settings.LiquidAmountOffset;
					float num3;
					if (element.IsLiquid || element.IsSolid)
					{
						num3 = 1f;
					}
					else
					{
						num3 = liquidAmountOffset + (1f - liquidAmountOffset) * Mathf.Min(Grid.Cell[num2].mass / liquidMaxMass, 1f);
						num3 = Mathf.Pow(Mathf.Min(Grid.Cell[num2].mass / liquidMaxMass, 1f), 0.45f);
					}
					region.SetBytes(i, j, (byte)(num3 * 255f), colour.r, colour.g, colour.b);
				}
				else
				{
					region.SetBytes(i, j, 0, 0, 0, 0);
				}
				element = element2;
			}
		}
	}

	private void UpdateSolidDigAmount(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		int elementIndex = ElementLoader.GetElementIndex(SimHashes.Void);
		for (int i = y0; i <= y1; i++)
		{
			int num = Grid.XYToCell(x0, i);
			int num2 = Grid.XYToCell(x1, i);
			int j = num;
			int num3 = x0;
			while (j <= num2)
			{
				byte b = 0;
				byte b2 = 0;
				byte b3 = 0;
				if ((int)Grid.Cell[j].elementIdx != elementIndex)
				{
					b3 = byte.MaxValue;
				}
				if (Grid.Solid[j])
				{
					b = byte.MaxValue;
					b2 = (byte)(255f * Grid.Damage[j]);
				}
				region.SetBytes(num3, i, b, b2, b3);
				j++;
				num3++;
			}
		}
	}

	private void UpdateSolidLiquidGasMass(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				Element element = Grid.Element[num];
				byte b = ((!element.IsSolid) ? 0 : byte.MaxValue);
				byte b2 = ((!element.IsLiquid) ? 0 : byte.MaxValue);
				byte b3 = ((!element.IsGas && !element.IsVacuum) ? 0 : byte.MaxValue);
				float num2 = Grid.Cell[num].mass / 2000f;
				num2 = Mathf.Min(num2, 1f);
				num2 = Mathf.Pow(num2, 0.45f);
				region.SetBytes(j, i, b, b2, b3, (byte)(num2 * 255f));
			}
		}
	}

	private void GetTemperatureAlpha(float t, out byte cold_alpha, out byte hot_alpha)
	{
		cold_alpha = 0;
		hot_alpha = 0;
		if (t <= this.coldRange.y)
		{
			float num = Mathf.Clamp01((this.coldRange.y - t) / (this.coldRange.y - this.coldRange.x));
			cold_alpha = (byte)(num * 255f);
		}
		else if (t >= this.hotRange.x)
		{
			float num2 = Mathf.Clamp01((t - this.hotRange.x) / (this.hotRange.y - this.hotRange.x));
			hot_alpha = (byte)(num2 * 255f);
		}
	}

	private void UpdateTemperature(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				float num2 = Grid.Temperature[num];
				byte b;
				byte b2;
				this.GetTemperatureAlpha(num2, out b, out b2);
				byte b3 = (byte)(255f * Mathf.Pow(Mathf.Clamp(num2 / 1000f, 0f, 1f), 0.45f));
				region.SetBytes(j, i, b, b2, b3);
			}
		}
	}

	private void UpdateWorldLight(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		if (!this.ForceLightEverywhere)
		{
			for (int i = y0; i <= y1; i++)
			{
				int num = Grid.XYToCell(x0, i);
				int num2 = Grid.XYToCell(x1, i);
				int j = num;
				int num3 = x0;
				while (j <= num2)
				{
					Color32 color = ((Grid.LightCount[j] <= 0) ? new Color32(0, 0, 0, byte.MaxValue) : Lighting.Instance.Settings.LightColour);
					region.SetBytes(num3, i, color.r, color.g, color.b, (color.r + color.g + color.b <= 0) ? 0 : byte.MaxValue);
					j++;
					num3++;
				}
			}
		}
		else
		{
			for (int k = y0; k <= y1; k++)
			{
				for (int l = x0; l <= x1; l++)
				{
					region.SetBytes(l, k, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				}
			}
		}
	}

	[NonSerialized]
	public bool ForceLightEverywhere;

	[SerializeField]
	private Vector2 PressureRange = new Vector2(15f, 200f);

	[SerializeField]
	[Range(0f, 1f)]
	private float TemperatureStateChangeRange = 0.05f;

	public static PropertyTextures instance;

	public static IntPtr externalFlowTex;

	public static IntPtr externalLiquidTex;

	public static IntPtr externalSolidDigAmountTex;

	[SerializeField]
	private Vector2 coldRange;

	[SerializeField]
	private Vector2 hotRange;

	public static float FogOfWarScale;

	public float MaxFlow;

	public float FlowTextureScale;

	private int WorldSizeID;

	private int FogOfWarScaleID;

	private int PropTexWsToCsID;

	private int PropTexCsToWsID;

	private int NextPropertyIdx;

	public TextureBuffer[] textureBuffers;

	public TextureLerper[] lerpers;

	private TexturePagePool texturePagePool;

	private Texture2D[] externallyUpdatedTextures;

	private PropertyTextures.TextureProperties[] textureProperties = new PropertyTextures.TextureProperties[]
	{
		new PropertyTextures.TextureProperties
		{
			simProperty = PropertyTextures.Property.Flow,
			textureFormat = TextureFormat.RGFloat,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = true,
			updatedExternally = true,
			blend = true,
			blendSpeed = 0.25f
		},
		new PropertyTextures.TextureProperties
		{
			simProperty = PropertyTextures.Property.Liquid,
			textureFormat = TextureFormat.ARGB32,
			filterMode = FilterMode.Point,
			updateEveryFrame = true,
			updatedExternally = true,
			blend = true,
			blendSpeed = 1f
		},
		new PropertyTextures.TextureProperties
		{
			simProperty = PropertyTextures.Property.SolidDigAmount,
			textureFormat = TextureFormat.RGB24,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = true,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		},
		new PropertyTextures.TextureProperties
		{
			simProperty = PropertyTextures.Property.GasColour,
			textureFormat = TextureFormat.ARGB32,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = true,
			blendSpeed = 0.25f
		},
		new PropertyTextures.TextureProperties
		{
			simProperty = PropertyTextures.Property.GasDanger,
			textureFormat = TextureFormat.Alpha8,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = true,
			blendSpeed = 0.25f
		},
		new PropertyTextures.TextureProperties
		{
			simProperty = PropertyTextures.Property.GasPressure,
			textureFormat = TextureFormat.Alpha8,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = true,
			blendSpeed = 0.25f
		},
		new PropertyTextures.TextureProperties
		{
			simProperty = PropertyTextures.Property.FogOfWar,
			textureFormat = TextureFormat.Alpha8,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		},
		new PropertyTextures.TextureProperties
		{
			simProperty = PropertyTextures.Property.WorldLight,
			textureFormat = TextureFormat.RGBA32,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		},
		new PropertyTextures.TextureProperties
		{
			simProperty = PropertyTextures.Property.StateChange,
			textureFormat = TextureFormat.Alpha8,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		},
		new PropertyTextures.TextureProperties
		{
			simProperty = PropertyTextures.Property.SolidLiquidGasMass,
			textureFormat = TextureFormat.RGBA32,
			filterMode = FilterMode.Point,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		},
		new PropertyTextures.TextureProperties
		{
			simProperty = PropertyTextures.Property.Temperature,
			textureFormat = TextureFormat.RGB24,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		}
	};

	private List<PropertyTextures.TextureProperties> allTextureProperties = new List<PropertyTextures.TextureProperties>();

	public enum Property
	{
		StateChange,
		GasPressure,
		GasColour,
		GasDanger,
		FogOfWar,
		Flow,
		SolidDigAmount,
		SolidLiquidGasMass,
		WorldLight,
		Liquid,
		Temperature,
		Num
	}

	private struct TextureProperties
	{
		public PropertyTextures.Property simProperty;

		public TextureFormat textureFormat;

		public FilterMode filterMode;

		public bool updateEveryFrame;

		public bool updatedExternally;

		public bool blend;

		public float blendSpeed;

		public string texturePropertyName;
	}
}
