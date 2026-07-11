using System;

namespace System.Runtime.Remoting
{
	[Serializable]
	internal class TypeInfo : IRemotingTypeInfo
	{
		public TypeInfo(Type type)
		{
			if (type.IsInterface)
			{
				this.serverType = typeof(MarshalByRefObject).AssemblyQualifiedName;
				this.serverHierarchy = new string[0];
				this.interfacesImplemented = new string[] { type.AssemblyQualifiedName };
				return;
			}
			this.serverType = type.AssemblyQualifiedName;
			int num = 0;
			Type type2 = type.BaseType;
			while (type2 != typeof(MarshalByRefObject) && type2 != null)
			{
				type2 = type2.BaseType;
				num++;
			}
			this.serverHierarchy = new string[num];
			type2 = type.BaseType;
			for (int i = 0; i < num; i++)
			{
				this.serverHierarchy[i] = type2.AssemblyQualifiedName;
				type2 = type2.BaseType;
			}
			Type[] interfaces = type.GetInterfaces();
			this.interfacesImplemented = new string[interfaces.Length];
			for (int j = 0; j < interfaces.Length; j++)
			{
				this.interfacesImplemented[j] = interfaces[j].AssemblyQualifiedName;
			}
		}

		public string TypeName
		{
			get
			{
				return this.serverType;
			}
			set
			{
				this.serverType = value;
			}
		}

		public bool CanCastTo(Type fromType, object o)
		{
			if (fromType == typeof(object))
			{
				return true;
			}
			if (fromType == typeof(MarshalByRefObject))
			{
				return true;
			}
			string text = fromType.AssemblyQualifiedName;
			int num = text.IndexOf(',');
			if (num != -1)
			{
				num = text.IndexOf(',', num + 1);
			}
			if (num != -1)
			{
				text = text.Substring(0, num + 1);
			}
			else
			{
				text += ",";
			}
			if ((this.serverType + ",").StartsWith(text))
			{
				return true;
			}
			if (this.serverHierarchy != null)
			{
				string[] array = this.serverHierarchy;
				for (int i = 0; i < array.Length; i++)
				{
					if ((array[i] + ",").StartsWith(text))
					{
						return true;
					}
				}
			}
			if (this.interfacesImplemented != null)
			{
				string[] array = this.interfacesImplemented;
				for (int i = 0; i < array.Length; i++)
				{
					if ((array[i] + ",").StartsWith(text))
					{
						return true;
					}
				}
			}
			return false;
		}

		private string serverType;

		private string[] serverHierarchy;

		private string[] interfacesImplemented;
	}
}
