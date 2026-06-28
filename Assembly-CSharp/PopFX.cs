using System;
using UnityEngine;
using UnityEngine.UI;

public class PopFX : KMonoBehaviour
{
	public void Recycle()
	{
		this.icon = null;
		this.text = string.Empty;
		this.targetTransform = null;
		this.lifeElapsed = 0f;
		this.trackTarget = false;
		this.startPos = Vector3.zero;
		this.IconDisplay.color = Color.white;
		this.TextDisplay.color = Color.white;
		PopFXManager.Instance.RecycleFX(this);
		this.canvasGroup.alpha = 0f;
		base.gameObject.SetActive(false);
		this.isLive = false;
	}

	public void Spawn(Sprite Icon, string Text, Transform TargetTransform, Vector3 Offset, float LifeTime = 1.5f, bool TrackTarget = false)
	{
		this.icon = Icon;
		this.text = Text;
		this.targetTransform = TargetTransform;
		this.trackTarget = TrackTarget;
		this.lifetime = LifeTime;
		this.offset = Offset;
		if (this.targetTransform != null)
		{
			this.startPos = this.targetTransform.GetPosition();
			int num;
			int num2;
			Grid.PosToXY(this.startPos, out num, out num2);
			if (num2 % 2 != 0)
			{
				this.startPos.x = this.startPos.x + 0.5f;
			}
		}
		this.TextDisplay.text = this.text;
		this.IconDisplay.sprite = this.icon;
		this.canvasGroup.alpha = 1f;
		this.isLive = true;
		this.Update();
	}

	private void Update()
	{
		if (!this.isLive)
		{
			return;
		}
		if (!PopFXManager.Instance.Ready())
		{
			return;
		}
		this.lifeElapsed += Time.unscaledDeltaTime;
		if (this.lifeElapsed >= this.lifetime)
		{
			this.Recycle();
		}
		if (this.trackTarget && this.targetTransform != null)
		{
			Vector3 vector = PopFXManager.Instance.WorldToScreen(this.targetTransform.GetPosition() + this.offset + Vector3.up * this.lifeElapsed * (this.Speed * this.lifeElapsed));
			vector.z = 0f;
			base.gameObject.rectTransform().anchoredPosition = vector;
		}
		else
		{
			Vector3 vector2 = PopFXManager.Instance.WorldToScreen(this.startPos + this.offset + Vector3.up * this.lifeElapsed * (this.Speed * (this.lifeElapsed / 2f)));
			vector2.z = 0f;
			base.gameObject.rectTransform().anchoredPosition = vector2;
		}
		this.canvasGroup.alpha = 1.5f * ((this.lifetime - this.lifeElapsed) / this.lifetime);
	}

	private float Speed = 2f;

	private Sprite icon;

	private string text;

	private Transform targetTransform;

	private Vector3 offset;

	public Image IconDisplay;

	public Text TextDisplay;

	public CanvasGroup canvasGroup;

	private Camera uiCamera;

	private float lifetime;

	private float lifeElapsed;

	private bool trackTarget;

	private Vector3 startPos;

	private bool isLive;
}
