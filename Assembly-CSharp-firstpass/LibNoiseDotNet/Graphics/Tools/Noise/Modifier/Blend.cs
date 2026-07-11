using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Blend : SelectorModule, IModule3D, IModule
	{
		public Blend()
		{
		}

		public Blend(IModule controlModule, IModule rightModule, IModule leftModule)
		{
			this._controlModule = controlModule;
			this._leftModule = leftModule;
			this._rightModule = rightModule;
		}

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

		public IModule ControlModule
		{
			get
			{
				return this._controlModule;
			}
			set
			{
				this._controlModule = value;
			}
		}

		public float GetValue(float x, float y, float z)
		{
			float value = ((IModule3D)this._leftModule).GetValue(x, y, z);
			float value2 = ((IModule3D)this._rightModule).GetValue(x, y, z);
			float num = (((IModule3D)this._controlModule).GetValue(x, y, z) + 1f) / 2f;
			return Libnoise.Lerp(value, value2, num);
		}

		protected IModule _controlModule;

		protected IModule _rightModule;

		protected IModule _leftModule;
	}
}
