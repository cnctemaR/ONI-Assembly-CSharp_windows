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

		public override string DisplayName
		{
			get
			{
				return "Default";
			}
		}

		public override void DeclareUnityMappings(Settings settings)
		{
		}

		public override bool IsIntrinsic
		{
			get
			{
				return true;
			}
		}

		public override void InitializeProperties()
		{
			base.InitializeProperties();
			Platform.PropertyAccessors.Plugins.Set(this, new List<string>());
			Platform.PropertyAccessors.StaticPlugins.Set(this, new List<string>());
		}

		public override void EnsurePropertiesAreValid()
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
