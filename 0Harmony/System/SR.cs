using System;
using System.Resources;
using System.Runtime.CompilerServices;
using FxResources.System.ValueTuple;

namespace System
{
	internal static class SR
	{
		private static ResourceManager ResourceManager
		{
			get
			{
				ResourceManager resourceManager;
				if ((resourceManager = global::System.SR.s_resourceManager) == null)
				{
					resourceManager = (global::System.SR.s_resourceManager = new ResourceManager(global::System.SR.ResourceType));
				}
				return resourceManager;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool UsingResourceKeys()
		{
			return false;
		}

		internal static string GetResourceString(string resourceKey, string defaultString)
		{
			string text = null;
			try
			{
				text = global::System.SR.ResourceManager.GetString(resourceKey);
			}
			catch (MissingManifestResourceException)
			{
			}
			if (defaultString != null && resourceKey.Equals(text, StringComparison.Ordinal))
			{
				return defaultString;
			}
			return text;
		}

		internal static string Format(string resourceFormat, params object[] args)
		{
			if (args == null)
			{
				return resourceFormat;
			}
			if (global::System.SR.UsingResourceKeys())
			{
				return resourceFormat + string.Join(", ", args);
			}
			return string.Format(resourceFormat, args);
		}

		internal static string Format(string resourceFormat, object p1)
		{
			if (global::System.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[] { resourceFormat, p1 });
			}
			return string.Format(resourceFormat, p1);
		}

		internal static string Format(string resourceFormat, object p1, object p2)
		{
			if (global::System.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[] { resourceFormat, p1, p2 });
			}
			return string.Format(resourceFormat, p1, p2);
		}

		internal static string Format(string resourceFormat, object p1, object p2, object p3)
		{
			if (global::System.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[] { resourceFormat, p1, p2, p3 });
			}
			return string.Format(resourceFormat, p1, p2, p3);
		}

		internal static Type ResourceType { get; } = typeof(FxResources.System.ValueTuple.SR);

		internal static string ArgumentException_ValueTupleIncorrectType
		{
			get
			{
				return global::System.SR.GetResourceString("ArgumentException_ValueTupleIncorrectType", null);
			}
		}

		internal static string ArgumentException_ValueTupleLastArgumentNotAValueTuple
		{
			get
			{
				return global::System.SR.GetResourceString("ArgumentException_ValueTupleLastArgumentNotAValueTuple", null);
			}
		}

		private static ResourceManager s_resourceManager;
	}
}
