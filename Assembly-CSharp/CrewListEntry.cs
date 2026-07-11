using System;
using Klei.AI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/CrewListEntry")]
public class CrewListEntry : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	public MinionIdentity Identity
	{
		get
		{
			return this.identity;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		this.mouseOver = true;
		this.BGImage.enabled = true;
		this.BorderHighlight.color = new Color(0.65882355f, 0.2901961f, 0.4745098f);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		this.mouseOver = false;
		this.BGImage.enabled = false;
		this.BorderHighlight.color = new Color(0.8f, 0.8f, 0.8f);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		bool flag = Time.unscaledTime - this.lastClickTime < 0.3f;
		this.SelectCrewMember(flag);
		this.lastClickTime = Time.unscaledTime;
	}

	public virtual void Populate(MinionIdentity _identity)
	{
		this.identity = _identity;
		if (this.portrait == null)
		{
			GameObject gameObject = ((this.crewPortraitParent != null) ? this.crewPortraitParent : base.gameObject);
			this.portrait = Util.KInstantiateUI<CrewPortrait>(this.PortraitPrefab.gameObject, gameObject, false);
			if (this.crewPortraitParent == null)
			{
				this.portrait.transform.SetSiblingIndex(2);
			}
		}
		this.portrait.SetIdentityObject(_identity, true);
	}

	public virtual void Refresh()
	{
	}

	public void RefreshCrewPortraitContent()
	{
		if (this.portrait != null)
		{
			this.portrait.ForceRefresh();
		}
	}

	private string seniorityString()
	{
		return this.identity.GetAttributes().GetProfessionString(true);
	}

	public void SelectCrewMember(bool focus)
	{
		if (focus)
		{
			SelectTool.Instance.SelectAndFocus(this.identity.transform.GetPosition(), this.identity.GetComponent<KSelectable>(), new Vector3(8f, 0f, 0f));
			return;
		}
		SelectTool.Instance.Select(this.identity.GetComponent<KSelectable>(), false);
	}

	protected MinionIdentity identity;

	protected CrewPortrait portrait;

	public CrewPortrait PortraitPrefab;

	public GameObject crewPortraitParent;

	protected bool mouseOver;

	public Image BorderHighlight;

	public Image BGImage;

	public float lastClickTime;
}
