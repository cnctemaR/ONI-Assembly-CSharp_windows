using System;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class StringValidatorAttribute : ConfigurationValidatorAttribute
	{
		public string InvalidCharacters
		{
			get
			{
				return this.invalidCharacters;
			}
			set
			{
				this.invalidCharacters = value;
				this.instance = null;
			}
		}

		public int MaxLength
		{
			get
			{
				return this.maxLength;
			}
			set
			{
				this.maxLength = value;
				this.instance = null;
			}
		}

		public int MinLength
		{
			get
			{
				return this.minLength;
			}
			set
			{
				this.minLength = value;
				this.instance = null;
			}
		}

		public override ConfigurationValidatorBase ValidatorInstance
		{
			get
			{
				if (this.instance == null)
				{
					this.instance = new StringValidator(this.minLength, this.maxLength, this.invalidCharacters);
				}
				return this.instance;
			}
		}

		private string invalidCharacters;

		private int maxLength = int.MaxValue;

		private int minLength;

		private ConfigurationValidatorBase instance;
	}
}
