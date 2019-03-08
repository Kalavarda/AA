using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;
using AA.Model;
using AA.Properties;
using AA.Windows;
using Microsoft.Win32;

namespace AA
{
	public partial class MainWindow
	{
		private Data _data = new Data();
		private Progress[] _progresses;
		private Character[] _characters;

		private Progress[] Progresses
		{
			get
			{
				if (_progresses == null)
					_progresses = string.IsNullOrWhiteSpace(Settings.Default.Progresses)
						? new Progress[0]
						: XElement.Parse(Settings.Default.Progresses).Deserialize<Progress[]>();

				return _progresses;
			}
			set
			{
				_progresses = value;
				SaveProgress();
			}
		}

		private void SaveProgress()
		{
			Settings.Default.Progresses = Utils.Serialize(_progresses).ToString();
			Settings.Default.Save();
		}

		private Character[] Characters
		{
			get
			{
				if (_characters == null)
					_characters = string.IsNullOrWhiteSpace(Settings.Default.Characters)
						? new Character[0]
						: XElement.Parse(Settings.Default.Characters).Deserialize<Character[]>();

				return _characters;
			}
			set
			{
				_characters = value;
				SaveCharacters();
			}
		}

		private void SaveCharacters()
		{
			Settings.Default.Characters = Utils.Serialize(_characters).ToString();
			Settings.Default.Save();
		}

		public MainWindow()
		{
			InitializeComponent();

			if (!DesignerProperties.GetIsInDesignMode(this))
				Loaded += MainWindow_Loaded;

			TuneControls(_data);
		}

		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(Settings.Default.LastFileName) && File.Exists(Settings.Default.LastFileName))
				Load(Settings.Default.LastFileName);

			#region Progress

			progressesCollection.Init(Progresses);

			progressesCollection.OnAdd = () =>
			{
				var progress = new Progress { Start = DateTime.Now, Duration = TimeSpan.FromMinutes(83)};
				var result = new ProgressWindow(progress) { Owner = this }.ShowDialog() == true;
				if (result)
				{
					Progresses = Progresses.Add(progress);
					return progress;
				}
				return null;
			};

			progressesCollection.OnEdit = obj =>
			{
				if (new ProgressWindow((Progress)obj) { Owner = this }.ShowDialog() == true)
					SaveProgress();
			};

			progressesCollection.OnRemove = obj =>
			{
				Progresses = Progresses.Remove((Progress)obj);
				return true;
			};

			progressesCollection.OnOrder = obj => ((Progress)obj).Remain;

			var timer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
			timer.Tick += (sender1, e1) => {
				progressesCollection.Refresh();
			};
			timer.Start();

			#endregion

			#region Characters

			charactersCollection.Init(Characters);

			charactersCollection.OnAdd = () =>
			{
				var character = new Character();
				var result = new CharacterWindow(character) { Owner = this }.ShowDialog() == true;
				if (result)
				{
					Characters = Characters.Add(character);
					return character;
				}
				return null;
			};

			charactersCollection.OnEdit = obj =>
			{
				if (new CharacterWindow((Character)obj) { Owner = this }.ShowDialog() == true)
					SaveCharacters();
			};

			charactersCollection.OnRemove = obj =>
			{
				Characters = Characters.Remove((Character)obj);
				return true;
			};

			charactersCollection.OnOrder = obj => ((Character)obj).Remain;

			var timer2 = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
			timer2.Tick += (sender2, e2) =>
			{
				charactersCollection.Refresh();
			};
			timer2.Start();
			
			#endregion
		}

		private void Load(string fileName)
		{
			using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				_data = Data.Load(file);
				App.CurrentCharacter = _data.Characters.FirstOrDefault();
				TuneControls(_data);
			}
		}

		private void OnVendorsClick(object sender, RoutedEventArgs e)
		{
			new VendorsWindow(_data) { Owner = this }.ShowDialog();
		}

		private void OnSaveClick(object sender, RoutedEventArgs e)
		{
			var fileDialog = new SaveFileDialog();
			TuneFileDialog(fileDialog);
			if (fileDialog.ShowDialog() == true)
			{
				using (var file = new FileStream(fileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
					_data.Save(file);

				Settings.Default.LastFileName = fileDialog.FileName;
				Settings.Default.Save();
			}
		}

		private void OnLoadClick(object sender, RoutedEventArgs e)
		{
			var fileDialog = new OpenFileDialog();
			TuneFileDialog(fileDialog);
			if (fileDialog.ShowDialog() == true)
			{
				Load(fileDialog.FileName);
				
				Settings.Default.LastFileName = fileDialog.FileName;
				Settings.Default.Save();
			}
		}

		private static void TuneFileDialog(FileDialog fileDialog)
		{
			fileDialog.DefaultExt = ".xml.aa";
			fileDialog.Filter = "AA|*.xml.aa|Все файлы|*.*";
		}

		private void TuneControls(Data data)
		{
			collectionControl.Clear();
			collectionControl.Init(data.Items);

			collectionControl.OnAdd = () =>
			{
				var item = new Item { Id = Guid.NewGuid() };
				var result = new ItemWindow(item, data) { Owner = this }.ShowDialog() == true;
				if (result)
				{
					data.Add(item);
					return item;
				}
				return null;
			};

			collectionControl.OnEdit = obj =>
			{
				new ItemWindow((Item)obj, data) { Owner = this }.ShowDialog();
			};

			collectionControl.OnRemove = obj =>
			{
				data.Remove((Item)obj);
				return true;
			};

			TuneCharactersMenuItem();
		}

		private void TuneCharactersMenuItem()
		{
			charactersMenuItem.IsEnabled = _data != null && _data.Characters.Any();
			charactersMenuItem.Items.Clear();
			if (_data != null)
			{
				foreach (var character in _data.Characters.OrderBy(ch => ch.Name))
				{
					var menuItem = new MenuItem
					{
						Header = character.Name,
						Tag = character,
						IsChecked = character == App.CurrentCharacter
					};
					menuItem.Click += (sender, e) =>
					{
						var character1 = (Character) ((FrameworkElement) sender).Tag;
						App.CurrentCharacter = character1;
						TuneControls(_data);
					};
					charactersMenuItem.Items.Add(menuItem);
				}
			}

			charactersMenuItem.Header = App.CurrentCharacter != null
				? string.Format("Персонажи ({0})", App.CurrentCharacter.Name)
				: "Персонажи";
		}

		private void OnExitClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void OnProfessionsClick(object sender, RoutedEventArgs e)
		{
			new ProfessionsWindow(_data) { Owner = this }.ShowDialog();
		}

		private void OnCharactersClick(object sender, RoutedEventArgs e)
		{
			new CharactersWindow(_data) { Owner = this }.ShowDialog();
		}
	}
}
