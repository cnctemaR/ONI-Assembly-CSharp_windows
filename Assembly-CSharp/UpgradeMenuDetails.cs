using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuDetails : SideTargetScreen
{
	public override void SetTarget(object obj)
	{
		if (obj == null)
		{
			return;
		}
		if (this.iconMap == null)
		{
			this.InitIconMap();
		}
		KeyValuePair<Upgradable.Upgrade, Upgradable> keyValuePair = (KeyValuePair<Upgradable.Upgrade, Upgradable>)obj;
		this.upgrade = keyValuePair.Key;
		this.upgradable = keyValuePair.Value;
		this.def = this.upgradable.GetBuildingDef;
		this.VerifyUpgradeType(this.upgrade.type);
		this.image.sprite = this.def.GetUISprite("ui");
		this.image.GetComponent<ToolTip>().toolTip = this.def.Flavor;
		if (this.upgrade.type != Upgradable.Upgrade.Target.None && this.iconMap.ContainsKey(this.upgrade.type))
		{
			this.upgradeIcon.sprite = this.iconMap[this.upgrade.type].icon;
			this.upgradeIcon.GetComponent<ToolTip>().toolTip = this.iconMap[this.upgrade.type].description;
		}
		if (this.matSelectionPanel == null)
		{
			this.matSelectionPanel = Util.KInstantiateUI<MaterialSelectionPanel>(this.matSelectionPanelPrefab.gameObject, base.gameObject, false);
		}
		this.SetBottomContent();
		SideDetailsScreen.Instance.SetTitle("Upgrade " + this.upgradable.GetBuildingDef.Name);
	}

	private void SetBottomContent()
	{
		if (this.upgradable.IsCurrentUpgrade(this.upgrade))
		{
			this.matSelectionPanel.gameObject.SetActive(false);
			this.cancelButton.gameObject.SetActive(true);
			this.cancelButton.ClearOnClick();
			this.cancelButton.onClick += delegate
			{
				this.upgradable.CancelCurrentUpgrade();
				this.SetBottomContent();
			};
		}
		else
		{
			this.cancelButton.gameObject.SetActive(false);
			this.matSelectionPanel.gameObject.SetActive(true);
			global::Debug.Break();
			this.matSelectionPanel.ConfigureScreen(null);
			this.matSelectionPanel.ClearSelectActions();
			this.matSelectionPanel.AddSelectAction(delegate
			{
				this.upgradable.DoUpgrade(this.upgrade, new Recipe.Ingredient(this.matSelectionPanel.CurrentSelectedElement.tag, this.upgrade.ingredients[0].amount));
				SideDetailsScreen.Instance.Show(false);
			});
		}
	}

	private void VerifyUpgradeType(Upgradable.Upgrade.Target type)
	{
		if (type != Upgradable.Upgrade.Target.EnergyConsumption)
		{
			if (type != Upgradable.Upgrade.Target.EnergyGeneration)
			{
				if (type != Upgradable.Upgrade.Target.MassConsumption)
				{
					if (type != Upgradable.Upgrade.Target.MassGeneration)
					{
						if (type != Upgradable.Upgrade.Target.Capacity)
						{
							if (type != Upgradable.Upgrade.Target.ExecutionTime)
							{
							}
						}
						else
						{
							Generator component = this.upgradable.GetComponent<Generator>();
							if (component != null)
							{
								this.previousAmountLabel.text = component.Capacity.ToString("0.0");
								this.afterAmountLabel.text = (component.BaseCapacity * this.upgradable.GetNextModifierForTarget(type)).ToString("0.0");
							}
							else
							{
								global::Debug.LogError("Something went wrong! We're trying to update a component that doesn't exist in the selected target!", null);
							}
						}
					}
					else
					{
						BuildingElementEmitter component2 = this.upgradable.GetComponent<BuildingElementEmitter>();
						if (component2 != null)
						{
							this.previousAmountLabel.text = component2.EmitRate.ToString("0.0");
							this.afterAmountLabel.text = (component2.EmitRate * this.upgradable.GetNextModifierForTarget(type)).ToString("0.0");
						}
						else
						{
							global::Debug.LogError("Something went wrong! We're trying to update a component that doesn't exist in the selected target!", null);
						}
					}
				}
			}
			else
			{
				Generator component3 = this.upgradable.GetComponent<Generator>();
				if (component3 != null)
				{
					this.previousAmountLabel.text = component3.WattageRating.ToString("0.0") + "W";
					this.afterAmountLabel.text = (component3.BaseWattageRating * BuildingDef.GetEnergyEfficiency(null, this.upgradable.GetNextModifierForTarget(type))).ToString("0.0") + "W";
				}
				else
				{
					global::Debug.LogError("Something went wrong! We're trying to update a component that doesn't exist in the selected target!", null);
				}
			}
		}
		else
		{
			EnergyConsumer component4 = this.upgradable.GetComponent<EnergyConsumer>();
			if (component4 != null)
			{
				this.previousAmountLabel.text = component4.WattsNeededWhenActive.ToString("0.0") + "W";
				this.afterAmountLabel.text = (component4.BaseWattsNeededWhenActive * this.upgradable.GetNextModifierForTarget(type)).ToString("0.0") + "W";
			}
			else
			{
				global::Debug.LogError("Something went wrong! We're trying to update a component that doesn't exist in the selected target!", null);
			}
		}
	}

	private void InitIconMap()
	{
		this.iconMap = new Dictionary<Upgradable.Upgrade.Target, UpgradeMenuDetails.IconTypes>();
		this.icons.ForEach(delegate(UpgradeMenuDetails.IconTypes ic)
		{
			this.iconMap.Add(ic.type, ic);
		});
	}

	private const string strFormat = "0.0";

	[SerializeField]
	private List<UpgradeMenuDetails.IconTypes> icons;

	private Dictionary<Upgradable.Upgrade.Target, UpgradeMenuDetails.IconTypes> iconMap;

	[SerializeField]
	private Image image;

	[SerializeField]
	private Image upgradeIcon;

	[SerializeField]
	private LocText previousAmountLabel;

	[SerializeField]
	private LocText afterAmountLabel;

	[SerializeField]
	private LocText details;

	[SerializeField]
	private KButton cancelButton;

	[SerializeField]
	private MaterialSelectionPanel matSelectionPanelPrefab;

	private MaterialSelectionPanel matSelectionPanel;

	private Upgradable upgradable;

	private Upgradable.Upgrade upgrade;

	private BuildingDef def;

	[Serializable]
	public struct IconTypes
	{
		public Upgradable.Upgrade.Target type;

		public Sprite icon;

		public string description;
	}
}
