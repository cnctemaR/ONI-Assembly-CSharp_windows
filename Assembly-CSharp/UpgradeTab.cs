using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeTab : TargetScreen
{
	public override void OnSelectTarget(GameObject target)
	{
		this.upgradeTarget = target.GetComponent<Upgradable>();
		if (this.upgradeTarget == null)
		{
			global::Debug.LogError("The target provided does not have an Upgradable component", null);
			return;
		}
		this.InitializeIconDataMap();
		if (this.upgradeMap == null)
		{
			this.upgradeMap = new Dictionary<KToggle, Upgradable.Upgrade>();
		}
		foreach (KeyValuePair<int, Upgradable.Upgrade> keyValuePair in this.upgradeTarget.upgrades)
		{
			bool flag = this.upgradeTarget.availableUpgrades.Contains(keyValuePair.Key);
			bool flag2 = this.upgradeTarget.currentUpgrades.Contains(keyValuePair.Key);
			if (flag || flag2)
			{
				KToggle ktoggle = Util.KInstantiateUI<KToggle>(this.togglePrefab.gameObject, this.availableGrid, false);
				ktoggle.gameObject.SetActive(true);
				ktoggle.transform.GetChild(0).GetComponent<Image>().sprite = this.iconDataMap[keyValuePair.Value.type].icon;
				if (flag)
				{
					this.SetAvailableUpgradeToggle(ktoggle, keyValuePair.Value);
				}
				else if (flag2)
				{
					ktoggle.transform.parent = this.installedGrid.transform;
				}
				this.upgradeMap.Add(ktoggle, keyValuePair.Value);
				this.SetTooltip(ktoggle, flag2);
			}
		}
	}

	public override void OnDeselectTarget(GameObject target)
	{
		this.ClearToggles();
	}

	private void SetAvailableUpgradeToggle(KToggle toggle, Upgradable.Upgrade upgrade)
	{
		toggle.onClick += delegate
		{
			this.OpenUpgradeMenuDetails(toggle);
		};
		GameObject pBar = Util.KInstantiate(this.progressBar.gameObject, toggle.gameObject, null);
		pBar.transform.localPosition = Vector3.one * -24f;
		pBar.GetComponent<ProgressBar>().SetUpdateFunc(() => this.GetProgressForUpgrade(toggle));
		upgrade.startCB = (Upgradable.StartCallback)Delegate.Combine(upgrade.startCB, new Upgradable.StartCallback(delegate
		{
			this.SetStartCB(toggle, pBar);
		}));
		upgrade.completeCB = (Upgradable.CompleteCallback)Delegate.Combine(upgrade.completeCB, new Upgradable.CompleteCallback(delegate
		{
			this.SetUpgradeDoneToggle(toggle, pBar);
			this.SetTooltip(toggle, true);
			toggle.transform.parent = this.installedGrid.transform;
			SideDetailsScreen.Instance.Show(false);
		}));
		upgrade.cancelCB = (Upgradable.CancelCallBack)Delegate.Combine(upgrade.cancelCB, new Upgradable.CancelCallBack(delegate
		{
			this.SetUpgradeDoneToggle(toggle, pBar);
			this.SetTooltip(toggle, false);
			toggle.onClick += delegate
			{
				this.OpenUpgradeMenuDetails(toggle);
			};
		}));
		pBar.SetActive(this.upgradeTarget.IsCurrentUpgrade(upgrade));
	}

	private void SetStartCB(KToggle toggle, GameObject pBar)
	{
		toggle.ClearOnClick();
		toggle.ClearPointerCallbacks();
		toggle.GetComponent<ToolTip>().toolTip = "Cancel Upgrade";
		pBar.SetActive(true);
		toggle.onClick += delegate
		{
			this.upgradeTarget.CancelCurrentUpgrade();
		};
		toggle.onPointerEnter += delegate
		{
			toggle.transform.GetChild(1).gameObject.SetActive(true);
		};
		toggle.onPointerExit += delegate
		{
			toggle.transform.GetChild(1).gameObject.SetActive(false);
		};
	}

	private void SetUpgradeDoneToggle(KToggle toggle, GameObject pBar)
	{
		pBar.SetActive(false);
		toggle.ClearOnClick();
		toggle.ClearPointerCallbacks();
		toggle.transform.GetChild(1).gameObject.SetActive(false);
	}

	private void SetTooltip(KToggle toggle, bool isInstalled = false)
	{
		string text = this.iconDataMap[this.upgradeMap[toggle].type].description;
		if (isInstalled)
		{
			text += " (installed)";
		}
		toggle.GetComponent<ToolTip>().toolTip = text;
	}

	private void ClearToggles()
	{
		if (this.upgradeMap == null)
		{
			return;
		}
		foreach (KeyValuePair<KToggle, Upgradable.Upgrade> keyValuePair in this.upgradeMap)
		{
			keyValuePair.Value.ClearCallbacks();
			global::UnityEngine.Object.Destroy(keyValuePair.Key.gameObject);
		}
		this.upgradeMap.Clear();
	}

	private void OpenUpgradeMenuDetails(KToggle toggle)
	{
		float num = -DetailsScreen.Instance.GetComponent<RectTransform>().rect.width;
		KeyValuePair<Upgradable.Upgrade, Upgradable> keyValuePair = new KeyValuePair<Upgradable.Upgrade, Upgradable>(this.upgradeMap[toggle], this.upgradeTarget);
		object obj = keyValuePair;
		SideDetailsScreen.Instance.SetScreen("UpgradeDetails", obj, num);
	}

	private float GetProgressForUpgrade(KToggle toggle)
	{
		return this.upgradeMap[toggle].progress;
	}

	private void InitializeIconDataMap()
	{
		if (this.iconDataMap != null)
		{
			return;
		}
		this.iconDataMap = new Dictionary<Upgradable.Upgrade.Target, UpgradeTab.UpgradeIconData>();
		this.icons.ForEach(delegate(UpgradeTab.UpgradeIconData ic)
		{
			this.iconDataMap.Add(ic.type, ic);
		});
	}

	[Header("UI Elements")]
	[SerializeField]
	private GameObject availableGrid;

	[SerializeField]
	private GameObject installedGrid;

	[SerializeField]
	private KToggle togglePrefab;

	[SerializeField]
	private ProgressBar progressBar;

	[SerializeField]
	[Header("References")]
	private List<UpgradeTab.UpgradeIconData> icons;

	private Upgradable upgradeTarget;

	private Dictionary<KToggle, Upgradable.Upgrade> upgradeMap;

	private Dictionary<Upgradable.Upgrade.Target, UpgradeTab.UpgradeIconData> iconDataMap;

	[Serializable]
	public struct UpgradeIconData
	{
		public Upgradable.Upgrade.Target type;

		public Sprite icon;

		public string description;
	}
}
