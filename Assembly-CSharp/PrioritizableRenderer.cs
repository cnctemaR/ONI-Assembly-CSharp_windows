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
		this.vertices = null;
		this.uvs = null;
		this.prioritizables = null;
		global::UnityEngine.Object.DestroyImmediate(this.mesh);
	}

	public void Render()
	{
		using (new KProfiler.Region("PrioritizableRenderer", null))
		{
			if (!(GameScreenManager.Instance == null))
			{
				if (!(SimDebugView.Instance == null) && SimDebugView.Instance.GetMode() == SimViewMode.Priorities)
				{
					this.prioritizables.Clear();
					for (int i = 0; i < Components.Prioritizables.Count; i++)
					{
						Prioritizable prioritizable = Components.Prioritizables[i];
						if (prioritizable != null && prioritizable.showIcon)
						{
							this.prioritizables.Add(prioritizable);
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
								vector = prioritizable2.transform.position;
							}
							Vector2 vector2 = new Vector2(0.2f, 0.3f);
							float num = -5f;
							int num2 = 4 * j;
							this.vertices[0 + num2] = new Vector3(vector.x - vector2.x, vector.y - vector2.y, num);
							this.vertices[1 + num2] = new Vector3(vector.x - vector2.x, vector.y + vector2.y, num);
							this.vertices[2 + num2] = new Vector3(vector.x + vector2.x, vector.y - vector2.y, num);
							this.vertices[3 + num2] = new Vector3(vector.x + vector2.x, vector.y + vector2.y, num);
							float num3 = 0.11111111f;
							float num4 = (float)(prioritizable2.GetMasterPriority() - 1);
							float num5 = num3 * num4;
							float num6 = 0f;
							float num7 = num3;
							float num8 = 1f;
							this.uvs[0 + num2] = new Vector2(num5, num6);
							this.uvs[1 + num2] = new Vector2(num5, num6 + num8);
							this.uvs[2 + num2] = new Vector2(num5 + num7, num6);
							this.uvs[3 + num2] = new Vector2(num5 + num7, num6 + num8);
							int num9 = 6 * j;
							this.triangles[0 + num9] = num2;
							this.triangles[1 + num9] = num2 + 1;
							this.triangles[2 + num9] = num2 + 2;
							this.triangles[3 + num9] = num2 + 2;
							this.triangles[4 + num9] = num2 + 1;
							this.triangles[5 + num9] = num2 + 3;
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
