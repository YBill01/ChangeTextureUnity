using System;
using Unity.Properties;

namespace YB.Helpers
{
	/// <summary>
	/// public readonly BindableProperty<string> $SomeProperty;
	/// ...
	/// $SomeProperty = BindableProperty<string>.Bind(() => model.$SomeProperty.ToString());
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BindableProperty<T>
	{
		private readonly Func<T> _getter;

		public BindableProperty(Func<T> getter)
		{
			_getter = getter;
		}

		[CreateProperty]
		public T Value => _getter();

		public static BindableProperty<T> Bind(Func<T> getter) => new BindableProperty<T>(getter);
	}
}