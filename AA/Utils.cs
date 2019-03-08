using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AA
{
	public static class Utils
	{
		public static T[] Add<T>(this IEnumerable<T> collection, T newItem)
		{
			var list = new List<T>(collection.Count() + 1);
			list.AddRange(collection);
			list.Add(newItem);
			return list.ToArray();
		}

		public static T[] Remove<T>(this IEnumerable<T> collection, T item)
		{
			var list = collection.ToList();
			list.Remove(item);
			return list.ToArray();
		}

		public static decimal ToDecimal(this string s)
		{
			return decimal.Parse(s.Replace(".", ","));
		}

		public static int ToInt(this string s)
		{
			return int.Parse(s);
		}

		public static string ToString(decimal value)
		{
			return Math.Round(value, 2).ToString();
		}

		public static XElement Serialize(object instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			var serializer = new XmlSerializer(instance.GetType());
			using (var stream = new MemoryStream())
			{
				serializer.Serialize(stream, instance);
				stream.Position = 0;
				return XElement.Load(XmlReader.Create(stream));
			}
		}

		public static T Deserialize<T>(this XElement xml)
		{
			var serializer = new XmlSerializer(typeof(T));
			using (var reader = xml.CreateReader())
				return (T)serializer.Deserialize(reader);
		}
	}
}
