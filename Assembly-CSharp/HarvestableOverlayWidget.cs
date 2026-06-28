using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;
using UnityEngine.UI;

public class HarvestableOverlayWidget : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.condition_sprites.Add(WiltCondition.Condition.AtmosphereElement, this.sprite_atmosphere);
		this.condition_sprites.Add(WiltCondition.Condition.Darkness, this.sprite_light);
		this.condition_sprites.Add(WiltCondition.Condition.Drowning, this.sprite_liquid);
		this.condition_sprites.Add(WiltCondition.Condition.DryingOut, this.sprite_liquid);
		this.condition_sprites.Add(WiltCondition.Condition.Fertilized, this.sprite_resource);
		this.condition_sprites.Add(WiltCondition.Condition.IlluminationComfort, this.sprite_light);
		this.condition_sprites.Add(WiltCondition.Condition.Irrigation, this.sprite_liquid);
		this.condition_sprites.Add(WiltCondition.Condition.Pressure, this.sprite_pressure);
		this.condition_sprites.Add(WiltCondition.Condition.Temperature, this.sprite_temperature);
		this.condition_sprites.Add(WiltCondition.Condition.Receptacle, this.sprite_receptacle);
		for (int i = 0; i < this.horizontal_containers.Length; i++)
		{
			GameObject gameObject = Util.KInstantiateUI(this.horizontal_container_prefab, this.vertical_container, false);
			this.horizontal_containers[i] = gameObject;
		}
		for (int j = 0; j < 11; j++)
		{
			if (this.condition_sprites.ContainsKey((WiltCondition.Condition)j))
			{
				GameObject gameObject2 = Util.KInstantiateUI(this.icon_gameobject_prefab, base.gameObject, false);
				gameObject2.GetComponent<Image>().sprite = this.condition_sprites[(WiltCondition.Condition)j];
				this.condition_icons.Add((WiltCondition.Condition)j, gameObject2);
			}
		}
	}

	public void Refresh(Harvestable target_harvestable)
	{
		Image image = this.bar.GetComponent<HierarchyReferences>().GetReference("Fill") as Image;
		AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(target_harvestable);
		if (amountInstance != null)
		{
			float num = amountInstance.value / amountInstance.GetMax();
			image.rectTransform.offsetMin = new Vector2(image.rectTransform.offsetMin.x, 3f);
			if (this.bar.activeSelf != !target_harvestable.CanBeHavested)
			{
				this.bar.SetActive(!target_harvestable.CanBeHavested);
			}
			float num2 = ((!target_harvestable.CanBeHavested) ? (19f - 19f * num + 3f) : 3f);
			image.rectTransform.offsetMax = new Vector2(image.rectTransform.offsetMax.x, -num2);
		}
		else if (this.bar.activeSelf)
		{
			this.bar.SetActive(false);
		}
		WiltCondition component = target_harvestable.GetComponent<WiltCondition>();
		if (component != null)
		{
			for (int i = 0; i < this.horizontal_containers.Length; i++)
			{
				this.horizontal_containers[i].SetActive(false);
			}
			foreach (KeyValuePair<WiltCondition.Condition, GameObject> keyValuePair in this.condition_icons)
			{
				keyValuePair.Value.SetActive(false);
			}
			if (component.IsWilting())
			{
				this.vertical_container.SetActive(true);
				image.color = HarvestableOverlayWidget.wilting_color;
				List<WiltCondition.Condition> list = component.CurrentWiltSources();
				if (list.Count > 0)
				{
					for (int j = 0; j < list.Count; j++)
					{
						if (this.condition_icons.ContainsKey(list[j]))
						{
							this.condition_icons[list[j]].SetActive(true);
							this.horizontal_containers[j / 2].SetActive(true);
							this.condition_icons[list[j]].transform.SetParent(this.horizontal_containers[j / 2].transform);
						}
					}
				}
			}
			else
			{
				this.vertical_container.SetActive(false);
				image.color = HarvestableOverlayWidget.growing_color;
			}
		}
		else
		{
			image.color = HarvestableOverlayWidget.growing_color;
			this.vertical_container.SetActive(false);
		}
	}

	[SerializeField]
	private GameObject vertical_container;

	[SerializeField]
	private GameObject bar;

	private const int icons_per_row = 2;

	private const float bar_fill_range = 19f;

	private const float bar_fill_offset = 3f;

	private static Color growing_color = new Color(0.9843137f, 0.6901961f, 0.23137255f, 1f);

	private static Color wilting_color = new Color(0.5647059f, 0.5647059f, 0.5647059f, 1f);

	[SerializeField]
	private Sprite sprite_liquid;

	[SerializeField]
	private Sprite sprite_atmosphere;

	[SerializeField]
	private Sprite sprite_pressure;

	[SerializeField]
	private Sprite sprite_temperature;

	[SerializeField]
	private Sprite sprite_resource;

	[SerializeField]
	private Sprite sprite_light;

	[SerializeField]
	private Sprite sprite_receptacle;

	[SerializeField]
	private GameObject horizontal_container_prefab;

	private GameObject[] horizontal_containers = new GameObject[5];

	[SerializeField]
	private GameObject icon_gameobject_prefab;

	private Dictionary<WiltCondition.Condition, GameObject> condition_icons = new Dictionary<WiltCondition.Condition, GameObject>();

	private Dictionary<WiltCondition.Condition, Sprite> condition_sprites = new Dictionary<WiltCondition.Condition, Sprite>();
}
