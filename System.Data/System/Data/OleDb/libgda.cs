using System;
using System.Runtime.InteropServices;

namespace System.Data.OleDb
{
	internal sealed class libgda
	{
		public static IntPtr GdaClient
		{
			get
			{
				if (libgda.gdaClient == IntPtr.Zero)
				{
					libgda.gdaClient = libgda.gda_client_new();
				}
				return libgda.gdaClient;
			}
		}

		[DllImport("gobject-2.0", EntryPoint = "g_object_unref")]
		public static extern void FreeObject(IntPtr obj);

		[DllImport("gda-2")]
		public static extern void gda_init(string app_id, string version, int nargs, string[] args);

		[DllImport("gda-2")]
		public static extern GdaValueType gda_value_get_type(IntPtr value);

		[DllImport("gda-2")]
		public static extern long gda_value_get_bigint(IntPtr value);

		[DllImport("gda-2")]
		public static extern bool gda_value_get_boolean(IntPtr value);

		[DllImport("gda-2")]
		public static extern IntPtr gda_value_get_date(IntPtr value);

		[DllImport("gda-2")]
		public static extern double gda_value_get_double(IntPtr value);

		[DllImport("gda-2")]
		public static extern int gda_value_get_integer(IntPtr value);

		[DllImport("gda-2")]
		public static extern float gda_value_get_single(IntPtr value);

		[DllImport("gda-2")]
		public static extern int gda_value_get_smallint(IntPtr value);

		[DllImport("gda-2")]
		public static extern string gda_value_get_string(IntPtr value);

		[DllImport("gda-2")]
		public static extern IntPtr gda_value_get_time(IntPtr value);

		[DllImport("gda-2")]
		public static extern IntPtr gda_value_get_timestamp(IntPtr value);

		[DllImport("gda-2")]
		public static extern byte gda_value_get_tinyint(IntPtr value);

		[DllImport("gda-2")]
		public static extern bool gda_value_is_null(IntPtr value);

		[DllImport("gda-2")]
		public static extern string gda_value_stringify(IntPtr value);

		[DllImport("gda-2")]
		public static extern IntPtr gda_parameter_list_new();

		[DllImport("gda-2")]
		public static extern string gda_type_to_string(GdaValueType type);

		[DllImport("gda-2")]
		public static extern int gda_data_model_get_n_rows(IntPtr model);

		[DllImport("gda-2")]
		public static extern int gda_data_model_get_n_columns(IntPtr model);

		[DllImport("gda-2")]
		public static extern IntPtr gda_data_model_get_value_at(IntPtr model, int col, int row);

		[DllImport("gda-2")]
		public static extern string gda_data_model_get_column_title(IntPtr model, int col);

		[DllImport("gda-2")]
		public static extern IntPtr gda_data_model_describe_column(IntPtr model, int col);

		[DllImport("gda-2")]
		public static extern int gda_data_model_get_column_position(IntPtr model, string name);

		[DllImport("gda-2")]
		public static extern void gda_field_attributes_free(IntPtr fa);

		[DllImport("gda-2")]
		public static extern string gda_field_attributes_get_name(IntPtr fa);

		[DllImport("gda-2")]
		public static extern GdaValueType gda_field_attributes_get_gdatype(IntPtr fa);

		[DllImport("gda-2")]
		public static extern long gda_field_attributes_get_defined_size(IntPtr fa);

		[DllImport("gda-2")]
		public static extern long gda_field_attributes_get_scale(IntPtr fa);

		[DllImport("gda-2")]
		public static extern bool gda_field_attributes_get_allow_null(IntPtr fa);

		[DllImport("gda-2")]
		public static extern bool gda_field_attributes_get_primary_key(IntPtr fa);

		[DllImport("gda-2")]
		public static extern bool gda_field_attributes_get_unique_key(IntPtr fa);

		[DllImport("gda-2")]
		public static extern IntPtr gda_client_new();

		[DllImport("gda-2")]
		public static extern IntPtr gda_client_open_connection(IntPtr client, string dsn, string username, string password, GdaConnectionOptions options);

		[DllImport("gda-2")]
		public static extern IntPtr gda_client_open_connection_from_string(IntPtr client, string provider, string cnc_string, GdaConnectionOptions options);

		[DllImport("gda-2")]
		public static extern bool gda_connection_is_open(IntPtr cnc);

		[DllImport("gda-2")]
		public static extern bool gda_connection_close(IntPtr cnc);

		[DllImport("gda-2")]
		public static extern string gda_connection_get_server_version(IntPtr cnc);

		[DllImport("gda-2")]
		public static extern string gda_connection_get_database(IntPtr cnc);

		[DllImport("gda-2")]
		public static extern string gda_connection_get_dsn(IntPtr cnc);

		[DllImport("gda-2")]
		public static extern string gda_connection_get_cnc_string(IntPtr cnc);

		[DllImport("gda-2")]
		public static extern string gda_connection_get_provider(IntPtr cnc);

		[DllImport("gda-2")]
		public static extern string gda_connection_get_username(IntPtr cnc);

		[DllImport("gda-2")]
		public static extern string gda_connection_get_password(IntPtr cnc);

		[DllImport("gda-2")]
		public static extern bool gda_connection_change_database(IntPtr cnc, string name);

		[DllImport("gda-2")]
		public static extern IntPtr gda_transaction_new(string name);

		[DllImport("gda-2")]
		public static extern IntPtr gda_transaction_get_name(IntPtr xaction);

		[DllImport("gda-2")]
		public static extern IntPtr gda_transaction_set_name(IntPtr xaction, string name);

		[DllImport("gda-2")]
		public static extern GdaTransactionIsolation gda_transaction_get_isolation_level(IntPtr xaction);

		[DllImport("gda-2")]
		public static extern void gda_transaction_set_isolation_level(IntPtr xaction, GdaTransactionIsolation level);

		[DllImport("gda-2")]
		public static extern bool gda_connection_begin_transaction(IntPtr cnc, IntPtr xaction);

		[DllImport("gda-2")]
		public static extern bool gda_connection_commit_transaction(IntPtr cnc, IntPtr xaction);

		[DllImport("gda-2")]
		public static extern bool gda_connection_rollback_transaction(IntPtr cnc, IntPtr xaction);

		[DllImport("gda-2")]
		public static extern IntPtr gda_connection_execute_command(IntPtr cnc, IntPtr cmd, IntPtr parameterList);

		[DllImport("gda-2")]
		public static extern int gda_connection_execute_non_query(IntPtr cnc, IntPtr command, IntPtr parameterList);

		[DllImport("gda-2")]
		public static extern IntPtr gda_connection_execute_single_command(IntPtr cnc, IntPtr command, IntPtr parameterList);

		[DllImport("gda-2")]
		public static extern IntPtr gda_connection_get_errors(IntPtr cnc);

		[DllImport("gda-2")]
		public static extern IntPtr gda_command_new(string text, GdaCommandType type, GdaCommandOptions options);

		[DllImport("gda-2")]
		public static extern void gda_command_set_text(IntPtr cmd, string text);

		[DllImport("gda-2")]
		public static extern void gda_command_set_command_type(IntPtr cmd, GdaCommandType type);

		[DllImport("gda-2")]
		public static extern string gda_error_get_description(IntPtr error);

		[DllImport("gda-2")]
		public static extern long gda_error_get_number(IntPtr error);

		[DllImport("gda-2")]
		public static extern string gda_error_get_source(IntPtr error);

		[DllImport("gda-2")]
		public static extern string gda_error_get_sqlstate(IntPtr error);

		private static IntPtr gdaClient = IntPtr.Zero;
	}
}
