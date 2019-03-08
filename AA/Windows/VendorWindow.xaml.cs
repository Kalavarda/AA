using System.Windows;
using AA.Model;

namespace AA.Windows
{
	public partial class VendorWindow
	{
		private readonly Vendor _vendor;

		public VendorWindow()
		{
			InitializeComponent();
		}

		public VendorWindow(Vendor vendor): this()
		{
			_vendor = vendor;
			tbName.Text = vendor.Name;
		}

		private void OnOkClick(object sender, RoutedEventArgs e)
		{
			_vendor.Name = tbName.Text;

			DialogResult = true;
		}
	}
}
