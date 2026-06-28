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
		this.Subscribe(493375141, new EventSystem.EventHandler(this.OnRefreshUserMenu));
		base.Subscribe(Game.Instance.gameObject, 493375141, new EventSystem.EventHandler(this.ForceRefreshUserMenu));
		base.Subscribe(WaterBodyProbe.Instance.gameObject, -263784810, new EventSystem.EventHandler(this.ForceRefreshUserMenu));
		Func<Element> func = () => this.element;
		this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalCategory, func);
		this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalTemperature, this);
		this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalMass, this);
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
					if (OverlayScreen.Instance.GetMode() == keyValuePair.Key)
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
		if (this.element.id == SimHashes.OxyRock)
		{
			this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.OxyRockEmitting, this);
			if (this.FlowRate <= 0f)
			{
				this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.OxyRockBlocked, this);
			}
			else
			{
				this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.OxyRockBlocked);
			}
		}
		else
		{
			this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.OxyRockEmitting);
			this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.OxyRockBlocked);
		}
		if (Game.Instance.GetComponent<EntombedItemVisualizer>().IsEntombedItem(this.selectedCell))
		{
			this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.BuriedItem, this);
		}
		else
		{
			this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.BuriedItem);
		}
	}

	public void OnObjectSelected(object o)
	{
		this.SelectedDisplaySprite.GetComponent<SpriteRenderer>().sprite = this.Sprite_Hover;
		if (SelectTool.Instance.selected == this.mSelectable)
		{
			this.selectedCell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(Input.mousePosition));
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
			this.userMenu.AddButton(buttonInfo);
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
