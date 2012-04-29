using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maze.Classes;
using Maze.Forms;

namespace MapEditor.Forms
{
    public partial class BlockEdit : MazeForm
    {
        private GridMap Block;

        public BlockEdit(GridMap Block)
        {
            this.Block = Block;
            InitializeComponent();
            CustomInitialize();
            FillComponents();
        }

        private void FillComponents()
        {
            BlockIDTextBox.Text = Block.ID.ToString(); ;

            // Add new or chage exsist MapBlock
            if (Block.ID == -1)
            {
                BlockIDTextBox.Text = Program.WorldMap.GetBlocksCount().ToString();
                ConfirmButton.Text = "Add";
                DeleteButton.Hide();
            }

            LocationXTextBox.Text = Block.Location.X.ToString();
            LocationYTextBox.Text = Block.Location.Y.ToString();
            LocationZTextBox.Text = Block.Location.Z.ToString();
            LocationLevelTextBox.Text = Block.Location.Level.ToString();

            // Movement
            UpCheckBox.Checked      = Block.CanMoveTo(Directions.Up);
            LeftCheckBox.Checked    = Block.CanMoveTo(Directions.Left);
            DownCheckBox.Checked    = Block.CanMoveTo(Directions.Down);
            RightCheckBox.Checked   = Block.CanMoveTo(Directions.Right);

            // Attributes
            StartCheckBox.Checked   = Block.HasAttribute(GridMapAttributes.IsStart);
            FinishCheckBox.Checked  = Block.HasAttribute(GridMapAttributes.IsFinish);
            CoinCheckBox.Checked    = Block.HasAttribute(GridMapAttributes.HasCoin);
        }
         
        void AcceptButtonClick(object sender, System.EventArgs e)
        {
            GridMap NewBlock = new GridMap();
            NewBlock.Initialize();

            NewBlock.ID = Convert.ToInt32(BlockIDTextBox.Text);
            NewBlock.Location.X = Convert.ToInt32(LocationXTextBox.Text);
            NewBlock.Location.Y = Convert.ToInt32(LocationYTextBox.Text);
            NewBlock.Location.Z = Convert.ToInt32(LocationZTextBox.Text);
            NewBlock.Location.Level = Convert.ToInt32(LocationLevelTextBox.Text);

            NewBlock.Type = (uint)Directions.None;
            if (UpCheckBox.Checked)     NewBlock.Type += (uint)Directions.Up;
            if (LeftCheckBox.Checked)   NewBlock.Type += (uint)Directions.Left;
            if (DownCheckBox.Checked)   NewBlock.Type += (uint)Directions.Down;
            if (RightCheckBox.Checked)  NewBlock.Type += (uint)Directions.Right;

            NewBlock.Attribute = 0;
            if (StartCheckBox.Checked)  NewBlock.Attribute += (uint)GridMapAttributes.IsStart;
            if (FinishCheckBox.Checked) NewBlock.Attribute += (uint)GridMapAttributes.IsFinish;
            if (CoinCheckBox.Checked)   NewBlock.Attribute += (uint)GridMapAttributes.HasCoin;

            Program.WorldMap.AddGridMap(NewBlock);
            Program.EditorForm.RebuildGraphMap();
            this.Close();
        }

        void CancelButtonClick(object sender, System.EventArgs e)
        {
            this.Close();
        }

        void DeleteButton_Click(object sender, System.EventArgs e)
        {
            Program.WorldMap.AddEmptyGrigMap(Block);
            Block.Type = 16;
            Program.EditorForm.RebuildGraphMap();
            this.Close();
        }
    }
}
