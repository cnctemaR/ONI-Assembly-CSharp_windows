using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public abstract class MulticastDelegate : Delegate
	{
		protected MulticastDelegate(object target, string method)
			: base(target, method)
		{
			this.prev = null;
		}

		protected MulticastDelegate(Type target, string method)
			: base(target, method)
		{
			this.prev = null;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		protected sealed override object DynamicInvokeImpl(object[] args)
		{
			if (this.prev != null)
			{
				this.prev.DynamicInvokeImpl(args);
			}
			return base.DynamicInvokeImpl(args);
		}

		public sealed override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			MulticastDelegate multicastDelegate = obj as MulticastDelegate;
			if (multicastDelegate == null)
			{
				return false;
			}
			if (this.prev == null)
			{
				return multicastDelegate.prev == null;
			}
			return this.prev.Equals(multicastDelegate.prev);
		}

		public sealed override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public sealed override Delegate[] GetInvocationList()
		{
			MulticastDelegate multicastDelegate = (MulticastDelegate)this.Clone();
			multicastDelegate.kpm_next = null;
			while (multicastDelegate.prev != null)
			{
				multicastDelegate.prev.kpm_next = multicastDelegate;
				multicastDelegate = multicastDelegate.prev;
			}
			if (multicastDelegate.kpm_next == null)
			{
				MulticastDelegate multicastDelegate2 = (MulticastDelegate)multicastDelegate.Clone();
				multicastDelegate2.prev = null;
				multicastDelegate2.kpm_next = null;
				return new Delegate[] { multicastDelegate2 };
			}
			ArrayList arrayList = new ArrayList();
			while (multicastDelegate != null)
			{
				MulticastDelegate multicastDelegate3 = (MulticastDelegate)multicastDelegate.Clone();
				multicastDelegate3.prev = null;
				multicastDelegate3.kpm_next = null;
				arrayList.Add(multicastDelegate3);
				multicastDelegate = multicastDelegate.kpm_next;
			}
			return (Delegate[])arrayList.ToArray(typeof(Delegate));
		}

		protected sealed override Delegate CombineImpl(Delegate follow)
		{
			if (base.GetType() != follow.GetType())
			{
				throw new ArgumentException(Locale.GetText("Incompatible Delegate Types."));
			}
			MulticastDelegate multicastDelegate = (MulticastDelegate)follow.Clone();
			multicastDelegate.SetMulticastInvoke();
			MulticastDelegate multicastDelegate2 = multicastDelegate;
			for (MulticastDelegate multicastDelegate3 = ((MulticastDelegate)follow).prev; multicastDelegate3 != null; multicastDelegate3 = multicastDelegate3.prev)
			{
				multicastDelegate2.prev = (MulticastDelegate)multicastDelegate3.Clone();
				multicastDelegate2 = multicastDelegate2.prev;
			}
			multicastDelegate2.prev = (MulticastDelegate)this.Clone();
			multicastDelegate2 = multicastDelegate2.prev;
			for (MulticastDelegate multicastDelegate3 = this.prev; multicastDelegate3 != null; multicastDelegate3 = multicastDelegate3.prev)
			{
				multicastDelegate2.prev = (MulticastDelegate)multicastDelegate3.Clone();
				multicastDelegate2 = multicastDelegate2.prev;
			}
			return multicastDelegate;
		}

		private bool BaseEquals(MulticastDelegate value)
		{
			return base.Equals(value);
		}

		private static MulticastDelegate KPM(MulticastDelegate needle, MulticastDelegate haystack, out MulticastDelegate tail)
		{
			MulticastDelegate multicastDelegate = needle;
			MulticastDelegate multicastDelegate2 = (needle.kpm_next = null);
			for (;;)
			{
				while (multicastDelegate2 != null && !multicastDelegate2.BaseEquals(multicastDelegate))
				{
					multicastDelegate2 = multicastDelegate2.kpm_next;
				}
				multicastDelegate = multicastDelegate.prev;
				if (multicastDelegate == null)
				{
					break;
				}
				multicastDelegate2 = ((multicastDelegate2 != null) ? multicastDelegate2.prev : needle);
				if (multicastDelegate.BaseEquals(multicastDelegate2))
				{
					multicastDelegate.kpm_next = multicastDelegate2.kpm_next;
				}
				else
				{
					multicastDelegate.kpm_next = multicastDelegate2;
				}
			}
			MulticastDelegate multicastDelegate3 = haystack;
			multicastDelegate2 = needle;
			multicastDelegate = haystack;
			for (;;)
			{
				while (multicastDelegate2 != null && !multicastDelegate2.BaseEquals(multicastDelegate))
				{
					multicastDelegate2 = multicastDelegate2.kpm_next;
					multicastDelegate3 = multicastDelegate3.prev;
				}
				multicastDelegate2 = ((multicastDelegate2 != null) ? multicastDelegate2.prev : needle);
				if (multicastDelegate2 == null)
				{
					break;
				}
				multicastDelegate = multicastDelegate.prev;
				if (multicastDelegate == null)
				{
					goto Block_8;
				}
			}
			tail = multicastDelegate.prev;
			return multicastDelegate3;
			Block_8:
			tail = null;
			return null;
		}

		protected sealed override Delegate RemoveImpl(Delegate value)
		{
			if (value == null)
			{
				return this;
			}
			MulticastDelegate multicastDelegate2;
			MulticastDelegate multicastDelegate = MulticastDelegate.KPM((MulticastDelegate)value, this, out multicastDelegate2);
			if (multicastDelegate == null)
			{
				return this;
			}
			MulticastDelegate multicastDelegate3 = null;
			MulticastDelegate multicastDelegate4 = null;
			for (MulticastDelegate multicastDelegate5 = this; multicastDelegate5 != multicastDelegate; multicastDelegate5 = multicastDelegate5.prev)
			{
				MulticastDelegate multicastDelegate6 = (MulticastDelegate)multicastDelegate5.Clone();
				if (multicastDelegate3 != null)
				{
					multicastDelegate3.prev = multicastDelegate6;
				}
				else
				{
					multicastDelegate4 = multicastDelegate6;
				}
				multicastDelegate3 = multicastDelegate6;
			}
			for (MulticastDelegate multicastDelegate5 = multicastDelegate2; multicastDelegate5 != null; multicastDelegate5 = multicastDelegate5.prev)
			{
				MulticastDelegate multicastDelegate7 = (MulticastDelegate)multicastDelegate5.Clone();
				if (multicastDelegate3 != null)
				{
					multicastDelegate3.prev = multicastDelegate7;
				}
				else
				{
					multicastDelegate4 = multicastDelegate7;
				}
				multicastDelegate3 = multicastDelegate7;
			}
			if (multicastDelegate3 != null)
			{
				multicastDelegate3.prev = null;
			}
			return multicastDelegate4;
		}

		public static bool operator ==(MulticastDelegate d1, MulticastDelegate d2)
		{
			if (d1 == null)
			{
				return d2 == null;
			}
			return d1.Equals(d2);
		}

		public static bool operator !=(MulticastDelegate d1, MulticastDelegate d2)
		{
			if (d1 == null)
			{
				return d2 != null;
			}
			return !d1.Equals(d2);
		}

		private MulticastDelegate prev;

		private MulticastDelegate kpm_next;
	}
}
