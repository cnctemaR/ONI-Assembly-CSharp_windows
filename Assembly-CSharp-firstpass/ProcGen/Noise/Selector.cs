using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Modifier;

namespace ProcGen.Noise
{
	public class Selector : NoiseBase
	{
		public override Type GetObjectType()
		{
			return typeof(Selector);
		}

		public Selector.SelectType selectType { get; set; }

		public float lower { get; set; }

		public float upper { get; set; }

		public float edge { get; set; }

		public Selector()
		{
			this.selectType = Selector.SelectType.Blend;
			this.lower = 0f;
			this.upper = 1f;
			this.edge = 0.02f;
		}

		public IModule3D CreateModule()
		{
			if (this.selectType == Selector.SelectType.Blend)
			{
				return new Blend();
			}
			Select select = new Select();
			select.SetBounds(this.lower, this.upper);
			select.EdgeFalloff = this.edge;
			return select;
		}

		public IModule3D CreateModule(IModule3D selectModule, IModule3D leftModule, IModule3D rightModule)
		{
			if (this.selectType == Selector.SelectType.Blend)
			{
				return new Blend(selectModule, rightModule, leftModule);
			}
			return new Select(selectModule, rightModule, leftModule, this.lower, this.upper, this.edge);
		}

		public void SetSouces(IModule3D target, IModule3D controlModule, IModule3D rightModule, IModule3D leftModule)
		{
			if (this.selectType == Selector.SelectType.Blend)
			{
				Blend blend = target as Blend;
				blend.ControlModule = controlModule;
				blend.RightModule = rightModule;
				blend.LeftModule = leftModule;
			}
			Select select = target as Select;
			select.ControlModule = controlModule;
			select.RightModule = rightModule;
			select.LeftModule = leftModule;
		}

		public enum SelectType
		{
			_UNSET_,
			Blend,
			Select
		}
	}
}
