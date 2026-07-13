using System;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("KMonoBehaviour/Plugins/KScreen")]
public class KScreen : KMonoBehaviour, IInputHandler, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
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

	protected bool isEditing
	{
		get
		{
			return this._isEditing;
		}
		set
		{
			this._isEditing = value;
			KScreenManager.Instance.RefreshStack();
		}
	}

	public void SetIsEditing(bool state)
	{
		this.isEditing = state;
	}

	public virtual float GetSortKey()
	{
		if (this.isEditing)
		{
			return 50f;
		}
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

	public bool ConsumeMouseScroll { get; set; }

	public virtual void SetHasFocus(bool has_focus)
	{
		this.hasFocus = has_focus;
	}

	public virtual bool IsScreenActive()
	{
		return base.isActiveAndEnabled;
	}

	public KScreen()
	{
		this.screenName = base.GetType().ToString();
		if (this.displayName == null || this.displayName == "")
		{
			this.displayName = this.screenName;
		}
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

	public virtual void OnDrag(PointerEventData eventData)
	{
	}

	public virtual void OnBeginDrag(PointerEventData eventData)
	{
	}

	public virtual void OnEndDrag(PointerEventData eventData)
	{
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this._canvas = base.GetComponentInParent<Canvas>();
		if (this._canvas != null)
		{
			this._rectTransform = this._canvas.GetComponentInParent<RectTransform>();
		}
		if (this.activateOnSpawn && KScreenManager.Instance != null && !this.isActive)
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
		if (this.isEditing)
		{
			e.Consumed = true;
		}
		if (!e.Consumed)
		{
			this.child_scroll_rects = base.GetComponentsInChildren<KScrollRect>();
		}
		if (this.mouseOver && this.ConsumeMouseScroll)
		{
			if (KInputManager.currentControllerIsGamepad && !e.Consumed)
			{
				foreach (KScrollRect kscrollRect in this.child_scroll_rects)
				{
					Vector2 vector = kscrollRect.rectTransform().InverseTransformPoint(KInputManager.GetMousePos());
					if (kscrollRect.rectTransform().rect.Contains(vector))
					{
						kscrollRect.mouseIsOver = true;
					}
					else
					{
						kscrollRect.mouseIsOver = false;
					}
					kscrollRect.OnKeyDown(e);
					if (e.Consumed)
					{
						break;
					}
				}
			}
			if (!e.Consumed && !e.TryConsume(global::Action.ZoomIn))
			{
				e.TryConsume(global::Action.ZoomOut);
			}
		}
		if (!e.Consumed)
		{
			foreach (KScrollRect kscrollRect2 in this.child_scroll_rects)
			{
				Vector2 vector2 = kscrollRect2.rectTransform().InverseTransformPoint(KInputManager.GetMousePos());
				if (kscrollRect2.rectTransform().rect.Contains(vector2))
				{
					kscrollRect2.mouseIsOver = true;
				}
				else
				{
					kscrollRect2.mouseIsOver = false;
				}
				kscrollRect2.OnKeyDown(e);
				if (e.Consumed)
				{
					break;
				}
			}
		}
	}

	public virtual void OnKeyUp(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			KScrollRect[] componentsInChildren = base.GetComponentsInChildren<KScrollRect>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OnKeyUp(e);
				if (e.Consumed)
				{
					break;
				}
			}
		}
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
		this.child_scroll_rects = base.GetComponentsInChildren<KScrollRect>();
		if (show && this.fadeIn)
		{
			base.gameObject.FindOrAddUnityComponent<WidgetTransition>().StartTransition();
		}
	}

	public virtual void Show(bool show = true)
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
		base.gameObject.FindOrAddUnityComponent<WidgetTransition>().SetTransitionType(this.transitionType);
	}

	[SerializeField]
	public bool activateOnSpawn;

	private bool _isEditing;

	public const float MODAL_SCREEN_SORT_KEY = 100f;

	public const float EDITING_SCREEN_SORT_KEY = 50f;

	public const float LOCKER_SORT_KEY = 40f;

	public const float PAUSE_MENU_SORT_KEY = 30f;

	public const float FULLSCREEN_SCREEN_SORT_KEY = 20f;

	private Canvas _canvas;

	private RectTransform _rectTransform;

	private bool isActive;

	protected bool mouseOver;

	public WidgetTransition.TransitionType transitionType;

	public bool fadeIn;

	public string displayName;

	public KScreen.PointerEnterActions pointerEnterActions;

	public KScreen.PointerExitActions pointerExitActions;

	private KScrollRect[] child_scroll_rects;

	private bool hasFocus;

	public delegate void PointerEnterActions(PointerEventData eventData);

	public delegate void PointerExitActions(PointerEventData eventData);
}
