using System;

namespace Unity.Properties
{
	[Serializable]
	public class InvalidContainerTypeException : Exception
	{
		public Type Type { get; }

		public InvalidContainerTypeException(Type type)
			: base(InvalidContainerTypeException.GetMessageForType(type))
		{
			this.Type = type;
		}

		public InvalidContainerTypeException(Type type, Exception inner)
			: base(InvalidContainerTypeException.GetMessageForType(type), inner)
		{
			this.Type = type;
		}

		private static string GetMessageForType(Type type)
		{
			return string.Concat(new string[] { "Invalid container Type=[", type.Name, ".", type.Name, "]" });
		}
	}
}
