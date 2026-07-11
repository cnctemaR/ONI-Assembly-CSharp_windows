using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	internal abstract class Win32NamedPipe : IPipe
	{
		public string Name
		{
			get
			{
				if (this.name_cache != null)
				{
					return this.name_cache;
				}
				byte[] array = new byte[200];
				int num;
				int num2;
				int num3;
				int num4;
				while (Win32Marshal.GetNamedPipeHandleState(this.Handle, out num, out num2, out num3, out num4, array, array.Length))
				{
					if (array[array.Length - 1] == 0)
					{
						this.name_cache = Encoding.Default.GetString(array);
						return this.name_cache;
					}
					array = new byte[array.Length * 10];
				}
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32Exception(lastWin32Error);
			}
		}

		public abstract SafePipeHandle Handle { get; }

		public void WaitForPipeDrain()
		{
			throw new NotImplementedException();
		}

		private string name_cache;
	}
}
