using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise
{
	public abstract class ModifierModule : IModule
	{
		public ModifierModule()
		{
		}

		public ModifierModule(IModule source)
		{
			this._sourceModule = source;
		}

		public IModule SourceModule
		{
			get
			{
				return this._sourceModule;
			}
			set
			{
				this._sourceModule = value;
			}
		}

		protected IModule _sourceModule;
	}
}
