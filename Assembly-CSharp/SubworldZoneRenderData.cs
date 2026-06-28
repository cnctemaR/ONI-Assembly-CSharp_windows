using System;
using Delaunay.Geo;
using Klei;
using UnityEngine;

public class SubworldZoneRenderData : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.regionTex = new Texture2D(Grid.WidthInCells, Grid.HeightInCells, TextureFormat.ARGB32, false);
		this.regionTex.name = "SubworldRegionData";
		this.regionTex.filterMode = FilterMode.Bilinear;
		this.regionTex.wrapMode = TextureWrapMode.Clamp;
		this.regionTex.anisoLevel = 0;
		byte[] array = new byte[Grid.WidthInCells * Grid.HeightInCells * 4];
		this.worldZoneTypes = new SubWorld.ZoneType[Grid.CellCount];
		WorldDetailSave worldDetailSave = SaveLoader.Instance.worldDetailSave;
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < worldDetailSave.overworldCells.Count; i++)
		{
			WorldDetailSave.OverworldCell overworldCell = worldDetailSave.overworldCells[i];
			Polygon poly = overworldCell.poly;
			zero.y = (float)((int)Mathf.Floor(poly.bounds.yMin));
			while (zero.y < Mathf.Ceil(poly.bounds.yMax))
			{
				zero.x = (float)((int)Mathf.Floor(poly.bounds.xMin));
				while (zero.x < Mathf.Ceil(poly.bounds.xMax))
				{
					if (poly.Contains(zero))
					{
						Color32 zoneColor = this.GetZoneColor(overworldCell.zoneType);
						int num = (int)(zero.x + zero.y * (float)Grid.WidthInCells) * 4;
						array[num] = zoneColor.a;
						array[num + 1] = zoneColor.r;
						array[num + 2] = zoneColor.g;
						array[num + 3] = zoneColor.b;
						int num2 = Grid.XYToCell((int)zero.x, (int)zero.y);
						if (Grid.IsValidCell(num2))
						{
							this.worldZoneTypes[num2] = overworldCell.zoneType;
						}
					}
					zero.x += 1f;
				}
				zero.y += 1f;
			}
		}
		this.regionTex.LoadRawTextureData(array);
		this.regionTex.Apply();
		this.OnShadersReloaded();
		ShaderReloader.Register(new global::System.Action(this.OnShadersReloaded));
	}

	private void OnShadersReloaded()
	{
		Shader.SetGlobalTexture("_WorldZoneTex", this.regionTex);
	}

	public SubWorld.ZoneType GetSubWorldZoneType(int cell)
	{
		if (cell >= 0 && cell < this.worldZoneTypes.Length)
		{
			return this.worldZoneTypes[cell];
		}
		return SubWorld.ZoneType.Sandstone;
	}

	private SubWorld.ZoneType GetSubWorldZoneType(Vector2I pos)
	{
		WorldDetailSave worldDetailSave = SaveLoader.Instance.worldDetailSave;
		if (worldDetailSave != null)
		{
			for (int i = 0; i < worldDetailSave.overworldCells.Count; i++)
			{
				if (worldDetailSave.overworldCells[i].poly.Contains(pos))
				{
					return worldDetailSave.overworldCells[i].zoneType;
				}
			}
		}
		return SubWorld.ZoneType.Sandstone;
	}

	private Color32 GetZoneColor(SubWorld.ZoneType zone_type)
	{
		Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 3);
		Debug.Assert(zone_type < (SubWorld.ZoneType)this.zoneColours.Length, "Need to add more colours to handle this zone");
		color = this.zoneColours[(int)zone_type];
		color.a = (byte)zone_type;
		return color;
	}

	private const string shaderPropertyName = "_WorldZoneTex";

	private Texture2D regionTex;

	private SubWorld.ZoneType[] worldZoneTypes;

	[SerializeField]
	private Color32[] zoneColours = new Color32[]
	{
		new Color32(145, 198, 213, 0),
		new Color32(135, 82, 160, 1),
		new Color32(123, 151, 75, 2),
		new Color32(236, 189, 89, 3),
		new Color32(201, 152, 181, 4),
		new Color32(222, 90, 59, 5)
	};
}
