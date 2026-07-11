using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise
{
	public abstract class ModifierModule : IModule
	{
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

		public ModifierModule()
		{
		}

		public ModifierModule(IModule source)
		{
			this._sourceModule = source;
		}

		protected IModule _sourceModule;
	}
}
