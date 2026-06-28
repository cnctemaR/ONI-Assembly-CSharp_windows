using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Model
{
	public class AbstractModel
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

		public AbstractModel()
		{
		}

		public AbstractModel(IModule module)
		{
			this._sourceModule = module;
		}

		protected IModule _sourceModule;
	}
}
