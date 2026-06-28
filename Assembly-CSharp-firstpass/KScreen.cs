using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class KScreen : KMonoBehaviour, IInputHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	public KScreen()
	{
		this.screenName = base.GetType().ToString();
		if (this.displayName == null || this.displayName == "")
		{
			this.displayName = this.screenName;
		}
	}

	public KInputHandler inputHandler { get; set; }

	public virtual bool HasFocus
	{
		get
		{
			return this.hasFocus;
		}
	}

	public virtual float GetSortKey()
	{
		return 0f;
	}

	public Canvas canvas
	{
		get
		{
			return this._canvas;
		}
	}

	public bool GetMouseOver
	{
		get
		{
			return this.mouseOver;
		}
	}

	public virtual void SetHasFocus(bool has_focus)
	{
		this.hasFocus = has_focus;
	}

	protected override void OnPrefabInit()
	{
		if (this.fadeIn)
		{
			this.InitWidgetTransition();
		}
	}

	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		this.mouseOver = true;
		if (this.pointerEnterActions != null)
		{
			this.pointerEnterActions(eventData);
		}
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		this.mouseOver = false;
		if (this.pointerExitActions != null)
		{
			this.pointerExitActions(eventData);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this._canvas = base.GetComponentInParent<Canvas>();
		if (this._canvas != null)
		{
			this._rectTransform = this._canvas.GetComponentInParent<RectTransform>();
		}
		if (this.activateOnSpawn)
		{
			if (KScreenManager.Instance != null)
			{
				this.Activate();
			}
		}
		if (this.ConsumeMouseScroll && !this.activateOnSpawn)
		{
			global::Debug.LogWarning("ConsumeMouseScroll is true on" + base.gameObject.name + " , but activateOnSpawn is disabled. Mouse scrolling might not work properly on this screen.", null);
		}
	}

	public virtual void OnKeyDown(KButtonEvent e)
	{
		if (this.mouseOver && this.ConsumeMouseScroll)
		{
			if (!e.Consumed)
			{
				if (!e.TryConsume(global::Action.ZoomIn))
				{
					if (e.TryConsume(global::Action.ZoomOut))
					{
					}
				}
			}
		}
	}

	public virtual void OnKeyUp(KButtonEvent e)
	{
	}

	public virtual bool IsModal()
	{
		return false;
	}

	public virtual void ScreenUpdate(bool topLevel)
	{
	}

	public bool IsActive()
	{
		return this.isActive;
	}

	public void Activate()
	{
		base.gameObject.SetActive(true);
		KScreenManager.Instance.PushScreen(this);
		this.OnActivate();
		this.isActive = true;
	}

	protected virtual void OnActivate()
	{
	}

	public virtual void Deactivate()
	{
		if (Application.isPlaying)
		{
			this.OnDeactivate();
			this.isActive = false;
			KScreenManager.Instance.PopScreen(this);
			if (this != null && base.gameObject != null)
			{
				base.gameObject.SetActive(false);
				global::UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	protected override void OnCleanUp()
	{
		if (this.isActive)
		{
			this.Deactivate();
		}
	}

	protected virtual void OnDeactivate()
	{
	}

	public string Name()
	{
		return this.screenName;
	}

	public Vector3 WorldToScreen(Vector3 pos)
	{
		Vector3 vector;
		if (this._rectTransform == null)
		{
			global::Debug.LogWarning("Hey you are calling this function too early!", null);
			vector = Vector3.zero;
		}
		else
		{
			Camera main = Camera.main;
			Vector3 vector2 = main.WorldToViewportPoint(pos);
			vector2.y = vector2.y * main.rect.height + main.rect.y;
			vector = new Vector2((vector2.x - 0.5f) * this._rectTransform.sizeDelta.x, (vector2.y - 0.5f) * this._rectTransform.sizeDelta.y);
		}
		return vector;
	}

	protected virtual void OnShow(bool show)
	{
		if (show && this.fadeIn)
		{
			base.gameObject.FindOrAddUnityComponent<WidgetTransition>().StartTransition();
		}
	}

	public void Show(bool show = true)
	{
		this.mouseOver = false;
		base.gameObject.SetActive(show);
		this.OnShow(show);
	}

	public void SetShouldFadeIn(bool bShouldFade)
	{
		this.fadeIn = bShouldFade;
		this.InitWidgetTransition();
	}

	private void InitWidgetTransition()
	{
		WidgetTransition widgetTransition = base.gameObject.FindOrAddUnityComponent<WidgetTransition>();
		widgetTransition.SetTransitionType(this.transitionType);
	}

	[SerializeField]
	public bool activateOnSpawn = false;

	private Canvas _canvas;

	private RectTransform _rectTransform;

	private string screenName;

	private bool isActive;

	protected bool mouseOver = false;

	protected bool ConsumeMouseScroll = false;

	public WidgetTransition.TransitionType transitionType = WidgetTransition.TransitionType.SlideFromRight;

	public bool fadeIn = false;

	public string displayName;

	public KScreen.PointerEnterActions pointerEnterActions;

	public KScreen.PointerExitActions pointerExitActions;

	private bool hasFocus = false;

	public delegate void PointerEnterActions(PointerEventData eventData);

	public delegate void PointerExitActions(PointerEventData eventData);
}
