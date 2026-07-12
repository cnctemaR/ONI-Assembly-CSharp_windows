using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Delaunay.Geo;
using KSerialization;
using MIConvexHull;
using UnityEngine;

namespace VoronoiTree
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class PowerDiagramSite : TriangulationCell<PowerDiagram.DualSite2d, PowerDiagramSite>
	{
		public bool dummy { get; set; }

		public PowerDiagramSite()
		{
			this.dummy = true;
		}

		public PowerDiagramSite(Vector2 pos)
		{
			this.position = pos;
			this.weight = Mathf.Epsilon;
			this.dummy = false;
			this.previousWeightAdaption = 1f;
		}

		public PowerDiagramSite(float x, float y)
			: this(new Vector2(x, y))
		{
		}

		public PowerDiagramSite(uint siteId, Vector2 pos, float siteWeight = 1f)
		{
			this.dummy = false;
			this.neighbours = new List<PowerDiagramSite>();
			this.id = (int)siteId;
			this.position = pos;
			this.weight = siteWeight;
			this.currentWeight = this.weight;
		}

		[OnDeserializing]
		internal void OnDeserializingMethod()
		{
			this.neighbours = new List<PowerDiagramSite>();
		}

		public PowerDiagram.DualSite3d ToDualSite()
		{
			return new PowerDiagram.DualSite3d((double)this.position.x, (double)this.position.y, (double)(this.position.x * this.position.x + this.position.y * this.position.y - this.currentWeight), this);
		}

		private double Det(double[,] m)
		{
			return m[0, 0] * (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]) - m[0, 1] * (m[1, 0] * m[2, 2] - m[2, 0] * m[1, 2]) + m[0, 2] * (m[1, 0] * m[2, 1] - m[2, 0] * m[1, 1]);
		}

		private double LengthSquared(double[] v)
		{
			double num = 0.0;
			foreach (double num2 in v)
			{
				num += num2 * num2;
			}
			return num;
		}

		private Vector2 GetCircumcenter()
		{
			PowerDiagram.DualSite2d[] vertices = base.Vertices;
			double[,] array = new double[3, 3];
			for (int i = 0; i < 3; i++)
			{
				array[i, 0] = vertices[i].Position[0];
				array[i, 1] = vertices[i].Position[1];
				array[i, 2] = 1.0;
			}
			double num = this.Det(array);
			double num2 = -1.0 / (2.0 * num);
			for (int j = 0; j < 3; j++)
			{
				array[j, 0] = this.LengthSquared(vertices[j].Position);
			}
			double num3 = -this.Det(array);
			for (int k = 0; k < 3; k++)
			{
				array[k, 1] = vertices[k].Position[0];
			}
			double num4 = this.Det(array);
			return new Vector2((float)(num2 * num3), (float)(num2 * num4));
		}

		private Vector2 GetCentroid()
		{
			return new Vector2((float)base.Vertices.Select<PowerDiagram.DualSite2d, double>((PowerDiagram.DualSite2d v) => v.Position[0]).Average(), (float)base.Vertices.Select<PowerDiagram.DualSite2d, double>((PowerDiagram.DualSite2d v) => v.Position[1]).Average());
		}

		public Vector2 Circumcenter
		{
			get
			{
				this.circumCenter = new Vector2?(this.circumCenter ?? this.GetCircumcenter());
				return this.circumCenter.Value;
			}
		}

		public Vector2 Centroid
		{
			get
			{
				if (this.poly != null)
				{
					return this.poly.Centroid();
				}
				this.centroid = new Vector2?(this.centroid ?? this.GetCentroid());
				return this.centroid.Value;
			}
		}

		public float weight;

		public float currentWeight;

		public float previousWeightAdaption;

		[Serialize]
		public int id = -1;

		[Serialize]
		public Vector2 position;

		[Serialize]
		public Polygon poly;

		[Serialize]
		public List<PowerDiagramSite> neighbours;

		private Vector2? circumCenter;

		private Vector2? centroid;
	}
}
