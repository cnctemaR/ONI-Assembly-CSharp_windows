using System;
using UnityEngine;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Scale2d : ModifierModule, IModule3D, IModule
	{
		public Vector2 Scale
		{
			get
			{
				return this._scale;
			}
			set
			{
				this._scale = value;
			}
		}

		public Scale2d()
		{
		}

		public Scale2d(IModule source)
			: base(source)
		{
		}

		public Scale2d(IModule source, Vector2 scale)
			: base(source)
		{
			this._scale = scale;
		}

		public float GetValue(float x, float y, float z)
		{
			return ((IModule3D)this._sourceModule).GetValue(x * this._scale.x, y, z * this._scale.y);
		}

		public const float DEFAULT_SCALE = 1f;

		protected Vector2 _scale = Vector2.one * 1f;
	}
}
