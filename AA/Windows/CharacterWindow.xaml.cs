using System;
using System.Windows;
using AA.Model;

namespace AA.Windows
{
	public partial class CharacterWindow
	{
		private readonly Character _character;

		public CharacterWindow()
		{
			InitializeComponent();
		}

		public CharacterWindow(Character character): this()
		{
			_character = character;

			tbName.Text = _character.Name;
			tbWorkScore.Text = _character.WorkScore.ToString();

			Loaded += (sender, e) => {
				tbWorkScore.Focus();
			};
		}

		private void OnOkClick(object sender, RoutedEventArgs e)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(tbName.Text))
					return;

				_character.Name = tbName.Text;
				_character.DateTime = DateTime.Now;
				_character.WorkScore = int.Parse(tbWorkScore.Text);

				DialogResult = true;
			}
			catch (Exception error)
			{
				App.ShowWarning(error.Message);
			}
		}
	}
}
