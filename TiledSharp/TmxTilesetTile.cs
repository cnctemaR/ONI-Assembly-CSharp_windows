using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TiledSharp
{
	public class TmxTilesetTile
	{
		public int Id { get; private set; }

		public List<TmxTerrain> TerrainEdges { get; private set; }

		public double Probability { get; private set; }

		public PropertyDict Properties { get; private set; }

		public TmxImage Image { get; private set; }

		public TmxList<TmxObjectGroup> ObjectGroups { get; private set; }

		public List<TmxAnimationFrame> AnimationFrames { get; private set; }

		public TmxTerrain TopLeft
		{
			get
			{
				return this.TerrainEdges[0];
			}
		}

		public TmxTerrain TopRight
		{
			get
			{
				return this.TerrainEdges[1];
			}
		}

		public TmxTerrain BottomLeft
		{
			get
			{
				return this.TerrainEdges[2];
			}
		}

		public TmxTerrain BottomRight
		{
			get
			{
				return this.TerrainEdges[3];
			}
		}

		public TmxTilesetTile(XElement xTile, TmxList<TmxTerrain> Terrains, string tmxDir = "")
		{
			this.Id = (int)xTile.Attribute("id");
			this.TerrainEdges = new List<TmxTerrain>(4);
			string text = ((string)xTile.Attribute("terrain")) ?? ",,,";
			foreach (string text2 in text.Split(new char[] { ',' }))
			{
				int num;
				bool flag = int.TryParse(text2, out num);
				TmxTerrain tmxTerrain;
				if (flag)
				{
					tmxTerrain = Terrains[num];
				}
				else
				{
					tmxTerrain = null;
				}
				this.TerrainEdges.Add(tmxTerrain);
			}
			double? num2 = (double?)xTile.Attribute("probability");
			this.Probability = ((num2 != null) ? num2.GetValueOrDefault() : 1.0);
			this.Image = new TmxImage(xTile.Element("image"), tmxDir);
			this.ObjectGroups = new TmxList<TmxObjectGroup>();
			foreach (XElement xelement in xTile.Elements("objectgroup"))
			{
				this.ObjectGroups.Add(new TmxObjectGroup(xelement));
			}
			this.AnimationFrames = new List<TmxAnimationFrame>();
			if (xTile.Element("animation") != null)
			{
				foreach (XElement xelement in xTile.Element("animation").Elements("frame"))
				{
					this.AnimationFrames.Add(new TmxAnimationFrame(xelement));
				}
			}
			this.Properties = new PropertyDict(xTile.Element("properties"));
		}
	}
}
