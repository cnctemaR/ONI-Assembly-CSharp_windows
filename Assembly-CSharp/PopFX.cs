using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/PopFX")]
public class PopFX : KMonoBehaviour
{
	public Vector3 StartPos
	{
		get
		{
			return this.startPos;
		}
	}

	public void Recycle()
	{
		this.icon = null;
		this.mainIcon = null;
		this.text = "";
		this.targetTransform = null;
		this.lifeElapsed = 0f;
		this.trackTarget = false;
		this.startPos = Vector3.zero;
		this.positionToGroup = true;
		this.canvasPaddingMultiplier = Vector3.zero;
		this.IconDisplay.color = Color.white;
		this.TextDisplay.color = Color.white;
		this.MainIconDisplay.color = Color.white;
		PopFXManager.Instance.RecycleFX(this);
		this.canvasGroup.alpha = 0f;
		this.IconDisplay.gameObject.SetActive(false);
		base.gameObject.SetActive(false);
		this.isLive = false;
		this.isActiveWorld = false;
		Game.Instance.Unsubscribe(ref this.eventid);
	}

	public void SetIconTint(Color color)
	{
		this.MainIconDisplay.color = color;
	}

	public void Run(Vector3 groupSpawnPosition, Vector3 canvasPaddingMultiplier)
	{
		base.gameObject.SetActive(true);
		this.canvasPaddingMultiplier = canvasPaddingMultiplier;
		if (this.positionToGroup && groupSpawnPosition != PopFxGroup.INVALID_SPAWN_POSITION)
		{
			this.startPos = groupSpawnPosition;
		}
		if (this.trackTarget && this.targetTransform != null)
		{
			this.startPos = this.targetTransform.GetPosition();
			int num;
			int num2;
			Grid.PosToXY(this.startPos, out num, out num2);
			this.startPos.x = this.startPos.x - 0.5f;
		}
		this.TextDisplay.text = this.text;
		this.IconDisplay.sprite = this.icon;
		this.IconDisplay.Opacity(1f);
		this.MainIconDisplay.Opacity(1f);
		this.MainIconDisplay.sprite = this.mainIcon;
		this.IconDisplay.gameObject.SetActive(this.icon != null);
		this.canvasGroup.alpha = 1f;
		this.isLive = true;
		this.eventid = Game.Instance.Subscribe(1983128072, PopFX.OnActiveWorldChangedDispatcher, this);
		this.SetWorldActive(ClusterManager.Instance.activeWorldId);
		this.Update();
	}

	public void Setup(Sprite MainIcon, Sprite SecondaryIcon, string Text, Transform TargetTransform, Vector3 Offset, bool PositionToGroup, float LifeTime = 1.5f, bool TrackTarget = false)
	{
		this.mainIcon = MainIcon;
		this.icon = SecondaryIcon;
		this.text = Text;
		this.targetTransform = TargetTransform;
		this.trackTarget = TrackTarget;
		this.lifetime = LifeTime;
		this.offset = Offset;
		this.positionToGroup = PositionToGroup;
		if (this.targetTransform != null)
		{
			this.startPos = this.targetTransform.GetPosition();
		}
		int num;
		int num2;
		Grid.PosToXY(this.startPos, out num, out num2);
		this.startPos.x = this.startPos.x - 0.5f;
	}

	private void OnActiveWorldChanged(object data)
	{
		global::Tuple<int, int> tuple = (global::Tuple<int, int>)data;
		if (this.isLive)
		{
			this.SetWorldActive(tuple.first);
		}
	}

	private void SetWorldActive(int worldId)
	{
		int num = Grid.PosToCell((this.trackTarget && this.targetTransform != null) ? this.targetTransform.position : (this.startPos + this.offset));
		this.isActiveWorld = !Grid.IsValidCell(num) || (int)Grid.WorldIdx[num] == worldId;
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
			Vector3 vector = PopFXManager.Instance.WorldToScreen(this.targetTransform.GetPosition() + this.offset + Vector3.up * this.lifeElapsed * (2f * this.lifeElapsed));
			vector.z = 0f;
			base.gameObject.rectTransform().anchoredPosition = vector;
		}
		else
		{
			Vector3 vector2 = PopFXManager.Instance.WorldToScreen(this.startPos + this.offset + Vector3.up * this.lifeElapsed * (2f * (this.lifeElapsed / 2f)));
			vector2.z = 0f;
			Vector3 vector3 = this.Pivot.rect.size;
			vector3.x *= this.canvasPaddingMultiplier.x;
			vector3.y *= this.canvasPaddingMultiplier.y;
			vector3.z *= this.canvasPaddingMultiplier.z;
			vector2 += vector3;
			base.gameObject.rectTransform().anchoredPosition = vector2;
		}
		float num = (CameraController.Instance.FreeCameraEnabled ? TuningData<CameraController.Tuning>.Get().maxOrthographicSizeDebug : 20f);
		float num2 = (CameraController.Instance.OrthographicSize - CameraController.Instance.minOrthographicSize) / (num - CameraController.Instance.minOrthographicSize);
		base.gameObject.rectTransform().localScale = Vector3.one * Mathf.Lerp(1f, 0.7f, num2);
		float num3 = Mathf.Clamp01((this.lifetime - this.lifeElapsed) / this.lifetime);
		float num4 = Mathf.Clamp01((1f - num3) / 0.1f);
		float num5 = Mathf.Clamp01(num3 / 0.2f);
		this.mask.fillAmount = Mathf.Lerp(0.16f * num5, 1f, num4);
		this.canvasGroup.alpha = (this.isActiveWorld ? num5 : 0f);
	}

	public const float Speed = 2f;

	private Sprite mainIcon;

	private Sprite icon;

	private string text;

	private Transform targetTransform;

	private Vector3 offset;

	private Vector3 canvasPaddingMultiplier;

	public RectTransform Pivot;

	public Image bg;

	public Image MainIconDisplay;

	public Image IconDisplay;

	public Image mask;

	public LocText TextDisplay;

	public CanvasGroup canvasGroup;

	private Camera uiCamera;

	private float lifetime;

	private float lifeElapsed;

	private bool trackTarget;

	private bool positionToGroup = true;

	private Vector3 startPos;

	private bool isLive;

	private bool isActiveWorld;

	private int eventid = -1;

	private static Action<object, object> OnActiveWorldChangedDispatcher = delegate(object context, object data)
	{
		Unsafe.As<PopFX>(context).OnActiveWorldChanged(data);
	};
}
