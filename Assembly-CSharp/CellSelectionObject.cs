using System;
using System.Collections.Generic;
using ProcGen;
using UnityEngine;

public class CellSelectionObject : KMonoBehaviour
{
	public int SelectedCell
	{
		get
		{
			return this.selectedCell;
		}
	}

	public float FlowRate
	{
		get
		{
			return Grid.AccumulatedFlow[this.selectedCell] / 3f;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.mCollider = base.GetComponent<KBoxCollider2D>();
		this.mCollider.size = new Vector2(1.1f, 1.1f);
		this.mSelectable = base.GetComponent<KSelectable>();
		this.SelectedDisplaySprite.transform.localScale = Vector3.one * 0.390625f;
		this.SelectedDisplaySprite.GetComponent<SpriteRenderer>().sprite = this.Sprite_Hover;
		base.Subscribe(Game.Instance.gameObject, 493375141, new Action<object>(this.ForceRefreshUserMenu));
		this.overlayFilterMap.Add(OverlayModes.Oxygen.ID, () => Grid.Element[this.mouseCell].IsGas);
		this.overlayFilterMap.Add(OverlayModes.GasConduits.ID, () => Grid.Element[this.mouseCell].IsGas);
		this.overlayFilterMap.Add(OverlayModes.LiquidConduits.ID, () => Grid.Element[this.mouseCell].IsLiquid);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		this.isAppFocused = focusStatus;
	}

	private void Update()
	{
		if (!this.isAppFocused || SelectTool.Instance == null)
		{
			return;
		}
		if (Game.Instance == null || !Game.Instance.GameStarted())
		{
			return;
		}
		this.SelectedDisplaySprite.SetActive(PlayerController.Instance.IsUsingDefaultTool() && !DebugHandler.HideUI);
		if (SelectTool.Instance.selected != this.mSelectable)
		{
			this.mouseCell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
			if (Grid.IsValidCell(this.mouseCell) && Grid.IsVisible(this.mouseCell))
			{
				bool flag = true;
				foreach (KeyValuePair<HashedString, Func<bool>> keyValuePair in this.overlayFilterMap)
				{
					if (keyValuePair.Value == null)
					{
						global::Debug.LogWarning("Filter value is null");
					}
					else if (OverlayScreen.Instance == null)
					{
						global::Debug.LogWarning("Overlay screen Instance is null");
					}
					else if (OverlayScreen.Instance.GetMode() == keyValuePair.Key)
					{
						flag = false;
						if (base.gameObject.layer != LayerMask.NameToLayer("MaskedOverlay"))
						{
							base.gameObject.layer = LayerMask.NameToLayer("MaskedOverlay");
						}
						if (!keyValuePair.Value())
						{
							this.SelectedDisplaySprite.SetActive(false);
							return;
						}
						break;
					}
				}
				if (flag && base.gameObject.layer != LayerMask.NameToLayer("Default"))
				{
					base.gameObject.layer = LayerMask.NameToLayer("Default");
				}
				Vector3 vector = Grid.CellToPos(this.mouseCell, 0f, 0f, 0f) + this.offset;
				vector.z = this.zDepth;
				base.transform.SetPosition(vector);
				this.mSelectable.SetName(Grid.Element[this.mouseCell].name);
			}
			if (SelectTool.Instance.hover != this.mSelectable)
			{
				this.SelectedDisplaySprite.SetActive(false);
			}
		}
		this.updateTimer += Time.deltaTime;
		if (this.updateTimer >= 0.5f)
		{
			this.updateTimer = 0f;
			if (SelectTool.Instance.selected == this.mSelectable)
			{
				this.UpdateValues();
			}
		}
	}

	public void UpdateValues()
	{
		if (!Grid.IsValidCell(this.selectedCell))
		{
			return;
		}
		this.Mass = Grid.Mass[this.selectedCell];
		this.element = Grid.Element[this.selectedCell];
		this.ElementName = this.element.name;
		this.state = this.element.state;
		this.tags = this.element.GetMaterialCategoryTag();
		this.temperature = Grid.Temperature[this.selectedCell];
		this.diseaseIdx = Grid.DiseaseIdx[this.selectedCell];
		this.diseaseCount = Grid.DiseaseCount[this.selectedCell];
		this.mSelectable.SetName(Grid.Element[this.selectedCell].name);
		DetailsScreen.Instance.Trigger(-1514841199, null);
		this.UpdateStatusItem();
		if (this.element.id == SimHashes.OxyRock)
		{
			this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.OxyRockEmitting, this);
			if (this.FlowRate <= 0f)
			{
				this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.OxyRockBlocked, this);
			}
			else
			{
				this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.OxyRockBlocked, false);
			}
		}
		else
		{
			this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.OxyRockEmitting, false);
			this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.OxyRockBlocked, false);
		}
		if (Game.Instance.GetComponent<EntombedItemVisualizer>().IsEntombedItem(this.selectedCell))
		{
			this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.BuriedItem, this);
		}
		else
		{
			this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.BuriedItem, true);
		}
		bool flag = CellSelectionObject.IsExposedToSpace(this.selectedCell);
		this.mSelectable.ToggleStatusItem(Db.Get().MiscStatusItems.Space, flag, null);
	}

	public static bool IsExposedToSpace(int cell)
	{
		return Game.Instance.world.zoneRenderData.GetSubWorldZoneType(cell) == SubWorld.ZoneType.Space && Grid.Objects[cell, 2] == null;
	}

	private void UpdateStatusItem()
	{
		if (this.element.id == SimHashes.Vacuum || this.element.id == SimHashes.Void)
		{
			this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalCategory, true);
			this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalTemperature, true);
			this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalMass, true);
			this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalDisease, true);
			return;
		}
		if (!this.mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalCategory))
		{
			Func<Element> func = () => this.element;
			this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalCategory, func);
		}
		if (!this.mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalTemperature))
		{
			this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalTemperature, this);
		}
		if (!this.mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalMass))
		{
			this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalMass, this);
		}
		if (!this.mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalDisease))
		{
			this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalDisease, this);
		}
	}

	public void OnObjectSelected(object o)
	{
		this.SelectedDisplaySprite.GetComponent<SpriteRenderer>().sprite = this.Sprite_Hover;
		this.UpdateStatusItem();
		if (SelectTool.Instance.selected == this.mSelectable)
		{
			this.selectedCell = Grid.PosToCell(base.gameObject);
			this.UpdateValues();
			Vector3 vector = Grid.CellToPos(this.selectedCell, 0f, 0f, 0f) + this.offset;
			vector.z = this.zDepthSelected;
			base.transform.SetPosition(vector);
			this.SelectedDisplaySprite.GetComponent<SpriteRenderer>().sprite = this.Sprite_Selected;
		}
	}

	public string MassString()
	{
		return string.Format("{0:0.00}", this.Mass);
	}

	private void ForceRefreshUserMenu(object data)
	{
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	[HideInInspector]
	public CellSelectionObject alternateSelectionObject;

	private float zDepth = -0.5f;

	private float zDepthSelected;

	private KBoxCollider2D mCollider;

	private KSelectable mSelectable;

	private Vector3 offset = new Vector3(0.5f, 0.5f, 0f);

	public GameObject SelectedDisplaySprite;

	public Sprite Sprite_Selected;

	public Sprite Sprite_Hover;

	public int mouseCell;

	private int selectedCell;

	public string ElementName;

	public Element element;

	public Element.State state;

	public float Mass;

	public float temperature;

	public Tag tags;

	public byte diseaseIdx;

	public int diseaseCount;

	private float updateTimer;

	private Dictionary<HashedString, Func<bool>> overlayFilterMap = new Dictionary<HashedString, Func<bool>>();

	private bool isAppFocused = true;
}
