using System;
using FMODUnity;
using Klei;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Substance
{
	public Substance.SubstanceTexture Texture
	{
		get
		{
			return this.texture;
		}
	}

	public bool LiquidCaustics
	{
		get
		{
			return this.usesCaustics;
		}
	}

	public bool Glows
	{
		get
		{
			return this.glows;
		}
	}

	public bool Metalic
	{
		get
		{
			return this.metalic;
		}
	}

	public bool IsOpaqueLiquid
	{
		get
		{
			return this.isOpaqueLiquid;
		}
	}

	public Gradient Gradient
	{
		get
		{
			if (this.gradient == null)
			{
				this.gradient = new Gradient
				{
					colorKeys = new GradientColorKey[]
					{
						new GradientColorKey
						{
							color = this.colour,
							time = 0f
						}
					}
				};
			}
			return this.gradient;
		}
	}

	public GameObject SpawnResource(Vector3 position, float mass, float temperature, byte disease_idx, int disease_count, bool prevent_merge = false, bool forceTemperature = false, bool manual_activation = false)
	{
		GameObject gameObject = null;
		PrimaryElement primaryElement = null;
		if (!prevent_merge)
		{
			int num = Grid.PosToCell(position);
			GameObject gameObject2 = Grid.Objects[num, 3];
			if (gameObject2 != null)
			{
				Pickupable component = gameObject2.GetComponent<Pickupable>();
				if (component != null)
				{
					Tag tag = GameTagExtensions.Create(this.elementID);
					for (ObjectLayerListItem objectLayerListItem = component.objectLayerListItem; objectLayerListItem != null; objectLayerListItem = objectLayerListItem.nextItem)
					{
						KPrefabID component2 = objectLayerListItem.gameObject.GetComponent<KPrefabID>();
						if (component2.PrefabTag == tag)
						{
							PrimaryElement component3 = component2.GetComponent<PrimaryElement>();
							if (component3.Mass + mass <= PrimaryElement.MAX_MASS)
							{
								gameObject = component2.gameObject;
								primaryElement = component3;
								temperature = SimUtil.CalculateFinalTemperature(primaryElement.Mass, primaryElement.Temperature, mass, temperature);
								position = gameObject.transform.GetPosition();
								break;
							}
						}
					}
				}
			}
		}
		if (gameObject == null)
		{
			gameObject = GameUtil.KInstantiate(Assets.GetPrefab(this.nameTag), Grid.SceneLayer.Ore, null, 0);
			primaryElement = gameObject.GetComponent<PrimaryElement>();
			primaryElement.Mass = mass;
		}
		else
		{
			global::Debug.Assert(primaryElement != null);
			Pickupable component4 = primaryElement.GetComponent<Pickupable>();
			if (component4 != null)
			{
				component4.TotalAmount += mass / primaryElement.MassPerUnit;
			}
			else
			{
				primaryElement.Mass += mass;
			}
		}
		primaryElement.Temperature = temperature;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
		gameObject.transform.SetPosition(position);
		if (!manual_activation)
		{
			this.ActivateSubstanceGameObject(gameObject, disease_idx, disease_count);
		}
		return gameObject;
	}

	public void ActivateSubstanceGameObject(GameObject obj, byte disease_idx, int disease_count)
	{
		obj.SetActive(true);
		obj.GetComponent<PrimaryElement>().AddDisease(disease_idx, disease_count, "Substances.SpawnResource");
	}

	private void SetTexture(MaterialPropertyBlock block, string texture_name)
	{
		Texture texture = this.material.GetTexture(texture_name);
		if (texture != null)
		{
			this.propertyBlock.SetTexture(texture_name, texture);
		}
	}

	public void RefreshPropertyBlock()
	{
		if (this.propertyBlock == null)
		{
			this.propertyBlock = new MaterialPropertyBlock();
		}
		if (this.material != null)
		{
			this.SetTexture(this.propertyBlock, "_MainTex");
			float @float = this.material.GetFloat("_WorldUVScale");
			this.propertyBlock.SetFloat("_WorldUVScale", @float);
			if (ElementLoader.FindElementByHash(this.elementID).IsSolid)
			{
				this.SetTexture(this.propertyBlock, "_MainTex2");
				this.SetTexture(this.propertyBlock, "_HeightTex2");
				this.propertyBlock.SetFloat("_Frequency", this.material.GetFloat("_Frequency"));
				this.propertyBlock.SetColor("_ShineColour", this.material.GetColor("_ShineColour"));
				this.propertyBlock.SetColor("_ColourTint", this.material.GetColor("_ColourTint"));
			}
		}
	}

	internal AmbienceType GetAmbience()
	{
		if (this.audioConfig == null)
		{
			return AmbienceType.None;
		}
		return this.audioConfig.ambienceType;
	}

	internal SolidAmbienceType GetSolidAmbience()
	{
		if (this.audioConfig == null)
		{
			return SolidAmbienceType.None;
		}
		return this.audioConfig.solidAmbienceType;
	}

	internal string GetMiningSound()
	{
		if (this.audioConfig == null)
		{
			return "";
		}
		return this.audioConfig.miningSound;
	}

	internal string GetMiningBreakSound()
	{
		if (this.audioConfig == null)
		{
			return "";
		}
		return this.audioConfig.miningBreakSound;
	}

	internal string GetOreBumpSound()
	{
		if (this.audioConfig == null)
		{
			return "";
		}
		return this.audioConfig.oreBumpSound;
	}

	internal string GetFloorEventAudioCategory()
	{
		if (this.audioConfig == null)
		{
			return "";
		}
		return this.audioConfig.floorEventAudioCategory;
	}

	internal string GetCreatureChewSound()
	{
		if (this.audioConfig == null)
		{
			return "";
		}
		return this.audioConfig.creatureChewSound;
	}

	public string name;

	public SimHashes elementID;

	internal Tag nameTag;

	public Color32 colour;

	[SerializeField]
	private bool usesCaustics;

	[SerializeField]
	private bool glows;

	[SerializeField]
	private bool metalic;

	[SerializeField]
	private bool isOpaqueLiquid;

	[SerializeField]
	private Gradient gradient;

	[SerializeField]
	private Substance.SubstanceTexture texture;

	[FormerlySerializedAs("debugColour")]
	public Color32 uiColour;

	[FormerlySerializedAs("overlayColour")]
	public Color32 conduitColour = Color.white;

	[NonSerialized]
	internal bool renderedByWorld;

	[NonSerialized]
	internal int idx;

	public Material material;

	public KAnimFile anim;

	[SerializeField]
	internal bool showInEditor = true;

	[NonSerialized]
	internal KAnimFile[] anims;

	[NonSerialized]
	internal ElementsAudio.ElementAudioConfig audioConfig;

	[NonSerialized]
	internal MaterialPropertyBlock propertyBlock;

	public EventReference fallingStartSound;

	public EventReference fallingStopSound;

	public static int SUBSTANCE_TEXTURE_COUNT = 11;

	public enum SubstanceTexture : byte
	{
		None,
		Magma,
		MoltenMetal,
		Polluted,
		Oil,
		Thick,
		Sap,
		CrystalFragments,
		Ovolene,
		Mucus,
		Ink,
		PollutedBrine
	}
}
