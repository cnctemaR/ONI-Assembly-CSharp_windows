using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
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

		public Vector2f rotation { get; set; }

		public Transformer()
		{
			this.transformerType = Transformer.TransformerType.Displace;
			this.power = 1f;
			this.rotation = new Vector2f(0, 0);
		}

		public IModule3D CreateModule()
		{
			if (this.transformerType == Transformer.TransformerType.Turbulence)
			{
				new Turbulence().Power = this.power;
			}
			else if (this.transformerType == Transformer.TransformerType.RotatePoint)
			{
				return new RotatePoint
				{
					XAngle = this.rotation.x,
					YAngle = this.rotation.y,
					ZAngle = 0f
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
				return new RotatePoint(sourceModule, this.rotation.x, this.rotation.y, 0f);
			}
			return new Displace(sourceModule, xModule, yModule, zModule);
		}

		public void SetSouces(IModule3D target, IModule3D sourceModule, IModule3D xModule, IModule3D yModule, IModule3D zModule)
		{
			if (this.transformerType == Transformer.TransformerType.Turbulence)
			{
				Turbulence turbulence = target as Turbulence;
				turbulence.SourceModule = sourceModule;
				turbulence.XDistortModule = xModule;
				turbulence.YDistortModule = yModule;
				turbulence.ZDistortModule = zModule;
				return;
			}
			if (this.transformerType == Transformer.TransformerType.RotatePoint)
			{
				(target as RotatePoint).SourceModule = sourceModule;
				return;
			}
			Displace displace = target as Displace;
			displace.SourceModule = sourceModule;
			displace.XDisplaceModule = xModule;
			displace.YDisplaceModule = yModule;
			displace.ZDisplaceModule = zModule;
		}

		public enum TransformerType
		{
			_UNSET_,
			Displace,
			Turbulence,
			RotatePoint
		}
	}
}
