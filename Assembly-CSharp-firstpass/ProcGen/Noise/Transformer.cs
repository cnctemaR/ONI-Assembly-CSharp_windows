using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Primitive;
using LibNoiseDotNet.Graphics.Tools.Noise.Tranformer;

namespace ProcGen.Noise
{
	public class Transformer : NoiseBase
	{
		public override Type GetObjectType()
		{
			return typeof(Transformer);
		}

		public Transformer.TransformerType transformerType { get; set; }

		public float power { get; set; }

		public Vector2f vector { get; set; }

		public Transformer()
		{
			this.transformerType = Transformer.TransformerType.Displace;
			this.power = 1f;
			this.vector = new Vector2f(0, 0);
		}

		public IModule3D CreateModule()
		{
			if (this.transformerType == Transformer.TransformerType.Turbulence)
			{
				return new Turbulence
				{
					Power = this.power
				};
			}
			if (this.transformerType == Transformer.TransformerType.RotatePoint)
			{
				return new RotatePoint
				{
					XAngle = this.vector.x,
					YAngle = this.vector.y,
					ZAngle = 0f
				};
			}
			if (this.transformerType == Transformer.TransformerType.TranslatePoint)
			{
				return new TranslatePoint
				{
					XTranslate = this.vector.x,
					ZTranslate = this.vector.y
				};
			}
			return new Displace();
		}

		public IModule3D CreateModule(IModule3D sourceModule, IModule3D xModule, IModule3D yModule, IModule3D zModule)
		{
			if (this.transformerType == Transformer.TransformerType.Turbulence)
			{
				return new Turbulence(sourceModule, xModule, yModule, zModule, this.power);
			}
			if (this.transformerType == Transformer.TransformerType.RotatePoint)
			{
				return new RotatePoint(sourceModule, this.vector.x, this.vector.y, 0f);
			}
			if (this.transformerType == Transformer.TransformerType.TranslatePoint)
			{
				return new TranslatePoint(sourceModule, this.vector.x, 0f, this.vector.y);
			}
			return new Displace(sourceModule, xModule, yModule, zModule);
		}

		public void SetSouces(IModule3D target, IModule3D sourceModule, IModule3D xModule, IModule3D yModule, IModule3D zModule)
		{
			if (this.transformerType == Transformer.TransformerType.Turbulence)
			{
				Turbulence turbulence = target as Turbulence;
				turbulence.SourceModule = sourceModule;
				IModule module;
				if (xModule == null)
				{
					IModule3D module3D = new Constant(0f);
					module = module3D;
				}
				else
				{
					module = xModule;
				}
				turbulence.XDistortModule = module;
				turbulence.YDistortModule = new Constant(0f);
				IModule module2;
				if (yModule == null)
				{
					IModule3D module3D = new Constant(0f);
					module2 = module3D;
				}
				else
				{
					module2 = yModule;
				}
				turbulence.ZDistortModule = module2;
				return;
			}
			if (this.transformerType == Transformer.TransformerType.RotatePoint)
			{
				(target as RotatePoint).SourceModule = sourceModule;
				return;
			}
			if (this.transformerType == Transformer.TransformerType.TranslatePoint)
			{
				(target as TranslatePoint).SourceModule = sourceModule;
				return;
			}
			Displace displace = target as Displace;
			displace.SourceModule = sourceModule;
			IModule module3;
			if (xModule == null)
			{
				IModule3D module3D = new Constant(0f);
				module3 = module3D;
			}
			else
			{
				module3 = xModule;
			}
			displace.XDisplaceModule = module3;
			displace.YDisplaceModule = new Constant(0f);
			IModule module4;
			if (yModule == null)
			{
				IModule3D module3D = new Constant(0f);
				module4 = module3D;
			}
			else
			{
				module4 = yModule;
			}
			displace.ZDisplaceModule = module4;
		}

		public enum TransformerType
		{
			_UNSET_,
			Displace,
			Turbulence,
			RotatePoint,
			TranslatePoint
		}
	}
}
