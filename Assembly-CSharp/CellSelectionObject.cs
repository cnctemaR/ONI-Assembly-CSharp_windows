using System;
using System.Collections.Generic;
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
		this.mCollider = base.GetComponent<BoxCollider2D>();
		this.mCollider.size = new Vector2(1.1f, 1.1f);
		this.mSelectable = base.GetComponent<KSelectable>();
		this.SelectedDisplaySprite.transform.localScale = Vector3.one * 0.390625f;
		this.SelectedDisplaySprite.GetComponent<SpriteRenderer>().sprite = this.Sprite_Hover;
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		base.Subscribe(Game.Instance.gameObject, 493375141, new Action<object>(this.ForceRefreshUserMenu));
		base.Subscribe(WaterBodyProbe.Instance.gameObject, -263784810, new Action<object>(this.ForceRefreshUserMenu));
		this.overlayFilterMap.Add(SimViewMode.OxygenMap, () => Grid.Element[this.mouseCell].IsGas);
		this.overlayFilterMap.Add(SimViewMode.GasVentMap, () => Grid.Element[this.mouseCell].IsGas);
		this.overlayFilterMap.Add(SimViewMode.LiquidVentMap, () => Grid.Element[this.mouseCell].IsLiquid);
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
		if (!this.isAppFocused)
		{
			return;
		}
		this.SelectedDisplaySprite.SetActive(PlayerController.Instance.IsUsingDefaultTool() && !DebugHandler.HideUI);
		if (SelectTool.Instance.selected != this.mSelectable)
		{
			this.mouseCell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(Input.mousePosition));
			if (Grid.IsValidCell(this.mouseCell) && (Grid.Visible[this.mouseCell] > 0 || DebugHandler.FreeCameraMode))
			{
				bool flag = true;
				foreach (KeyValuePair<SimViewMode, Func<bool>> keyValuePair in this.overlayFilterMap)
				{
					if (keyValuePair.Value == null)
					{
						Debug.LogWarning("Filter value is null");
					}
					else if (OverlayScreen.Instance == null)
					{
						Debug.LogWarning("Overlay screen Instance is null");
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
				if (this.previousHoverCell != this.mouseCell)
				{
					if (this.hoverTextScreen != null)
					{
						this.hoverTextScreen.ResetHoverDelay();
					}
					else
					{
						this.hoverTextScreen = global::UnityEngine.Object.FindObjectOfType<HoverTextScreen>();
					}
					this.previousHoverCell = this.mouseCell;
				}
				Vector3 vector = Grid.CellToPos(this.mouseCell, 0f, 0f, 0f) + this.offset;
				vector.z = this.zDepth;
				this.transform.SetPosition(vector);
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
		this.Mass = Grid.Cell[this.selectedCell].mass;
		this.element = Grid.Element[this.selectedCell];
		this.ElementName = this.element.name;
		this.state = this.element.state;
		this.tags = this.element.GetMaterialCategoryTag();
		this.temperature = Grid.Cell[this.selectedCell].temperature;
		this.mSelectable.SetName(Grid.Element[this.selectedCell].name);
		if (SimpleInfoScreen.Instance != null)
		{
			SimpleInfoScreen.Instance.Refresh(true);
		}
		this.UpdateMassStatusItem();
		this.UpdateTemperatureStatusItem();
		this.UpdateCategoryStatusItem();
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
	}

	private void UpdateCategoryStatusItem()
	{
		if (this.element.id == SimHashes.Vacuum || this.element.id == SimHashes.Void)
		{
			this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalCategory, true);
		}
		else if (!this.mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalCategory))
		{
			Func<Element> func = () => this.element;
			this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalCategory, func);
		}
	}

	private void UpdateTemperatureStatusItem()
	{
		if (this.element.id == SimHashes.Vacuum || this.element.id == SimHashes.Void)
		{
			this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalTemperature, true);
		}
		else if (!this.mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalTemperature))
		{
			this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalTemperature, this);
		}
	}

	private void UpdateMassStatusItem()
	{
		if (this.element.id == SimHashes.Vacuum || this.element.id == SimHashes.Void)
		{
			this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalMass, true);
		}
		else if (!this.mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalMass))
		{
			this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalMass, this);
		}
	}

	public void OnObjectSelected(object o)
	{
		this.SelectedDisplaySprite.GetComponent<SpriteRenderer>().sprite = this.Sprite_Hover;
		this.UpdateMassStatusItem();
		this.UpdateCategoryStatusItem();
		this.UpdateTemperatureStatusItem();
		if (SelectTool.Instance.selected == this.mSelectable)
		{
			this.selectedCell = Grid.PosToCell(base.gameObject);
			this.UpdateValues();
			Vector3 vector = Grid.CellToPos(this.selectedCell, 0f, 0f, 0f) + this.offset;
			vector.z = this.zDepthSelected;
			this.transform.SetPosition(vector);
			this.SelectedDisplaySprite.GetComponent<SpriteRenderer>().sprite = this.Sprite_Selected;
		}
	}

	public string MassString()
	{
		return string.Format("{0:0.00}", this.Mass);
	}

	private void ForceRefreshUserMenu(object data)
	{
		this.userMenu.Refresh();
	}

	public virtual void OnRefreshUserMenu(object data)
	{
		this.cellButtons.Clear();
		if (SelectTool.Instance.selected == this.mSelectable && Grid.IsSubstantialLiquid(this.selectedCell, 0.35f) && WaterBodyProbe.Instance.GetBodyIfKnown(this.selectedCell))
		{
			BodyOfWater bodyIfKnown = WaterBodyProbe.Instance.GetBodyIfKnown(this.selectedCell);
			if (bodyIfKnown != null)
			{
			}
		}
		foreach (KIconButtonMenu.ButtonInfo buttonInfo in this.cellButtons)
		{
			this.userMenu.AddButton(buttonInfo, 1f);
		}
	}

	[HideInInspector]
	public CellSelectionObject alternateSelectionObject;

	private float zDepth = -0.5f;

	private float zDepthSelected;

	private BoxCollider2D mCollider;

	private KSelectable mSelectable;

	private Vector3 offset = new Vector3(0.5f, 0.5f, 0f);

	public GameObject SelectedDisplaySprite;

	public Sprite Sprite_Selected;

	public Sprite Sprite_Hover;

	public int mouseCell;

	public int previousHoverCell;

	private HoverTextScreen hoverTextScreen;

	[MyCmpAdd]
	private UserMenu userMenu;

	private int selectedCell;

	public string ElementName;

	public Element element;

	public Element.State state;

	public float Mass;

	public float temperature;

	public Tag tags;

	private float updateTimer;

	private List<KIconButtonMenu.ButtonInfo> cellButtons = new List<KIconButtonMenu.ButtonInfo>();

	private Dictionary<SimViewMode, Func<bool>> overlayFilterMap = new Dictionary<SimViewMode, Func<bool>>();

	private bool isAppFocused = true;
}
