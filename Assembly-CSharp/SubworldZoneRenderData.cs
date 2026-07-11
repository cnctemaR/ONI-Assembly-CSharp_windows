using System;
using Delaunay.Geo;
using Klei;
using ProcGen;
using UnityEngine;

public class SubworldZoneRenderData : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.GenerateTexture();
	}

	public void GenerateTexture()
	{
		this.colourTex = new Texture2D(Grid.WidthInCells, Grid.HeightInCells, TextureFormat.RGB24, false);
		this.colourTex.name = "SubworldRegionColourData";
		this.colourTex.filterMode = FilterMode.Bilinear;
		this.colourTex.wrapMode = TextureWrapMode.Clamp;
		this.colourTex.anisoLevel = 0;
		this.indexTex = new Texture2D(Grid.WidthInCells, Grid.HeightInCells, TextureFormat.Alpha8, false);
		this.indexTex.name = "SubworldRegionIndexData";
		this.indexTex.filterMode = FilterMode.Point;
		this.indexTex.wrapMode = TextureWrapMode.Clamp;
		this.indexTex.anisoLevel = 0;
		byte[] array = new byte[Grid.WidthInCells * Grid.HeightInCells * 3];
		byte[] array2 = new byte[Grid.WidthInCells * Grid.HeightInCells];
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
						int num = (int)(zero.x + zero.y * (float)Grid.WidthInCells);
						array2[num] = ((overworldCell.zoneType != SubWorld.ZoneType.Space) ? ((byte)overworldCell.zoneType) : byte.MaxValue);
						Color32 color = this.zoneColours[(int)overworldCell.zoneType];
						array[num * 3] = color.r;
						array[num * 3 + 1] = color.g;
						array[num * 3 + 2] = color.b;
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
		this.colourTex.LoadRawTextureData(array);
		this.indexTex.LoadRawTextureData(array2);
		this.colourTex.Apply();
		this.indexTex.Apply();
		this.OnShadersReloaded();
		ShaderReloader.Register(new global::System.Action(this.OnShadersReloaded));
		this.InitSimZones(array2);
	}

	private void OnShadersReloaded()
	{
		Shader.SetGlobalTexture("_WorldZoneTex", this.colourTex);
		Shader.SetGlobalTexture("_WorldZoneIndexTex", this.indexTex);
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
		global::Debug.Assert(zone_type < (SubWorld.ZoneType)this.zoneColours.Length, string.Concat(new object[]
		{
			"Need to add more colours to handle this zone",
			(int)zone_type,
			"<",
			this.zoneColours.Length
		}));
		return color;
	}

	private unsafe void InitSimZones(byte[] bytes)
	{
		fixed (byte* ptr = (ref bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *null))
		{
			Sim.SIM_HandleMessage(-457308393, bytes.Length, ptr);
		}
	}

	[SerializeField]
	private Texture2D colourTex;

	[SerializeField]
	private Texture2D indexTex;

	[HideInInspector]
	public SubWorld.ZoneType[] worldZoneTypes;

	[SerializeField]
	[HideInInspector]
	public Color32[] zoneColours = new Color32[]
	{
		new Color32(145, 198, 213, 0),
		new Color32(135, 82, 160, 1),
		new Color32(123, 151, 75, 2),
		new Color32(236, 189, 89, 3),
		new Color32(201, 152, 181, 4),
		new Color32(222, 90, 59, 5),
		new Color32(201, 152, 181, 6),
		new Color32(byte.MaxValue, 0, 0, 7),
		new Color32(201, 201, 151, 8),
		new Color32(236, 90, 110, 9),
		new Color32(110, 236, 110, 10)
	};
}
