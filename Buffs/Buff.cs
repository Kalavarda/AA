using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace Buffs
{
	public class Buff
	{
		public DateTime? StartTime { get; private set; }

		public TimeSpan? Duration { get; set; }

		public DateTime? FinishTime
		{
			get
			{
				if (StartTime == null)
					return null;

				return StartTime.Value + Duration;
			}
		}

		public TimeSpan? Remain
		{
			get
			{
				if (FinishTime == null)
					return null;

				return FinishTime.Value - DateTime.Now;
			}
		}

		public TimeSpan DefaultDuration { get; private set; }

		public bool IsOn
		{
			get { return StartTime != null; }
		}

		public Buff(TimeSpan defaultDuration)
		{
			DefaultDuration = defaultDuration;
		}

		public void On()
		{
			StartTime = DateTime.Now;
			Duration = DefaultDuration;
		}

		public void Off()
		{
			StartTime = null;
			Duration = null;
		}
	}

	public class BuffType
	{
		private const char Separator = '|';

		public string ImageUri { get; set; }

		public TimeSpan Duration { get; set; }

		public override string ToString()
		{
			if (string.IsNullOrWhiteSpace(ImageUri))
				return base.ToString();

			return string.Format("{0} ({1})", new FileInfo(ImageUri).Name, Duration);
		}

		public string Serialize()
		{
			return ImageUri + Separator + Duration;
		}

		private static BuffType Deserialize(string value)
		{
			var parts = value.Split(Separator);
			return new BuffType
			{
				ImageUri = parts[0],
				Duration = TimeSpan.Parse(parts[1])
			};
		}

		public static IEnumerable<BuffType> Deserialize(StringCollection collection)
		{
			return collection == null
				? Enumerable.Empty<BuffType>()
				: from string line in collection select Deserialize(line);
		}
	}
}
