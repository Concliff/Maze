using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maze.Classes;

namespace Maze.Forms
{
    public partial class GameForm : MazeForm
    {
        private enum FormInterface
        {
            Initial,
            MapEditor,
            MapEditorNew,
            MapEditorOpen,
            Play,
        };

        private FormInterface CurrentInterface;

        public GameForm()
        {
            InitializeComponent();
            CustomInitialize();
            ChangeInterface(FormInterface.MapEditor, false);
            ChangeInterface(FormInterface.MapEditorNew, false);
            ChangeInterface(FormInterface.MapEditorOpen, false);
            ChangeInterface(FormInterface.Play, false);

            CurrentInterface = FormInterface.Initial;
            ChangeInterface(FormInterface.Initial, true);

            FillComboBox();

        }

        private void FillComboBox()
        {
            MapComboBox.Items.AddRange(GetWorldMap().GetMapNamesList());
            MapComboBox.SelectedIndex = 0;
        }

        void ButtonClick(object sender, System.EventArgs e)
        {
            if (sender == BackButton)
                ReturnInterface(CurrentInterface);
            else if (sender == MapEditorButton)
                SetInterface(FormInterface.MapEditor);
            else if (sender == OpenMapButton)
                SetInterface(FormInterface.MapEditorOpen);
            else if (sender == CreateMapButton)
                SetInterface(FormInterface.MapEditorNew);
            else if (sender == GameButton)
                SetInterface(FormInterface.Play);
        }


        private void SetInterface(FormInterface NewInterface)
        {
            ChangeInterface(CurrentInterface, false);
            ChangeInterface(NewInterface, true);
            CurrentInterface = NewInterface;
        }

        private void ReturnInterface(FormInterface Interface)
        {
            switch (Interface)
            {
                case FormInterface.MapEditor: SetInterface(FormInterface.Initial); break;
                case FormInterface.MapEditorNew: SetInterface(FormInterface.MapEditor); break;
                case FormInterface.MapEditorOpen: SetInterface(FormInterface.MapEditor); break;
                case FormInterface.Play: SetInterface(FormInterface.Initial); break;
            }
        }

        private void ChangeInterface(FormInterface Interface, bool Show)
        {
            if (Interface == FormInterface.Initial)
                BackButton.Hide();
            else
                BackButton.Show();

            switch (Interface)
            {
                case FormInterface.Initial:
                    {
                        if (Show) MapEditorButton.Show(); else MapEditorButton.Hide();
                        if (Show) GameButton.Show(); else GameButton.Hide();
                        break;
                    }
                case FormInterface.MapEditor:
                    {
                        if (Show) OpenMapButton.Show(); else OpenMapButton.Hide();
                        if (Show) CreateMapButton.Show(); else CreateMapButton.Hide();
                        break;
                    }
                case FormInterface.MapEditorNew:
                    {
                        if (Show) MapNameTextBox.Show(); else MapNameTextBox.Hide();
                        if (Show) MapNameLabel.Show(); else MapNameLabel.Hide();
                        if (Show) CreateNewMapButton.Show(); else CreateNewMapButton.Hide();
                        break;
                    }
                case FormInterface.MapEditorOpen:
                    {
                        if (Show) OpenSelectedMapButton.Show(); else OpenSelectedMapButton.Hide();
                        if (Show) CurrentMapLabel.Show(); else CurrentMapLabel.Hide();
                        if (Show) MapComboBox.Show(); else MapComboBox.Hide();
                        break;
                    }
                case FormInterface.Play:
                    {
                        if (Show) PlayerNameTextBox.Show(); else PlayerNameTextBox.Hide();
                        if (Show) PlayerNameLabel.Show(); else PlayerNameLabel.Hide();
                        if (Show) CurrentMapLabel.Show(); else CurrentMapLabel.Hide();
                        if (Show) MapComboBox.Show(); else MapComboBox.Hide();
                        if (Show) PlayButton.Show(); else PlayButton.Hide();
                        break;
                    }
            }
        }

        void PlayButtonClick(object sender, System.EventArgs e)
        {
            SetNextAction(WorldNextAction.GamePlay);
            GetWorldMap().LoadMap(MapComboBox.SelectedIndex);
            World.CreatePlayForm();
            this.Close();
        }

        void OpenSelectedMapButtonClick(object sender, System.EventArgs e)
        {
            SetNextAction(WorldNextAction.MapEdit);
            GetWorldMap().LoadMap(MapComboBox.SelectedIndex);
            World.CreateMapEditorForm();
            this.Close();
        }

        void CreateNewMapButtonClick(object sender, System.EventArgs e)
        {
            if (!GetWorldMap().CreateMap(MapNameTextBox.Text))
            {
                MessageBox.Show("Map with this Map Name is exist");
                return;
            }
            SetNextAction(WorldNextAction.MapEdit);
            World.CreateMapEditorForm();
            this.Close();
        }
    }
}
