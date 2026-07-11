using System;
using System.Collections.Generic;
using UnityEngine;

public class PrioritizableRenderer
{
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

	public void RenderEveryTick()
	{
		using (new KProfiler.Region("PrioritizableRenderer", null))
		{
			if (!(GameScreenManager.Instance == null))
			{
				if (!(SimDebugView.Instance == null) && !(SimDebugView.Instance.GetMode() != OverlayModes.Priorities.ID))
				{
					this.prioritizables.Clear();
					for (int i = 0; i < Components.Prioritizables.Count; i++)
					{
						Prioritizable prioritizable = Components.Prioritizables[i];
						if (prioritizable != null && prioritizable.showIcon && prioritizable.IsPrioritizable())
						{
							int num = Grid.PosToCell(prioritizable);
							if (Grid.IsVisible(num))
							{
								this.prioritizables.Add(prioritizable);
							}
						}
					}
					if (this.prioritizableCount != this.prioritizables.Count)
					{
						this.prioritizableCount = this.prioritizables.Count;
						this.vertices = new Vector3[4 * this.prioritizableCount];
						this.uvs = new Vector2[4 * this.prioritizableCount];
						this.triangles = new int[6 * this.prioritizableCount];
					}
					if (this.prioritizableCount != 0)
					{
						for (int j = 0; j < this.prioritizables.Count; j++)
						{
							Prioritizable prioritizable2 = this.prioritizables[j];
							Vector3 vector = Vector3.zero;
							KAnimControllerBase component = prioritizable2.GetComponent<KAnimControllerBase>();
							if (component != null)
							{
								vector = component.GetWorldPivot();
							}
							else
							{
								vector = prioritizable2.transform.GetPosition();
							}
							vector.x += prioritizable2.iconOffset.x;
							vector.y += prioritizable2.iconOffset.y;
							Vector2 vector2 = new Vector2(0.2f, 0.3f) * prioritizable2.iconScale;
							float num2 = -5f;
							int num3 = 4 * j;
							this.vertices[num3] = new Vector3(vector.x - vector2.x, vector.y - vector2.y, num2);
							this.vertices[1 + num3] = new Vector3(vector.x - vector2.x, vector.y + vector2.y, num2);
							this.vertices[2 + num3] = new Vector3(vector.x + vector2.x, vector.y - vector2.y, num2);
							this.vertices[3 + num3] = new Vector3(vector.x + vector2.x, vector.y + vector2.y, num2);
							float num4 = 0.1f;
							PrioritySetting masterPriority = prioritizable2.GetMasterPriority();
							float num5 = -1f;
							if (masterPriority.priority_class >= PriorityScreen.PriorityClass.high)
							{
								num5 += 9f;
							}
							if (masterPriority.priority_class >= PriorityScreen.PriorityClass.emergency)
							{
								num5 = num5;
							}
							num5 += (float)masterPriority.priority_value;
							float num6 = num4 * num5;
							float num7 = 0f;
							float num8 = num4;
							float num9 = 1f;
							this.uvs[num3] = new Vector2(num6, num7);
							this.uvs[1 + num3] = new Vector2(num6, num7 + num9);
							this.uvs[2 + num3] = new Vector2(num6 + num8, num7);
							this.uvs[3 + num3] = new Vector2(num6 + num8, num7 + num9);
							int num10 = 6 * j;
							this.triangles[num10] = num3;
							this.triangles[1 + num10] = num3 + 1;
							this.triangles[2 + num10] = num3 + 2;
							this.triangles[3 + num10] = num3 + 2;
							this.triangles[4 + num10] = num3 + 1;
							this.triangles[5 + num10] = num3 + 3;
						}
						this.mesh.Clear();
						this.mesh.vertices = this.vertices;
						this.mesh.uv = this.uvs;
						this.mesh.SetTriangles(this.triangles, 0);
						this.mesh.RecalculateBounds();
						Graphics.DrawMesh(this.mesh, Vector3.zero, Quaternion.identity, this.material, this.layer, GameScreenManager.Instance.worldSpaceCanvas.GetComponent<Canvas>().worldCamera, 0, null, false, false);
					}
				}
			}
		}
	}

	private Mesh mesh;

	private int layer;

	private Material material;

	private int prioritizableCount;

	private Vector3[] vertices;

	private Vector2[] uvs;

	private int[] triangles;

	private List<Prioritizable> prioritizables;
}
