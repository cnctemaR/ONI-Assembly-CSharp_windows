using System;
using System.Text;

namespace FileHelpers
{
	internal static class DataBaseHelper
	{
		public static string GetAccessConnection(string db)
		{
			return DataBaseHelper.GetAccessConnection(db, "");
		}

		public static string GetAccessConnection(string db, string password)
		{
			string text = "Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Registry Path=;Jet OLEDB:Database Locking Mode=1;Jet OLEDB:Database Password=<PASSWORD>;Data Source=\"<BASE>\";Password=;Jet OLEDB:Engine Type=5;Jet OLEDB:Global Bulk Transactions=1;Provider=\"Microsoft.Jet.OLEDB.4.0\";Jet OLEDB:System database=;Jet OLEDB:SFP=False;Extended Properties=;Mode=Share Deny None;Jet OLEDB:New Database Password=;Jet OLEDB:Create System Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;User ID=Admin;Jet OLEDB:Encrypt Database=False";
			text = text.Replace("<BASE>", db);
			return text.Replace("<PASSWORD>", password ?? string.Empty);
		}

		public static string SqlConnectionString(string server, string dbName)
		{
			return DataBaseHelper.SqlConnectionString(server, dbName, "", "");
		}

		public static string SqlConnectionString(string server, string dbName, string user, string pass)
		{
			StringBuilder stringBuilder = new StringBuilder(300);
			if (user.Length == 0 && pass.Length == 0)
			{
				stringBuilder = new StringBuilder("data source=<SERVER>;persist security info=True;initial catalog=<BASE>;integrated security=SSPI;packet size=4096;Connection Timeout=10;Application Name=" + DataBaseHelper.AppName);
			}
			else
			{
				stringBuilder = new StringBuilder("data source=<SERVER>;persist security info=True;initial catalog=<BASE>;User Id=\"<USER>\";Password=\"<PASS>\";packet size=4096;Connection Timeout=10;Application Name=" + DataBaseHelper.AppName);
				stringBuilder.Replace("<USER>", user);
				stringBuilder.Replace("<PASS>", pass);
			}
			stringBuilder.Replace("<SERVER>", server);
			stringBuilder.Replace("<BASE>", dbName);
			return stringBuilder.ToString();
		}

		private const string AccessConnStr = "Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Registry Path=;Jet OLEDB:Database Locking Mode=1;Jet OLEDB:Database Password=<PASSWORD>;Data Source=\"<BASE>\";Password=;Jet OLEDB:Engine Type=5;Jet OLEDB:Global Bulk Transactions=1;Provider=\"Microsoft.Jet.OLEDB.4.0\";Jet OLEDB:System database=;Jet OLEDB:SFP=False;Extended Properties=;Mode=Share Deny None;Jet OLEDB:New Database Password=;Jet OLEDB:Create System Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;User ID=Admin;Jet OLEDB:Encrypt Database=False";

		private static string AppName = "FileHelpers";
	}
}
