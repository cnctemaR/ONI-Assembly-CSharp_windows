using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portrait : KMonoBehaviour
{
	public bool ShouldCapture
	{
		get
		{
			return this.go != null && (!this.singleCapture || !this.hasCaptured);
		}
	}

	protected override void OnPrefabInit()
	{
		this.portraitLayer = LayerMask.NameToLayer("Portrait");
		this.portraitCamera = base.GetComponent<Camera>();
		this.light = base.GetComponentInChildren<Light>();
		this.light.enabled = false;
	}

	public RenderTexture CreateTexture(int width, int height)
	{
		RenderTexture renderTexture = new RenderTexture(width, height, 32);
		renderTexture.filterMode = FilterMode.Trilinear;
		this.portraitCamera.aspect = (float)(width / height);
		this.portraitCamera.targetTexture = renderTexture;
		return this.portraitCamera.targetTexture;
	}

	public void SetTarget(GameObject go)
	{
		this.go = go;
	}

	private Renderer GetRenderer(GameObject go)
	{
		return go.GetComponent<MeshRenderer>();
	}

	private void SetLayer(GameObject go)
	{
		float num = 0f;
		Renderer renderer = this.GetRenderer(go);
		if (renderer != null && this.maintainHighlight)
		{
			num = renderer.material.GetFloat("_Highlight");
			renderer.material.SetFloat("_Highlight", 0f);
		}
		Portrait.PortraitObject portraitObject = new Portrait.PortraitObject
		{
			go = go,
			layer = go.layer,
			highlight = num
		};
		this.portraitObjects.Add(portraitObject);
		go.layer = this.portraitLayer;
		IEnumerator enumerator = go.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				this.SetLayer(transform.gameObject);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
	}

	private void OnPreCull()
	{
		if (this.ShouldCapture)
		{
			this.light.enabled = true;
			this.portraitObjects.Clear();
			this.SetLayer(this.go);
		}
	}

	private void OnPostRender()
	{
		if (this.ShouldCapture)
		{
			this.light.enabled = false;
			int count = this.portraitObjects.Count;
			for (int i = 0; i < count; i++)
			{
				this.portraitObjects[i].go.layer = this.portraitObjects[i].layer;
				Renderer renderer = this.GetRenderer(this.portraitObjects[i].go);
				if (renderer != null && this.maintainHighlight)
				{
					renderer.material.SetFloat("_Highlight", this.portraitObjects[i].highlight);
				}
			}
		}
		if (!this.firstCapture)
		{
			this.hasCaptured = true;
		}
		this.firstCapture = false;
	}

	private void Update()
	{
		if (this.ShouldCapture)
		{
			KSelectable component = this.go.GetComponent<KSelectable>();
			Vector3 vector = this.go.transform.position;
			if (component != null)
			{
				vector = component.GetPortraitLocation();
				this.portraitCamera.orthographicSize = component.GetZoom();
			}
			else
			{
				Bounds bounds = Util.GetBounds(this.go);
				this.portraitCamera.orthographicSize = 1.05f * Mathf.Max(bounds.extents.x, bounds.extents.y);
				vector = bounds.center;
			}
			vector = new Vector3(vector.x, vector.y, vector.z - 20f);
			this.portraitCamera.transform.localPosition = vector;
		}
		else if (this.go != null)
		{
			this.portraitCamera.gameObject.SetActive(false);
		}
	}

	protected override void OnCleanUp()
	{
		if (this.destroyTargetOnCleanup)
		{
			if (this.go != null)
			{
				global::UnityEngine.Object.Destroy(this.go);
			}
		}
	}

	private List<Portrait.PortraitObject> portraitObjects = new List<Portrait.PortraitObject>();

	private int portraitLayer;

	public Camera portraitCamera;

	public GameObject go;

	public bool maintainHighlight;

	public bool destroyTargetOnCleanup;

	public bool singleCapture;

	private bool hasCaptured;

	private bool firstCapture = true;

	private Light light;

	private struct PortraitObject
	{
		public GameObject go;

		public int layer;

		public float highlight;
	}
}
