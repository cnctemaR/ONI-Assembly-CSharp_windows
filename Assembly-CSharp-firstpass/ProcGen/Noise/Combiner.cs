using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Combiner;

namespace ProcGen.Noise
{
	public class Combiner : NoiseBase
	{
		public override Type GetObjectType()
		{
			return typeof(Combiner);
		}

		public Combiner.CombinerType combineType { get; set; }

		public Combiner()
		{
			this.combineType = Combiner.CombinerType.Add;
		}

		public IModule3D CreateModule()
		{
			switch (this.combineType)
			{
			case Combiner.CombinerType.Add:
				return new Add();
			case Combiner.CombinerType.Max:
				return new Max();
			case Combiner.CombinerType.Min:
				return new Min();
			case Combiner.CombinerType.Multiply:
				return new Multiply();
			case Combiner.CombinerType.Power:
				return new Power();
			default:
				return null;
			}
		}

		public IModule3D CreateModule(IModule3D leftModule, IModule3D rightModule)
		{
			switch (this.combineType)
			{
			case Combiner.CombinerType.Add:
				return new Add(leftModule, rightModule);
			case Combiner.CombinerType.Max:
				return new Max(leftModule, rightModule);
			case Combiner.CombinerType.Min:
				return new Min(leftModule, rightModule);
			case Combiner.CombinerType.Multiply:
				return new Multiply(leftModule, rightModule);
			case Combiner.CombinerType.Power:
				return new Power(leftModule, rightModule);
			default:
				return null;
			}
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
