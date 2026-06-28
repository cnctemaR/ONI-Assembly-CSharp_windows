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
			IModule3D module3D;
			switch (this.modifyType)
			{
			case Modifier.ModifyType.Abs:
				module3D = new Abs();
				break;
			case Modifier.ModifyType.Clamp:
				module3D = new Clamp
				{
					LowerBound = this.lower,
					UpperBound = this.upper
				};
				break;
			case Modifier.ModifyType.Exponent:
				module3D = new Exponent
				{
					ExponentValue = this.exponent
				};
				break;
			case Modifier.ModifyType.Invert:
				module3D = new Invert();
				break;
			case Modifier.ModifyType.ScaleBias:
				module3D = new ScaleBias
				{
					Scale = this.scale,
					Bias = this.bias
				};
				break;
			case Modifier.ModifyType.Scale2d:
				module3D = new Scale2d
				{
					Scale = this.scale2d
				};
				break;
			case Modifier.ModifyType.Curve:
				module3D = new Curve();
				break;
			case Modifier.ModifyType.Terrace:
				module3D = new Terrace();
				break;
			default:
				module3D = null;
				break;
			}
			return module3D;
		}

		public IModule3D CreateModule(IModule3D sourceModule)
		{
			IModule3D module3D;
			switch (this.modifyType)
			{
			case Modifier.ModifyType.Abs:
				module3D = new Abs(sourceModule);
				break;
			case Modifier.ModifyType.Clamp:
				module3D = new Clamp(sourceModule, this.lower, this.upper);
				break;
			case Modifier.ModifyType.Exponent:
				module3D = new Exponent(sourceModule, this.exponent);
				break;
			case Modifier.ModifyType.Invert:
				module3D = new Invert(sourceModule);
				break;
			case Modifier.ModifyType.ScaleBias:
				module3D = new ScaleBias(sourceModule, this.scale, this.bias);
				break;
			case Modifier.ModifyType.Scale2d:
				module3D = new Scale2d(sourceModule, this.scale2d);
				break;
			case Modifier.ModifyType.Curve:
				module3D = new Curve(sourceModule);
				break;
			case Modifier.ModifyType.Terrace:
				module3D = new Terrace(sourceModule);
				break;
			default:
				module3D = null;
				break;
			}
			return module3D;
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
