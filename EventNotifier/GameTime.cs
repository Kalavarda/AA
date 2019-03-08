using System;
using EventNotifier.Properties;

namespace EventNotifier
{
	public static class GameTime
	{
		public static DateTime Now
		{
			get
			{
				var startPoint = DateTime.Now.Date + Settings.Default.TimeShift;	// начало отсчёта времени
				var dTime = (DateTime.Now - startPoint).TotalMilliseconds;
				return startPoint + TimeSpan.FromMilliseconds(dTime * Settings.Default.GameTimeRatio);
			}
		}
	}
}
