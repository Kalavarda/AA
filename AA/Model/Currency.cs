using System;
using System.Linq;

namespace AA.Model
{
	public class Vendor
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}

	public class Price
	{
		public Guid VendorId { get; set; }

		public Guid CurrencyId { get; set; }

		public decimal Value { get; set; }

		public CraftItem[] AddItems { get; set; }

		public Price()
		{
			AddItems = new CraftItem[0];
		}

		public string ToString(Data data)
		{
			var vendor = data.Vendors.First(v => v.Id == VendorId);
			var currency = data.Items.First(v => v.Id == CurrencyId);
			var currencyName = string.IsNullOrWhiteSpace(currency.ShortName) ? currency.Name : currency.ShortName;
			return Value + " " + currencyName + " (" + vendor.Name + ")";
		}
	}
}
