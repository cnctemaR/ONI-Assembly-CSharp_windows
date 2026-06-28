using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.World
{
	public class Brush
	{
		public Brush(int id, string name, Material material, Mask mask, List<Brush> active_brushes, List<Brush> dirty_brushes, int width_in_tiles, MaterialPropertyBlock property_block)
		{
			this.Id = id;
			this.material = material;
			this.mask = mask;
			this.mesh = new DynamicMesh(name, new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, 0f)));
			this.activeBrushes = active_brushes;
			this.dirtyBrushes = dirty_brushes;
			this.layer = LayerMask.NameToLayer("World");
			this.widthInTiles = width_in_tiles;
			this.propertyBlock = property_block;
		}

		public int Id { get; private set; }

		public void Add(int tile_idx)
		{
			this.tiles.Add(tile_idx);
			if (!this.dirty)
			{
				this.dirtyBrushes.Add(this);
				this.dirty = true;
			}
		}

		public void Remove(int tile_idx)
		{
			this.tiles.Remove(tile_idx);
			if (!this.dirty)
			{
				this.dirtyBrushes.Add(this);
				this.dirty = true;
			}
		}

		public void SetMaskOffset(int offset)
		{
			this.mask.SetOffset(offset);
		}

		public void Refresh()
		{
			bool flag = this.mesh.Meshes.Length > 0;
			int count = this.tiles.Count;
			int num = count * 4;
			int num2 = count * 6;
			this.mesh.Reserve(num, num2);
			if (this.mesh.SetTriangles)
			{
				int num3 = 0;
				for (int i = 0; i < count; i++)
				{
					this.mesh.AddTriangle(0 + num3);
					this.mesh.AddTriangle(2 + num3);
					this.mesh.AddTriangle(1 + num3);
					this.mesh.AddTriangle(1 + num3);
					this.mesh.AddTriangle(2 + num3);
					this.mesh.AddTriangle(3 + num3);
					num3 += 4;
				}
			}
			foreach (int num4 in this.tiles)
			{
				float num5 = (float)(num4 % this.widthInTiles);
				float num6 = (float)(num4 / this.widthInTiles);
				float num7 = 0f;
				this.mesh.AddVertex(new Vector3(num5 - 0.5f, num6 - 0.5f, num7));
				this.mesh.AddVertex(new Vector3(num5 + 0.5f, num6 - 0.5f, num7));
				this.mesh.AddVertex(new Vector3(num5 - 0.5f, num6 + 0.5f, num7));
				this.mesh.AddVertex(new Vector3(num5 + 0.5f, num6 + 0.5f, num7));
			}
			if (this.mesh.SetUVs)
			{
				for (int j = 0; j < count; j++)
				{
					this.mesh.AddUV(this.mask.UV0);
					this.mesh.AddUV(this.mask.UV1);
					this.mesh.AddUV(this.mask.UV2);
					this.mesh.AddUV(this.mask.UV3);
				}
			}
			this.dirty = false;
			this.mesh.Commit();
			if (this.mesh.Meshes.Length > 0)
			{
				if (!flag)
				{
					this.activeBrushes.Add(this);
				}
			}
			else if (flag)
			{
				this.activeBrushes.Remove(this);
			}
		}

		public void Render()
		{
			Vector3 vector = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.Ground));
			this.mesh.Render(vector, Quaternion.identity, this.material, this.layer, this.propertyBlock);
		}

		public void SetMaterial(Material material, MaterialPropertyBlock property_block)
		{
			this.material = material;
			this.propertyBlock = property_block;
		}

		private bool dirty;

		private Material material;

		private int layer;

		private HashSet<int> tiles = new HashSet<int>();

		private List<Brush> activeBrushes;

		private List<Brush> dirtyBrushes;

		private int widthInTiles;

		private Mask mask;

		private DynamicMesh mesh;

		private MaterialPropertyBlock propertyBlock;
	}
}
