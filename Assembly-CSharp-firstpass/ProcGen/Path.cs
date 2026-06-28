using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptOut)]
	public class Path
	{
		public Path()
		{
			this.pathElements = new List<Segment>();
		}

		public void AddSegment(Segment segment)
		{
			this.pathElements.Add(segment);
		}

		public void AddSegment(Vector2 e0, Vector2 e1)
		{
			this.pathElements.Add(new Segment(e0, e1));
		}

		public void Stagger(SeededRandom rnd, float maxDistance = 10f, float staggerRange = 3f)
		{
			List<Segment> list = new List<Segment>();
			for (int i = 0; i < this.pathElements.Count; i++)
			{
				list.AddRange(this.pathElements[i].Stagger(rnd, maxDistance, staggerRange));
			}
			this.pathElements = list;
		}

		public List<Segment> pathElements;
	}
}
