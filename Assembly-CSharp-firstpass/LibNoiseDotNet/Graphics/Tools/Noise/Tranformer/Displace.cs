using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Tranformer
{
	public class Displace : TransformerModule, IModule3D, IModule
	{
		public Displace()
		{
		}

		public Displace(IModule source, IModule xDisplaceModule, IModule yDisplaceModule, IModule zDisplaceModule)
		{
			this._sourceModule = source;
			this._xDisplaceModule = xDisplaceModule;
			this._yDisplaceModule = yDisplaceModule;
			this._zDisplaceModule = zDisplaceModule;
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

		public IModule XDisplaceModule
		{
			get
			{
				return this._xDisplaceModule;
			}
			set
			{
				this._xDisplaceModule = value;
			}
		}

		public IModule YDisplaceModule
		{
			get
			{
				return this._yDisplaceModule;
			}
			set
			{
				this._yDisplaceModule = value;
			}
		}

		public IModule ZDisplaceModule
		{
			get
			{
				return this._zDisplaceModule;
			}
			set
			{
				this._zDisplaceModule = value;
			}
		}

		public float GetValue(float x, float y, float z)
		{
			float num = x + ((IModule3D)this._xDisplaceModule).GetValue(x, y, z);
			float num2 = y + ((IModule3D)this._yDisplaceModule).GetValue(x, y, z);
			float num3 = z + ((IModule3D)this._zDisplaceModule).GetValue(x, y, z);
			return ((IModule3D)this._sourceModule).GetValue(num, num2, num3);
		}

		protected IModule _sourceModule;

		protected IModule _xDisplaceModule;

		protected IModule _yDisplaceModule;

		protected IModule _zDisplaceModule;
	}
}
