using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptOut)]
	public struct Segment
	{
		public Segment(Vector2 e0, Vector2 e1)
		{
			this.e0 = e0;
			this.e1 = e1;
		}

		public List<Segment> Stagger(SeededRandom rnd, float maxDistance = 10f, float staggerRange = 3f)
		{
			List<Segment> list = new List<Segment>();
			Vector2 vector = this.e1 - this.e0;
			Vector2 vector2 = this.e0;
			Vector2 vector3 = this.e1;
			float num = vector.magnitude / maxDistance;
			Vector2 normalized = new Vector2(-vector.y, vector.x).normalized;
			int num2 = 0;
			while ((float)num2 < num)
			{
				vector3 = this.e0 + vector * (1f / num) * (float)num2 + normalized * rnd.RandomRange(-staggerRange, staggerRange);
				list.Add(new Segment(vector2, vector3));
				vector2 = vector3;
				num2++;
			}
			list.Add(new Segment(vector3, this.e1));
			return list;
		}

		public Vector2 e0;

		public Vector2 e1;
	}
}
