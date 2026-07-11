using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise
{
	public abstract class CombinerModule : IModule
	{
		public IModule LeftModule
		{
			get
			{
				return this._leftModule;
			}
			set
			{
				this._leftModule = value;
			}
		}

		public IModule RightModule
		{
			get
			{
				return this._rightModule;
			}
			set
			{
				this._rightModule = value;
			}
		}

		public CombinerModule()
		{
		}

		public CombinerModule(IModule left, IModule right)
		{
			this._leftModule = left;
			this._rightModule = right;
		}

		protected IModule _rightModule;

		protected IModule _leftModule;
	}
}
