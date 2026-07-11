using System;

namespace Mono.Security.Interface
{
	public class ValidationResult
	{
		public ValidationResult(bool trusted, bool user_denied, int error_code, MonoSslPolicyErrors? policy_errors)
		{
			this.trusted = trusted;
			this.user_denied = user_denied;
			this.error_code = error_code;
			this.policy_errors = policy_errors;
		}

		internal ValidationResult(bool trusted, bool user_denied, int error_code)
		{
			this.trusted = trusted;
			this.user_denied = user_denied;
			this.error_code = error_code;
		}

		public bool Trusted
		{
			get
			{
				return this.trusted;
			}
		}

		public bool UserDenied
		{
			get
			{
				return this.user_denied;
			}
		}

		public int ErrorCode
		{
			get
			{
				return this.error_code;
			}
		}

		public MonoSslPolicyErrors? PolicyErrors
		{
			get
			{
				return this.policy_errors;
			}
		}

		private bool trusted;

		private bool user_denied;

		private int error_code;

		private MonoSslPolicyErrors? policy_errors;
	}
}
