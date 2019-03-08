using System;
using AA.Properties;

namespace AA.Model
{
	public class Character
	{
		public string Name { get; set; }

		public DateTime DateTime { get; set; }

		public int WorkScore { get; set; }

		/// <summary>
		/// Сколько времени осталось до достижения максимума ОР
		/// </summary>
		public TimeSpan Remain
		{
			get
			{
				var remain = (Settings.Default.WorkScoreLimit - CurrentWorkScore);
				if (remain <= 0)
					return TimeSpan.Zero;

				return TimeSpan.FromHours((double)remain / Settings.Default.WorkScorePerHour);
			}
		}

		/// <summary>
		/// Вычисляет - сколько сейчас ОР накопилось
		/// </summary>
		public int CurrentWorkScore
		{
			get
			{
				var hours = (DateTime.Now - DateTime).TotalHours;
				var current = WorkScore + (int)(hours * Settings.Default.WorkScorePerHour);
				return Math.Min(current, Settings.Default.WorkScoreLimit);
			}
		}

		public override string ToString()
		{
			var s = Remain != TimeSpan.Zero
				? "осталось " + Progress.ToString(Remain)
				: "Достигнут лимит!";
			return string.Format("{0} - {1} ({2})", CurrentWorkScore, Name, s);
		}
	}
}
