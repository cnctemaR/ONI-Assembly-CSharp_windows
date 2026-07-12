using System;

namespace System.Xml
{
	internal abstract class ArrayHelper<TArgument, TArray>
	{
		public TArray[] ReadArray(XmlDictionaryReader reader, TArgument localName, TArgument namespaceUri, int maxArrayLength)
		{
			TArray[][] array = null;
			int num = 0;
			int num2 = 0;
			int num3;
			if (reader.TryGetArrayLength(out num3))
			{
				if (num3 > maxArrayLength)
				{
					XmlExceptionHelper.ThrowMaxArrayLengthOrMaxItemsQuotaExceeded(reader, maxArrayLength);
				}
				if (num3 > 65535)
				{
					num3 = 65535;
				}
			}
			else
			{
				num3 = 32;
			}
			TArray[] array2;
			for (;;)
			{
				array2 = new TArray[num3];
				int i;
				int num4;
				for (i = 0; i < array2.Length; i += num4)
				{
					num4 = this.ReadArray(reader, localName, namespaceUri, array2, i, array2.Length - i);
					if (num4 == 0)
					{
						break;
					}
				}
				if (num2 > maxArrayLength - i)
				{
					XmlExceptionHelper.ThrowMaxArrayLengthOrMaxItemsQuotaExceeded(reader, maxArrayLength);
				}
				num2 += i;
				if (i < array2.Length || reader.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				if (array == null)
				{
					array = new TArray[32][];
				}
				array[num++] = array2;
				num3 *= 2;
			}
			if (num2 != array2.Length || num > 0)
			{
				TArray[] array3 = new TArray[num2];
				int num5 = 0;
				for (int j = 0; j < num; j++)
				{
					Array.Copy(array[j], 0, array3, num5, array[j].Length);
					num5 += array[j].Length;
				}
				Array.Copy(array2, 0, array3, num5, num2 - num5);
				array2 = array3;
			}
			return array2;
		}

		public void WriteArray(XmlDictionaryWriter writer, string prefix, TArgument localName, TArgument namespaceUri, XmlDictionaryReader reader)
		{
			int num;
			if (reader.TryGetArrayLength(out num))
			{
				num = Math.Min(num, 256);
			}
			else
			{
				num = 256;
			}
			TArray[] array = new TArray[num];
			for (;;)
			{
				int num2 = this.ReadArray(reader, localName, namespaceUri, array, 0, array.Length);
				if (num2 == 0)
				{
					break;
				}
				this.WriteArray(writer, prefix, localName, namespaceUri, array, 0, num2);
			}
		}

		protected abstract int ReadArray(XmlDictionaryReader reader, TArgument localName, TArgument namespaceUri, TArray[] array, int offset, int count);

		protected abstract void WriteArray(XmlDictionaryWriter writer, string prefix, TArgument localName, TArgument namespaceUri, TArray[] array, int offset, int count);
	}
}
