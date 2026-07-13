using System;
using KSerialization;
using UnityEngine;

public class ParallaxBackgroundObject : KMonoBehaviour
{
	public static Mesh Mesh
	{
		get
		{
			if (ParallaxBackgroundObject.mesh == null)
			{
				ParallaxBackgroundObject.mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
			}
			return ParallaxBackgroundObject.mesh;
		}
	}

	public static int Layer
	{
		get
		{
			int num = ParallaxBackgroundObject.layer.GetValueOrDefault();
			if (ParallaxBackgroundObject.layer == null)
			{
				num = LayerMask.NameToLayer("Default");
				ParallaxBackgroundObject.layer = new int?(num);
			}
			return ParallaxBackgroundObject.layer.Value;
		}
	}

	public static float Depth
	{
		get
		{
			float num = ParallaxBackgroundObject.depth.GetValueOrDefault();
			if (ParallaxBackgroundObject.depth == null)
			{
				num = Grid.GetLayerZ(Grid.SceneLayer.Background) + 0.8f;
				ParallaxBackgroundObject.depth = new float?(num);
			}
			return ParallaxBackgroundObject.depth.Value;
		}
	}

	private void OnActiveWorldChanged(object data)
	{
		if (this.worldId == null)
		{
			return;
		}
		int first = ((global::Tuple<int, int>)data).first;
		this.visible = first == this.worldId.Value;
	}

	public void Initialize(string texture)
	{
		this.sprite = Assets.GetSprite(texture);
	}

	public void SetVisibilityState(bool visible)
	{
		this.visible = visible;
	}

	public void PlayPlayerClickFeedback()
	{
		this.material.SetFloat("_LastTimePlayerClickedNotification", Time.unscaledTime);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Game.Instance.Subscribe(1983128072, new Action<object>(this.OnActiveWorldChanged));
		this.distanceUpdate = true;
		this.startOffset = new Vector2(0f, 0f);
		this.endOffset = new Vector2(0.5f, 0.2f);
		Material material = Assets.GetMaterial("BGPlanet");
		this.material = new Material(material);
		this.material.SetTexture("_MainTex", this.sprite.texture);
		this.material.SetFloat("_LastTimeDamaged", float.MinValue);
		this.material.SetFloat("_LastTimePlayerClickedNotification", float.MinValue);
		this.material.SetFloat("_SizeProgress", 0f);
		this.material.renderQueue = RenderQueues.Stars;
	}

	public void TriggerShaderDamagedEffect(int _)
	{
		this.material.SetFloat("_LastTimeDamaged", Time.unscaledTime);
	}

	public float lastScaleUsed { get; private set; } = 1f;

	private void LateUpdate()
	{
		if (this.motion == null)
		{
			return;
		}
		if (!this.visible)
		{
			return;
		}
		if (this.distanceUpdate)
		{
			float duration = this.motion.GetDuration();
			this.normalizedDistance = ((duration == 0f) ? 1f : (1f - Mathf.Pow(this.motion.GetETA() / duration, 4f)));
			this.motion.OnNormalizedDistanceChanged(this.normalizedDistance);
		}
		Color color = new Color(0.16862746f, 0.22745098f, 0.36078432f, 0f);
		this.material.color = Color.Lerp(color, Color.white, this.normalizedDistance);
		float num = Mathf.Lerp(this.scaleMin, this.scaleMax, this.normalizedDistance);
		this.lastScaleUsed = num;
		Vector2 vector = Vector2.Lerp(this.startOffset, this.endOffset, this.normalizedDistance);
		Vector3 position = CameraController.Instance.baseCamera.transform.position;
		Vector3 vector2 = new Vector3(position.x * this.parallaxFactor, position.y * this.parallaxFactor, ParallaxBackgroundObject.Depth);
		float num2 = CameraController.Instance.baseCamera.orthographicSize / 1f;
		Vector3 vector3 = vector2 + vector * num2;
		Vector3 vector4 = num * num2 * Vector3.one;
		Quaternion quaternion = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0f, 0f, -20f), this.normalizedDistance);
		this.material.SetFloat("_UnscaledTime", Time.unscaledTime);
		this.material.SetFloat("_SizeProgress", this.normalizedDistance);
		Matrix4x4 matrix4x = Matrix4x4.Translate(vector3) * Matrix4x4.Scale(vector4) * Matrix4x4.Rotate(quaternion);
		Graphics.DrawMesh(ParallaxBackgroundObject.Mesh, matrix4x, this.material, ParallaxBackgroundObject.Layer);
	}

	private static Mesh mesh;

	private static int? layer;

	private static float? depth;

	[SerializeField]
	private Sprite sprite;

	[SerializeField]
	private float parallaxFactor = 1f;

	[Range(0f, 5f)]
	public float scaleMin = 0.25f;

	[Range(0f, 5f)]
	public float scaleMax = 3f;

	[Serialize]
	private bool visible = true;

	private const string SHADER_DAMAGED_TIME_VARIABLE_NAME = "_LastTimeDamaged";

	private const string SHADER_PLAYER_CLICKED_TIME_VARIABLE_NAME = "_LastTimePlayerClickedNotification";

	private const string SHADER_SIZE_PROGRESS_VARIABLE_NAME = "_SizeProgress";

	[SerializeField]
	private Material material;

	[SerializeField]
	[Range(0f, 1f)]
	private float normalizedDistance;

	[SerializeField]
	private bool distanceUpdate;

	[SerializeField]
	private Vector2 startOffset;

	[SerializeField]
	private Vector2 endOffset;

	[Serialize]
	public int? worldId;

	public ParallaxBackgroundObject.IMotion motion;

	public interface IMotion
	{
		float GetETA();

		float GetDuration();

		void OnNormalizedDistanceChanged(float normalizedDistance);
	}
}
