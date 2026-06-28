using System;
using System.Collections;
using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Builder;
using NodeEditorFramework;
using ProcGen.Noise;
using ProcGenGame;
using UnityEngine;

[Node(false, "Noise/Display", new Type[] { typeof(NoiseNodeCanvas) })]
public class DisplayNodeEditor : BaseNodeEditor
{
	public override string GetID
	{
		get
		{
			return "displayNodeEditor";
		}
	}

	public override Type GetObjectType
	{
		get
		{
			return typeof(DisplayNodeEditor);
		}
	}

	public override NoiseBase GetTarget()
	{
		return null;
	}

	public override Node Create(Vector2 pos)
	{
		DisplayNodeEditor displayNodeEditor = ScriptableObject.CreateInstance<DisplayNodeEditor>();
		displayNodeEditor.rect = new Rect(pos.x, pos.y, 266f, 301f);
		displayNodeEditor.name = "Noise Display Node";
		displayNodeEditor.CreateInput("Source Node", "IModule3D", NodeSide.Left, 40f);
		return displayNodeEditor;
	}

	public override bool Calculate()
	{
		bool flag;
		if (!base.allInputsReady() || base.settings == null)
		{
			flag = false;
		}
		else
		{
			IModule3D value = this.Inputs[0].GetValue<IModule3D>();
			if (value == null)
			{
				flag = false;
			}
			else
			{
				this.InitSettings();
				Vector2f lowerBound = base.settings.lowerBound;
				Vector2f upperBound = base.settings.upperBound;
				NoiseMapBuilderPlane noiseMapBuilderPlane = new NoiseMapBuilderPlane(lowerBound.x, upperBound.x, lowerBound.y, upperBound.y, base.settings.seamless);
				noiseMapBuilderPlane.SetSize(256, 256);
				noiseMapBuilderPlane.SourceModule = value;
				Vector2 zero = Vector2.zero;
				float[] noise = WorldGen.GenerateNoise(zero, base.settings.zoom, noiseMapBuilderPlane, 256, 256, null);
				if (base.settings.normalise)
				{
					WorldGen.Normalise(noise);
				}
				GetColourDelegate getColourDelegate = null;
				DisplayNodeEditor.DisplayType displayType = this.displayType;
				if (displayType != DisplayNodeEditor.DisplayType.DefaultColour)
				{
					if (displayType == DisplayNodeEditor.DisplayType.ElementColourFeature || displayType == DisplayNodeEditor.DisplayType.ElementColourBiome)
					{
						getColourDelegate = delegate(int cell)
						{
							Color color;
							if (this.biome == null)
							{
								color = Color.black;
							}
							else
							{
								float num = noise[cell];
								Element element = ElementLoader.FindElementByName(this.biome[this.biome.Count - 1].content);
								for (int i = 0; i < this.biome.Count; i++)
								{
									if (num < this.biome[i].maxValue)
									{
										element = ElementLoader.FindElementByName(this.biome[i].content);
										break;
									}
								}
								color = element.substance.debugColour;
							}
							return color;
						};
					}
				}
				else
				{
					getColourDelegate = (int cell) => Color.HSVToRGB((40f + 320f * noise[cell]) / 360f, 1f, 1f);
				}
				if (getColourDelegate != null)
				{
					this.SetColours(getColourDelegate);
				}
				flag = true;
			}
		}
		return flag;
	}

	private void SetColours(GetColourDelegate getColourCall)
	{
		byte[] array;
		this.texture = SimDebugView.CreateTexture(out array, 256, 256);
		for (int i = 0; i < 65536; i++)
		{
			Color color = getColourCall(i);
			int num = i * 4;
			array[num] = (byte)(Mathf.Min(color.r, 1f) * 255f);
			array[num + 1] = (byte)(Mathf.Min(color.g, 1f) * 255f);
			array[num + 2] = (byte)(Mathf.Min(color.b, 1f) * 255f);
			array[num + 3] = byte.MaxValue;
		}
		this.texture.LoadRawTextureData(array);
		this.texture.Apply();
	}

	private void InitSettings()
	{
		if (WorldGen.Settings == null)
		{
			Hashtable hashtable = new Hashtable();
			ElementLoader.Load(ref hashtable, this.simElementsSolidsFile.text, this.simElementsLiquidsFile.text, this.simElementsGasesFile.text, this.substanceTable);
			WorldGen.LoadSettings();
		}
	}

	private void GetBiomeOptions()
	{
		if (this.biomeOptions == null)
		{
			this.InitSettings();
			this.biomeOptions = WorldGen.Settings.biomes.GetNames();
		}
	}

	private void GetFeatureOptions()
	{
		if (this.featureOptions == null)
		{
			this.InitSettings();
			this.featureOptions = WorldGen.Settings.features.GetNames();
		}
	}

	protected override void NodeGUI()
	{
		base.NodeGUI();
	}

	private const string Id = "displayNodeEditor";

	[SerializeField]
	public DisplayNodeEditor.DisplayType displayType = DisplayNodeEditor.DisplayType.DefaultColour;

	private const int width = 256;

	private const int height = 256;

	private Texture2D texture = null;

	private ElementBandConfiguration biome = null;

	private string[] biomeOptions = null;

	private string[] featureOptions = null;

	public TextAsset simElementsSolidsFile;

	public TextAsset simElementsLiquidsFile;

	public TextAsset simElementsGasesFile;

	public SubstanceTable substanceTable;

	public enum DisplayType
	{
		DefaultColour,
		ElementColourBiome,
		ElementColourFeature
	}
}
