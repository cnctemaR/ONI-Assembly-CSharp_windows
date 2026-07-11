using System;

namespace System.Data.SqlClient
{
	internal sealed class SQLMessage
	{
		private SQLMessage()
		{
		}

		internal static string CultureIdError()
		{
			return SR.GetString("The Collation specified by SQL Server is not supported.");
		}

		internal static string EncryptionNotSupportedByClient()
		{
			return SR.GetString("The instance of SQL Server you attempted to connect to requires encryption but this machine does not support it.");
		}

		internal static string EncryptionNotSupportedByServer()
		{
			return SR.GetString("The instance of SQL Server you attempted to connect to does not support encryption.");
		}

		internal static string OperationCancelled()
		{
			return SR.GetString("Operation cancelled by user.");
		}

		internal static string SevereError()
		{
			return SR.GetString("A severe error occurred on the current command.  The results, if any, should be discarded.");
		}

		internal static string SSPIInitializeError()
		{
			return SR.GetString("Cannot initialize SSPI package.");
		}

		internal static string SSPIGenerateError()
		{
			return SR.GetString("Failed to generate SSPI context.");
		}

		internal static string SqlServerBrowserNotAccessible()
		{
			return SR.GetString("Cannot connect to SQL Server Browser. Ensure SQL Server Browser has been started.");
		}

		internal static string KerberosTicketMissingError()
		{
			return SR.GetString("Cannot access Kerberos ticket. Ensure Kerberos has been initialized with 'kinit'.");
		}

		internal static string Timeout()
		{
			return SR.GetString("Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.");
		}

		internal static string Timeout_PreLogin_Begin()
		{
			return SR.GetString("Connection Timeout Expired.  The timeout period elapsed at the start of the pre-login phase.  This could be because of insufficient time provided for connection timeout.");
		}

		internal static string Timeout_PreLogin_InitializeConnection()
		{
			return SR.GetString("Connection Timeout Expired.  The timeout period elapsed while attempting to create and initialize a socket to the server.  This could be either because the server was unreachable or unable to respond back in time.");
		}

		internal static string Timeout_PreLogin_SendHandshake()
		{
			return SR.GetString("Connection Timeout Expired.  The timeout period elapsed while making a pre-login handshake request.  This could be because the server was unable to respond back in time.");
		}

		internal static string Timeout_PreLogin_ConsumeHandshake()
		{
			return SR.GetString("Connection Timeout Expired.  The timeout period elapsed while attempting to consume the pre-login handshake acknowledgement.  This could be because the pre-login handshake failed or the server was unable to respond back in time.");
		}

		internal static string Timeout_Login_Begin()
		{
			return SR.GetString("Connection Timeout Expired.  The timeout period elapsed at the start of the login phase.  This could be because of insufficient time provided for connection timeout.");
		}

		internal static string Timeout_Login_ProcessConnectionAuth()
		{
			return SR.GetString("Connection Timeout Expired.  The timeout period elapsed while attempting to authenticate the login.  This could be because the server failed to authenticate the user or the server was unable to respond back in time.");
		}

		internal static string Timeout_PostLogin()
		{
			return SR.GetString("Connection Timeout Expired.  The timeout period elapsed during the post-login phase.  The connection could have timed out while waiting for server to complete the login process and respond; Or it could have timed out while attempting to create multiple active connections.");
		}

		internal static string Timeout_FailoverInfo()
		{
			return SR.GetString("This failure occurred while attempting to connect to the {0} server.");
		}

		internal static string Timeout_RoutingDestination()
		{
			return SR.GetString("This failure occurred while attempting to connect to the routing destination. The duration spent while attempting to connect to the original server was - [Pre-Login] initialization={0}; handshake={1}; [Login] initialization={2}; authentication={3}; [Post-Login] complete={4};  ");
		}

		internal static string Duration_PreLogin_Begin(long PreLoginBeginDuration)
		{
			return SR.GetString("The duration spent while attempting to connect to this server was - [Pre-Login] initialization={0};", new object[] { PreLoginBeginDuration });
		}

		internal static string Duration_PreLoginHandshake(long PreLoginBeginDuration, long PreLoginHandshakeDuration)
		{
			return SR.GetString("The duration spent while attempting to connect to this server was - [Pre-Login] initialization={0}; handshake={1}; ", new object[] { PreLoginBeginDuration, PreLoginHandshakeDuration });
		}

		internal static string Duration_Login_Begin(long PreLoginBeginDuration, long PreLoginHandshakeDuration, long LoginBeginDuration)
		{
			return SR.GetString("The duration spent while attempting to connect to this server was - [Pre-Login] initialization={0}; handshake={1}; [Login] initialization={2}; ", new object[] { PreLoginBeginDuration, PreLoginHandshakeDuration, LoginBeginDuration });
		}

		internal static string Duration_Login_ProcessConnectionAuth(long PreLoginBeginDuration, long PreLoginHandshakeDuration, long LoginBeginDuration, long LoginAuthDuration)
		{
			return SR.GetString("The duration spent while attempting to connect to this server was - [Pre-Login] initialization={0}; handshake={1}; [Login] initialization={2}; authentication={3}; ", new object[] { PreLoginBeginDuration, PreLoginHandshakeDuration, LoginBeginDuration, LoginAuthDuration });
		}

		internal static string Duration_PostLogin(long PreLoginBeginDuration, long PreLoginHandshakeDuration, long LoginBeginDuration, long LoginAuthDuration, long PostLoginDuration)
		{
			return SR.GetString("The duration spent while attempting to connect to this server was - [Pre-Login] initialization={0}; handshake={1}; [Login] initialization={2}; authentication={3}; [Post-Login] complete={4}; ", new object[] { PreLoginBeginDuration, PreLoginHandshakeDuration, LoginBeginDuration, LoginAuthDuration, PostLoginDuration });
		}

		internal static string UserInstanceFailure()
		{
			return SR.GetString("A user instance was requested in the connection string but the server specified does not support this option.");
		}

		internal static string PreloginError()
		{
			return SR.GetString("A connection was successfully established with the server, but then an error occurred during the pre-login handshake.");
		}

		internal static string ExClientConnectionId()
		{
			return SR.GetString("ClientConnectionId:{0}");
		}

		internal static string ExErrorNumberStateClass()
		{
			return SR.GetString("Error Number:{0},State:{1},Class:{2}");
		}

		internal static string ExOriginalClientConnectionId()
		{
			return SR.GetString("ClientConnectionId before routing:{0}");
		}

		internal static string ExRoutingDestination()
		{
			return SR.GetString("Routing Destination:{0}");
		}
	}
}
