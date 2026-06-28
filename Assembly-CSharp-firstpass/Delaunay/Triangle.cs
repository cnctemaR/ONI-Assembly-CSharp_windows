using System;
using System.Collections.Generic;
using Delaunay.Utils;

namespace Delaunay
{
	public sealed class Triangle : Delaunay.Utils.IDisposable
	{
		public Triangle(Site a, Site b, Site c)
		{
			this._sites = new List<Site> { a, b, c };
		}

		public List<Site> sites
		{
			get
			{
				return this._sites;
			}
		}

		public void Dispose()
		{
			this._sites.Clear();
			this._sites = null;
		}

		private List<Site> _sites;
	}
}
