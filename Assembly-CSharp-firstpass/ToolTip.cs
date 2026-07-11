using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTip : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	public string toolTip
	{
		set
		{
			this.SetSimpleTooltip(value);
		}
	}

	public int multiStringCount
	{
		get
		{
			return this.multiStringToolTips.Count;
		}
	}

	public Func<string> OnToolTip
	{
		get
		{
			return this._OnToolTip;
		}
		set
		{
			this._OnToolTip = value;
		}
	}

	protected override void OnPrefabInit()
	{
		if (base.gameObject.GetComponents<ToolTip>().Length > 1)
		{
			global::Debug.LogError("The object " + base.gameObject.name + " has more than one ToolTip, it conflict when displaying this tooltip.");
		}
		base.Subscribe<ToolTip>(2098165161, ToolTip.OnClickDelegate);
		if (this.UseFixedStringKey)
		{
			string text = Strings.Get(new StringKey(this.FixedStringKey));
			this.toolTip = text;
		}
		switch (this.toolTipPosition)
		{
		case ToolTip.TooltipPosition.TopLeft:
			this.tooltipPivot = new Vector2(1f, 0f);
			this.tooltipPositionOffset = new Vector2(0f, 20f);
			this.parentPositionAnchor = new Vector2(0.5f, 0.5f);
			break;
		case ToolTip.TooltipPosition.TopCenter:
			this.tooltipPivot = new Vector2(0.5f, 0f);
			this.tooltipPositionOffset = new Vector2(0f, 20f);
			this.parentPositionAnchor = new Vector2(0.5f, 0.5f);
			break;
		case ToolTip.TooltipPosition.TopRight:
			this.tooltipPivot = new Vector2(0f, 0f);
			this.tooltipPositionOffset = new Vector2(0f, 20f);
			this.parentPositionAnchor = new Vector2(0.5f, 0.5f);
			break;
		case ToolTip.TooltipPosition.BottomLeft:
			this.tooltipPivot = new Vector2(1f, 1f);
			this.tooltipPositionOffset = new Vector2(0f, -25f);
			this.parentPositionAnchor = new Vector2(0.5f, 0.5f);
			break;
		case ToolTip.TooltipPosition.BottomCenter:
			this.tooltipPivot = new Vector2(0.5f, 1f);
			this.tooltipPositionOffset = new Vector2(0f, -25f);
			this.parentPositionAnchor = new Vector2(0.5f, 0.5f);
			break;
		case ToolTip.TooltipPosition.BottomRight:
			this.tooltipPivot = new Vector2(0f, 1f);
			this.tooltipPositionOffset = new Vector2(0f, -25f);
			this.parentPositionAnchor = new Vector2(0.5f, 0.5f);
			break;
		}
	}

	protected override void OnSpawn()
	{
		if (!this.worldSpace)
		{
			Canvas componentInParent = base.gameObject.GetComponentInParent<Canvas>();
			this.worldSpace = componentInParent != null && componentInParent.worldCamera != null;
		}
	}

	public void SetSimpleTooltip(string message)
	{
		this.ClearMultiStringTooltip();
		this.AddMultiStringTooltip(message, PluginAssets.Instance.defaultTextStyleSetting);
	}

	public void AddMultiStringTooltip(string newString, ScriptableObject styleSetting)
	{
		this.multiStringToolTips.Add(newString);
		this.styleSettings.Add(styleSetting);
	}

	public void ClearMultiStringTooltip()
	{
		this.multiStringToolTips.Clear();
		this.styleSettings.Clear();
	}

	public string GetMultiString(int idx)
	{
		return this.multiStringToolTips[idx];
	}

	public ScriptableObject GetStyleSetting(int idx)
	{
		return this.styleSettings[idx];
	}

	public void SetFixedStringKey(string newKey)
	{
		this.FixedStringKey = newKey;
		string text = Strings.Get(new StringKey(this.FixedStringKey));
		this.toolTip = text;
	}

	public void RebuildDynamicTooltip()
	{
		if (this.OnToolTip != null)
		{
			this.ClearMultiStringTooltip();
			string text = this.OnToolTip();
			if (!string.IsNullOrEmpty(text))
			{
				this.AddMultiStringTooltip(text, PluginAssets.Instance.defaultTextStyleSetting);
			}
		}
		else if (this.OnComplexToolTip != null)
		{
			this.ClearMultiStringTooltip();
			List<Tuple<string, ScriptableObject>> list = this.OnComplexToolTip();
			foreach (Tuple<string, ScriptableObject> tuple in list)
			{
				this.AddMultiStringTooltip(tuple.first, tuple.second);
			}
		}
	}

	public void OnPointerEnter(PointerEventData data)
	{
		this.OnHoverStateChanged(true);
		this.isHovering = true;
	}

	public void OnPointerExit(PointerEventData data)
	{
		this.OnHoverStateChanged(false);
		this.isHovering = false;
	}

	private void OnClick(object data)
	{
		ToolTipScreen.Instance.ClearToolTip(this);
	}

	private void OnDisable()
	{
		if (ToolTipScreen.Instance)
		{
			ToolTipScreen.Instance.MarkTooltipDirty(this);
		}
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if (ToolTipScreen.Instance)
		{
			ToolTipScreen.Instance.MarkTooltipDirty(this);
		}
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		if (ToolTipScreen.Instance)
		{
			ToolTipScreen.Instance.MakeDirtyTooltipClean(this);
		}
	}

	private void OnHoverStateChanged(bool is_over)
	{
		if (ToolTipScreen.Instance == null)
		{
			return;
		}
		if (is_over)
		{
			ToolTipScreen.Instance.SetToolTip(this);
		}
		else
		{
			ToolTipScreen.Instance.ClearToolTip(this);
		}
	}

	protected override void OnCleanUp()
	{
		if (ToolTipScreen.Instance != null)
		{
			ToolTipScreen.Instance.ClearToolTip(this);
		}
	}

	public void UpdateWhileHovered()
	{
		if (!this.forceRefresh && !this.refreshWhileHovering)
		{
			return;
		}
		if (Time.unscaledTime - this.lastUpdateTime > 0.2f)
		{
			this.lastUpdateTime = Time.unscaledTime;
			if (this.isHovering)
			{
				this.RebuildDynamicTooltip();
				for (int i = 0; i < this.multiStringToolTips.Count; i++)
				{
					ToolTipScreen.Instance.HotSwapTooltipString(this.multiStringToolTips[i], i);
				}
			}
		}
	}

	public bool UseFixedStringKey;

	public string FixedStringKey = string.Empty;

	private List<string> multiStringToolTips = new List<string>();

	private List<ScriptableObject> styleSettings = new List<ScriptableObject>();

	public bool worldSpace;

	public bool forceRefresh;

	public bool refreshWhileHovering;

	private bool isHovering;

	private float lastUpdateTime;

	public ToolTip.TooltipPosition toolTipPosition = ToolTip.TooltipPosition.BottomCenter;

	public Vector2 tooltipPivot = new Vector2(0f, 1f);

	public Vector2 tooltipPositionOffset = new Vector2(0f, -25f);

	public Vector2 parentPositionAnchor = new Vector2(0.5f, 0.5f);

	public RectTransform overrideParentObject;

	public ToolTip.ToolTipSizeSetting SizingSetting = ToolTip.ToolTipSizeSetting.DynamicWidthNoWrap;

	public float WrapWidth = 256f;

	private Func<string> _OnToolTip;

	public Func<List<Tuple<string, ScriptableObject>>> OnComplexToolTip;

	private static readonly global::EventSystem.IntraObjectHandler<ToolTip> OnClickDelegate = new global::EventSystem.IntraObjectHandler<ToolTip>(delegate(ToolTip component, object data)
	{
		component.OnClick(data);
	});

	public enum TooltipPosition
	{
		TopLeft,
		TopCenter,
		TopRight,
		BottomLeft,
		BottomCenter,
		BottomRight,
		Custom
	}

	public enum ToolTipSizeSetting
	{
		MaxWidthWrapContent,
		DynamicWidthNoWrap
	}
}
