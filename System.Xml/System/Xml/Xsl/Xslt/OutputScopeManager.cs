using System;

namespace System.Xml.Xsl.Xslt
{
	internal class OutputScopeManager
	{
		public OutputScopeManager()
		{
			this.Reset();
		}

		public void Reset()
		{
			this.records[0].prefix = null;
			this.records[0].nsUri = null;
			this.PushScope();
		}

		public void PushScope()
		{
			this.lastScopes++;
		}

		public void PopScope()
		{
			if (0 < this.lastScopes)
			{
				this.lastScopes--;
				return;
			}
			OutputScopeManager.ScopeReord[] array;
			int num;
			do
			{
				array = this.records;
				num = this.lastRecord - 1;
				this.lastRecord = num;
			}
			while (array[num].scopeCount == 0);
			this.lastScopes = this.records[this.lastRecord].scopeCount;
			this.lastScopes--;
		}

		public void AddNamespace(string prefix, string uri)
		{
			this.AddRecord(prefix, uri);
		}

		private void AddRecord(string prefix, string uri)
		{
			this.records[this.lastRecord].scopeCount = this.lastScopes;
			this.lastRecord++;
			if (this.lastRecord == this.records.Length)
			{
				OutputScopeManager.ScopeReord[] array = new OutputScopeManager.ScopeReord[this.lastRecord * 2];
				Array.Copy(this.records, 0, array, 0, this.lastRecord);
				this.records = array;
			}
			this.lastScopes = 0;
			this.records[this.lastRecord].prefix = prefix;
			this.records[this.lastRecord].nsUri = uri;
		}

		public void InvalidateAllPrefixes()
		{
			if (this.records[this.lastRecord].prefix == null)
			{
				return;
			}
			this.AddRecord(null, null);
		}

		public void InvalidateNonDefaultPrefixes()
		{
			string text = this.LookupNamespace(string.Empty);
			if (text == null)
			{
				this.InvalidateAllPrefixes();
				return;
			}
			if (this.records[this.lastRecord].prefix.Length == 0 && this.records[this.lastRecord - 1].prefix == null)
			{
				return;
			}
			this.AddRecord(null, null);
			this.AddRecord(string.Empty, text);
		}

		public string LookupNamespace(string prefix)
		{
			int num = this.lastRecord;
			while (this.records[num].prefix != null)
			{
				if (this.records[num].prefix == prefix)
				{
					return this.records[num].nsUri;
				}
				num--;
			}
			return null;
		}

		private OutputScopeManager.ScopeReord[] records = new OutputScopeManager.ScopeReord[32];

		private int lastRecord;

		private int lastScopes;

		public struct ScopeReord
		{
			public int scopeCount;

			public string prefix;

			public string nsUri;
		}
	}
}
