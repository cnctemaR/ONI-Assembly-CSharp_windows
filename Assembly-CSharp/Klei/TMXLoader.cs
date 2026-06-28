using System;
using System.Collections.Generic;
using TiledSharp;

namespace Klei
{
	public class TMXLoader
	{
		public static Sim.Cell[] Load()
		{
			string text = TMXLoader.StreamingAssetPath + "/" + TMXLoader.FileName + ".tmx";
			TmxMap tmxMap = new TmxMap(text);
			TmxLayer tileLayer = tmxMap.GetTileLayer("Element");
			TmxLayer tileLayer2 = tmxMap.GetTileLayer("Pressure");
			TmxLayer tileLayer3 = tmxMap.GetTileLayer("Heat");
			TmxLayer tileLayer4 = tmxMap.GetTileLayer("Mass");
			TmxLayer tileLayer5 = tmxMap.GetTileLayer("Wire");
			List<Element> elements = ElementLoader.elements;
			Element element = ElementLoader.FindElementByHash(SimHashes.Void);
			Sim.Cell[] array = new Sim.Cell[tmxMap.Width * tmxMap.Height];
			for (int i = 0; i < Grid.HeightInCells; i++)
			{
				for (int j = 0; j < Grid.WidthInCells; j++)
				{
					Element element2 = element;
					int num = j + i * tmxMap.Width;
					TmxLayerTile tmxLayerTile = tileLayer.Tiles[num];
					TmxTilesetTile tilesetTileForLayerTile = tmxMap.GetTilesetTileForLayerTile(tmxLayerTile);
					if (tilesetTileForLayerTile != null)
					{
						string text2 = tilesetTileForLayerTile.Properties["simhash"];
						if (text2 != string.Empty)
						{
							SimHashes simHashes = (SimHashes)((int)Enum.Parse(typeof(SimHashes), text2, true));
							element2 = ElementLoader.FindElementByHash(simHashes);
							if (element2 == null)
							{
								element2 = element;
							}
						}
					}
					Sim.PhysicsData defaultValues = element2.defaultValues;
					TmxLayerTile tmxLayerTile2 = ((tileLayer2 != null) ? tileLayer2.Tiles[num] : null);
					if (tmxLayerTile2 != null && tmxLayerTile2.Gid != 0)
					{
						TmxTileset tilesetForLayerTile = tmxMap.GetTilesetForLayerTile(tmxLayerTile2);
						if (tilesetForLayerTile != null)
						{
							defaultValues.pressure = (float)(tmxLayerTile2.Gid - tilesetForLayerTile.FirstGid) / 100f * 10000f;
						}
					}
					TmxLayerTile tmxLayerTile3 = ((tileLayer3 != null) ? tileLayer3.Tiles[num] : null);
					if (tmxLayerTile3 != null && tmxLayerTile3.Gid != 0)
					{
						TmxTileset tilesetForLayerTile2 = tmxMap.GetTilesetForLayerTile(tmxLayerTile3);
						if (tilesetForLayerTile2 != null)
						{
							defaultValues.temperature = (float)(tmxLayerTile3.Gid - tilesetForLayerTile2.FirstGid) / 100f * 10000f;
						}
					}
					TmxLayerTile tmxLayerTile4 = ((tileLayer4 != null) ? tileLayer4.Tiles[num] : null);
					if (tmxLayerTile4 != null && tmxLayerTile4.Gid != 0)
					{
						TmxTileset tilesetForLayerTile3 = tmxMap.GetTilesetForLayerTile(tmxLayerTile4);
						if (tilesetForLayerTile3 != null)
						{
							defaultValues.mass = (float)(tmxLayerTile4.Gid - tilesetForLayerTile3.FirstGid) / 100f * 10000f;
						}
					}
					array[(tmxMap.Height - 1 - i) * Grid.WidthInCells + j].SetValues(element2, defaultValues, elements);
					TmxLayerTile tmxLayerTile5 = ((tileLayer5 != null) ? tileLayer5.Tiles[num] : null);
					if (tmxLayerTile5 != null && tmxLayerTile5.Gid != 0)
					{
						TmxTileset tilesetForLayerTile4 = tmxMap.GetTilesetForLayerTile(tmxLayerTile5);
						if (tilesetForLayerTile4 != null)
						{
						}
					}
				}
			}
			for (int k = 0; k < tmxMap.ObjectGroups.Count; k++)
			{
				TmxObjectGroup tmxObjectGroup = tmxMap.ObjectGroups[k];
				if (tmxObjectGroup.Name == "Override")
				{
					for (int l = 0; l < tmxObjectGroup.Objects.Count; l++)
					{
						TmxObjectGroup.TmxObject tmxObject = tmxObjectGroup.Objects[l];
						float num2 = ((!tmxObject.Properties.ContainsKey("Temperature")) ? (-1f) : float.Parse(tmxObject.Properties["Temperature"]));
						float num3 = ((!tmxObject.Properties.ContainsKey("Pressure")) ? (-1f) : float.Parse(tmxObject.Properties["Pressure"]));
						float num4 = ((!tmxObject.Properties.ContainsKey("Mass")) ? (-1f) : float.Parse(tmxObject.Properties["Mass"]));
						bool flag = num2 != -1f || num3 != -1f || num4 != -1f;
						if (flag)
						{
							int num5 = (int)tmxObject.X;
							while ((double)num5 < tmxObject.X + tmxObject.Width)
							{
								int num6 = (int)tmxObject.Y;
								while ((double)num6 < tmxObject.Y + tmxObject.Height)
								{
									num6++;
								}
								num5++;
							}
						}
					}
				}
			}
			return array;
		}

		public static string FileName = "test";

		public static string StreamingAssetPath = string.Empty;
	}
}
