using System;
using EventNotifier.Properties;

namespace EventNotifier
{
	public class Event
	{
		public string Name { get; set; }

		public bool RealTime { get; set; }

		public DateTime StartTime { get; set; }

		public TimeSpan Period { get; set; }

		public TimeSpan RemainReal
		{
			get
			{
				return NextTimeReal - DateTime.Now;
			}
		}

		public TimeSpan RemainGame
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public DateTime NextTimeReal
		{
			get
			{
				var t = RealTime ? StartTime : GameToReal(StartTime);

				var dt = Period;
				if (!RealTime)
					dt = TimeSpan.FromSeconds(dt.TotalSeconds / Settings.Default.GameTimeRatio);

				while (t < DateTime.Now)
					t += dt;

				return t;
			}
		}

		public static DateTime GameToReal(DateTime gameTime)
		{
			var dt = gameTime - gameTime.Date;
			dt = TimeSpan.FromSeconds(dt.TotalSeconds / Settings.Default.GameTimeRatio);
			return gameTime.Date + dt;
		}

		public static DateTime RealToGame(DateTime realTime)
		{
			var dt = realTime - realTime.Date;
			dt = TimeSpan.FromSeconds(dt.TotalSeconds * Settings.Default.GameTimeRatio);
			return realTime.Date + dt;
		}
	}
}
