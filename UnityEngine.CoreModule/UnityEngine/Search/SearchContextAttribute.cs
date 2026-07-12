using System;

namespace UnityEngine.Search
{
	[AttributeUsage(AttributeTargets.Field)]
	public class SearchContextAttribute : PropertyAttribute
	{
		public string query { get; private set; }

		public string[] providerIds { get; private set; }

		public Type[] instantiableProviders { get; private set; }

		public SearchViewFlags flags { get; private set; }

		public SearchContextAttribute(string query)
			: this(query, null, SearchViewFlags.None)
		{
		}

		public SearchContextAttribute(string query, SearchViewFlags flags)
			: this(query, null, flags)
		{
		}

		public SearchContextAttribute(string query, string providerIdsCommaSeparated)
			: this(query, providerIdsCommaSeparated, SearchViewFlags.None)
		{
		}

		public SearchContextAttribute(string query, string providerIdsCommaSeparated, SearchViewFlags flags)
			: this(query, flags, providerIdsCommaSeparated, null)
		{
		}

		public SearchContextAttribute(string query, params Type[] instantiableProviders)
			: this(query, SearchViewFlags.None, null, instantiableProviders)
		{
		}

		public SearchContextAttribute(string query, SearchViewFlags flags, params Type[] instantiableProviders)
			: this(query, flags, null, instantiableProviders)
		{
		}

		public SearchContextAttribute(string query, SearchViewFlags flags, string providerIdsCommaSeparated, params Type[] instantiableProviders)
		{
			this.query = query;
			this.providerIds = ((providerIdsCommaSeparated != null) ? providerIdsCommaSeparated.Split(new char[] { ',', ';' }) : null) ?? new string[0];
			this.instantiableProviders = instantiableProviders ?? new Type[0];
			this.flags = flags | SearchViewFlags.ObjectPicker;
		}
	}
}
