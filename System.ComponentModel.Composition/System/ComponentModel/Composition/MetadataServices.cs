using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
	internal static class MetadataServices
	{
		public static IDictionary<string, object> AsReadOnly(this IDictionary<string, object> metadata)
		{
			if (metadata == null)
			{
				return MetadataServices.EmptyMetadata;
			}
			if (metadata is ReadOnlyDictionary<string, object>)
			{
				return metadata;
			}
			return new ReadOnlyDictionary<string, object>(metadata);
		}

		public static T GetValue<T>(this IDictionary<string, object> metadata, string key)
		{
			Assumes.NotNull<IDictionary<string, object>, string>(metadata, "metadata");
			object obj = true;
			if (!metadata.TryGetValue(key, out obj))
			{
				return default(T);
			}
			if (obj is T)
			{
				return (T)((object)obj);
			}
			return default(T);
		}

		public static readonly IDictionary<string, object> EmptyMetadata = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>(0));
	}
}
