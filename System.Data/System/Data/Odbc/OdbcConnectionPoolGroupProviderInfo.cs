using System;
using System.Data.ProviderBase;

namespace System.Data.Odbc
{
	internal sealed class OdbcConnectionPoolGroupProviderInfo : DbConnectionPoolGroupProviderInfo
	{
		internal string DriverName
		{
			get
			{
				return this._driverName;
			}
			set
			{
				this._driverName = value;
			}
		}

		internal string DriverVersion
		{
			get
			{
				return this._driverVersion;
			}
			set
			{
				this._driverVersion = value;
			}
		}

		internal bool HasQuoteChar
		{
			get
			{
				return this._hasQuoteChar;
			}
		}

		internal bool HasEscapeChar
		{
			get
			{
				return this._hasEscapeChar;
			}
		}

		internal string QuoteChar
		{
			get
			{
				return this._quoteChar;
			}
			set
			{
				this._quoteChar = value;
				this._hasQuoteChar = true;
			}
		}

		internal char EscapeChar
		{
			get
			{
				return this._escapeChar;
			}
			set
			{
				this._escapeChar = value;
				this._hasEscapeChar = true;
			}
		}

		internal bool IsV3Driver
		{
			get
			{
				return this._isV3Driver;
			}
			set
			{
				this._isV3Driver = value;
			}
		}

		internal int SupportedSQLTypes
		{
			get
			{
				return this._supportedSQLTypes;
			}
			set
			{
				this._supportedSQLTypes = value;
			}
		}

		internal int TestedSQLTypes
		{
			get
			{
				return this._testedSQLTypes;
			}
			set
			{
				this._testedSQLTypes = value;
			}
		}

		internal int RestrictedSQLBindTypes
		{
			get
			{
				return this._restrictedSQLBindTypes;
			}
			set
			{
				this._restrictedSQLBindTypes = value;
			}
		}

		internal bool NoCurrentCatalog
		{
			get
			{
				return this._noCurrentCatalog;
			}
			set
			{
				this._noCurrentCatalog = value;
			}
		}

		internal bool NoConnectionDead
		{
			get
			{
				return this._noConnectionDead;
			}
			set
			{
				this._noConnectionDead = value;
			}
		}

		internal bool NoQueryTimeout
		{
			get
			{
				return this._noQueryTimeout;
			}
			set
			{
				this._noQueryTimeout = value;
			}
		}

		internal bool NoSqlSoptSSNoBrowseTable
		{
			get
			{
				return this._noSqlSoptSSNoBrowseTable;
			}
			set
			{
				this._noSqlSoptSSNoBrowseTable = value;
			}
		}

		internal bool NoSqlSoptSSHiddenColumns
		{
			get
			{
				return this._noSqlSoptSSHiddenColumns;
			}
			set
			{
				this._noSqlSoptSSHiddenColumns = value;
			}
		}

		internal bool NoSqlCASSColumnKey
		{
			get
			{
				return this._noSqlCASSColumnKey;
			}
			set
			{
				this._noSqlCASSColumnKey = value;
			}
		}

		internal bool NoSqlPrimaryKeys
		{
			get
			{
				return this._noSqlPrimaryKeys;
			}
			set
			{
				this._noSqlPrimaryKeys = value;
			}
		}

		private string _driverName;

		private string _driverVersion;

		private string _quoteChar;

		private char _escapeChar;

		private bool _hasQuoteChar;

		private bool _hasEscapeChar;

		private bool _isV3Driver;

		private int _supportedSQLTypes;

		private int _testedSQLTypes;

		private int _restrictedSQLBindTypes;

		private bool _noCurrentCatalog;

		private bool _noConnectionDead;

		private bool _noQueryTimeout;

		private bool _noSqlSoptSSNoBrowseTable;

		private bool _noSqlSoptSSHiddenColumns;

		private bool _noSqlCASSColumnKey;

		private bool _noSqlPrimaryKeys;
	}
}
