using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tween : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private void Awake()
	{
		this.Selectable = base.GetComponent<Selectable>();
	}

	public void OnPointerEnter(PointerEventData data)
	{
		this.Direction = 1f;
	}

	public void OnPointerExit(PointerEventData data)
	{
		this.Direction = -1f;
	}

	private void Update()
	{
		if (this.Selectable.interactable)
		{
			float x = base.transform.localScale.x;
			float num = x + this.Direction * Time.unscaledDeltaTime * Tween.ScaleSpeed;
			num = Mathf.Min(num, Tween.Scale);
			num = Mathf.Max(num, 1f);
			if (num != x)
			{
				base.transform.localScale = new Vector3(num, num, 1f);
			}
		}
	}

	private static float Scale = 1.025f;

	private static float ScaleSpeed = 0.5f;

	private Selectable Selectable;

	private float Direction = -1f;
}
