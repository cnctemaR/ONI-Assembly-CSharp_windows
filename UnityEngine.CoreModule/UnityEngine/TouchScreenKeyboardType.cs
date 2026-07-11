using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Enumeration of the different types of supported touchscreen keyboards.</para>
	/// </summary>
	public enum TouchScreenKeyboardType
	{
		/// <summary>
		///   <para>The default keyboard layout of the target platform.</para>
		/// </summary>
		Default,
		/// <summary>
		///   <para>Keyboard with standard ASCII keys.</para>
		/// </summary>
		ASCIICapable,
		/// <summary>
		///   <para>Keyboard with numbers and punctuation mark keys.</para>
		/// </summary>
		NumbersAndPunctuation,
		/// <summary>
		///   <para>Keyboard with keys for URL entry.</para>
		/// </summary>
		URL,
		/// <summary>
		///   <para>Keyboard with standard numeric keys.</para>
		/// </summary>
		NumberPad,
		/// <summary>
		///   <para>Keyboard with a layout suitable for typing telephone numbers.</para>
		/// </summary>
		PhonePad,
		/// <summary>
		///   <para>Keyboard with alphanumeric keys.</para>
		/// </summary>
		NamePhonePad,
		/// <summary>
		///   <para>Keyboard with additional keys suitable for typing email addresses.</para>
		/// </summary>
		EmailAddress,
		/// <summary>
		///   <para>Keyboard for the Nintendo Network (Deprecated).</para>
		/// </summary>
		[Obsolete("Wii U is no longer supported as of Unity 2018.1.")]
		NintendoNetworkAccount,
		/// <summary>
		///   <para>Keyboard with symbol keys often used on social media, such as Twitter.</para>
		/// </summary>
		Social,
		/// <summary>
		///   <para>Keyboard with the "." key beside the space key, suitable for typing search terms.</para>
		/// </summary>
		Search
	}
}
