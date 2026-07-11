using System;

namespace UnityEngine.Yoga
{
	internal class YogaConfig
	{
		private YogaConfig(IntPtr ygConfig)
		{
			this._ygConfig = ygConfig;
			if (this._ygConfig == IntPtr.Zero)
			{
				throw new InvalidOperationException("Failed to allocate native memory");
			}
		}

		public YogaConfig()
			: this(Native.YGConfigNew())
		{
		}

		~YogaConfig()
		{
			if (this.Handle != YogaConfig.Default.Handle)
			{
				Native.YGConfigFree(this.Handle);
			}
		}

		internal IntPtr Handle
		{
			get
			{
				return this._ygConfig;
			}
		}

		public Logger Logger
		{
			get
			{
				return this._logger;
			}
			set
			{
				this._logger = value;
			}
		}

		public void SetExperimentalFeatureEnabled(YogaExperimentalFeature feature, bool enabled)
		{
			Native.YGConfigSetExperimentalFeatureEnabled(this._ygConfig, feature, enabled);
		}

		public bool IsExperimentalFeatureEnabled(YogaExperimentalFeature feature)
		{
			return Native.YGConfigIsExperimentalFeatureEnabled(this._ygConfig, feature);
		}

		public bool UseWebDefaults
		{
			get
			{
				return Native.YGConfigGetUseWebDefaults(this._ygConfig);
			}
			set
			{
				Native.YGConfigSetUseWebDefaults(this._ygConfig, value);
			}
		}

		public float PointScaleFactor
		{
			set
			{
				Native.YGConfigSetPointScaleFactor(this._ygConfig, value);
			}
		}

		public static int GetInstanceCount()
		{
			return Native.YGConfigGetInstanceCount();
		}

		public static void SetDefaultLogger(Logger logger)
		{
			YogaConfig.Default.Logger = logger;
		}

		internal static readonly YogaConfig Default = new YogaConfig(Native.YGConfigGetDefault());

		private IntPtr _ygConfig;

		private Logger _logger;
	}
}
