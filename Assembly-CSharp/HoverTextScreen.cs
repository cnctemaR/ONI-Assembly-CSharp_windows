using System;
using UnityEngine;

public class HoverTextScreen : KScreen
{
	protected override void OnActivate()
	{
		base.OnActivate();
		HoverTextScreen.Instance = this;
		this.drawer = new HoverTextDrawer(this.skin.skin, base.GetComponent<RectTransform>());
	}

	public HoverTextDrawer BeginDrawing()
	{
		Vector2 zero = Vector2.zero;
		Vector2 vector = Input.mousePosition;
		RectTransform rectTransform = base.transform.parent as RectTransform;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, vector, base.transform.parent.GetComponent<Canvas>().worldCamera, out zero);
		zero.x += rectTransform.sizeDelta.x / 2f;
		zero.y -= rectTransform.sizeDelta.y / 2f;
		this.drawer.BeginDrawing(zero);
		return this.drawer;
	}

	private void Update()
	{
		Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if (OverlayScreen.Instance == null || vector.x < 0f || vector.x > Grid.WidthInMeters || vector.y < 0f || vector.y > Grid.HeightInMeters)
		{
			this.drawer.SetEnabled(false);
			return;
		}
		if (PlayerController.Instance.IsUsingDefaultTool())
		{
			KSelectable hover = SelectTool.Instance.hover;
			bool flag = hover != null;
			this.drawer.SetEnabled(flag);
		}
		else
		{
			bool flag2 = PlayerController.Instance.ActiveTool.ShowHoverUI();
			this.drawer.SetEnabled(flag2);
		}
	}

	public Sprite GetSprite(string byName)
	{
		foreach (Sprite sprite in this.HoverIcons)
		{
			if (sprite != null && sprite.name == byName)
			{
				return sprite;
			}
		}
		global::Debug.LogWarning("No icon named " + byName + " was found on HoverTextScreen.prefab", null);
		return null;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.drawer.Cleanup();
	}

	[SerializeField]
	private HoverTextSkin skin;

	public Sprite[] HoverIcons;

	public HoverTextDrawer drawer;

	public static HoverTextScreen Instance;
}
