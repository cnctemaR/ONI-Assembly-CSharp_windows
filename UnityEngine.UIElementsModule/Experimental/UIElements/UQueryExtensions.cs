using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>UQuery is a set of extension methods allowing you to select individual or collection of visualElements inside a complex hierarchy.</para>
	/// </summary>
	public static class UQueryExtensions
	{
		public static T Q<T>(this VisualElement e, string name = null, params string[] classes) where T : VisualElement
		{
			return e.Query<T>(name, classes).Build().First();
		}

		public static T Q<T>(this VisualElement e, string name = null, string className = null) where T : VisualElement
		{
			return e.Query<T>(name, className).Build().First();
		}

		/// <summary>
		///   <para>Convenience overload, shorthand for Query&lt;T&gt;.Build().First().</para>
		/// </summary>
		/// <param name="e">Root VisualElement on which the selector will be applied.</param>
		/// <param name="name">If specified, will select elements with this name.</param>
		/// <param name="classes">If specified, will select elements with the given class (not to be confused with Type).</param>
		/// <param name="className">If specified, will select elements with the given class (not to be confused with Type).</param>
		/// <returns>
		///   <para>The first element matching all the criteria, or null if none was found.</para>
		/// </returns>
		public static VisualElement Q(this VisualElement e, string name = null, params string[] classes)
		{
			return e.Query<VisualElement>(name, classes).Build().First();
		}

		/// <summary>
		///   <para>Convenience overload, shorthand for Query&lt;T&gt;.Build().First().</para>
		/// </summary>
		/// <param name="e">Root VisualElement on which the selector will be applied.</param>
		/// <param name="name">If specified, will select elements with this name.</param>
		/// <param name="classes">If specified, will select elements with the given class (not to be confused with Type).</param>
		/// <param name="className">If specified, will select elements with the given class (not to be confused with Type).</param>
		/// <returns>
		///   <para>The first element matching all the criteria, or null if none was found.</para>
		/// </returns>
		public static VisualElement Q(this VisualElement e, string name = null, string className = null)
		{
			return e.Query<VisualElement>(name, className).Build().First();
		}

		/// <summary>
		///   <para>Initializes a QueryBuilder with the specified selection rules.</para>
		/// </summary>
		/// <param name="e">Root VisualElement on which the selector will be applied.</param>
		/// <param name="name">If specified, will select elements with this name.</param>
		/// <param name="className">If specified, will select elements with the given class (not to be confused with Type).</param>
		/// <param name="classes">If specified, will select elements with the given class (not to be confused with Type).</param>
		/// <returns>
		///   <para>QueryBuilder configured with the associated selection rules.</para>
		/// </returns>
		public static UQuery.QueryBuilder<VisualElement> Query(this VisualElement e, string name = null, params string[] classes)
		{
			return e.Query<VisualElement>(name, classes);
		}

		/// <summary>
		///   <para>Initializes a QueryBuilder with the specified selection rules. Template parameter specifies the type of elements the selector applies to (ie: Label, Button, etc).</para>
		/// </summary>
		/// <param name="e">Root VisualElement on which the selector will be applied.</param>
		/// <param name="name">If specified, will select elements with this name.</param>
		/// <param name="classes">If specified, will select elements with the given class (not to be confused with Type).</param>
		/// <param name="className">If specified, will select elements with the given class (not to be confused with Type).</param>
		/// <returns>
		///   <para>QueryBuilder configured with the associated selection rules.</para>
		/// </returns>
		public static UQuery.QueryBuilder<VisualElement> Query(this VisualElement e, string name = null, string className = null)
		{
			return e.Query<VisualElement>(name, className);
		}

		public static UQuery.QueryBuilder<T> Query<T>(this VisualElement e, string name = null, params string[] classes) where T : VisualElement
		{
			UQuery.QueryBuilder<VisualElement> queryBuilder = new UQuery.QueryBuilder<VisualElement>(e);
			return queryBuilder.OfType<T>(name, classes);
		}

		public static UQuery.QueryBuilder<T> Query<T>(this VisualElement e, string name = null, string className = null) where T : VisualElement
		{
			UQuery.QueryBuilder<VisualElement> queryBuilder = new UQuery.QueryBuilder<VisualElement>(e);
			return queryBuilder.OfType<T>(name, className);
		}

		/// <summary>
		///   <para>Initializes an empty QueryBuilder on a specified root element.</para>
		/// </summary>
		/// <param name="e">Root VisualElement on which the selector will be applied.</param>
		/// <returns>
		///   <para>An empty QueryBuilder on a specified root element.</para>
		/// </returns>
		public static UQuery.QueryBuilder<VisualElement> Query(this VisualElement e)
		{
			return new UQuery.QueryBuilder<VisualElement>(e);
		}
	}
}
