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
							position = gameObject.transform.GetPosition();
							break;
						}
					}
				}
			}
		}
		if (gameObject == null)
		{
			GameObject prefab = Assets.GetPrefab(this.nameTag);
			gameObject = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Ore, null, 0);
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
				this.propertyBlock.SetColor("_ColourTint", this.material.GetColor("_ColourTint"));
			}
		}
	}

	internal AmbienceType GetAmbience()
	{
		return (this.audioConfig == null) ? AmbienceType.None : this.audioConfig.ambienceType;
	}

	internal SolidAmbienceType GetSolidAmbience()
	{
		return (this.audioConfig == null) ? SolidAmbienceType.None : this.audioConfig.solidAmbienceType;
	}

	internal string GetMiningSound()
	{
		return (this.audioConfig == null) ? string.Empty : this.audioConfig.miningSound;
	}

	internal string GetMiningBreakSound()
	{
		return (this.audioConfig == null) ? string.Empty : this.audioConfig.miningBreakSound;
	}

	internal string GetOreBumpSound()
	{
		return (this.audioConfig == null) ? string.Empty : this.audioConfig.oreBumpSound;
	}

	internal string GetFloorEventAudioCategory()
	{
		return (this.audioConfig == null) ? string.Empty : this.audioConfig.floorEventAudioCategory;
	}

	internal string GetCreatureChewSound()
	{
		return (this.audioConfig == null) ? string.Empty : this.audioConfig.creatureChewSound;
	}

	public string name;

	public SimHashes elementID;

	internal Tag nameTag;

	public Color32 colour;

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

	[EventRef]
	public string fallingStartSound;

	[EventRef]
	public string fallingStopSound;
}
