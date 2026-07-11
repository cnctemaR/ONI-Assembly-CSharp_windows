using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/OrbitalMechanics")]
public class OrbitalMechanics : KMonoBehaviour, IRenderEveryTick
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.orbitingObjects = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Rebuild();
	}

	public void RenderEveryTick(float dt)
	{
		if (this.orbitData == null)
		{
			return;
		}
		if (this.orbitingObjects == null)
		{
			return;
		}
		float time = GameClock.Instance.GetTime();
		for (int i = 0; i < this.orbitingObjects.Length; i++)
		{
			OrbitalMechanics.OrbitData orbitData = this.orbitData[i];
			bool flag;
			Vector3 vector = this.CalculatePos(ref orbitData, time, out flag);
			vector.y -= 0.5f;
			Vector3 vector2 = vector;
			vector2.x = Camera.main.ViewportToWorldPoint(vector).x;
			vector2.y = Camera.main.ViewportToWorldPoint(vector).y;
			bool flag2 = !orbitData.rotatesBehind || !flag;
			GameObject gameObject = this.orbitingObjects[i];
			if (gameObject != null)
			{
				gameObject.transform.SetPosition(vector2);
				gameObject.transform.localScale = Vector3.one * Camera.main.orthographicSize / orbitData.distance;
				if (gameObject.activeSelf != flag2)
				{
					gameObject.SetActive(flag2);
				}
			}
		}
	}

	[ContextMenu("Rebuild")]
	private void Rebuild()
	{
		if (this.orbitingObjects != null)
		{
			GameObject[] array = this.orbitingObjects;
			for (int i = 0; i < array.Length; i++)
			{
				Util.KDestroyGameObject(array[i]);
			}
			this.orbitingObjects = null;
		}
		if (this.orbitData != null && this.orbitData.Length != 0)
		{
			float time = GameClock.Instance.GetTime();
			this.orbitingObjects = new GameObject[this.orbitData.Length];
			for (int j = 0; j < this.orbitData.Length; j++)
			{
				OrbitalMechanics.OrbitData orbitData = this.orbitData[j];
				GameObject prefab = Assets.GetPrefab(orbitData.prefabTag);
				bool flag;
				Vector3 vector = this.CalculatePos(ref orbitData, time, out flag);
				GameObject gameObject = Util.KInstantiate(prefab, vector);
				gameObject.SetActive(true);
				this.orbitingObjects[j] = gameObject;
			}
		}
	}

	private Vector3 CalculatePos(ref OrbitalMechanics.OrbitData data, float time, out bool behind)
	{
		float num = data.periodInCycles * 600f;
		float num2 = (this.applyOverrides ? (this.overridePercent / 100f) : (time / num - (float)((int)(time / num)))) * 2f * 3.1415927f;
		float num3 = 0.5f * data.radiusScale;
		float yGridPercent = data.yGridPercent;
		Vector3 vector = new Vector3(0.5f, yGridPercent, 0f);
		Vector3 vector2 = new Vector3(Mathf.Cos(num2), 0f, Mathf.Sin(num2));
		behind = vector2.z > data.behindZ;
		Vector3 vector3 = Quaternion.Euler(data.angle, 0f, 0f) * (vector2 * num3);
		Vector3 vector4 = vector + vector3;
		vector4.z = data.renderZ;
		return vector4;
	}

	[SerializeField]
	private OrbitalMechanics.OrbitData[] orbitData;

	[SerializeField]
	private bool applyOverrides;

	[SerializeField]
	[Range(0f, 100f)]
	private float overridePercent;

	[SerializeField]
	private GameObject[] orbitingObjects;

	[Serializable]
	private struct OrbitData
	{
		public string prefabTag;

		public float periodInCycles;

		public float yGridPercent;

		public float angle;

		public float radiusScale;

		public bool rotatesBehind;

		public float behindZ;

		public Vector3 scale;

		public float distance;

		public float renderZ;
	}
}
