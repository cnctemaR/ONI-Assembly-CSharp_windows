using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class KSelectable : KMonoBehaviour
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
		KPrefabID component = base.GetComponent<KPrefabID>();
		if (component != null)
		{
		}
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

	public virtual string GetName()
	{
		string name;
		if (this.entityName == null || this.entityName == "" || this.entityName.Length <= 0)
		{
			Output.LogWithObj(base.gameObject, new object[] { "Warning Item has blank name!" });
			name = base.name;
		}
		else
		{
			name = this.entityName;
		}
		return name;
	}

	public void SetStatusIndicatorOffset(Vector3 offset)
	{
		if (this.statusItemGroup != null)
		{
			this.statusItemGroup.SetOffset(offset);
		}
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
		base.Trigger(-1201923725, false);
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.HighlightColour = new Color(0f, 0f, 0f, 0f);
		}
	}

	private void ApplyHighlight(float highlight)
	{
		RenderUtil.AddMaterialBlockVector(base.gameObject.transform, "_Highlight", new Vector4(highlight, highlight, highlight, highlight));
		base.Trigger(-1201923725, true);
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
		base.Trigger(-1503271301, true);
	}

	public void Unselect()
	{
		if (this.selected)
		{
			this.selected = false;
			this.ClearHighlight();
			base.Trigger(-1503271301, false);
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
		if (!(base.GetComponent<CellSelectionObject>() != null))
		{
			UISounds.PlaySound(UISounds.Sound.Object_Mouseover);
		}
	}

	public void Unhover()
	{
		if (!this.selected)
		{
			this.ClearHighlight();
		}
	}

	public Guid ToggleStatusItem(StatusItem status_item, bool on, object data = null)
	{
		Guid guid;
		if (on)
		{
			guid = this.AddStatusItem(status_item, data);
		}
		else
		{
			guid = this.RemoveStatusItem(status_item, false);
		}
		return guid;
	}

	public Guid SetStatusItem(StatusItemCategory category, StatusItem status_item, object data = null)
	{
		Guid guid;
		if (this.statusItemGroup == null)
		{
			guid = Guid.Empty;
		}
		else
		{
			guid = this.statusItemGroup.SetStatusItem(category, status_item, data);
		}
		return guid;
	}

	public Guid ReplaceStatusItem(Guid guid, StatusItem status_item, object data = null)
	{
		Guid guid2;
		if (this.statusItemGroup == null)
		{
			guid2 = Guid.Empty;
		}
		else
		{
			if (guid != Guid.Empty)
			{
				this.statusItemGroup.RemoveStatusItem(guid, false);
			}
			guid2 = this.AddStatusItem(status_item, data);
		}
		return guid2;
	}

	public Guid AddStatusItem(StatusItem status_item, object data = null)
	{
		Guid guid;
		if (this.statusItemGroup == null)
		{
			guid = Guid.Empty;
		}
		else
		{
			guid = this.statusItemGroup.AddStatusItem(status_item, data, null);
		}
		return guid;
	}

	public Guid RemoveStatusItem(StatusItem status_item, bool immediate = false)
	{
		Guid guid;
		if (this.statusItemGroup == null)
		{
			guid = Guid.Empty;
		}
		else
		{
			this.statusItemGroup.RemoveStatusItem(status_item, immediate);
			guid = Guid.Empty;
		}
		return guid;
	}

	public Guid RemoveStatusItem(Guid guid, bool immediate = false)
	{
		Guid guid2;
		if (this.statusItemGroup == null)
		{
			guid2 = Guid.Empty;
		}
		else
		{
			this.statusItemGroup.RemoveStatusItem(guid, immediate);
			guid2 = Guid.Empty;
		}
		return guid2;
	}

	public bool HasStatusItem(StatusItem status_item)
	{
		return this.statusItemGroup != null && this.statusItemGroup.HasStatusItem(status_item);
	}

	public StatusItemGroup.Entry GetStatusItem(StatusItemCategory category)
	{
		return this.statusItemGroup.GetStatusItem(category);
	}

	public StatusItemGroup GetStatusItemGroup()
	{
		return this.statusItemGroup;
	}

	protected override void OnLoadLevel()
	{
		this.OnCleanUp();
		base.OnLoadLevel();
	}

	protected override void OnCleanUp()
	{
		this.statusItemGroup.Destroy();
		this.statusItemGroup = null;
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

	private bool selected = false;

	[SerializeField]
	private bool selectable = true;

	[SerializeField]
	private bool disableSelectMarker;

	[SerializeField]
	private string entityNameLocString;

	private StatusItemGroup statusItemGroup;
}
