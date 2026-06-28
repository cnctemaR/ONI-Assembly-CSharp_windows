using System;
using System.Collections.Generic;
using LibNoiseDotNet.Graphics.Tools.Noise.Modifier;
using UnityEngine;

namespace ProcGen.Noise
{
	[Serializable]
	public class ControlPointList : NoiseBase
	{
		public ControlPointList()
		{
			this.points = new List<ControlPointList.Control>();
		}

		public override Type GetObjectType()
		{
			return typeof(ControlPointList);
		}

		[SerializeField]
		public List<ControlPointList.Control> points { get; set; }

		public List<ControlPoint> GetControls()
		{
			List<ControlPoint> list = new List<ControlPoint>();
			for (int i = 0; i < this.points.Count; i++)
			{
				list.Add(new ControlPoint(this.points[i].input, this.points[i].output));
			}
			return list;
		}

		public class Control
		{
			public Control()
			{
				this.input = 0f;
				this.output = 0f;
			}

			public Control(float i, float o)
			{
				this.input = i;
				this.output = o;
			}

			public float input { get; set; }

			public float output { get; set; }
		}
	}
}
