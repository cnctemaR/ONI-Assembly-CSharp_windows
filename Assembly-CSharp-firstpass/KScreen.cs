using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class KScreen : KMonoBehaviour, IInputHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	public KScreen()
	{
		this.screenName = base.GetType().ToString();
		if (this.displayName == null || this.displayName == string.Empty)
		{
			this.displayName = this.screenName;
		}
	}

	public string handlerName
	{
		get
		{
			return base.gameObject.name;
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

	public string screenName { get; private set; }

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
		if (this.activateOnSpawn && KScreenManager.Instance != null)
		{
			this.Activate();
		}
		if (this.ConsumeMouseScroll && !this.IsActive())
		{
			global::Debug.LogWarning("ConsumeMouseScroll is true on" + base.gameObject.name + " , but screen has not been activated. Mouse scrolling might not work properly on this screen.");
		}
	}

	public virtual void OnKeyDown(KButtonEvent e)
	{
		if (this.mouseOver && this.ConsumeMouseScroll && !e.Consumed)
		{
			if (!e.TryConsume(global::Action.ZoomIn))
			{
				if (e.TryConsume(global::Action.ZoomOut))
				{
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
		if (!Application.isPlaying)
		{
			return;
		}
		this.OnDeactivate();
		this.isActive = false;
		KScreenManager.Instance.PopScreen(this);
		if (this != null && base.gameObject != null)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
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
		if (this._rectTransform == null)
		{
			global::Debug.LogWarning("Hey you are calling this function too early!");
			return Vector3.zero;
		}
		Camera main = Camera.main;
		Vector3 vector = main.WorldToViewportPoint(pos);
		vector.y = vector.y * main.rect.height + main.rect.y;
		return new Vector2((vector.x - 0.5f) * this._rectTransform.sizeDelta.x, (vector.y - 0.5f) * this._rectTransform.sizeDelta.y);
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
	public bool activateOnSpawn;

	private Canvas _canvas;

	private RectTransform _rectTransform;

	private bool isActive;

	protected bool mouseOver;

	protected bool ConsumeMouseScroll;

	public WidgetTransition.TransitionType transitionType;

	public bool fadeIn;

	public string displayName;

	public KScreen.PointerEnterActions pointerEnterActions;

	public KScreen.PointerExitActions pointerExitActions;

	private bool hasFocus;

	public delegate void PointerEnterActions(PointerEventData eventData);

	public delegate void PointerExitActions(PointerEventData eventData);
}
