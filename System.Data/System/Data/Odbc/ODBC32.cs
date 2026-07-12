using System;
using System.Data.Common;
using System.Text;

namespace System.Data.Odbc
{
	public static class ODBC32
	{
		internal static string RetcodeToString(ODBC32.RetCode retcode)
		{
			switch (retcode)
			{
			case ODBC32.RetCode.INVALID_HANDLE:
				return "INVALID_HANDLE";
			case ODBC32.RetCode.ERROR:
				break;
			case ODBC32.RetCode.SUCCESS:
				return "SUCCESS";
			case ODBC32.RetCode.SUCCESS_WITH_INFO:
				return "SUCCESS_WITH_INFO";
			default:
				if (retcode == ODBC32.RetCode.NO_DATA)
				{
					return "NO_DATA";
				}
				break;
			}
			return "ERROR";
		}

		internal static OdbcErrorCollection GetDiagErrors(string source, OdbcHandle hrHandle, ODBC32.RetCode retcode)
		{
			OdbcErrorCollection odbcErrorCollection = new OdbcErrorCollection();
			ODBC32.GetDiagErrors(odbcErrorCollection, source, hrHandle, retcode);
			return odbcErrorCollection;
		}

		internal static void GetDiagErrors(OdbcErrorCollection errors, string source, OdbcHandle hrHandle, ODBC32.RetCode retcode)
		{
			if (retcode != ODBC32.RetCode.SUCCESS)
			{
				short num = 0;
				short num2 = 0;
				StringBuilder stringBuilder = new StringBuilder(1024);
				bool flag = true;
				while (flag)
				{
					num += 1;
					string text;
					int num3;
					retcode = hrHandle.GetDiagnosticRecord(num, out text, stringBuilder, out num3, out num2);
					if (ODBC32.RetCode.SUCCESS_WITH_INFO == retcode && stringBuilder.Capacity - 1 < (int)num2)
					{
						stringBuilder.Capacity = (int)(num2 + 1);
						retcode = hrHandle.GetDiagnosticRecord(num, out text, stringBuilder, out num3, out num2);
					}
					flag = retcode == ODBC32.RetCode.SUCCESS || retcode == ODBC32.RetCode.SUCCESS_WITH_INFO;
					if (flag)
					{
						errors.Add(new OdbcError(source, stringBuilder.ToString(), text, num3));
					}
				}
			}
		}

		internal const short SQL_COMMIT = 0;

		internal const short SQL_ROLLBACK = 1;

		internal static readonly IntPtr SQL_AUTOCOMMIT_OFF = ADP.PtrZero;

		internal static readonly IntPtr SQL_AUTOCOMMIT_ON = new IntPtr(1);

		private const int SIGNED_OFFSET = -20;

		private const int UNSIGNED_OFFSET = -22;

		internal const short SQL_ALL_TYPES = 0;

		internal static readonly IntPtr SQL_HANDLE_NULL = ADP.PtrZero;

		internal const int SQL_NULL_DATA = -1;

		internal const int SQL_NO_TOTAL = -4;

		internal const int SQL_DEFAULT_PARAM = -5;

		internal const int COLUMN_NAME = 4;

		internal const int COLUMN_TYPE = 5;

		internal const int DATA_TYPE = 6;

		internal const int COLUMN_SIZE = 8;

		internal const int DECIMAL_DIGITS = 10;

		internal const int NUM_PREC_RADIX = 11;

		internal static readonly IntPtr SQL_OV_ODBC3 = new IntPtr(3);

		internal const int SQL_NTS = -3;

		internal static readonly IntPtr SQL_CP_OFF = new IntPtr(0);

		internal static readonly IntPtr SQL_CP_ONE_PER_DRIVER = new IntPtr(1);

		internal static readonly IntPtr SQL_CP_ONE_PER_HENV = new IntPtr(2);

		internal const int SQL_CD_TRUE = 1;

		internal const int SQL_CD_FALSE = 0;

		internal const int SQL_DTC_DONE = 0;

		internal const int SQL_IS_POINTER = -4;

		internal const int SQL_IS_PTR = 1;

		internal const int MAX_CONNECTION_STRING_LENGTH = 1024;

		internal const short SQL_DIAG_SQLSTATE = 4;

		internal const short SQL_RESULT_COL = 3;

		internal enum SQL_HANDLE : short
		{
			ENV = 1,
			DBC,
			STMT,
			DESC
		}

		public enum RETCODE
		{
			SUCCESS,
			SUCCESS_WITH_INFO,
			ERROR = -1,
			INVALID_HANDLE = -2,
			NO_DATA = 100
		}

		internal enum RetCode : short
		{
			SUCCESS,
			SUCCESS_WITH_INFO,
			ERROR = -1,
			INVALID_HANDLE = -2,
			NO_DATA = 100
		}

		internal enum SQL_CONVERT : ushort
		{
			BIGINT = 53,
			BINARY,
			BIT,
			CHAR,
			DATE,
			DECIMAL,
			DOUBLE,
			FLOAT,
			INTEGER,
			LONGVARCHAR,
			NUMERIC,
			REAL,
			SMALLINT,
			TIME,
			TIMESTAMP,
			TINYINT,
			VARBINARY,
			VARCHAR,
			LONGVARBINARY
		}

		[Flags]
		internal enum SQL_CVT
		{
			CHAR = 1,
			NUMERIC = 2,
			DECIMAL = 4,
			INTEGER = 8,
			SMALLINT = 16,
			FLOAT = 32,
			REAL = 64,
			DOUBLE = 128,
			VARCHAR = 256,
			LONGVARCHAR = 512,
			BINARY = 1024,
			VARBINARY = 2048,
			BIT = 4096,
			TINYINT = 8192,
			BIGINT = 16384,
			DATE = 32768,
			TIME = 65536,
			TIMESTAMP = 131072,
			LONGVARBINARY = 262144,
			INTERVAL_YEAR_MONTH = 524288,
			INTERVAL_DAY_TIME = 1048576,
			WCHAR = 2097152,
			WLONGVARCHAR = 4194304,
			WVARCHAR = 8388608,
			GUID = 16777216
		}

		internal enum STMT : short
		{
			CLOSE,
			DROP,
			UNBIND,
			RESET_PARAMS
		}

		internal enum SQL_MAX
		{
			NUMERIC_LEN = 16
		}

		internal enum SQL_IS
		{
			POINTER = -4,
			INTEGER = -6,
			UINTEGER,
			SMALLINT = -8
		}

		internal enum SQL_HC
		{
			OFF,
			ON
		}

		internal enum SQL_NB
		{
			OFF,
			ON
		}

		internal enum SQL_CA_SS
		{
			BASE = 1200,
			COLUMN_HIDDEN = 1211,
			COLUMN_KEY,
			VARIANT_TYPE = 1215,
			VARIANT_SQL_TYPE,
			VARIANT_SERVER_TYPE
		}

		internal enum SQL_SOPT_SS
		{
			BASE = 1225,
			HIDDEN_COLUMNS = 1227,
			NOBROWSETABLE
		}

		internal enum SQL_TRANSACTION
		{
			READ_UNCOMMITTED = 1,
			READ_COMMITTED,
			REPEATABLE_READ = 4,
			SERIALIZABLE = 8,
			SNAPSHOT = 32
		}

		internal enum SQL_PARAM
		{
			INPUT = 1,
			INPUT_OUTPUT,
			OUTPUT = 4,
			RETURN_VALUE
		}

		internal enum SQL_API : ushort
		{
			SQLCOLUMNS = 40,
			SQLEXECDIRECT = 11,
			SQLGETTYPEINFO = 47,
			SQLPROCEDURECOLUMNS = 66,
			SQLPROCEDURES,
			SQLSTATISTICS = 53,
			SQLTABLES
		}

		internal enum SQL_DESC : short
		{
			COUNT = 1001,
			TYPE,
			LENGTH,
			OCTET_LENGTH_PTR,
			PRECISION,
			SCALE,
			DATETIME_INTERVAL_CODE,
			NULLABLE,
			INDICATOR_PTR,
			DATA_PTR,
			NAME,
			UNNAMED,
			OCTET_LENGTH,
			ALLOC_TYPE = 1099,
			CONCISE_TYPE = 2,
			DISPLAY_SIZE = 6,
			UNSIGNED = 8,
			UPDATABLE = 10,
			AUTO_UNIQUE_VALUE,
			TYPE_NAME = 14,
			TABLE_NAME,
			SCHEMA_NAME,
			CATALOG_NAME,
			BASE_COLUMN_NAME = 22,
			BASE_TABLE_NAME
		}

		internal enum SQL_COLUMN
		{
			COUNT,
			NAME,
			TYPE,
			LENGTH,
			PRECISION,
			SCALE,
			DISPLAY_SIZE,
			NULLABLE,
			UNSIGNED,
			MONEY,
			UPDATABLE,
			AUTO_INCREMENT,
			CASE_SENSITIVE,
			SEARCHABLE,
			TYPE_NAME,
			TABLE_NAME,
			OWNER_NAME,
			QUALIFIER_NAME,
			LABEL
		}

		internal enum SQL_GROUP_BY
		{
			NOT_SUPPORTED,
			GROUP_BY_EQUALS_SELECT,
			GROUP_BY_CONTAINS_SELECT,
			NO_RELATION,
			COLLATE
		}

		internal enum SQL_SQL92_RELATIONAL_JOIN_OPERATORS
		{
			CORRESPONDING_CLAUSE = 1,
			CROSS_JOIN,
			EXCEPT_JOIN = 4,
			FULL_OUTER_JOIN = 8,
			INNER_JOIN = 16,
			INTERSECT_JOIN = 32,
			LEFT_OUTER_JOIN = 64,
			NATURAL_JOIN = 128,
			RIGHT_OUTER_JOIN = 256,
			UNION_JOIN = 512
		}

		internal enum SQL_OJ_CAPABILITIES
		{
			LEFT = 1,
			RIGHT,
			FULL = 4,
			NESTED = 8,
			NOT_ORDERED = 16,
			INNER = 32,
			ALL_COMPARISON_OPS = 64
		}

		internal enum SQL_UPDATABLE
		{
			READONLY,
			WRITE,
			READWRITE_UNKNOWN
		}

		internal enum SQL_IDENTIFIER_CASE
		{
			UPPER = 1,
			LOWER,
			SENSITIVE,
			MIXED
		}

		internal enum SQL_INDEX : short
		{
			UNIQUE,
			ALL
		}

		internal enum SQL_STATISTICS_RESERVED : short
		{
			QUICK,
			ENSURE
		}

		internal enum SQL_SPECIALCOLS : ushort
		{
			BEST_ROWID = 1,
			ROWVER
		}

		internal enum SQL_SCOPE : ushort
		{
			CURROW,
			TRANSACTION,
			SESSION
		}

		internal enum SQL_NULLABILITY : ushort
		{
			NO_NULLS,
			NULLABLE,
			UNKNOWN
		}

		internal enum SQL_SEARCHABLE
		{
			UNSEARCHABLE,
			LIKE_ONLY,
			ALL_EXCEPT_LIKE,
			SEARCHABLE
		}

		internal enum SQL_UNNAMED
		{
			NAMED,
			UNNAMED
		}

		internal enum HANDLER
		{
			IGNORE,
			THROW
		}

		internal enum SQL_STATISTICSTYPE
		{
			TABLE_STAT,
			INDEX_CLUSTERED,
			INDEX_HASHED,
			INDEX_OTHER
		}

		internal enum SQL_PROCEDURETYPE
		{
			UNKNOWN,
			PROCEDURE,
			FUNCTION
		}

		internal enum SQL_C : short
		{
			CHAR = 1,
			WCHAR = -8,
			SLONG = -16,
			SSHORT,
			REAL = 7,
			DOUBLE,
			BIT = -7,
			UTINYINT = -28,
			SBIGINT = -25,
			UBIGINT = -27,
			BINARY = -2,
			TIMESTAMP = 11,
			TYPE_DATE = 91,
			TYPE_TIME,
			TYPE_TIMESTAMP,
			NUMERIC = 2,
			GUID = -11,
			DEFAULT = 99,
			ARD_TYPE = -99
		}

		internal enum SQL_TYPE : short
		{
			CHAR = 1,
			VARCHAR = 12,
			LONGVARCHAR = -1,
			WCHAR = -8,
			WVARCHAR = -9,
			WLONGVARCHAR = -10,
			DECIMAL = 3,
			NUMERIC = 2,
			SMALLINT = 5,
			INTEGER = 4,
			REAL = 7,
			FLOAT = 6,
			DOUBLE = 8,
			BIT = -7,
			TINYINT,
			BIGINT,
			BINARY = -2,
			VARBINARY = -3,
			LONGVARBINARY = -4,
			TYPE_DATE = 91,
			TYPE_TIME,
			TIMESTAMP = 11,
			TYPE_TIMESTAMP = 93,
			GUID = -11,
			SS_VARIANT = -150,
			SS_UDT = -151,
			SS_XML = -152,
			SS_UTCDATETIME = -153,
			SS_TIME_EX = -154
		}

		internal enum SQL_ATTR
		{
			APP_ROW_DESC = 10010,
			APP_PARAM_DESC,
			IMP_ROW_DESC,
			IMP_PARAM_DESC,
			METADATA_ID,
			ODBC_VERSION = 200,
			CONNECTION_POOLING,
			AUTOCOMMIT = 102,
			TXN_ISOLATION = 108,
			CURRENT_CATALOG,
			LOGIN_TIMEOUT = 103,
			QUERY_TIMEOUT = 0,
			CONNECTION_DEAD = 1209,
			SQL_COPT_SS_BASE = 1200,
			SQL_COPT_SS_ENLIST_IN_DTC = 1207,
			SQL_COPT_SS_TXN_ISOLATION = 1227
		}

		internal enum SQL_INFO : ushort
		{
			DATA_SOURCE_NAME = 2,
			SERVER_NAME = 13,
			DRIVER_NAME = 6,
			DRIVER_VER,
			ODBC_VER = 10,
			SEARCH_PATTERN_ESCAPE = 14,
			DBMS_VER = 18,
			DBMS_NAME = 17,
			IDENTIFIER_CASE = 28,
			IDENTIFIER_QUOTE_CHAR,
			CATALOG_NAME_SEPARATOR = 41,
			DRIVER_ODBC_VER = 77,
			GROUP_BY = 88,
			KEYWORDS,
			ORDER_BY_COLUMNS_IN_SELECT,
			QUOTED_IDENTIFIER_CASE = 93,
			SQL_OJ_CAPABILITIES_30 = 115,
			SQL_OJ_CAPABILITIES_20 = 65003,
			SQL_SQL92_RELATIONAL_JOIN_OPERATORS = 161
		}

		internal enum SQL_DRIVER
		{
			NOPROMPT,
			COMPLETE,
			PROMPT,
			COMPLETE_REQUIRED
		}

		internal enum SQL_PRIMARYKEYS : short
		{
			COLUMNNAME = 4
		}

		internal enum SQL_STATISTICS : short
		{
			INDEXNAME = 6,
			ORDINAL_POSITION = 8,
			COLUMN_NAME
		}

		internal enum SQL_SPECIALCOLUMNSET : short
		{
			COLUMN_NAME = 2
		}
	}
}
