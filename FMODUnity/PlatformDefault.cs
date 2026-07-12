using System;
using System.Collections.Generic;

namespace FMODUnity
{
	public class PlatformDefault : Platform
	{
		public PlatformDefault()
		{
			base.Identifier = "default";
		}

		internal override string DisplayName
		{
			get
			{
				return "Default";
			}
		}

		internal override void DeclareRuntimePlatforms(Settings settings)
		{
		}

		internal override bool IsIntrinsic
		{
			get
			{
				return true;
			}
		}

		internal override void InitializeProperties()
		{
			base.InitializeProperties();
			Platform.PropertyAccessors.Plugins.Set(this, new List<string>());
			Platform.PropertyAccessors.StaticPlugins.Set(this, new List<string>());
		}

		internal override void EnsurePropertiesAreValid()
		{
			base.EnsurePropertiesAreValid();
			if (base.StaticPlugins == null)
			{
				Platform.PropertyAccessors.StaticPlugins.Set(this, new List<string>());
			}
		}

		public const string ConstIdentifier = "default";
	}
}
