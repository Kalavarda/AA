using System;
using System.Xml.Serialization;

namespace AA.Model
{
	public class Progress
	{
		public string Name { get; set; }

		public DateTime Start { get; set; }

		[XmlIgnore]
		public TimeSpan Duration
		{
			get
			{
				return TimeSpan.FromMinutes(DurationInt);
			}
			set { DurationInt = (int)value.TotalMinutes; }
		}

		public int DurationInt { get; set; }

		public DateTime End
		{
			get
			{
				return Start + Duration;
			}
		}

		public TimeSpan Remain
		{
			get
			{
				return End - DateTime.Now;
			}
		}

		public override string ToString()
		{
			if (End > DateTime.Now)
				return string.Format("{0} [ {1} ] {2}", Name, ToString(Remain), End.ToShortTimeString());

			return "-[ Завершено ]- " + Name;
		}

		internal static string ToString(TimeSpan duration)
		{
			var m = (int) duration.TotalMinutes;
			var h = m / 60;
			m = m - h * 60;

			var hh = h.ToString();
			var mm = m.ToString();
			if (h > 0 && mm.Length < 2)
				mm = "0" + mm;

			if (h > 6)
				return hh + " ч.";

			if (h == 0)
				return mm + " мин.";

			return hh + " ч. " + mm + " мин.";
		}
	}
}
