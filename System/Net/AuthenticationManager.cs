using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Configuration;

namespace System.Net
{
	public class AuthenticationManager
	{
		private AuthenticationManager()
		{
		}

		private static void EnsureModules()
		{
			object obj = AuthenticationManager.locker;
			lock (obj)
			{
				if (AuthenticationManager.modules == null)
				{
					AuthenticationManager.modules = new ArrayList();
					object section = ConfigurationManager.GetSection("system.net/authenticationModules");
					global::System.Net.Configuration.AuthenticationModulesSection authenticationModulesSection = section as global::System.Net.Configuration.AuthenticationModulesSection;
					if (authenticationModulesSection != null)
					{
						foreach (object obj2 in authenticationModulesSection.AuthenticationModules)
						{
							global::System.Net.Configuration.AuthenticationModuleElement authenticationModuleElement = (global::System.Net.Configuration.AuthenticationModuleElement)obj2;
							IAuthenticationModule authenticationModule = null;
							try
							{
								Type type = Type.GetType(authenticationModuleElement.Type, true);
								authenticationModule = (IAuthenticationModule)Activator.CreateInstance(type);
							}
							catch
							{
							}
							AuthenticationManager.modules.Add(authenticationModule);
						}
					}
				}
			}
		}

		public static ICredentialPolicy CredentialPolicy
		{
			get
			{
				return AuthenticationManager.credential_policy;
			}
			set
			{
				AuthenticationManager.credential_policy = value;
			}
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		[global::System.MonoTODO]
		public static global::System.Collections.Specialized.StringDictionary CustomTargetNameDictionary
		{
			get
			{
				throw AuthenticationManager.GetMustImplement();
			}
		}

		public static IEnumerator RegisteredModules
		{
			get
			{
				AuthenticationManager.EnsureModules();
				return AuthenticationManager.modules.GetEnumerator();
			}
		}

		internal static void Clear()
		{
			AuthenticationManager.EnsureModules();
			ArrayList arrayList = AuthenticationManager.modules;
			lock (arrayList)
			{
				AuthenticationManager.modules.Clear();
			}
		}

		public static Authorization Authenticate(string challenge, WebRequest request, ICredentials credentials)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (credentials == null)
			{
				throw new ArgumentNullException("credentials");
			}
			if (challenge == null)
			{
				throw new ArgumentNullException("challenge");
			}
			return AuthenticationManager.DoAuthenticate(challenge, request, credentials);
		}

		private static Authorization DoAuthenticate(string challenge, WebRequest request, ICredentials credentials)
		{
			AuthenticationManager.EnsureModules();
			ArrayList arrayList = AuthenticationManager.modules;
			lock (arrayList)
			{
				foreach (object obj in AuthenticationManager.modules)
				{
					IAuthenticationModule authenticationModule = (IAuthenticationModule)obj;
					Authorization authorization = authenticationModule.Authenticate(challenge, request, credentials);
					if (authorization != null)
					{
						authorization.Module = authenticationModule;
						return authorization;
					}
				}
			}
			return null;
		}

		public static Authorization PreAuthenticate(WebRequest request, ICredentials credentials)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (credentials == null)
			{
				return null;
			}
			AuthenticationManager.EnsureModules();
			ArrayList arrayList = AuthenticationManager.modules;
			lock (arrayList)
			{
				foreach (object obj in AuthenticationManager.modules)
				{
					IAuthenticationModule authenticationModule = (IAuthenticationModule)obj;
					Authorization authorization = authenticationModule.PreAuthenticate(request, credentials);
					if (authorization != null)
					{
						authorization.Module = authenticationModule;
						return authorization;
					}
				}
			}
			return null;
		}

		public static void Register(IAuthenticationModule authenticationModule)
		{
			if (authenticationModule == null)
			{
				throw new ArgumentNullException("authenticationModule");
			}
			AuthenticationManager.DoUnregister(authenticationModule.AuthenticationType, false);
			ArrayList arrayList = AuthenticationManager.modules;
			lock (arrayList)
			{
				AuthenticationManager.modules.Add(authenticationModule);
			}
		}

		public static void Unregister(IAuthenticationModule authenticationModule)
		{
			if (authenticationModule == null)
			{
				throw new ArgumentNullException("authenticationModule");
			}
			AuthenticationManager.DoUnregister(authenticationModule.AuthenticationType, true);
		}

		public static void Unregister(string authenticationScheme)
		{
			if (authenticationScheme == null)
			{
				throw new ArgumentNullException("authenticationScheme");
			}
			AuthenticationManager.DoUnregister(authenticationScheme, true);
		}

		private static void DoUnregister(string authenticationScheme, bool throwEx)
		{
			AuthenticationManager.EnsureModules();
			ArrayList arrayList = AuthenticationManager.modules;
			lock (arrayList)
			{
				IAuthenticationModule authenticationModule = null;
				foreach (object obj in AuthenticationManager.modules)
				{
					IAuthenticationModule authenticationModule2 = (IAuthenticationModule)obj;
					string authenticationType = authenticationModule2.AuthenticationType;
					if (string.Compare(authenticationType, authenticationScheme, true) == 0)
					{
						authenticationModule = authenticationModule2;
						break;
					}
				}
				if (authenticationModule == null)
				{
					if (throwEx)
					{
						throw new InvalidOperationException("Scheme not registered.");
					}
				}
				else
				{
					AuthenticationManager.modules.Remove(authenticationModule);
				}
			}
		}

		private static ArrayList modules;

		private static object locker = new object();

		private static ICredentialPolicy credential_policy = null;
	}
}
