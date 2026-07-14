using System;
using STRINGS;
using UnityEngine;

public class BackwallSelectionObject : KMonoBehaviour, ICellSelectionProxy
{
	public static BackwallSelectionObject Instance
	{
		get
		{
			return BackwallSelectionObject.instance;
		}
	}

	public int SelectedCell
	{
		get
		{
			return this.selectedCell;
		}
	}

	Element ICellSelectionProxy.Element
	{
		get
		{
			return this.element;
		}
	}

	protected override void OnPrefabInit()
	{
		BackwallSelectionObject.instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.mSelectable = base.GetComponent<KSelectable>();
		this.mSelectable.IsSelectable = true;
		this.mCollider = base.GetComponent<KBoxCollider2D>();
		this.mCollider.size = new Vector2(1.1f, 1.1f);
		this.hoverCursor = BackwallSelectionObject.CreateHoverCursor(base.transform);
		base.Subscribe(Game.Instance.gameObject, -1503271301, new Action<object>(this.OnObjectSelected));
	}

	private static GameObject CreateHoverCursor(Transform parent)
	{
		GameObject gameObject = new GameObject("Backwall Selection Hover Cursor");
		gameObject.transform.SetParent(parent, false);
		gameObject.transform.localPosition = new Vector3(0f, 0f, -10f);
		gameObject.transform.localScale = new Vector3(0.39f, 0.39f, 1f);
		SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = Assets.GetSprite("cursorIcon");
		spriteRenderer.sortingOrder = 1;
		return gameObject;
	}

	protected override void OnCleanUp()
	{
		BackwallSelectionObject.instance = null;
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
		if (!PlayerController.Instance.IsUsingDefaultTool())
		{
			return;
		}
		if (SelectTool.Instance.selected == this.mSelectable)
		{
			this.hoverCursor.SetActive(false);
			this.updateTimer += Time.deltaTime;
			if (this.updateTimer >= 0.5f)
			{
				this.updateTimer = 0f;
				this.UpdateValues();
				return;
			}
		}
		else
		{
			int num = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
			bool flag = Grid.IsValidCell(num) && Grid.IsVisible(num) && BackwallManager.HasBackwall(num);
			this.mCollider.enabled = flag;
			bool flag2 = SelectTool.Instance.hover == this.mSelectable;
			this.hoverCursor.SetActive(flag && flag2);
			if (flag)
			{
				Vector3 vector = Grid.CellToPos(num, 0f, 0f, 0f) + BackwallSelectionObject.offset;
				vector.z = this.zDepth;
				base.transform.SetPosition(vector);
				this.mSelectable.SetName(BackwallManager.At(num).Element.nameUpperCase + " " + UI.TOOLS.GENERIC.NATURAL_BACKWALL_LABEL);
			}
		}
	}

	public void OnObjectSelected(object o)
	{
		if (SelectTool.Instance.selected != this.mSelectable)
		{
			return;
		}
		this.selectedCell = Grid.PosToCell(base.gameObject);
		this.updateTimer = 0f;
		this.UpdateValues();
	}

	public void UpdateValues()
	{
		GameObject gameObject = Grid.Objects[this.selectedCell, 2];
		if (BackwallManager.HasBackwall(this.selectedCell))
		{
			this.element = BackwallManager.At(this.SelectedCell).Element;
			this.Mass = BackwallManager.At(this.SelectedCell).Mass;
			this.temperature = BackwallManager.At(this.SelectedCell).Temperature;
		}
		else
		{
			if (!(gameObject != null))
			{
				return;
			}
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			if (component == null)
			{
				return;
			}
			this.element = component.Element;
			this.Mass = component.Mass;
			this.temperature = component.Temperature;
		}
		this.mSelectable.SetName(this.element.name + " " + UI.TOOLS.GENERIC.NATURAL_BACKWALL_LABEL_TITLECASE);
		if (!this.mSelectable.HasStatusItem(Db.Get().MiscStatusItems.BackwallMass))
		{
			this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.BackwallMass, this);
		}
		if (!this.mSelectable.HasStatusItem(Db.Get().MiscStatusItems.BackwallTemperature))
		{
			this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.BackwallTemperature, this);
		}
	}

	public static bool IsBackwallSelectionObject(GameObject go)
	{
		return BackwallSelectionObject.instance != null && go == BackwallSelectionObject.instance.gameObject;
	}

	private static BackwallSelectionObject instance;

	private KSelectable mSelectable;

	private KBoxCollider2D mCollider;

	private GameObject hoverCursor;

	private int selectedCell;

	private float updateTimer;

	public Element element;

	public float Mass;

	public float temperature;

	private static readonly Vector3 offset = new Vector3(0.5f, 0.5f, 0f);

	private float zDepth = Grid.GetLayerZ(Grid.SceneLayer.WorldSelection) + -0.5f;

	private bool isAppFocused = true;
}
