using System;
using System.Collections.Generic;
using Delaunay.Geo;
using Delaunay.Utils;
using UnityEngine;

namespace Delaunay
{
	public sealed class SiteList : Delaunay.Utils.IDisposable
	{
		public SiteList()
		{
			this._sites = new List<Site>();
			this._sorted = false;
		}

		public void Dispose()
		{
			if (this._sites != null)
			{
				for (int i = 0; i < this._sites.Count; i++)
				{
					Site site = this._sites[i];
					site.Dispose();
				}
				this._sites.Clear();
				this._sites = null;
			}
		}

		public int Add(Site site)
		{
			this._sorted = false;
			this._sites.Add(site);
			return this._sites.Count;
		}

		public int Count
		{
			get
			{
				return this._sites.Count;
			}
		}

		public Site Next()
		{
			if (!this._sorted)
			{
				global::UnityEngine.Debug.LogError("SiteList::next():  sites have not been sorted");
			}
			if (this._currentIndex < this._sites.Count)
			{
				return this._sites[this._currentIndex++];
			}
			return null;
		}

		internal Rect GetSitesBounds()
		{
			if (!this._sorted)
			{
				Site.SortSites(this._sites);
				this._currentIndex = 0;
				this._sorted = true;
			}
			if (this._sites.Count == 0)
			{
				return new Rect(0f, 0f, 0f, 0f);
			}
			float num = float.MaxValue;
			float num2 = float.MinValue;
			for (int i = 0; i < this._sites.Count; i++)
			{
				Site site = this._sites[i];
				if (site.x < num)
				{
					num = site.x;
				}
				if (site.x > num2)
				{
					num2 = site.x;
				}
			}
			float y = this._sites[0].y;
			float y2 = this._sites[this._sites.Count - 1].y;
			return new Rect(num, y, num2 - num, y2 - y);
		}

		public List<uint> SiteColors()
		{
			List<uint> list = new List<uint>();
			for (int i = 0; i < this._sites.Count; i++)
			{
				Site site = this._sites[i];
				list.Add(site.color);
			}
			return list;
		}

		public List<Vector2> SiteCoords()
		{
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < this._sites.Count; i++)
			{
				Site site = this._sites[i];
				list.Add(site.Coord);
			}
			return list;
		}

		public List<Circle> Circles()
		{
			List<Circle> list = new List<Circle>();
			for (int i = 0; i < this._sites.Count; i++)
			{
				Site site = this._sites[i];
				float num = 0f;
				Edge edge = site.NearestEdge();
				if (!edge.IsPartOfConvexHull())
				{
					num = edge.SitesDistance() * 0.5f;
				}
				list.Add(new Circle(site.x, site.y, num));
			}
			return list;
		}

		public List<List<Vector2>> Regions(Rect plotBounds)
		{
			List<List<Vector2>> list = new List<List<Vector2>>();
			for (int i = 0; i < this._sites.Count; i++)
			{
				Site site = this._sites[i];
				list.Add(site.Region(plotBounds));
			}
			return list;
		}

		public List<List<Vector2>> Regions(Polygon plotBounds)
		{
			List<List<Vector2>> list = new List<List<Vector2>>();
			for (int i = 0; i < this._sites.Count; i++)
			{
				Site site = this._sites[i];
				list.Add(site.Region(plotBounds));
			}
			return list;
		}

		private List<Site> _sites;

		private int _currentIndex;

		private bool _sorted;
	}
}
