using System;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.Utilities
{
	internal sealed class ObjectAnchorCollection
	{
		public void Add(string anchor, object @object)
		{
			this.objectsByAnchor.Add(anchor, @object);
			if (@object != null)
			{
				this.anchorsByObject.Add(@object, anchor);
			}
		}

		public bool TryGetAnchor(object @object, out string anchor)
		{
			return this.anchorsByObject.TryGetValue(@object, out anchor);
		}

		public object this[string anchor]
		{
			get
			{
				object obj;
				if (this.objectsByAnchor.TryGetValue(anchor, out obj))
				{
					return obj;
				}
				throw new AnchorNotFoundException(string.Format(CultureInfo.InvariantCulture, "The anchor '{0}' does not exists", anchor));
			}
		}

		private readonly IDictionary<string, object> objectsByAnchor = new Dictionary<string, object>();

		private readonly IDictionary<object, string> anchorsByObject = new Dictionary<object, string>();
	}
}
