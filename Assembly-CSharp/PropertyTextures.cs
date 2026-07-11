using System;
using System.Collections.Generic;
using Klei;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class PropertyTextures : KMonoBehaviour, ISim200ms
{
	public static void DestroyInstance()
	{
		ShaderReloader.Unregister(new global::System.Action(PropertyTextures.instance.OnShadersReloaded));
		PropertyTextures.externalFlowTex = IntPtr.Zero;
		PropertyTextures.externalLiquidTex = IntPtr.Zero;
		PropertyTextures.externalExposedToSunlight = IntPtr.Zero;
		PropertyTextures.externalSolidDigAmountTex = IntPtr.Zero;
		PropertyTextures.instance = null;
	}

	protected override void OnPrefabInit()
	{
		PropertyTextures.instance = this;
		base.OnPrefabInit();
		ShaderReloader.Register(new global::System.Action(this.OnShadersReloaded));
	}

	public static bool IsFogOfWarEnabled
	{
		get
		{
			return PropertyTextures.FogOfWarScale < 1f;
		}
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
		if (GenericGameSettings.instance.disableFogOfWar)
		{
			PropertyTextures.FogOfWarScale = 1f;
		}
		this.WorldSizeID = Shader.PropertyToID("_WorldSizeInfo");
		this.FogOfWarScaleID = Shader.PropertyToID("_FogOfWarScale");
		this.PropTexWsToCsID = Shader.PropertyToID("_PropTexWsToCs");
		this.PropTexCsToWsID = Shader.PropertyToID("_PropTexCsToWs");
		this.TopBorderHeightID = Shader.PropertyToID("_TopBorderHeight");
	}

	public void OnReset(object data = null)
	{
		this.lerpers = new TextureLerper[12];
		this.texturePagePool = new TexturePagePool();
		this.textureBuffers = new TextureBuffer[12];
		this.externallyUpdatedTextures = new Texture2D[12];
		for (int i = 0; i < 12; i++)
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
			PropertyTextures.Property property = (PropertyTextures.Property)i;
			textureProperties.name = property.ToString();
			if (this.externallyUpdatedTextures[i] != null)
			{
				global::UnityEngine.Object.Destroy(this.externallyUpdatedTextures[i]);
				this.externallyUpdatedTextures[i] = null;
			}
			Texture texture;
			if (textureProperties.updatedExternally)
			{
				this.externallyUpdatedTextures[i] = new Texture2D(Grid.WidthInCells, Grid.HeightInCells, TextureUtil.TextureFormatToGraphicsFormat(textureProperties.textureFormat), TextureCreationFlags.None);
				texture = this.externallyUpdatedTextures[i];
			}
			else
			{
				TextureBuffer[] array = this.textureBuffers;
				int num = i;
				PropertyTextures.Property property2 = (PropertyTextures.Property)i;
				array[num] = new TextureBuffer(property2.ToString(), Grid.WidthInCells, Grid.HeightInCells, textureProperties.textureFormat, textureProperties.filterMode, this.texturePagePool);
				texture = this.textureBuffers[i].texture;
			}
			if (textureProperties.blend)
			{
				TextureLerper[] array2 = this.lerpers;
				int num2 = i;
				Texture texture2 = texture;
				PropertyTextures.Property property3 = (PropertyTextures.Property)i;
				array2[num2] = new TextureLerper(texture2, property3.ToString(), texture.filterMode, textureProperties.textureFormat);
				this.lerpers[i].Speed = textureProperties.blendSpeed;
			}
			string shaderPropertyName = this.GetShaderPropertyName((PropertyTextures.Property)i);
			texture.name = shaderPropertyName;
			textureProperties.texturePropertyName = shaderPropertyName;
			Shader.SetGlobalTexture(shaderPropertyName, texture);
			this.allTextureProperties.Add(textureProperties);
		}
	}

	private void OnShadersReloaded()
	{
		for (int i = 0; i < 12; i++)
		{
			TextureLerper textureLerper = this.lerpers[i];
			if (textureLerper != null)
			{
				Shader.SetGlobalTexture(this.allTextureProperties[i].texturePropertyName, textureLerper.Update());
			}
		}
	}

	public void Sim200ms(float dt)
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

	private void UpdateTextureThreaded(TextureRegion texture_region, int x0, int y0, int x1, int y1, PropertyTextures.WorkItem.Callback update_texture_cb)
	{
		this.workItems.Reset(null);
		int num = 16;
		for (int i = y0; i <= y1; i += num)
		{
			int num2 = Math.Min(i + num - 1, y1);
			this.workItems.Add(new PropertyTextures.WorkItem(texture_region, x0, i, x1, num2, update_texture_cb));
		}
		GlobalJobManager.Run(this.workItems);
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
				this.UpdateTextureThreaded(textureRegion, x0, y0, x1, y1, new PropertyTextures.WorkItem.Callback(PropertyTextures.UpdateStateChange));
				break;
			case PropertyTextures.Property.GasPressure:
				this.UpdateTextureThreaded(textureRegion, x0, y0, x1, y1, new PropertyTextures.WorkItem.Callback(PropertyTextures.UpdatePressure));
				break;
			case PropertyTextures.Property.GasColour:
				this.UpdateTextureThreaded(textureRegion, x0, y0, x1, y1, new PropertyTextures.WorkItem.Callback(PropertyTextures.UpdateGasColour));
				break;
			case PropertyTextures.Property.GasDanger:
				this.UpdateTextureThreaded(textureRegion, x0, y0, x1, y1, new PropertyTextures.WorkItem.Callback(PropertyTextures.UpdateDanger));
				break;
			case PropertyTextures.Property.FogOfWar:
				this.UpdateTextureThreaded(textureRegion, x0, y0, x1, y1, new PropertyTextures.WorkItem.Callback(PropertyTextures.UpdateFogOfWar));
				break;
			case PropertyTextures.Property.SolidDigAmount:
				this.UpdateTextureThreaded(textureRegion, x0, y0, x1, y1, new PropertyTextures.WorkItem.Callback(PropertyTextures.UpdateSolidDigAmount));
				break;
			case PropertyTextures.Property.SolidLiquidGasMass:
				this.UpdateTextureThreaded(textureRegion, x0, y0, x1, y1, new PropertyTextures.WorkItem.Callback(PropertyTextures.UpdateSolidLiquidGasMass));
				break;
			case PropertyTextures.Property.WorldLight:
				this.UpdateTextureThreaded(textureRegion, x0, y0, x1, y1, new PropertyTextures.WorkItem.Callback(PropertyTextures.UpdateWorldLight));
				break;
			case PropertyTextures.Property.Temperature:
				this.UpdateTextureThreaded(textureRegion, x0, y0, x1, y1, new PropertyTextures.WorkItem.Callback(PropertyTextures.UpdateTemperature));
				break;
			}
			textureRegion.Unlock();
		}
		else
		{
			PropertyTextures.Property simProperty2 = p.simProperty;
			if (simProperty2 != PropertyTextures.Property.Flow)
			{
				if (simProperty2 != PropertyTextures.Property.Liquid)
				{
					if (simProperty2 == PropertyTextures.Property.ExposedToSunlight)
					{
						this.externallyUpdatedTextures[simProperty].LoadRawTextureData(PropertyTextures.externalExposedToSunlight, Grid.WidthInCells * Grid.HeightInCells);
					}
				}
				else
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
		Shader.SetGlobalFloat(this.TopBorderHeightID, (float)Grid.TopBorderHeight);
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
		for (int j = 0; j < 12; j++)
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

	private static void UpdateFogOfWar(TextureRegion region, int x0, int y0, int x1, int y1)
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

	private static void UpdatePressure(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		Vector2 pressureRange = PropertyTextures.instance.PressureRange;
		float minPressureVisibility = PropertyTextures.instance.MinPressureVisibility;
		float num = pressureRange.y - pressureRange.x;
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num2 = Grid.XYToCell(j, i);
				float num3 = 0f;
				Element element = Grid.Element[num2];
				if (element.IsGas)
				{
					float num4 = Grid.Pressure[num2];
					float num5 = ((num4 <= 0f) ? 0f : minPressureVisibility);
					num3 = Mathf.Max(Mathf.Clamp01((num4 - pressureRange.x) / num), num5);
				}
				else if (element.IsLiquid)
				{
					int num6 = Grid.CellAbove(num2);
					if (Grid.IsValidCell(num6) && Grid.Element[num6].IsGas)
					{
						float num7 = Grid.Pressure[num6];
						float num8 = ((num7 <= 0f) ? 0f : minPressureVisibility);
						num3 = Mathf.Max(Mathf.Clamp01((num7 - pressureRange.x) / num), num8);
					}
				}
				region.SetBytes(j, i, (byte)(num3 * 255f));
			}
		}
	}

	private static void UpdateDanger(TextureRegion region, int x0, int y0, int x1, int y1)
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

	private static void UpdateStateChange(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		float temperatureStateChangeRange = PropertyTextures.instance.TemperatureStateChangeRange;
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
					float num4 = element.lowTemp * temperatureStateChangeRange;
					float num5 = Mathf.Abs(num3 - element.lowTemp);
					float num6 = num5 / num4;
					float num7 = element.highTemp * temperatureStateChangeRange;
					float num8 = Mathf.Abs(num3 - element.highTemp);
					float num9 = num8 / num7;
					num2 = Mathf.Max(num2, 1f - Mathf.Min(num6, num9));
				}
				region.SetBytes(j, i, (byte)(num2 * 255f));
			}
		}
	}

	private static void UpdateGasColour(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				Element element = Grid.Element[num];
				if (element.IsGas)
				{
					region.SetBytes(j, i, element.substance.colour.r, element.substance.colour.g, element.substance.colour.b, byte.MaxValue);
				}
				else if (element.IsLiquid)
				{
					int num2 = Grid.CellAbove(num);
					if (Grid.IsValidCell(num2))
					{
						region.SetBytes(j, i, element.substance.colour.r, element.substance.colour.g, element.substance.colour.b, byte.MaxValue);
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

	private static void UpdateLiquid(TextureRegion region, int x0, int y0, int x1, int y1)
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
						num3 = liquidAmountOffset + (1f - liquidAmountOffset) * Mathf.Min(Grid.Mass[num2] / liquidMaxMass, 1f);
						num3 = Mathf.Pow(Mathf.Min(Grid.Mass[num2] / liquidMaxMass, 1f), 0.45f);
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

	private static void UpdateSolidDigAmount(TextureRegion region, int x0, int y0, int x1, int y1)
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
				if ((int)Grid.ElementIdx[j] != elementIndex)
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

	private static void UpdateSolidLiquidGasMass(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				Element element = Grid.Element[num];
				byte b = 0;
				byte b2 = 0;
				byte b3 = 0;
				if (element.IsSolid)
				{
					b = byte.MaxValue;
				}
				else if (element.IsLiquid)
				{
					b2 = byte.MaxValue;
				}
				else if (element.IsGas || element.IsVacuum)
				{
					b3 = byte.MaxValue;
				}
				float num2 = Grid.Mass[num];
				float num3 = Mathf.Min(1f, num2 / 2000f);
				if (num2 > 0f)
				{
					num3 = Mathf.Max(0.003921569f, num3);
				}
				region.SetBytes(j, i, b, b2, b3, (byte)(num3 * 255f));
			}
		}
	}

	private static void GetTemperatureAlpha(float t, Vector2 cold_range, Vector2 hot_range, out byte cold_alpha, out byte hot_alpha)
	{
		cold_alpha = 0;
		hot_alpha = 0;
		if (t <= cold_range.y)
		{
			float num = Mathf.Clamp01((cold_range.y - t) / (cold_range.y - cold_range.x));
			cold_alpha = (byte)(num * 255f);
		}
		else if (t >= hot_range.x)
		{
			float num2 = Mathf.Clamp01((t - hot_range.x) / (hot_range.y - hot_range.x));
			hot_alpha = (byte)(num2 * 255f);
		}
	}

	private static void UpdateTemperature(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		Vector2 vector = PropertyTextures.instance.coldRange;
		Vector2 vector2 = PropertyTextures.instance.hotRange;
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				float num2 = Grid.Temperature[num];
				byte b;
				byte b2;
				PropertyTextures.GetTemperatureAlpha(num2, vector, vector2, out b, out b2);
				byte b3 = (byte)(255f * Mathf.Pow(Mathf.Clamp(num2 / 1000f, 0f, 1f), 0.45f));
				region.SetBytes(j, i, b, b2, b3);
			}
		}
	}

	private static void UpdateWorldLight(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		if (!PropertyTextures.instance.ForceLightEverywhere)
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
	private float MinPressureVisibility = 0.1f;

	[SerializeField]
	[Range(0f, 1f)]
	private float TemperatureStateChangeRange = 0.05f;

	public static PropertyTextures instance;

	public static IntPtr externalFlowTex;

	public static IntPtr externalLiquidTex;

	public static IntPtr externalExposedToSunlight;

	public static IntPtr externalSolidDigAmountTex;

	[SerializeField]
	private Vector2 coldRange;

	[SerializeField]
	private Vector2 hotRange;

	public static float FogOfWarScale;

	private int WorldSizeID;

	private int FogOfWarScaleID;

	private int PropTexWsToCsID;

	private int PropTexCsToWsID;

	private int TopBorderHeightID;

	private int NextPropertyIdx;

	public TextureBuffer[] textureBuffers;

	public TextureLerper[] lerpers;

	private TexturePagePool texturePagePool;

	[SerializeField]
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
			textureFormat = TextureFormat.RGBA32,
			filterMode = FilterMode.Point,
			updateEveryFrame = true,
			updatedExternally = true,
			blend = true,
			blendSpeed = 1f
		},
		new PropertyTextures.TextureProperties
		{
			simProperty = PropertyTextures.Property.ExposedToSunlight,
			textureFormat = TextureFormat.Alpha8,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = true,
			updatedExternally = true,
			blend = false,
			blendSpeed = 0f
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
			textureFormat = TextureFormat.RGBA32,
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
			updateEveryFrame = true,
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
			updateEveryFrame = true,
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

	private WorkItemCollection<PropertyTextures.WorkItem, object> workItems = new WorkItemCollection<PropertyTextures.WorkItem, object>();

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
		ExposedToSunlight,
		Num
	}

	private struct TextureProperties
	{
		public string name;

		public PropertyTextures.Property simProperty;

		public TextureFormat textureFormat;

		public FilterMode filterMode;

		public bool updateEveryFrame;

		public bool updatedExternally;

		public bool blend;

		public float blendSpeed;

		public string texturePropertyName;
	}

	private struct WorkItem : IWorkItem<object>
	{
		public WorkItem(TextureRegion texture_region, int x0, int y0, int x1, int y1, PropertyTextures.WorkItem.Callback update_texture_cb)
		{
			this.textureRegion = texture_region;
			this.x0 = x0;
			this.y0 = y0;
			this.x1 = x1;
			this.y1 = y1;
			this.updateTextureCb = update_texture_cb;
		}

		public void Run(object shared_data)
		{
			this.updateTextureCb(this.textureRegion, this.x0, this.y0, this.x1, this.y1);
		}

		private int x0;

		private int y0;

		private int x1;

		private int y1;

		private TextureRegion textureRegion;

		private PropertyTextures.WorkItem.Callback updateTextureCb;

		public delegate void Callback(TextureRegion texture_region, int x0, int y0, int x1, int y1);
	}
}
