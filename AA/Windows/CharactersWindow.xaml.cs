using AA.Model;

namespace AA.Windows
{
	public partial class CharactersWindow
	{
		public CharactersWindow()
		{
			InitializeComponent();
		}

		public CharactersWindow(Data data): this()
		{
			charactersCollection.Init(data.Characters);

			charactersCollection.OnAdd = () =>
			{
				var character = new Character();
				var result = new CharacterWindow(character) { Owner = this }.ShowDialog() == true;
				if (result)
				{
					data.Add(character);
					return character;
				}
				return null;
			};

			charactersCollection.OnEdit = obj =>
			{
				new CharacterWindow((Character)obj) { Owner = this }.ShowDialog();
			};

			charactersCollection.OnRemove = obj =>
			{
				data.Remove((Character)obj);
				return true;
			};
		}
	}
}
