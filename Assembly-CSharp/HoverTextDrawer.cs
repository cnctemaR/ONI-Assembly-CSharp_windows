using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverTextDrawer
{
	public HoverTextDrawer(HoverTextDrawer.Skin skin, RectTransform parent)
	{
		this.shadowBars = new HoverTextDrawer.Pool<Image>(skin.shadowBarWidget.gameObject, parent);
		this.selectBorders = new HoverTextDrawer.Pool<Image>(skin.selectBorderWidget.gameObject, parent);
		this.textWidgets = new HoverTextDrawer.Pool<LocText>(skin.textWidget.gameObject, parent);
		this.iconWidgets = new HoverTextDrawer.Pool<Image>(skin.iconWidget.gameObject, parent);
		this.skin = skin;
	}

	public void SetEnabled(bool enabled)
	{
		this.shadowBars.SetEnabled(enabled);
		this.textWidgets.SetEnabled(enabled);
		this.iconWidgets.SetEnabled(enabled);
		this.selectBorders.SetEnabled(enabled);
	}

	public void BeginDrawing(Vector2 root_pos)
	{
		this.BeginSample("BeginDrawing");
		this.rootPos = root_pos + this.skin.baseOffset;
		if (this.skin.enableDebugOffset)
		{
			this.rootPos += this.skin.debugOffset;
		}
		this.currentPos = this.rootPos;
		this.textWidgets.BeginDrawing();
		this.iconWidgets.BeginDrawing();
		this.shadowBars.BeginDrawing();
		this.selectBorders.BeginDrawing();
		this.firstShadowBar = true;
		this.minLineHeight = 0;
		this.EndSample();
	}

	public void EndDrawing()
	{
		this.BeginSample("EndDrawing");
		this.shadowBars.EndDrawing();
		this.iconWidgets.EndDrawing();
		this.textWidgets.EndDrawing();
		this.selectBorders.EndDrawing();
		this.EndSample();
	}

	public void DrawText(string text, TextStyleSetting style)
	{
		if (!this.skin.drawWidgets)
		{
			return;
		}
		this.BeginSample("DrawText");
		LocText widget = this.textWidgets.Draw(this.currentPos).widget;
		if (widget.textStyleSetting != style)
		{
			widget.textStyleSetting = style;
			widget.ApplySettings();
		}
		if (widget.text != text)
		{
			widget.text = text;
			widget.KForceUpdateDirty();
		}
		Vector2 vector = widget.bounds.extents;
		this.currentPos.x = this.currentPos.x + vector.x * 2f;
		this.maxShadowX = Mathf.Max(this.currentPos.x, this.maxShadowX);
		this.minLineHeight = (int)Mathf.Max((float)this.minLineHeight, vector.y * 2f);
		this.EndSample();
	}

	public void AddIndent(int width = 36)
	{
		if (!this.skin.drawWidgets)
		{
			return;
		}
		this.currentPos.x = this.currentPos.x + (float)width;
	}

	public void NewLine(int min_height = 26)
	{
		if (!this.skin.drawWidgets)
		{
			return;
		}
		this.currentPos.y = this.currentPos.y - (float)Math.Max(min_height, this.minLineHeight);
		this.currentPos.x = this.rootPos.x;
		this.minLineHeight = 0;
	}

	public void DrawIcon(Sprite icon, int min_width = 18)
	{
		this.DrawIcon(icon, Color.white, min_width, 2);
	}

	public void DrawIcon(Sprite icon, Color color, int image_size = 18, int horizontal_spacing = 2)
	{
		if (!this.skin.drawWidgets)
		{
			return;
		}
		this.BeginSample("DrawIcon");
		this.AddIndent(horizontal_spacing);
		HoverTextDrawer.Pool<Image>.Entry entry = this.iconWidgets.Draw(this.currentPos + this.skin.shadowImageOffset);
		entry.widget.sprite = icon;
		entry.widget.color = this.skin.shadowImageColor;
		entry.rect.sizeDelta = new Vector2((float)image_size, (float)image_size);
		HoverTextDrawer.Pool<Image>.Entry entry2 = this.iconWidgets.Draw(this.currentPos);
		entry2.widget.sprite = icon;
		entry2.widget.color = color;
		entry2.rect.sizeDelta = new Vector2((float)image_size, (float)image_size);
		this.AddIndent(horizontal_spacing);
		this.currentPos.x = this.currentPos.x + (float)image_size;
		this.maxShadowX = Mathf.Max(this.currentPos.x, this.maxShadowX);
		this.EndSample();
	}

	public void BeginShadowBar(bool selected = false)
	{
		if (!this.skin.drawWidgets)
		{
			return;
		}
		if (this.firstShadowBar)
		{
			this.firstShadowBar = false;
		}
		else
		{
			this.NewLine(22);
		}
		this.isShadowBarSelected = selected;
		this.shadowStartPos = this.currentPos;
		this.maxShadowX = this.rootPos.x;
	}

	public void EndShadowBar()
	{
		if (!this.skin.drawWidgets)
		{
			return;
		}
		this.NewLine(22);
		HoverTextDrawer.Pool<Image>.Entry entry = this.shadowBars.Draw(this.currentPos);
		entry.rect.anchoredPosition = this.shadowStartPos + new Vector2(-this.skin.shadowBarBorder.x, this.skin.shadowBarBorder.y);
		entry.rect.sizeDelta = new Vector2(this.maxShadowX - this.rootPos.x + this.skin.shadowBarBorder.x * 2f, this.shadowStartPos.y - this.currentPos.y + this.skin.shadowBarBorder.y * 2f);
		if (this.isShadowBarSelected)
		{
			HoverTextDrawer.Pool<Image>.Entry entry2 = this.selectBorders.Draw(this.currentPos);
			entry2.rect.anchoredPosition = this.shadowStartPos + new Vector2(-this.skin.shadowBarBorder.x - this.skin.selectBorder.x, this.skin.shadowBarBorder.y + this.skin.selectBorder.y);
			entry2.rect.sizeDelta = new Vector2(this.maxShadowX - this.rootPos.x + this.skin.shadowBarBorder.x * 2f + this.skin.selectBorder.x * 2f, this.shadowStartPos.y - this.currentPos.y + this.skin.shadowBarBorder.y * 2f + this.skin.selectBorder.y * 2f);
		}
	}

	public void Cleanup()
	{
		this.shadowBars.Cleanup();
		this.textWidgets.Cleanup();
		this.iconWidgets.Cleanup();
	}

	private void BeginSample(string section_name)
	{
		if (this.skin.enableProfiling)
		{
		}
	}

	private void EndSample()
	{
		if (this.skin.enableProfiling)
		{
		}
	}

	public HoverTextDrawer.Skin skin;

	private Vector2 currentPos;

	private Vector2 rootPos;

	private Vector2 shadowStartPos;

	private float maxShadowX;

	private bool firstShadowBar;

	private bool isShadowBarSelected;

	private int minLineHeight;

	private HoverTextDrawer.Pool<LocText> textWidgets;

	private HoverTextDrawer.Pool<Image> iconWidgets;

	private HoverTextDrawer.Pool<Image> shadowBars;

	private HoverTextDrawer.Pool<Image> selectBorders;

	[Serializable]
	public class Skin
	{
		public Vector2 baseOffset;

		public LocText textWidget;

		public Image iconWidget;

		public Vector2 shadowImageOffset;

		public Color shadowImageColor;

		public Image shadowBarWidget;

		public Image selectBorderWidget;

		public Vector2 shadowBarBorder;

		public Vector2 selectBorder;

		public bool drawWidgets;

		public bool enableProfiling;

		public bool enableDebugOffset;

		public Vector2 debugOffset;
	}

	private class Pool<WidgetType> where WidgetType : MonoBehaviour
	{
		public Pool(GameObject prefab, RectTransform master_root)
		{
			this.prefab = prefab;
			GameObject gameObject = new GameObject(typeof(WidgetType).Name);
			this.root = gameObject.AddComponent<RectTransform>();
			this.root.SetParent(master_root);
			this.root.anchoredPosition = Vector2.zero;
			this.root.anchorMin = Vector2.zero;
			this.root.anchorMax = Vector2.one;
			this.root.sizeDelta = Vector2.zero;
			gameObject.AddComponent<CanvasGroup>();
		}

		public HoverTextDrawer.Pool<WidgetType>.Entry Draw(Vector2 pos)
		{
			HoverTextDrawer.Pool<WidgetType>.Entry entry;
			if (this.drawnWidgets < this.entries.Count)
			{
				entry = this.entries[this.drawnWidgets];
				if (!entry.widget.gameObject.activeSelf)
				{
					entry.widget.gameObject.SetActive(true);
				}
			}
			else
			{
				GameObject gameObject = Util.KInstantiateUI(this.prefab, this.root.gameObject, false);
				gameObject.SetActive(true);
				entry.widget = gameObject.GetComponent<WidgetType>();
				entry.rect = gameObject.GetComponent<RectTransform>();
				this.entries.Add(entry);
			}
			entry.rect.anchoredPosition = new Vector2(pos.x, pos.y);
			this.drawnWidgets++;
			return entry;
		}

		public void BeginDrawing()
		{
			this.drawnWidgets = 0;
		}

		public void EndDrawing()
		{
			for (int i = this.drawnWidgets; i < this.entries.Count; i++)
			{
				if (this.entries[i].widget.gameObject.activeSelf)
				{
					this.entries[i].widget.gameObject.SetActive(false);
				}
			}
		}

		public void SetEnabled(bool enabled)
		{
			if (enabled)
			{
				this.root.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
			}
			else
			{
				this.root.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
			}
		}

		public void Cleanup()
		{
			foreach (HoverTextDrawer.Pool<WidgetType>.Entry entry in this.entries)
			{
				global::UnityEngine.Object.Destroy(entry.widget.gameObject);
			}
			this.entries.Clear();
		}

		private GameObject prefab;

		private RectTransform root;

		private List<HoverTextDrawer.Pool<WidgetType>.Entry> entries = new List<HoverTextDrawer.Pool<WidgetType>.Entry>();

		private int drawnWidgets;

		public struct Entry
		{
			public WidgetType widget;

			public RectTransform rect;
		}
	}
}
