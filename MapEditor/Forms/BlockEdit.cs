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
        private Cell Block;

        public BlockEdit(Cell Block)
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
                BlockIDTextBox.Text = Map.WorldMap.CellsCount.ToString();
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
            StartCheckBox.Checked   = Block.HasAttribute(CellAttributes.IsStart);
            FinishCheckBox.Checked  = Block.HasAttribute(CellAttributes.IsFinish);
            DropCheckBox.Checked    = Block.HasAttribute(CellAttributes.HasDrop);

            // Options
            PortalCheckBox.Checked = Block.HasOption(CellOptions.Portal);
            OptionValueTextBox.Text = Block.OptionValue.ToString();
        }
         
        void AcceptButtonClick(object sender, System.EventArgs e)
        {
            Cell NewBlock = new Cell();
            NewBlock.Initialize();

            NewBlock.ID = Convert.ToInt32(BlockIDTextBox.Text);
            NewBlock.Location.X = Convert.ToInt32(LocationXTextBox.Text);
            NewBlock.Location.Y = Convert.ToInt32(LocationYTextBox.Text);
            NewBlock.Location.Z = Convert.ToInt32(LocationZTextBox.Text);
            NewBlock.Location.Level = Convert.ToInt32(LocationLevelTextBox.Text);

            NewBlock.Type = 0;
            if (UpCheckBox.Checked)     NewBlock.Type += (uint)Directions.Up;
            if (LeftCheckBox.Checked)   NewBlock.Type += (uint)Directions.Left;
            if (DownCheckBox.Checked)   NewBlock.Type += (uint)Directions.Down;
            if (RightCheckBox.Checked)  NewBlock.Type += (uint)Directions.Right;

            NewBlock.Attribute = 0;
            if (StartCheckBox.Checked)  NewBlock.Attribute += (uint)CellAttributes.IsStart;
            if (FinishCheckBox.Checked) NewBlock.Attribute += (uint)CellAttributes.IsFinish;
            if (DropCheckBox.Checked) NewBlock.Attribute += (uint)CellAttributes.HasDrop;

            NewBlock.Option = 0;
            if (PortalCheckBox.Checked) NewBlock.Option += (uint)CellOptions.Portal;

            NewBlock.OptionValue = Convert.ToInt32(OptionValueTextBox.Text);

            Map.WorldMap.AddCell(NewBlock);
            Program.EditorForm.RebuildGraphMap();
            this.Close();
        }

        void CancelButtonClick(object sender, System.EventArgs e)
        {
            this.Close();
        }

        void DeleteButton_Click(object sender, System.EventArgs e)
        {
            Map.WorldMap.AddEmptyCell(Block);
            Block.Type = 16;
            Program.EditorForm.RebuildGraphMap();
            this.Close();
        }
    }
}
