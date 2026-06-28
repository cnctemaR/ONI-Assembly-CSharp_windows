using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace TiledSharp
{
	public class TmxTileset : TmxDocument, ITmxElement
	{
		public int FirstGid { get; private set; }

		public string Name { get; private set; }

		public int TileWidth { get; private set; }

		public int TileHeight { get; private set; }

		public int Spacing { get; private set; }

		public int Margin { get; private set; }

		public List<TmxTilesetTile> Tiles { get; private set; }

		public TmxTileOffset TileOffset { get; private set; }

		public PropertyDict Properties { get; private set; }

		public TmxImage Image { get; private set; }

		public TmxList<TmxTerrain> Terrains { get; private set; }

		public TmxTileset(XDocument xDoc, string tmxDir)
			: this(xDoc.Element("tileset"), tmxDir)
		{
		}

		public TmxTileset(XElement xTileset, string tmxDir = "")
		{
			XAttribute xattribute = xTileset.Attribute("firstgid");
			string text = (string)xTileset.Attribute("source");
			if (text != null)
			{
				text = Path.Combine(tmxDir, text);
				this.FirstGid = (int)xattribute;
				XDocument xdocument = base.ReadXml(text);
				TmxTileset tmxTileset = new TmxTileset(xdocument, base.TmxDirectory);
				this.Name = tmxTileset.Name;
				this.TileWidth = tmxTileset.TileWidth;
				this.TileHeight = tmxTileset.TileHeight;
				this.Spacing = tmxTileset.Spacing;
				this.Margin = tmxTileset.Margin;
				this.TileOffset = tmxTileset.TileOffset;
				this.Image = tmxTileset.Image;
				this.Terrains = tmxTileset.Terrains;
				this.Tiles = tmxTileset.Tiles;
				this.Properties = tmxTileset.Properties;
			}
			else
			{
				if (xattribute != null)
				{
					this.FirstGid = (int)xattribute;
				}
				this.Name = (string)xTileset.Attribute("name");
				this.TileWidth = (int)xTileset.Attribute("tilewidth");
				this.TileHeight = (int)xTileset.Attribute("tileheight");
				this.Spacing = ((int?)xTileset.Attribute("spacing")) ?? 0;
				this.Margin = ((int?)xTileset.Attribute("margin")) ?? 0;
				this.TileOffset = new TmxTileOffset(xTileset.Element("tileoffset"));
				this.Image = new TmxImage(xTileset.Element("image"), tmxDir);
				this.Terrains = new TmxList<TmxTerrain>();
				XElement xelement = xTileset.Element("terraintypes");
				if (xelement != null)
				{
					foreach (XElement xelement2 in xelement.Elements("terrain"))
					{
						this.Terrains.Add(new TmxTerrain(xelement2));
					}
				}
				this.Tiles = new List<TmxTilesetTile>();
				foreach (XElement xelement3 in xTileset.Elements("tile"))
				{
					TmxTilesetTile tmxTilesetTile = new TmxTilesetTile(xelement3, this.Terrains, tmxDir);
					this.Tiles.Add(tmxTilesetTile);
				}
				this.Properties = new PropertyDict(xTileset.Element("properties"));
			}
		}

		public TmxTilesetTile GetTileForGid(int Gid)
		{
			TmxTilesetTile tmxTilesetTile;
			if (Gid == 0 || Gid < this.FirstGid)
			{
				tmxTilesetTile = null;
			}
			else
			{
				int num = Gid - this.FirstGid;
				if (num >= this.Tiles.Count)
				{
					tmxTilesetTile = null;
				}
				else
				{
					tmxTilesetTile = this.Tiles[num];
				}
			}
			return tmxTilesetTile;
		}
	}
}
