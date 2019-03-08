using System;
using AA.Model;

namespace AA.Windows
{
	public partial class VendorsWindow
	{
		public VendorsWindow()
		{
			InitializeComponent();
		}

		public VendorsWindow(Data data): this()
		{
			collectionControl.Init(data.Vendors);

			collectionControl.OnAdd = () =>
			{
				var vendor = new Vendor { Id = Guid.NewGuid() };
				var result = new VendorWindow(vendor) { Owner = this }.ShowDialog() == true;
				if (result)
				{
					data.Vendors = data.Vendors.Add(vendor);
					return vendor;
				}
				return null;
			};

			collectionControl.OnEdit = obj =>
			{
				new VendorWindow((Vendor)obj) { Owner = this }.ShowDialog();
			};

			collectionControl.OnRemove = obj =>
			{
				data.Remove((Vendor)obj);
				return true;
			};
		}
	}
}
