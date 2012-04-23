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
        private bool IsAdding;

        public BlockEdit(GridMap Block)
        {
            this.Block = Block;
            InitializeComponent();
            CustomInitialize();
            FillComponents();
            IsAdding = false;
        }

        private void FillComponents()
        {
            BlockIDTextBox.Text = Block.ID.ToString(); ;

            // Add new or chage exsist MapBlock
            if (Block.ID == -1)
            {
                BlockIDTextBox.Text = Program.WorldMap.GetBlocksCount().ToString();
                ConfirmButton.Text = "Add";
                IsAdding = true;
            }

            LocationXTextBox.Text = Block.Location.X.ToString();
            LocationYTextBox.Text = Block.Location.Y.ToString();
            LocationZTextBox.Text = Block.Location.Z.ToString();
            LocationLevelTextBox.Text = Block.Location.Level.ToString();

            // Movement
            UpCheckBox.Checked = HasBit(Block.Type, (byte)Directions.Up);
            LeftCheckBox.Checked = HasBit(Block.Type, (byte)Directions.Left);
            DownCheckBox.Checked = HasBit(Block.Type, (byte)Directions.Down);
            RightCheckBox.Checked = HasBit(Block.Type, (byte)Directions.Right);

            // Attributes
            StartCheckBox.Checked = HasBit(Block.Attribute,(byte)Attributes.IsStart);
            FinishCheckBox.Checked = HasBit(Block.Attribute,(byte)Attributes.IsFinish);
            CoinCheckBox.Checked = HasBit(Block.Attribute,(byte)Attributes.HasCoin);
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

            NewBlock.Type = 0;
            if (UpCheckBox.Checked) SetBit(ref NewBlock.Type, (byte)Directions.Up);
            if (LeftCheckBox.Checked) SetBit(ref NewBlock.Type, (byte)Directions.Left);
            if (DownCheckBox.Checked) SetBit(ref NewBlock.Type, (byte)Directions.Down);
            if (RightCheckBox.Checked) SetBit(ref NewBlock.Type, (byte)Directions.Right);

            NewBlock.Attribute = 0;
            if (StartCheckBox.Checked) SetBit(ref NewBlock.Attribute, (byte)Attributes.IsStart);
            if (FinishCheckBox.Checked) SetBit(ref NewBlock.Attribute, (byte)Attributes.IsFinish);
            if (CoinCheckBox.Checked) SetBit(ref NewBlock.Attribute, (byte)Attributes.HasCoin);

            Program.WorldMap.AddGridMap(NewBlock);
            Program.EditorForm.RebuildGraphMap();
            this.Close();
        }

        void CancelButtonClick(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
