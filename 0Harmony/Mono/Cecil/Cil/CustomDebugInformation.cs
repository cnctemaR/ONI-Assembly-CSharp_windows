using System;

namespace Mono.Cecil.Cil
{
	internal abstract class CustomDebugInformation : DebugInformation
	{
		public Guid Identifier
		{
			get
			{
				return this.identifier;
			}
		}

		public abstract CustomDebugInformationKind Kind { get; }

		internal CustomDebugInformation(Guid identifier)
		{
			this.identifier = identifier;
			this.token = new MetadataToken(TokenType.CustomDebugInformation);
		}

		private Guid identifier;
	}
}
