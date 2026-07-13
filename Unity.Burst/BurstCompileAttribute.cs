using System;

namespace Unity.Burst
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
	public class BurstCompileAttribute : Attribute
	{
		public FloatMode FloatMode { get; set; }

		public FloatPrecision FloatPrecision { get; set; }

		public bool CompileSynchronously
		{
			get
			{
				return this._compileSynchronously != null && this._compileSynchronously.Value;
			}
			set
			{
				this._compileSynchronously = new bool?(value);
			}
		}

		public bool Debug
		{
			get
			{
				return this._debug != null && this._debug.Value;
			}
			set
			{
				this._debug = new bool?(value);
			}
		}

		public bool DisableSafetyChecks
		{
			get
			{
				return this._disableSafetyChecks != null && this._disableSafetyChecks.Value;
			}
			set
			{
				this._disableSafetyChecks = new bool?(value);
			}
		}

		public bool DisableDirectCall
		{
			get
			{
				return this._disableDirectCall != null && this._disableDirectCall.Value;
			}
			set
			{
				this._disableDirectCall = new bool?(value);
			}
		}

		public OptimizeFor OptimizeFor { get; set; }

		internal string[] Options { get; set; }

		public BurstCompileAttribute()
		{
		}

		public BurstCompileAttribute(FloatPrecision floatPrecision, FloatMode floatMode)
		{
			this.FloatMode = floatMode;
			this.FloatPrecision = floatPrecision;
		}

		internal BurstCompileAttribute(string[] options)
		{
			this.Options = options;
		}

		internal bool? _compileSynchronously;

		internal bool? _debug;

		internal bool? _disableSafetyChecks;

		internal bool? _disableDirectCall;
	}
}
