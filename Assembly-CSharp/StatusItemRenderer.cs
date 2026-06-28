using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusItemRenderer
{
	public StatusItemRenderer()
	{
		this.layer = LayerMask.NameToLayer("Overlay");
		this.entries = new StatusItemRenderer.Entry[10000];
		Shader shader = Shader.Find("Klei/StatusItem");
		for (int i = 0; i < this.entries.Length; i++)
		{
			StatusItemRenderer.Entry entry = default(StatusItemRenderer.Entry);
			entry.statusItems = new List<StatusItem>();
			entry.mesh = new Mesh();
			entry.mesh.name = "StatusItem" + i;
			entry.name = string.Empty;
			entry.dirty = true;
			entry.material = new Material(shader);
			this.entries[i] = entry;
		}
		this.backgroundColor = new Color32(244, 74, 71, byte.MaxValue);
		this.selectedColor = new Color32(225, 181, 180, byte.MaxValue);
		this.arrowSprite = Assets.GetSprite("StatusBubbleTop");
		this.backgroundSprite = Assets.GetSprite("StatusBubble");
		this.scale = 1f;
		Game.Instance.Subscribe(2095258329, new EventSystem.EventHandler(this.OnHighlightObject));
	}

	public int layer { get; private set; }

	public int selectedHandle { get; private set; }

	public int highlightHandle { get; private set; }

	public Color32 backgroundColor { get; private set; }

	public Color32 selectedColor { get; private set; }

	public Sprite arrowSprite { get; private set; }

	public Sprite backgroundSprite { get; private set; }

	public float scale { get; private set; }

	public int GetIdx(Transform transform)
	{
		int instanceID = transform.GetInstanceID();
		int num = 0;
		if (!this.handleTable.TryGetValue(instanceID, out num))
		{
			num = this.entryCount++;
			this.handleTable[instanceID] = num;
			StatusItemRenderer.Entry entry = this.entries[num];
			entry.handle = instanceID;
			entry.name = transform.name;
			entry.transform = transform;
			this.entries[num] = entry;
		}
		return num;
	}

	public void Add(Transform transform, StatusItem status_item)
	{
		int idx = this.GetIdx(transform);
		StatusItemRenderer.Entry entry = this.entries[idx];
		entry.Add(status_item);
		this.entries[idx] = entry;
	}

	public void Remove(Transform transform, StatusItem status_item)
	{
		int instanceID = transform.GetInstanceID();
		int num = 0;
		if (!this.handleTable.TryGetValue(instanceID, out num))
		{
			return;
		}
		StatusItemRenderer.Entry entry = this.entries[num];
		if (entry.statusItems.Count == 0)
		{
			return;
		}
		entry.Remove(status_item);
		this.entries[num] = entry;
		if (entry.statusItems.Count == 0)
		{
			this.ClearIdx(num);
		}
	}

	private void ClearIdx(int idx)
	{
		StatusItemRenderer.Entry entry = this.entries[idx];
		this.handleTable.Remove(entry.handle);
		if (idx != this.entryCount - 1)
		{
			entry.Replace(this.entries[this.entryCount - 1]);
			this.entries[idx] = entry;
			this.handleTable[entry.handle] = idx;
		}
		entry = this.entries[this.entryCount - 1];
		entry.Clear();
		this.entries[this.entryCount - 1] = entry;
		this.entryCount--;
	}

	private SimViewMode GetMode()
	{
		if (OverlayScreen.Instance != null)
		{
			return OverlayScreen.Instance.mode;
		}
		return SimViewMode.None;
	}

	public void MarkAllDirty()
	{
		for (int i = 0; i < this.entryCount; i++)
		{
			this.entries[i].MarkDirty();
		}
	}

	public void Render()
	{
		this.scale = 1f + Mathf.Sin(Time.unscaledTime * 8f) * 0.1f;
		Shader.SetGlobalVector("_StatusItemParameters", new Vector4(this.scale, 0f, 0f, 0f));
		Vector3 vector = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.transform.position.z));
		Vector3 vector2 = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.transform.position.z));
		for (int i = 0; i < this.entryCount; i++)
		{
			this.entries[i].Render(this, vector2, vector, this.GetMode());
		}
	}

	public void GetIntersections(Vector2 pos, List<SelectTool.Intersection> intersections)
	{
		for (int i = 0; i < this.entryCount; i++)
		{
			this.entries[i].GetIntersection(pos, intersections, this.scale, this.GetMode());
		}
	}

	public void GetIntersections(Vector2 pos, List<KSelectable> selectables)
	{
		for (int i = 0; i < this.entryCount; i++)
		{
			this.entries[i].GetIntersection(pos, selectables, this.scale, this.GetMode());
		}
	}

	public void SetOffset(Transform transform, Vector3 offset)
	{
		int num = 0;
		if (this.handleTable.TryGetValue(transform.GetInstanceID(), out num))
		{
			this.entries[num].offset = offset;
		}
	}

	private void OnSelectObject(object data)
	{
		int num = 0;
		if (this.handleTable.TryGetValue(this.selectedHandle, out num))
		{
			this.entries[num].MarkDirty();
		}
		GameObject gameObject = (GameObject)data;
		if (gameObject != null)
		{
			this.selectedHandle = gameObject.transform.GetInstanceID();
			if (this.handleTable.TryGetValue(this.selectedHandle, out num))
			{
				this.entries[num].MarkDirty();
			}
		}
		else
		{
			this.highlightHandle = -1;
		}
	}

	private void OnHighlightObject(object data)
	{
		int num = 0;
		if (this.handleTable.TryGetValue(this.highlightHandle, out num))
		{
			StatusItemRenderer.Entry entry = this.entries[num];
			entry.MarkDirty();
			this.entries[num] = entry;
		}
		GameObject gameObject = (GameObject)data;
		if (gameObject != null)
		{
			this.highlightHandle = gameObject.transform.GetInstanceID();
			if (this.handleTable.TryGetValue(this.highlightHandle, out num))
			{
				StatusItemRenderer.Entry entry2 = this.entries[num];
				entry2.MarkDirty();
				this.entries[num] = entry2;
			}
		}
		else
		{
			this.highlightHandle = -1;
		}
	}

	public void Destroy()
	{
		Game.Instance.Unsubscribe(-1503271301, new EventSystem.EventHandler(this.OnSelectObject));
		Game.Instance.Unsubscribe(-1201923725, new EventSystem.EventHandler(this.OnHighlightObject));
	}

	private StatusItemRenderer.Entry[] entries;

	private int entryCount;

	private Dictionary<int, int> handleTable = new Dictionary<int, int>();

	private struct Entry
	{
		public void Render(StatusItemRenderer renderer, Vector3 camera_bl, Vector3 camera_tr, SimViewMode overlay)
		{
			Vector3 vector = Vector3.zero;
			if (!(this.transform != null))
			{
				string text = "Error cleaning up status items on " + this.name + ":";
				foreach (StatusItem statusItem in this.statusItems)
				{
					text += statusItem.Id;
				}
				Debug.LogWarning(text);
				return;
			}
			vector = this.transform.position;
			if (vector.x < camera_bl.x || vector.x > camera_tr.x || vector.y < camera_bl.y || vector.y > camera_tr.y)
			{
				return;
			}
			if (this.dirty)
			{
				int num = 0;
				foreach (StatusItem statusItem2 in this.statusItems)
				{
					bool flag = overlay != SimViewMode.None && statusItem2.conditionalOverlayCallback != null && statusItem2.conditionalOverlayCallback(overlay, this.transform);
					if (overlay == SimViewMode.None || statusItem2.overlay == overlay || flag)
					{
						num++;
					}
				}
				this.hasVisibleStatusItems = num != 0;
				StatusItemRenderer.Entry.MeshBuilder meshBuilder = new StatusItemRenderer.Entry.MeshBuilder(num + 6, this.material);
				float num2 = 0.25f;
				float num3 = -5f;
				Vector2 vector2 = new Vector2(0.05f, -0.05f);
				float num4 = 0.02f;
				Color32 color = new Color32(0, 0, 0, byte.MaxValue);
				Color32 color2 = new Color32(0, 0, 0, 75);
				Color32 color3 = renderer.backgroundColor;
				if (renderer.selectedHandle == this.handle || renderer.highlightHandle == this.handle)
				{
					color3 = renderer.selectedColor;
				}
				meshBuilder.AddQuad(new Vector2(0f, 0.29f) + vector2, new Vector2(0.05f, 0.05f), num3, renderer.arrowSprite, color2);
				meshBuilder.AddQuad(new Vector2(0f, 0f) + vector2, new Vector2(num2 * (float)num, num2), num3, renderer.backgroundSprite, color2);
				meshBuilder.AddQuad(new Vector2(0f, 0f), new Vector2(num2 * (float)num + num4, num2 + num4), num3, renderer.backgroundSprite, color);
				meshBuilder.AddQuad(new Vector2(0f, 0f), new Vector2(num2 * (float)num, num2), num3, renderer.backgroundSprite, color3);
				int num5 = 0;
				for (int i = 0; i < this.statusItems.Count; i++)
				{
					bool flag2 = overlay != SimViewMode.None && this.statusItems[i].conditionalOverlayCallback != null && this.statusItems[i].conditionalOverlayCallback(overlay, this.transform);
					if (overlay == SimViewMode.None || this.statusItems[i].overlay == overlay || flag2)
					{
						float num6 = (float)num5 * num2 * 2f - num2 * (float)(num - 1);
						Sprite sprite = this.statusItems[i].sprite.sprite;
						meshBuilder.AddQuad(new Vector2(num6, 0f), new Vector2(num2, num2), num3, sprite, color);
						num5++;
					}
				}
				meshBuilder.AddQuad(new Vector2(0f, 0.29f + num4), new Vector2(0.05f + num4, 0.05f + num4), num3, renderer.arrowSprite, color);
				meshBuilder.AddQuad(new Vector2(0f, 0.29f), new Vector2(0.05f, 0.05f), num3, renderer.arrowSprite, color3);
				meshBuilder.End(this.mesh);
				this.dirty = false;
			}
			if (this.hasVisibleStatusItems)
			{
				Graphics.DrawMesh(this.mesh, vector + this.offset, Quaternion.identity, this.material, renderer.layer, null, 0, null, false, false);
			}
		}

		public void Add(StatusItem status_item)
		{
			this.statusItems.Add(status_item);
			this.dirty = true;
		}

		public void Remove(StatusItem status_item)
		{
			this.statusItems.Remove(status_item);
			this.dirty = true;
		}

		public void Replace(StatusItemRenderer.Entry entry)
		{
			this.handle = entry.handle;
			this.transform = entry.transform;
			this.dirty = true;
			this.name = entry.name;
			this.statusItems.Clear();
			this.statusItems.AddRange(entry.statusItems);
		}

		private bool Intersects(Vector2 pos, float scale, SimViewMode overlay)
		{
			Vector3 vector = this.transform.position + this.offset;
			Bounds bounds = this.mesh.bounds;
			bounds.size *= scale;
			bounds.center += vector;
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			return pos.x >= min.x && pos.x <= max.x && pos.y >= min.y && pos.y <= max.y;
		}

		public void GetIntersection(Vector2 pos, List<SelectTool.Intersection> intersections, float scale, SimViewMode overlay)
		{
			if (this.Intersects(pos, scale, overlay))
			{
				intersections.Add(new SelectTool.Intersection
				{
					component = this.transform.GetComponent<KSelectable>(),
					distance = -100f
				});
			}
		}

		public void GetIntersection(Vector2 pos, List<KSelectable> selectables, float scale, SimViewMode overlay)
		{
			if (this.Intersects(pos, scale, overlay))
			{
				KSelectable component = this.transform.GetComponent<KSelectable>();
				if (!selectables.Contains(component))
				{
					selectables.Add(component);
				}
			}
		}

		public void Clear()
		{
			this.statusItems.Clear();
			this.dirty = false;
		}

		public void MarkDirty()
		{
			this.dirty = true;
		}

		public int handle;

		public Transform transform;

		public List<StatusItem> statusItems;

		public Mesh mesh;

		public bool dirty;

		public int layer;

		public Material material;

		public string name;

		public Vector3 offset;

		public bool hasVisibleStatusItems;

		private struct MeshBuilder
		{
			public MeshBuilder(int quad_count, Material material)
			{
				this.vertices = new Vector3[4 * quad_count];
				this.uvs = new Vector2[4 * quad_count];
				this.uv2s = new Vector2[4 * quad_count];
				this.colors = new Color32[4 * quad_count];
				this.triangles = new int[6 * quad_count];
				this.material = material;
				this.quadIdx = 0;
			}

			public void AddQuad(Vector2 center, Vector2 half_size, float z, Sprite sprite, Color color)
			{
				if (this.quadIdx == StatusItemRenderer.Entry.MeshBuilder.textureIds.Length)
				{
					return;
				}
				Rect rect = sprite.rect;
				Rect textureRect = sprite.textureRect;
				float num = textureRect.width / rect.width;
				float num2 = textureRect.height / rect.height;
				int num3 = 4 * this.quadIdx;
				this.vertices[0 + num3] = new Vector3((center.x - half_size.x) * num, (center.y - half_size.y) * num2, z);
				this.vertices[1 + num3] = new Vector3((center.x - half_size.x) * num, (center.y + half_size.y) * num2, z);
				this.vertices[2 + num3] = new Vector3((center.x + half_size.x) * num, (center.y - half_size.y) * num2, z);
				this.vertices[3 + num3] = new Vector3((center.x + half_size.x) * num, (center.y + half_size.y) * num2, z);
				float num4 = textureRect.x / (float)sprite.texture.width;
				float num5 = textureRect.y / (float)sprite.texture.height;
				float num6 = textureRect.width / (float)sprite.texture.width;
				float num7 = textureRect.height / (float)sprite.texture.height;
				this.uvs[0 + num3] = new Vector2(num4, num5);
				this.uvs[1 + num3] = new Vector2(num4, num5 + num7);
				this.uvs[2 + num3] = new Vector2(num4 + num6, num5);
				this.uvs[3 + num3] = new Vector2(num4 + num6, num5 + num7);
				this.colors[0 + num3] = color;
				this.colors[1 + num3] = color;
				this.colors[2 + num3] = color;
				this.colors[3 + num3] = color;
				float num8 = (float)this.quadIdx + 0.5f;
				this.uv2s[0 + num3] = new Vector2(num8, 0f);
				this.uv2s[1 + num3] = new Vector2(num8, 0f);
				this.uv2s[2 + num3] = new Vector2(num8, 0f);
				this.uv2s[3 + num3] = new Vector2(num8, 0f);
				int num9 = 6 * this.quadIdx;
				this.triangles[0 + num9] = num3;
				this.triangles[1 + num9] = num3 + 1;
				this.triangles[2 + num9] = num3 + 2;
				this.triangles[3 + num9] = num3 + 2;
				this.triangles[4 + num9] = num3 + 1;
				this.triangles[5 + num9] = num3 + 3;
				this.material.SetTexture(StatusItemRenderer.Entry.MeshBuilder.textureIds[this.quadIdx], sprite.texture);
				this.quadIdx++;
			}

			public void End(Mesh mesh)
			{
				mesh.Clear();
				mesh.vertices = this.vertices;
				mesh.uv = this.uvs;
				mesh.uv2 = this.uv2s;
				mesh.colors32 = this.colors;
				mesh.SetTriangles(this.triangles, 0);
				mesh.RecalculateBounds();
			}

			private Vector3[] vertices;

			private Vector2[] uvs;

			private Vector2[] uv2s;

			private int[] triangles;

			private Color32[] colors;

			private int quadIdx;

			private Material material;

			private static int[] textureIds = new int[]
			{
				Shader.PropertyToID("_Tex0"),
				Shader.PropertyToID("_Tex1"),
				Shader.PropertyToID("_Tex2"),
				Shader.PropertyToID("_Tex3"),
				Shader.PropertyToID("_Tex4"),
				Shader.PropertyToID("_Tex5"),
				Shader.PropertyToID("_Tex6"),
				Shader.PropertyToID("_Tex7"),
				Shader.PropertyToID("_Tex8"),
				Shader.PropertyToID("_Tex9"),
				Shader.PropertyToID("_Tex10")
			};
		}
	}
}
