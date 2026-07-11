using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	internal struct CachedContactPoints2D
	{
		public unsafe ContactPoint2D this[int i]
		{
			get
			{
				fixed (ContactPoint2D* ptr = &this.m_Contact0)
				{
					return ptr[(IntPtr)i * (IntPtr)sizeof(ContactPoint2D)];
				}
			}
		}

		private ContactPoint2D m_Contact0;

		private ContactPoint2D m_Contact1;

		private ContactPoint2D m_Contact2;

		private ContactPoint2D m_Contact3;

		private ContactPoint2D m_Contact4;

		private ContactPoint2D m_Contact5;

		private ContactPoint2D m_Contact6;

		private ContactPoint2D m_Contact7;

		private ContactPoint2D m_Contact8;

		private ContactPoint2D m_Contact9;

		private ContactPoint2D m_Contact10;

		private ContactPoint2D m_Contact11;

		private ContactPoint2D m_Contact12;

		private ContactPoint2D m_Contact13;

		private ContactPoint2D m_Contact14;

		private ContactPoint2D m_Contact15;

		private ContactPoint2D m_Contact16;

		private ContactPoint2D m_Contact17;

		private ContactPoint2D m_Contact18;

		private ContactPoint2D m_Contact19;

		private ContactPoint2D m_Contact20;

		private ContactPoint2D m_Contact21;

		private ContactPoint2D m_Contact22;

		private ContactPoint2D m_Contact23;

		private ContactPoint2D m_Contact24;

		private ContactPoint2D m_Contact25;

		private ContactPoint2D m_Contact26;

		private ContactPoint2D m_Contact27;

		private ContactPoint2D m_Contact28;

		private ContactPoint2D m_Contact29;

		private ContactPoint2D m_Contact30;

		private ContactPoint2D m_Contact31;

		private ContactPoint2D m_Contact32;

		private ContactPoint2D m_Contact33;

		private ContactPoint2D m_Contact34;

		private ContactPoint2D m_Contact35;

		private ContactPoint2D m_Contact36;

		private ContactPoint2D m_Contact37;

		private ContactPoint2D m_Contact38;

		private ContactPoint2D m_Contact39;

		private ContactPoint2D m_Contact40;

		private ContactPoint2D m_Contact41;

		private ContactPoint2D m_Contact42;

		private ContactPoint2D m_Contact43;

		private ContactPoint2D m_Contact44;

		private ContactPoint2D m_Contact45;

		private ContactPoint2D m_Contact46;

		private ContactPoint2D m_Contact47;

		private ContactPoint2D m_Contact48;

		private ContactPoint2D m_Contact49;

		private ContactPoint2D m_Contact50;

		private ContactPoint2D m_Contact51;

		private ContactPoint2D m_Contact52;

		private ContactPoint2D m_Contact53;

		private ContactPoint2D m_Contact54;

		private ContactPoint2D m_Contact55;

		private ContactPoint2D m_Contact56;

		private ContactPoint2D m_Contact57;

		private ContactPoint2D m_Contact58;

		private ContactPoint2D m_Contact59;

		private ContactPoint2D m_Contact60;

		private ContactPoint2D m_Contact61;

		private ContactPoint2D m_Contact62;

		private ContactPoint2D m_Contact63;
	}
}
