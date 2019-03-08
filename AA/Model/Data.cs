using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace AA.Model
{
	public class Data
	{
		public Vendor[] Vendors { get; set; }

		public Item[] Items { get; set; }

		public Item MainCurrency
		{
			get { return Items.FirstOrDefault(i => i.IsMainCurrency); }
		}

		public Profession[] Professions { get; set; }

		public Character[] Characters { get; set; }

		public Data()
		{
			Vendors = new Vendor[0];
			Items = new []
			{
				Item.CreateGoldItem(),
				Item.CreateWorkScoreItem(),
				Item.CreateCraftsReputationItem()
			};
			Professions = new Profession[0];
			Characters = new Character[0];
		}

		public void Save(Stream stream)
		{
			var serializer = new XmlSerializer(typeof(Data));
			serializer.Serialize(stream, this);
			stream.Flush();
		}

		public static Data Load(Stream stream)
		{
			var serializer = new XmlSerializer(typeof(Data));
			var data = (Data)serializer.Deserialize(stream);

			if (data.Items.All(i => i.Id != Item.GoldId))
				data.Add(Item.CreateGoldItem());

			if (data.Items.All(i => i.Id != Item.WorkScoreId))
				data.Add(Item.CreateWorkScoreItem());

			if (data.Items.All(i => i.Id != Item.CraftsReputationId))
				data.Add(Item.CreateCraftsReputationItem());

			return data;
		}

		public void Add(Item item)
		{
			if (Items.Any(i => i.Id == item.Id))
				throw new Exception("Item с таким Id уже есть.");

			Items = Items.Add(item);
		}

		public void Remove(Vendor vendor)
		{
			if (vendor == null) throw new ArgumentNullException("vendor");

			foreach (var item in Items)
				if (item.SalePrices.Any(p => p.VendorId == vendor.Id) || item.BuyPrices.Any(p => p.VendorId == vendor.Id))
					throw new Exception(string.Format("Вендор используется в \"{0}\", удаление невозможно", item));

			Vendors = Vendors.Remove(vendor);
		}

		public void Remove(Profession profession)
		{
			if (profession == null) throw new ArgumentNullException("profession");

			foreach (var item in Items)
				if (item.Crafts.Any(c => c.ProfessionId == profession.Id))
					throw new Exception(string.Format("Профессия используется в \"{0}\", удаление невозможно", item));

			Professions = Professions.Remove(profession);
		}

		public void Remove(Item item)
		{
			if (item == null) throw new ArgumentNullException("item");

			if (item.Id == Item.GoldId || item.Id == Item.WorkScoreId)
				throw new Exception("Это основная валюта, удаление невозможно");

			foreach (var i in Items)
				if (i.Crafts.SelectMany(c => c.CraftItems).Any(ci => ci.ItemId == item.Id))
					throw new Exception(string.Format("Используется в \"{0}\", удаление невозможно", i));

			Items = Items.Remove(item);
		}

		public void Add(Character character)
		{
			if (Characters.Any(i => i.Name == character.Name))
				throw new Exception("Перс с таким именем уже есть.");

			Characters = Characters.Add(character);
		}

		public void Remove(Character character)
		{
			if (character == null) throw new ArgumentNullException("character");

			Characters = Characters.Remove(character);
		}
	}
}
