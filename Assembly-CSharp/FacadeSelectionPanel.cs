using System;
using System.Collections.Generic;
using Database;
using UnityEngine;
using UnityEngine.UI;

public class FacadeSelectionPanel : KMonoBehaviour
{
	public string SelectedBuildingDefID
	{
		get
		{
			return this.selectedBuildingDefID;
		}
	}

	public string SelectedFacade
	{
		get
		{
			return this._selectedFacade;
		}
		set
		{
			if (this._selectedFacade != value)
			{
				this._selectedFacade = value;
				this.RefreshToggles();
				if (this.OnFacadeSelectionChanged != null)
				{
					this.OnFacadeSelectionChanged();
				}
			}
		}
	}

	public void SetBuildingDef(string defID)
	{
		this.ClearToggles();
		this.selectedBuildingDefID = defID;
		this.SelectedFacade = "DEFAULT_FACADE";
		this.RefreshToggles();
		base.gameObject.SetActive(Assets.GetBuildingDef(defID).AvailableFacades.Count != 0);
	}

	private void ClearToggles()
	{
		foreach (KeyValuePair<string, FacadeSelectionPanel.FacadeToggle> keyValuePair in this.activeFacadeToggles)
		{
			this.pooledFacadeToggles.Add(keyValuePair.Value.gameObject);
			keyValuePair.Value.gameObject.SetActive(false);
		}
		this.activeFacadeToggles.Clear();
	}

	private void RefreshToggles()
	{
		this.AddDefaultFacadeToggle();
		foreach (string text in Assets.GetBuildingDef(this.selectedBuildingDefID).AvailableFacades)
		{
			PermitResource permitResource = Db.Get().Permits.TryGet(text);
			if (permitResource != null && permitResource.IsUnlocked())
			{
				this.AddNewToggle(text);
			}
		}
		foreach (KeyValuePair<string, FacadeSelectionPanel.FacadeToggle> keyValuePair in this.activeFacadeToggles)
		{
			keyValuePair.Value.multiToggle.ChangeState((this.SelectedFacade == keyValuePair.Key) ? 1 : 0);
		}
		this.activeFacadeToggles["DEFAULT_FACADE"].gameObject.transform.SetAsFirstSibling();
		this.storeButton.gameObject.transform.SetAsLastSibling();
		LayoutElement component = this.scrollRect.GetComponent<LayoutElement>();
		component.minHeight = (float)(58 * ((this.activeFacadeToggles.Count <= 5) ? 1 : 2));
		component.preferredHeight = component.minHeight;
	}

	private void AddDefaultFacadeToggle()
	{
		this.AddNewToggle("DEFAULT_FACADE");
	}

	private void AddNewToggle(string facadeID)
	{
		if (this.activeFacadeToggles.ContainsKey(facadeID))
		{
			return;
		}
		GameObject gameObject;
		if (this.pooledFacadeToggles.Count > 0)
		{
			gameObject = this.pooledFacadeToggles[0];
			this.pooledFacadeToggles.RemoveAt(0);
		}
		else
		{
			gameObject = Util.KInstantiateUI(this.togglePrefab, this.toggleContainer.gameObject, false);
		}
		FacadeSelectionPanel.FacadeToggle newToggle = new FacadeSelectionPanel.FacadeToggle(facadeID, this.selectedBuildingDefID, gameObject);
		MultiToggle multiToggle = newToggle.multiToggle;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			this.SelectedFacade = newToggle.id;
		}));
		this.activeFacadeToggles.Add(newToggle.id, newToggle);
	}

	[SerializeField]
	private GameObject togglePrefab;

	[SerializeField]
	private RectTransform toggleContainer;

	[SerializeField]
	private LayoutElement scrollRect;

	private Dictionary<string, FacadeSelectionPanel.FacadeToggle> activeFacadeToggles = new Dictionary<string, FacadeSelectionPanel.FacadeToggle>();

	private List<GameObject> pooledFacadeToggles = new List<GameObject>();

	[SerializeField]
	private KButton storeButton;

	public global::System.Action OnFacadeSelectionChanged;

	private string selectedBuildingDefID;

	private string _selectedFacade;

	public const string DEFAULT_FACADE_ID = "DEFAULT_FACADE";

	private struct FacadeToggle
	{
		public FacadeToggle(string facadeID, string buildingPrefabID, GameObject gameObject)
		{
			this.id = facadeID;
			this.gameObject = gameObject;
			gameObject.SetActive(true);
			this.multiToggle = gameObject.GetComponent<MultiToggle>();
			this.multiToggle.onClick = null;
			if (facadeID != "DEFAULT_FACADE")
			{
				BuildingFacadeResource buildingFacadeResource = Db.GetBuildingFacades().Get(facadeID);
				gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("FGImage").sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(buildingFacadeResource.AnimFile), "ui", false, "");
				this.gameObject.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ApplyBoldString(buildingFacadeResource.Name) + "\n\n" + buildingFacadeResource.Description);
				return;
			}
			gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("FGImage").sprite = Def.GetUISprite(buildingPrefabID, "ui", false).first;
			StringEntry stringEntry;
			Strings.TryGet(string.Concat(new string[]
			{
				"STRINGS.BUILDINGS.PREFABS.",
				buildingPrefabID.ToUpperInvariant(),
				".FACADES.DEFAULT_",
				buildingPrefabID.ToUpperInvariant(),
				".NAME"
			}), out stringEntry);
			StringEntry stringEntry2;
			Strings.TryGet(string.Concat(new string[]
			{
				"STRINGS.BUILDINGS.PREFABS.",
				buildingPrefabID.ToUpperInvariant(),
				".FACADES.DEFAULT_",
				buildingPrefabID.ToUpperInvariant(),
				".DESC"
			}), out stringEntry2);
			GameObject prefab = Assets.GetPrefab(buildingPrefabID);
			this.gameObject.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ApplyBoldString((stringEntry != null) ? stringEntry.String : prefab.GetProperName()) + "\n\n" + ((stringEntry2 != null) ? stringEntry2.String : ""));
		}

		public string id { readonly get; set; }

		public GameObject gameObject { readonly get; set; }

		public MultiToggle multiToggle { readonly get; set; }
	}
}
