using System.Collections.Generic;

namespace EventNotifier
{
	public partial class NotifyWindow
	{
		public NotifyWindow()
		{
			InitializeComponent();
		}

		public NotifyWindow(IEnumerable<Event> events)
			: this()
		{
		}
	}
}
