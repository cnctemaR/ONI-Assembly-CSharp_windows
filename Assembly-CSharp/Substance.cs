using System;
using FMODUnity;
using Klei;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Substance
{
	public GameObject SpawnResource(Vector3 position, float mass, float temperature, byte disease_idx, int disease_count, bool prevent_merge = false, bool forceTemperature = false)
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
							gameObject = component2.gameObject;
							primaryElement = component2.GetComponent<PrimaryElement>();
							temperature = SimUtil.CalculateFinalTemperature(primaryElement.Mass, primaryElement.Temperature, mass, temperature);
							position = gameObject.transform.position;
							break;
						}
					}
				}
			}
		}
		if (gameObject == null)
		{
			gameObject = GameUtil.KInstantiate(Assets.GetPrefab(GameTagExtensions.Create(this.elementID)), Grid.SceneLayer.Ore, Folder.Loot, null, 0);
			primaryElement = gameObject.GetComponent<PrimaryElement>();
			primaryElement.Mass = mass;
		}
		else
		{
			primaryElement.Mass += mass;
		}
		primaryElement.InternalTemperature = temperature;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
		gameObject.transform.SetPosition(position);
		gameObject.SetActive(true);
		primaryElement.AddDisease(disease_idx, disease_count, "Substances.SpawnResource");
		return gameObject;
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
		this.propertyBlock.SetVector("_HueSaturation", new Vector4(this.hue, this.saturation, 0f, 0f));
		if (this.material != null)
		{
			this.SetTexture(this.propertyBlock, "_MainTex");
			float @float = this.material.GetFloat("_WorldUVScale");
			this.propertyBlock.SetFloat("_WorldUVScale", @float);
			Element element = ElementLoader.FindElementByHash(this.elementID);
			if (element.IsSolid)
			{
				this.SetTexture(this.propertyBlock, "_MainTex2");
				this.SetTexture(this.propertyBlock, "_HeightTex2");
				this.propertyBlock.SetFloat("_Frequency", this.material.GetFloat("_Frequency"));
				this.propertyBlock.SetColor("_ShineColour", this.material.GetColor("_ShineColour"));
			}
		}
	}

	public AmbienceType GetAmbience()
	{
		AmbienceType ambienceType;
		if (this.audioConfig == null)
		{
			ambienceType = AmbienceType.None;
		}
		else
		{
			ambienceType = this.audioConfig.ambienceType;
		}
		return ambienceType;
	}

	public SolidAmbienceType GetSolidAmbience()
	{
		SolidAmbienceType solidAmbienceType;
		if (this.audioConfig == null)
		{
			solidAmbienceType = SolidAmbienceType.None;
		}
		else
		{
			solidAmbienceType = this.audioConfig.solidAmbienceType;
		}
		return solidAmbienceType;
	}

	public string GetMiningSound()
	{
		string text;
		if (this.audioConfig == null)
		{
			text = "";
		}
		else
		{
			text = this.audioConfig.miningSound;
		}
		return text;
	}

	public string GetMiningBreakSound()
	{
		string text;
		if (this.audioConfig == null)
		{
			text = "";
		}
		else
		{
			text = this.audioConfig.miningBreakSound;
		}
		return text;
	}

	public string GetOreBumpSound()
	{
		string text;
		if (this.audioConfig == null)
		{
			text = "";
		}
		else
		{
			text = this.audioConfig.oreBumpSound;
		}
		return text;
	}

	public string GetFloorEventAudioCategory()
	{
		string text;
		if (this.audioConfig == null)
		{
			text = "";
		}
		else
		{
			text = this.audioConfig.floorEventAudioCategory;
		}
		return text;
	}

	public string name;

	public SimHashes elementID;

	public Color32 colour;

	public Color32 debugColour;

	public Color32 overlayColour = Color.white;

	public GameObject hitEffect;

	[EventRef]
	[FormerlySerializedAs("fallingStartSoundMigrated")]
	public string fallingStartSound;

	[EventRef]
	[FormerlySerializedAs("fallingStopSoundMigrated")]
	public string fallingStopSound;

	[NonSerialized]
	public bool renderedByWorld;

	[NonSerialized]
	public int idx;

	public Texture2D buildingTexture;

	public Material material;

	public KAnimFile anim;

	[NonSerialized]
	public KAnimFile[] anims;

	public float hue = 0f;

	public float saturation = 1f;

	public MaterialPropertyBlock propertyBlock;

	public ElementsAudio.ElementAudioConfig audioConfig;

	public bool showInEditor = true;

	[Serializable]
	public struct Loot
	{
		public void FreeResources()
		{
			this.item = null;
			this.spawnOnFloor = false;
		}

		public GameObject item;

		public bool spawnOnFloor;

		public bool isEntombedItem;
	}
}
