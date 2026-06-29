using System;
using Klei.AI;

public class SandboxSettings
{
	public KPrefabID Entity
	{
		get
		{
			return this.entity;
		}
		set
		{
			this.entity = value;
		}
	}

	public Element Element
	{
		get
		{
			return this.element;
		}
		set
		{
			this.SelectElement(value);
		}
	}

	public Disease Disease
	{
		get
		{
			return this.disease;
		}
		set
		{
			this.SelectDisease(value);
		}
	}

	public int BrushSize
	{
		get
		{
			return this.brushSize;
		}
		set
		{
			this.SetBrushSize(value);
		}
	}

	public float NoiseScale
	{
		get
		{
			return this.noiseScale;
		}
		set
		{
			this.SetNoiseScale(value);
		}
	}

	public float NoiseDensity
	{
		get
		{
			return this.noiseDensity;
		}
		set
		{
			this.SetNoiseDensity(value);
		}
	}

	public float Mass
	{
		get
		{
			return this.mass;
		}
		set
		{
			this.mass = value;
		}
	}

	public bool InstantBuild
	{
		get
		{
			return this.instantBuild;
		}
		set
		{
			this.instantBuild = value;
		}
	}

	public void SelectEntity(KPrefabID entity)
	{
		this.entity = entity;
		this.OnChangeEntity();
	}

	public void SelectElement(Element element)
	{
		this.element = element;
		this.OnChangeElement();
	}

	public void SelectDisease(Disease disease)
	{
		this.disease = disease;
		this.OnChangeDisease();
	}

	public void SetBrushSize(int size)
	{
		this.brushSize = size;
		this.OnChangeBrushSize();
	}

	public void SetNoiseScale(float amount)
	{
		this.noiseScale = amount;
		this.OnChangeNoiseScale();
	}

	public void SetNoiseDensity(float amount)
	{
		this.noiseDensity = amount;
		this.OnChangeNoiseDensity();
	}

	private KPrefabID entity;

	private Element element;

	private Disease disease;

	private int brushSize;

	private float noiseScale;

	private float noiseDensity;

	private float mass;

	private bool instantBuild;

	public float temperature;

	public float temperatureAdditive;

	public int diseaseCount;

	public global::System.Action OnChangeElement;

	public global::System.Action OnChangeDisease;

	public global::System.Action OnChangeEntity;

	public global::System.Action OnChangeBrushSize;

	public global::System.Action OnChangeNoiseScale;

	public global::System.Action OnChangeNoiseDensity;
}
