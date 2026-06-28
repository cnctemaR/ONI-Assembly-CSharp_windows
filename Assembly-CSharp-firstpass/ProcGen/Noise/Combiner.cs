using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Combiner;

namespace ProcGen.Noise
{
	public class Combiner : NoiseBase
	{
		public Combiner()
		{
			this.combineType = Combiner.CombinerType.Add;
		}

		public override Type GetObjectType()
		{
			return typeof(Combiner);
		}

		public Combiner.CombinerType combineType { get; set; }

		public IModule3D CreateModule()
		{
			IModule3D module3D;
			switch (this.combineType)
			{
			case Combiner.CombinerType.Add:
				module3D = new Add();
				break;
			case Combiner.CombinerType.Max:
				module3D = new Max();
				break;
			case Combiner.CombinerType.Min:
				module3D = new Min();
				break;
			case Combiner.CombinerType.Multiply:
				module3D = new Multiply();
				break;
			case Combiner.CombinerType.Power:
				module3D = new Power();
				break;
			default:
				module3D = null;
				break;
			}
			return module3D;
		}

		public IModule3D CreateModule(IModule3D leftModule, IModule3D rightModule)
		{
			IModule3D module3D;
			switch (this.combineType)
			{
			case Combiner.CombinerType.Add:
				module3D = new Add(leftModule, rightModule);
				break;
			case Combiner.CombinerType.Max:
				module3D = new Max(leftModule, rightModule);
				break;
			case Combiner.CombinerType.Min:
				module3D = new Min(leftModule, rightModule);
				break;
			case Combiner.CombinerType.Multiply:
				module3D = new Multiply(leftModule, rightModule);
				break;
			case Combiner.CombinerType.Power:
				module3D = new Power(leftModule, rightModule);
				break;
			default:
				module3D = null;
				break;
			}
			return module3D;
		}

		public void SetSouces(IModule3D target, IModule3D leftModule, IModule3D rightModule)
		{
			(target as CombinerModule).LeftModule = leftModule;
			(target as CombinerModule).RightModule = rightModule;
		}

		public enum CombinerType
		{
			_UNSET_,
			Add,
			Max,
			Min,
			Multiply,
			Power
		}
	}
}
