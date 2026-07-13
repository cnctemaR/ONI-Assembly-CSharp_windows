using System;
using System.Collections.Generic;
using UnityEngine;

public class PrioritizableRenderer
{
	public PrioritizeTool currentTool
	{
		get
		{
			return this.tool;
		}
		set
		{
			this.tool = value;
		}
	}

	public PrioritizableRenderer()
	{
		this.layer = LayerMask.NameToLayer("UI");
		Shader shader = Shader.Find("Klei/Prioritizable");
		Texture2D texture = Assets.GetTexture("priority_overlay_atlas");
		this.material = new Material(shader);
		this.material.SetTexture(Shader.PropertyToID("_MainTex"), texture);
		this.prioritizables = new List<Prioritizable>();
		this.mesh = new Mesh();
		this.mesh.name = "Prioritizables";
		this.mesh.MarkDynamic();
	}

	public void Cleanup()
	{
		this.material = null;
		this.vertices = null;
		this.uvs = null;
		this.prioritizables = null;
		this.triangles = null;
		global::UnityEngine.Object.DestroyImmediate(this.mesh);
		this.mesh = null;
	}

	private static Util.IterationInstruction renderEveryTickVisitHelper(object obj, PrioritizableRenderer self)
	{
		Prioritizable prioritizable = (Prioritizable)obj;
		if (prioritizable != null && prioritizable.showIcon && prioritizable.IsPrioritizable() && self.tool.IsActiveLayer(self.tool.GetFilterLayerFromGameObject(prioritizable.gameObject)) && prioritizable.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
		{
			self.prioritizables.Add(prioritizable);
		}
		return Util.IterationInstruction.Continue;
	}

	public void RenderEveryTick()
	{
		if (GameScreenManager.Instance == null)
		{
			return;
		}
		if (SimDebugView.Instance == null || SimDebugView.Instance.GetMode() != OverlayModes.Priorities.ID)
		{
			return;
		}
		this.prioritizables.Clear();
		Vector2I vector2I;
		Vector2I vector2I2;
		Grid.GetVisibleExtents(out vector2I, out vector2I2);
		int num = vector2I2.y - vector2I.y;
		int num2 = vector2I2.x - vector2I.x;
		Extents extents = new Extents(vector2I.x, vector2I.y, num2, num);
		GameScenePartitioner.Instance.VisitEntries<PrioritizableRenderer>(extents.x, extents.y, extents.width, extents.height, GameScenePartitioner.Instance.prioritizableObjects, new Func<object, PrioritizableRenderer, Util.IterationInstruction>(PrioritizableRenderer.renderEveryTickVisitHelper), this);
		if (this.prioritizableCount != this.prioritizables.Count)
		{
			this.prioritizableCount = this.prioritizables.Count;
			this.vertices = new Vector3[4 * this.prioritizableCount];
			this.uvs = new Vector2[4 * this.prioritizableCount];
			this.triangles = new int[6 * this.prioritizableCount];
		}
		if (this.prioritizableCount == 0)
		{
			return;
		}
		for (int i = 0; i < this.prioritizables.Count; i++)
		{
			Prioritizable prioritizable = this.prioritizables[i];
			Vector3 vector = Vector3.zero;
			KAnimControllerBase component = prioritizable.GetComponent<KAnimControllerBase>();
			if (component != null)
			{
				vector = component.GetWorldPivot();
			}
			else
			{
				vector = prioritizable.transform.GetPosition();
			}
			vector.x += prioritizable.iconOffset.x;
			vector.y += prioritizable.iconOffset.y;
			Vector2 vector2 = new Vector2(0.2f, 0.3f) * prioritizable.iconScale;
			float num3 = -5f;
			int num4 = 4 * i;
			this.vertices[num4] = new Vector3(vector.x - vector2.x, vector.y - vector2.y, num3);
			this.vertices[1 + num4] = new Vector3(vector.x - vector2.x, vector.y + vector2.y, num3);
			this.vertices[2 + num4] = new Vector3(vector.x + vector2.x, vector.y - vector2.y, num3);
			this.vertices[3 + num4] = new Vector3(vector.x + vector2.x, vector.y + vector2.y, num3);
			float num5 = 0.1f;
			PrioritySetting masterPriority = prioritizable.GetMasterPriority();
			float num6 = -1f;
			if (masterPriority.priority_class >= PriorityScreen.PriorityClass.high)
			{
				num6 += 9f;
			}
			if (masterPriority.priority_class >= PriorityScreen.PriorityClass.topPriority)
			{
				num6 += 0f;
			}
			num6 += (float)masterPriority.priority_value;
			float num7 = num5 * num6;
			float num8 = 0f;
			float num9 = num5;
			float num10 = 1f;
			this.uvs[num4] = new Vector2(num7, num8);
			this.uvs[1 + num4] = new Vector2(num7, num8 + num10);
			this.uvs[2 + num4] = new Vector2(num7 + num9, num8);
			this.uvs[3 + num4] = new Vector2(num7 + num9, num8 + num10);
			int num11 = 6 * i;
			this.triangles[num11] = num4;
			this.triangles[1 + num11] = num4 + 1;
			this.triangles[2 + num11] = num4 + 2;
			this.triangles[3 + num11] = num4 + 2;
			this.triangles[4 + num11] = num4 + 1;
			this.triangles[5 + num11] = num4 + 3;
		}
		this.mesh.Clear();
		this.mesh.vertices = this.vertices;
		this.mesh.uv = this.uvs;
		this.mesh.SetTriangles(this.triangles, 0);
		this.mesh.RecalculateBounds();
		Graphics.DrawMesh(this.mesh, Vector3.zero, Quaternion.identity, this.material, this.layer, GameScreenManager.Instance.worldSpaceCanvas.GetComponent<Canvas>().worldCamera, 0, null, false, false);
	}

	private Mesh mesh;

	private int layer;

	private Material material;

	private int prioritizableCount;

	private Vector3[] vertices;

	private Vector2[] uvs;

	private int[] triangles;

	private List<Prioritizable> prioritizables;

	private PrioritizeTool tool;
}
