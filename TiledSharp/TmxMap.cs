using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TiledSharp
{
	public class TmxMap : TmxDocument
	{
		public string Version { get; private set; }

		public int Width { get; private set; }

		public int Height { get; private set; }

		public int TileWidth { get; private set; }

		public int TileHeight { get; private set; }

		public int? HexSideLength { get; private set; }

		public TmxMap.OrientationType Orientation { get; private set; }

		public TmxMap.StaggerAxisType StaggerAxis { get; private set; }

		public TmxMap.StaggerIndexType StaggerIndex { get; private set; }

		public TmxMap.RenderOrderType RenderOrder { get; private set; }

		public TmxColor BackgroundColor { get; private set; }

		public int? NextObjectID { get; private set; }

		public TmxList<TmxTileset> Tilesets { get; private set; }

		public TmxList<TmxLayer> Layers { get; private set; }

		public TmxList<TmxObjectGroup> ObjectGroups { get; private set; }

		public TmxList<TmxImageLayer> ImageLayers { get; private set; }

		public PropertyDict Properties { get; private set; }

		public TmxLayer GetTileLayer(string layerName)
		{
			layerName = layerName.ToLower();
			for (int i = 0; i < this.Layers.Count; i++)
			{
				if (this.Layers[i].Name.ToLower() == layerName)
				{
					return this.Layers[i];
				}
			}
			return null;
		}

		public TmxTileset GetTileSet(string tilesetName)
		{
			tilesetName = tilesetName.ToLower();
			for (int i = 0; i < this.Tilesets.Count; i++)
			{
				if (this.Tilesets[i].Name.ToLower() == tilesetName)
				{
					return this.Tilesets[i];
				}
			}
			return null;
		}

		public TmxTilesetTile GetTilesetTileForLayerTile(TmxLayerTile id)
		{
			TmxTilesetTile tmxTilesetTile;
			if (id.Gid == 0)
			{
				tmxTilesetTile = null;
			}
			else
			{
				for (int i = 0; i < this.Tilesets.Count; i++)
				{
					if (id.Gid >= this.Tilesets[i].FirstGid)
					{
						int num = id.Gid - this.Tilesets[i].FirstGid;
						if (num < this.Tilesets[i].Tiles.Count)
						{
							return this.Tilesets[i].Tiles[num];
						}
					}
				}
				tmxTilesetTile = null;
			}
			return tmxTilesetTile;
		}

		public TmxTileset GetTilesetForLayerTile(TmxLayerTile id)
		{
			TmxTileset tmxTileset;
			if (id.Gid == 0)
			{
				tmxTileset = null;
			}
			else
			{
				TmxTileset tmxTileset2 = null;
				for (int i = 0; i < this.Tilesets.Count; i++)
				{
					if (id.Gid >= this.Tilesets[i].FirstGid)
					{
						if (tmxTileset2 == null || this.Tilesets[i].FirstGid >= tmxTileset2.FirstGid)
						{
							tmxTileset2 = this.Tilesets[i];
						}
					}
				}
				tmxTileset = tmxTileset2;
			}
			return tmxTileset;
		}

		public TmxMap(string filename)
		{
			XDocument xdocument = base.ReadXml(filename);
			XElement xelement = xdocument.Element("map");
			this.Version = (string)xelement.Attribute("version");
			this.Width = (int)xelement.Attribute("width");
			this.Height = (int)xelement.Attribute("height");
			this.TileWidth = (int)xelement.Attribute("tilewidth");
			this.TileHeight = (int)xelement.Attribute("tileheight");
			this.HexSideLength = (int?)xelement.Attribute("hexsidelength");
			Dictionary<string, TmxMap.OrientationType> dictionary = new Dictionary<string, TmxMap.OrientationType>
			{
				{
					"unknown",
					TmxMap.OrientationType.Unknown
				},
				{
					"orthogonal",
					TmxMap.OrientationType.Orthogonal
				},
				{
					"isometric",
					TmxMap.OrientationType.Isometric
				},
				{
					"staggered",
					TmxMap.OrientationType.Staggered
				},
				{
					"hexagonal",
					TmxMap.OrientationType.Hexagonal
				}
			};
			string text = (string)xelement.Attribute("orientation");
			if (text != null)
			{
				this.Orientation = dictionary[text];
			}
			Dictionary<string, TmxMap.StaggerAxisType> dictionary2 = new Dictionary<string, TmxMap.StaggerAxisType>
			{
				{
					"x",
					TmxMap.StaggerAxisType.X
				},
				{
					"y",
					TmxMap.StaggerAxisType.Y
				}
			};
			string text2 = (string)xelement.Attribute("staggeraxis");
			if (text2 != null)
			{
				this.StaggerAxis = dictionary2[text2];
			}
			Dictionary<string, TmxMap.StaggerIndexType> dictionary3 = new Dictionary<string, TmxMap.StaggerIndexType>
			{
				{
					"odd",
					TmxMap.StaggerIndexType.Odd
				},
				{
					"even",
					TmxMap.StaggerIndexType.Even
				}
			};
			string text3 = (string)xelement.Attribute("staggerindex");
			if (text3 != null)
			{
				this.StaggerIndex = dictionary3[text3];
			}
			Dictionary<string, TmxMap.RenderOrderType> dictionary4 = new Dictionary<string, TmxMap.RenderOrderType>
			{
				{
					"right-down",
					TmxMap.RenderOrderType.RightDown
				},
				{
					"right-up",
					TmxMap.RenderOrderType.RightUp
				},
				{
					"left-down",
					TmxMap.RenderOrderType.LeftDown
				},
				{
					"left-up",
					TmxMap.RenderOrderType.LeftUp
				}
			};
			string text4 = (string)xelement.Attribute("renderorder");
			if (text4 != null)
			{
				this.RenderOrder = dictionary4[text4];
			}
			this.NextObjectID = (int?)xelement.Attribute("nextobjectid");
			this.BackgroundColor = new TmxColor(xelement.Attribute("backgroundcolor"));
			this.Properties = new PropertyDict(xelement.Element("properties"));
			this.Tilesets = new TmxList<TmxTileset>();
			foreach (XElement xelement2 in xelement.Elements("tileset"))
			{
				this.Tilesets.Add(new TmxTileset(xelement2, base.TmxDirectory));
			}
			this.Layers = new TmxList<TmxLayer>();
			foreach (XElement xelement2 in xelement.Elements("layer"))
			{
				this.Layers.Add(new TmxLayer(xelement2, this.Width, this.Height));
			}
			this.ObjectGroups = new TmxList<TmxObjectGroup>();
			foreach (XElement xelement2 in xelement.Elements("objectgroup"))
			{
				this.ObjectGroups.Add(new TmxObjectGroup(xelement2));
			}
			this.ImageLayers = new TmxList<TmxImageLayer>();
			foreach (XElement xelement2 in xelement.Elements("imagelayer"))
			{
				this.ImageLayers.Add(new TmxImageLayer(xelement2, base.TmxDirectory));
			}
		}

		public enum OrientationType
		{
			Unknown,
			Orthogonal,
			Isometric,
			Staggered,
			Hexagonal
		}

		public enum StaggerAxisType
		{
			X,
			Y
		}

		public enum StaggerIndexType
		{
			Odd,
			Even
		}

		public enum RenderOrderType
		{
			RightDown,
			RightUp,
			LeftDown,
			LeftUp
		}
	}
}
