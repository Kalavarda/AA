using System;
using AA.Model;

namespace AA.Windows
{
	public partial class ProfessionsWindow
	{
		public ProfessionsWindow()
		{
			InitializeComponent();
		}

		public ProfessionsWindow(Data data)
			: this()
		{
			collectionControl.Init(data.Professions);

			collectionControl.OnAdd = () =>
			{
				var profession = new Profession { Id = Guid.NewGuid() };
				var result = new ProfessionWindow(profession) { Owner = this }.ShowDialog() == true;
				if (result)
				{
					data.Professions = data.Professions.Add(profession);
					return profession;
				}
				return null;
			};

			collectionControl.OnEdit = obj =>
			{
				new ProfessionWindow((Profession)obj) { Owner = this }.ShowDialog();
			};

			collectionControl.OnRemove = obj =>
			{
				data.Remove((Profession)obj);
				return true;
			};
		}
	}
}
