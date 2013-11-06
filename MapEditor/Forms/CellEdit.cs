using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maze.Classes;

namespace MapEditor.Forms
{
    public partial class CellEdit : Form
    {
        private Cell cell;

        /// <summary>
        /// Used to prevent infinite loop between CheckBox.CheckedChanged events.
        /// </summary>
        private bool isCheckAllClicked;

        public CellEdit(Cell cell)
        {
            this.cell = cell;
            this.isCheckAllClicked = true;

            InitializeComponent();
            CustomInitialize();
            FillComponents();
        }

        private void FillComponents()
        {
            this.tbxBlockId.Text = this.cell.ID.ToString(); ;

            // Add new or chage exsist MapBlock
            if (this.cell.ID == -1)
            {
                this.tbxBlockId.Text = Program.EditorForm.NewCellID.ToString();
                this.btnConfirm.Text = "Add";
                this.btnDelete.Hide();
            }

            this.tbxLocationX.Text = this.cell.Location.X.ToString();
            this.tbxLocationY.Text = this.cell.Location.Y.ToString();
            this.tbxLocationZ.Text = this.cell.Location.Z.ToString();
            this.tbxLocationLevel.Text = this.cell.Location.Level.ToString();

            // Movement
            this.chkUp.Checked = this.cell.CanMoveTo(Directions.Up);
            this.chkLeft.Checked = this.cell.CanMoveTo(Directions.Left);
            this.chkDown.Checked = this.cell.CanMoveTo(Directions.Down);
            this.chkRight.Checked = this.cell.CanMoveTo(Directions.Right);

            // Attributes
            this.chkStart.Checked = this.cell.HasAttribute(CellAttributes.IsStart);
            this.chkFinish.Checked = this.cell.HasAttribute(CellAttributes.IsFinish);
            this.chkDrop.Checked = this.cell.HasAttribute(CellAttributes.HasDrop);

            // Options
            this.chkPortal.Checked = cell.HasOption(CellOptions.Portal);
            this.tbxOptionValue.Text = cell.OptionValue.ToString();
        }
         
        private void btnAccept_Click(object sender, System.EventArgs e)
        {
            Cell newCell = new Cell();
            newCell.Initialize();

            newCell.ID = Convert.ToInt32(this.tbxBlockId.Text);
            newCell.Location.X = Convert.ToInt32(this.tbxLocationX.Text);
            newCell.Location.Y = Convert.ToInt32(this.tbxLocationY.Text);
            newCell.Location.Z = Convert.ToInt32(this.tbxLocationZ.Text);
            newCell.Location.Level = Convert.ToInt32(this.tbxLocationLevel.Text);

            newCell.Type = 0;
            if (this.chkUp.Checked) newCell.Type += (uint)Directions.Up;
            if (this.chkLeft.Checked) newCell.Type += (uint)Directions.Left;
            if (this.chkDown.Checked) newCell.Type += (uint)Directions.Down;
            if (this.chkRight.Checked) newCell.Type += (uint)Directions.Right;

            newCell.Attribute = 0;
            if (this.chkStart.Checked) newCell.Attribute += (uint)CellAttributes.IsStart;
            if (this.chkFinish.Checked) newCell.Attribute += (uint)CellAttributes.IsFinish;
            if (this.chkDrop.Checked) newCell.Attribute += (uint)CellAttributes.HasDrop;

            newCell.Option = 0;
            if (this.chkPortal.Checked) newCell.Option += (uint)CellOptions.Portal;

            newCell.OptionValue = Convert.ToInt32(this.tbxOptionValue.Text);

            Program.EditorForm.AddCell(newCell);
            Program.EditorForm.Invalidate();
            this.Close();
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void btnDelete_Click(object sender, System.EventArgs e)
        {
            Program.EditorForm.RemoveCell(cell);
            this.cell.Type = 16;
            Program.EditorForm.Invalidate();
            this.Close();
        }

        private void CellEdit_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.btnCancel.PerformClick();
                    break;
                case Keys.Enter:
                    this.btnConfirm.PerformClick();
                    break;
                case Keys.W:
                    this.chkUp.Checked = !this.chkUp.Checked;
                    break;
                case Keys.A:
                    this.chkLeft.Checked = !this.chkLeft.Checked;
                    break;
                case Keys.S:
                    this.chkDown.Checked = !this.chkDown.Checked;
                    break;
                case Keys.D:
                    this.chkRight.Checked = !this.chkRight.Checked;
                    break;
                case Keys.Z:
                    this.chkAll.Checked = !this.chkAll.Checked;
                    break;
                case Keys.R:
                    this.btnDelete.PerformClick();
                    break;
            }
        }

        private void CellEdit_Shown(object sender, System.EventArgs e)
        {
            this.Focus();
        }

        private void chkDirection_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!this.isCheckAllClicked)
                return;

            bool isAllChecked = false;

            if (chkUp.Checked && chkDown.Checked && chkLeft.Checked && chkRight.Checked)
                isAllChecked = true;

            this.isCheckAllClicked = false;
            chkAll.Checked = isAllChecked;
            this.isCheckAllClicked = true;
        }

        private void chkAll_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!this.isCheckAllClicked)
                return;

            this.chkUp.Checked = this.chkDown.Checked = this.chkLeft.Checked = this.chkRight.Checked = this.chkAll.Checked;
        }


    }
}
