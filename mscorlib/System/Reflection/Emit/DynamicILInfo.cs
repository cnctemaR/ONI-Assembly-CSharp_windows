using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	public class DynamicILInfo
	{
		internal DynamicILInfo()
		{
		}

		internal DynamicILInfo(DynamicMethod method)
		{
			this.method = method;
		}

		public DynamicMethod DynamicMethod
		{
			get
			{
				return this.method;
			}
		}

		[MonoTODO]
		public int GetTokenFor(byte[] signature)
		{
			throw new NotImplementedException();
		}

		public int GetTokenFor(DynamicMethod method)
		{
			return this.method.GetILGenerator().TokenGenerator.GetToken(method, false);
		}

		public int GetTokenFor(RuntimeFieldHandle field)
		{
			return this.method.GetILGenerator().TokenGenerator.GetToken(FieldInfo.GetFieldFromHandle(field), false);
		}

		public int GetTokenFor(RuntimeMethodHandle method)
		{
			MethodBase methodFromHandle = MethodBase.GetMethodFromHandle(method);
			return this.method.GetILGenerator().TokenGenerator.GetToken(methodFromHandle, false);
		}

		public int GetTokenFor(RuntimeTypeHandle type)
		{
			Type typeFromHandle = Type.GetTypeFromHandle(type);
			return this.method.GetILGenerator().TokenGenerator.GetToken(typeFromHandle, false);
		}

		public int GetTokenFor(string literal)
		{
			return this.method.GetILGenerator().TokenGenerator.GetToken(literal);
		}

		[MonoTODO]
		public int GetTokenFor(RuntimeMethodHandle method, RuntimeTypeHandle contextType)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public int GetTokenFor(RuntimeFieldHandle field, RuntimeTypeHandle contextType)
		{
			throw new NotImplementedException();
		}

		public void SetCode(byte[] code, int maxStackSize)
		{
			if (code == null)
			{
				throw new ArgumentNullException("code");
			}
			this.method.GetILGenerator().SetCode(code, maxStackSize);
		}

		[CLSCompliant(false)]
		public unsafe void SetCode(byte* code, int codeSize, int maxStackSize)
		{
			if (code == null)
			{
				throw new ArgumentNullException("code");
			}
			this.method.GetILGenerator().SetCode(code, codeSize, maxStackSize);
		}

		[MonoTODO]
		public void SetExceptions(byte[] exceptions)
		{
			throw new NotImplementedException();
		}

		[CLSCompliant(false)]
		[MonoTODO]
		public unsafe void SetExceptions(byte* exceptions, int exceptionsSize)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void SetLocalSignature(byte[] localSignature)
		{
			throw new NotImplementedException();
		}

		[CLSCompliant(false)]
		public unsafe void SetLocalSignature(byte* localSignature, int signatureSize)
		{
			byte[] array = new byte[signatureSize];
			for (int i = 0; i < signatureSize; i++)
			{
				array[i] = localSignature[i];
			}
		}

		private DynamicMethod method;
	}
}
