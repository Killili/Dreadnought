using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dreadnought.Common {
	public static class Extensions {
		public static void Each<T>(this IEnumerable<T> source, Action<T> action) {
			foreach(T item in source)
				action(item);
		}

		public static void Map<TKey,TValue>(this IDictionary<TKey,TValue> source, Func<TKey,TValue,TValue> function) {
			foreach(KeyValuePair<TKey,TValue> item in source) {
				source[item.Key] = function(item.Key,item.Value);
			}
		}
		public static void Each<TKey, TValue>(this IDictionary<TKey, TValue> source, Action<TKey, TValue> action) {
			foreach(KeyValuePair<TKey, TValue> item in source) {
				action(item.Key, item.Value);
			}
		}
	}
}
