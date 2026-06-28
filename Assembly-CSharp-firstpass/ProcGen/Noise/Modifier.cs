using System;
using System.Collections.Generic;
using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Modifier;

namespace ProcGen.Noise
{
	public class Modifier : NoiseBase
	{
		public Modifier()
		{
			this.modifyType = Modifier.ModifyType.Abs;
			this.lower = -1f;
			this.upper = 1f;
			this.exponent = 0.02f;
			this.invert = false;
			this.scale = 1f;
			this.bias = 0f;
			this.scale2d = new Vector2f(1, 1);
		}

		public override Type GetObjectType()
		{
			return typeof(Modifier);
		}

		public Modifier.ModifyType modifyType { get; set; }

		public float lower { get; set; }

		public float upper { get; set; }

		public float exponent { get; set; }

		public bool invert { get; set; }

		public float scale { get; set; }

		public float bias { get; set; }

		public Vector2f scale2d { get; set; }

		public IModule3D CreateModule()
		{
			switch (this.modifyType)
			{
			case Modifier.ModifyType.Abs:
				return new Abs();
			case Modifier.ModifyType.Clamp:
				return new Clamp
				{
					LowerBound = this.lower,
					UpperBound = this.upper
				};
			case Modifier.ModifyType.Exponent:
				return new Exponent
				{
					ExponentValue = this.exponent
				};
			case Modifier.ModifyType.Invert:
				return new Invert();
			case Modifier.ModifyType.ScaleBias:
				return new ScaleBias
				{
					Scale = this.scale,
					Bias = this.bias
				};
			case Modifier.ModifyType.Scale2d:
				return new Scale2d
				{
					Scale = this.scale2d
				};
			case Modifier.ModifyType.Curve:
				return new Curve();
			case Modifier.ModifyType.Terrace:
				return new Terrace();
			default:
				return null;
			}
		}

		public IModule3D CreateModule(IModule3D sourceModule)
		{
			switch (this.modifyType)
			{
			case Modifier.ModifyType.Abs:
				return new Abs(sourceModule);
			case Modifier.ModifyType.Clamp:
				return new Clamp(sourceModule, this.lower, this.upper);
			case Modifier.ModifyType.Exponent:
				return new Exponent(sourceModule, this.exponent);
			case Modifier.ModifyType.Invert:
				return new Invert(sourceModule);
			case Modifier.ModifyType.ScaleBias:
				return new ScaleBias(sourceModule, this.scale, this.bias);
			case Modifier.ModifyType.Scale2d:
				return new Scale2d(sourceModule, this.scale2d);
			case Modifier.ModifyType.Curve:
				return new Curve(sourceModule);
			case Modifier.ModifyType.Terrace:
				return new Terrace(sourceModule);
			default:
				return null;
			}
		}

		public void SetSouces(IModule3D target, IModule3D sourceModule, FloatList controlFloats, ControlPointList controlPoints)
		{
			(target as ModifierModule).SourceModule = sourceModule;
			if (this.modifyType == Modifier.ModifyType.Curve)
			{
				Curve curve = target as Curve;
				curve.ClearControlPoints();
				List<ControlPoint> controls = controlPoints.GetControls();
				foreach (ControlPoint controlPoint in controls)
				{
					curve.AddControlPoint(controlPoint);
				}
			}
			else if (this.modifyType == Modifier.ModifyType.Terrace)
			{
				Terrace terrace = target as Terrace;
				terrace.ClearControlPoints();
				foreach (float num in controlFloats.points)
				{
					float num2 = num;
					terrace.AddControlPoint(num2);
				}
			}
		}

		public enum ModifyType
		{
			_UNSET_,
			Abs,
			Clamp,
			Exponent,
			Invert,
			ScaleBias,
			Scale2d,
			Curve,
			Terrace
		}
	}
}
