using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class KSelectable : KMonoBehaviour, ISaveLoadableJson
{
	public bool IsSelected
	{
		get
		{
			return this.selected;
		}
	}

	public bool IsSelectable
	{
		get
		{
			return this.selectable && base.isActiveAndEnabled;
		}
		set
		{
			this.selectable = value;
		}
	}

	public bool DisableSelectMarker
	{
		get
		{
			return this.disableSelectMarker;
		}
	}

	public string EntityNameLocString
	{
		get
		{
			return this.entityNameLocString;
		}
	}

	protected override void OnPrefabInit()
	{
		this.statusItemGroup = new StatusItemGroup(base.gameObject);
		if (this.entityNameLocString != null && this.entityNameLocString.Length > 0)
		{
			string text = Strings.Get(this.entityNameLocString);
			if (text != null && text.Length > 0)
			{
				this.entityName = text;
			}
		}
		if (this.entityName == null || this.entityName.Length <= 0)
		{
			this.SetName(base.name);
		}
	}

	protected override void OnSpawn()
	{
		KPrefabID component = base.GetComponent<KPrefabID>();
		if (component != null)
		{
			component.AddLog(this.log);
			component.AddLog(this.statusItemGroup.GetLog());
		}
		if (this.initialOffset != Vector3.zero)
		{
			this.SetStatusIndicatorOffset(this.initialOffset);
		}
	}

	public virtual string GetName()
	{
		if (this.entityName == null || this.entityName == string.Empty || this.entityName.Length <= 0)
		{
			Output.LogWithObj(base.gameObject, new object[] { "Warning Item has blank name!" });
			return base.name;
		}
		return this.entityName;
	}

	public void SetStatusIndicatorOffset(Vector3 offset)
	{
		this.statusItemGroup.SetOffset(offset);
	}

	public void SetName(string name)
	{
		this.entityName = name;
	}

	public float GetZoom()
	{
		Bounds bounds = Util.GetBounds(base.gameObject);
		return 1.05f * Mathf.Max(bounds.extents.x, bounds.extents.y);
	}

	public Vector3 GetPortraitLocation()
	{
		Vector3 vector = default(Vector3);
		return Util.GetBounds(base.gameObject).center;
	}

	private void ClearHighlight()
	{
		RenderUtil.RemoveMaterialBlockVector(base.gameObject.transform, "_Highlight");
		this.Trigger(-1201923725, false);
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.HighlightColour = new Color(0f, 0f, 0f, 0f);
		}
	}

	private void ApplyHighlight(float highlight)
	{
		RenderUtil.AddMaterialBlockVector(base.gameObject.transform, "_Highlight", new Vector4(highlight, highlight, highlight, highlight));
		this.Trigger(-1201923725, true);
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.HighlightColour = new Color(highlight, highlight, highlight, highlight);
		}
	}

	public void Select()
	{
		this.selected = true;
		this.ClearHighlight();
		this.ApplyHighlight(0.2f);
		this.Trigger(-1503271301, true);
	}

	public void Unselect()
	{
		if (this.selected)
		{
			this.selected = false;
			this.ClearHighlight();
			this.Trigger(-1503271301, false);
		}
	}

	public void Hover(bool playAudio)
	{
		this.ClearHighlight();
		if (!DebugHandler.HideUI)
		{
			this.ApplyHighlight(0.25f);
		}
		if (playAudio)
		{
			this.PlayHoverSound();
		}
	}

	private void PlayHoverSound()
	{
		if (base.GetComponent<CellSelectionObject>() != null)
		{
			return;
		}
		UISounds.PlaySound(UISounds.Sound.Object_Mouseover);
	}

	public void Unhover()
	{
		if (!this.selected)
		{
			this.ClearHighlight();
		}
	}

	public void ToggleStatusItem(StatusItem status_item, bool on, object data = null)
	{
		if (on)
		{
			this.AddStatusItem(status_item, data);
		}
		else
		{
			this.RemoveStatusItem(status_item);
		}
	}

	public void SetStatusItem(StatusItemCategory category, StatusItem status_item, object data = null)
	{
		this.statusItemGroup.SetStatusItem(category, status_item, data);
	}

	public Guid ReplaceStatusItem(Guid guid, StatusItem status_item, object data = null)
	{
		if (guid != Guid.Empty)
		{
			this.statusItemGroup.RemoveStatusItem(guid);
		}
		return this.AddStatusItem(status_item, data);
	}

	public Guid AddStatusItem(StatusItem status_item, object data = null)
	{
		return this.statusItemGroup.AddStatusItem(status_item, data, null);
	}

	public Guid RemoveStatusItem(StatusItem status_item)
	{
		return this.statusItemGroup.RemoveStatusItem(status_item);
	}

	public Guid RemoveStatusItem(Guid guid)
	{
		return this.statusItemGroup.RemoveStatusItem(guid);
	}

	public bool HasStatusItem(StatusItem status_item)
	{
		return this.statusItemGroup.HasStatusItem(status_item);
	}

	public StatusItemGroup.Entry GetStatusItem(StatusItemCategory category)
	{
		return this.statusItemGroup.GetStatusItem(category);
	}

	public StatusItemGroup GetStatusItemGroup()
	{
		return this.statusItemGroup;
	}

	protected override void OnCleanUp()
	{
		this.statusItemGroup.Destroy();
		if (this.selected)
		{
			if (SelectTool.Instance.selected == this)
			{
				SelectTool.Instance.Select(null, true);
			}
			else
			{
				this.Unselect();
			}
		}
	}

	private const float hoverHighlight = 0.25f;

	private const float selectHighlight = 0.2f;

	public string entityName;

	private bool selected;

	[SerializeField]
	private bool selectable = true;

	[SerializeField]
	private bool disableSelectMarker;

	[SerializeField]
	private Vector3 initialOffset;

	[SerializeField]
	private string entityNameLocString;

	private StatusItemGroup statusItemGroup;

	private LoggerFSS log = new LoggerFSS("KSelectable");
}
