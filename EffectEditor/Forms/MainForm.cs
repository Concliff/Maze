using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace EffectEditor
{
    public partial class EffectEditor : Form
    {
        private const int AttributesCount = 10;
        private const int AttributesDigits = 3;

        public int AttributeValues;

        private int tabIndex;

        private Maze.Classes.EffectEntry effectInfo;

        Attributes attrForm;

        public EffectEditor()
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            InitializeComponent();

            this.cmbInfoID.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbTargets.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbType.DropDownStyle = ComboBoxStyle.DropDownList;

            Maze.Classes.DBStores.Load();
            EffectIDsLoad();

            attrForm = new Attributes(this);

            effectInfo = new Maze.Classes.EffectEntry();

            tabIndex = 1;
        }

        private void EffectIDsLoad()
        {
            foreach (Maze.Classes.EffectEntry entry in Maze.Classes.DBStores.EffectStore)
            {
                if(entry.ID != 0)
                    cmbInfoID.Items.Add(entry.ID + " - " + entry.EffectName);
            }
        }

        private void TargetsLoad()
        {
            for (int i = 0; i < Enum.GetNames(typeof(Maze.Classes.EffectTargets)).Length; ++i)
            {
                cmbTargets.Items.Add(Enum.GetName(typeof(Maze.Classes.EffectTargets), i));
            }
        }

        private void TypesLoad()
        {
            for (int i = 0; i < Enum.GetNames(typeof(Maze.Classes.EffectTypes)).Length; ++i)
            {
                cmbType.Items.Add(Enum.GetName(typeof(Maze.Classes.EffectTypes), i));
            }
        }

        private void cmbInfoID_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtEffectInfo.Clear();
            ptbMap.Image = null;
            ptbMap.Invalidate();
            ptbAura.Image = null;
            ptbAura.Invalidate();

            int selectedValue = System.Convert.ToInt32(cmbInfoID.Text.Split(' ')[0]);

            if (selectedValue < 1 && selectedValue > Maze.Classes.DBStores.EffectStore.Count)
                return;
            Maze.Classes.EffectEntry entry = Maze.Classes.DBStores.EffectStore[selectedValue];

            txtEffectInfo.AppendText("ID - " + entry.ID.ToString() + Environment.NewLine );
            txtEffectInfo.AppendText(entry.EffectName + Environment.NewLine);
            txtEffectInfo.AppendText("------------------------------------------------" + Environment.NewLine);
            txtEffectInfo.AppendText(entry.Description + Environment.NewLine);
            txtEffectInfo.AppendText("------------------------------------------------" + Environment.NewLine);

            txtEffectInfo.AppendText("Effect type = " + ((int)entry.EffectType).ToString() + " (" +
                entry.EffectType.ToString() + ")" + Environment.NewLine + Environment.NewLine);

            txtEffectInfo.AppendText("Attributes = ");

            byte[] binaryArray = GetBinaryArray(entry.Attributes);

            string atrStr = "";
            for (int i = 0; i < AttributesCount - 1; ++i)
            {
                if (binaryArray[i] == 0)
                    continue;

                if (!String.IsNullOrEmpty(atrStr))
                    atrStr += " | ";
                atrStr += GetHexValue((int)Math.Pow(2, i)) +
                    " (" + Enum.GetName(typeof(Maze.Classes.EffectAttributes), (int)Math.Pow(2, i)) + ")";

            }

            if (String.IsNullOrEmpty(atrStr))
                txtEffectInfo.AppendText("0" + Environment.NewLine + Environment.NewLine);
            else
                txtEffectInfo.AppendText(atrStr + Environment.NewLine + Environment.NewLine);

            txtEffectInfo.AppendText("Range = " + entry.Range.ToString() + Environment.NewLine + Environment.NewLine);

            if ((int)entry.Targets == 0)
                txtEffectInfo.AppendText("Targets = 0\n");
            else
                txtEffectInfo.AppendText("Targets = " + ((int)entry.Targets).ToString() + " (" +
                    Enum.GetName(typeof(Maze.Classes.EffectTargets), entry.Targets) + ")" + Environment.NewLine + Environment.NewLine);

            txtEffectInfo.AppendText("Value = " + entry.Value.ToString() + Environment.NewLine + Environment.NewLine);

            if((int)entry.Duration == -1)
                txtEffectInfo.AppendText("Duration = " + entry.Duration.ToString() + " (OneTact)" + Environment.NewLine + Environment.NewLine);
            else
                txtEffectInfo.AppendText("Duration = " + entry.Duration.ToString() + Environment.NewLine + Environment.NewLine);

            for (int i = 1; i < 5; ++i)
                txtEffectInfo.AppendText("ND" + i.ToString() + " = 0\n" + Environment.NewLine);

            string path = Maze.Classes.GlobalConstants.IMAGES_PATH + "Effects\\Map" + entry.ID.ToString() + ".png";
            if(File.Exists(path))
                ptbMap.Image = Image.FromFile(path);

            path = Maze.Classes.GlobalConstants.IMAGES_PATH + "Effects\\Aura" + entry.ID.ToString() + ".png";
            if (File.Exists(path))
                ptbAura.Image = Image.FromFile(path);
        }

        private byte[] GetBinaryArray(int number)
        {
            byte[] binaryArray = new byte[AttributesCount];
            int i = 0;
            while (number > 0)
            {
                binaryArray[i] = (byte)(number % 2);
                number /= 2;
                ++i;
            }

            return binaryArray;
        }

        private string GetHexValue(int number)
        {
            string hexValue = "";

            while(number > 0)
            {
                hexValue += (number % 16).ToString();
                number /= 16;
            }

            while (hexValue.Length < AttributesDigits)
                hexValue = "0" + hexValue;

            return "0x" + hexValue;
        }

        private void btnSelectMap_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = Environment.CurrentDirectory;
            DialogResult dialogresult = folderBrowserDialog1.ShowDialog();
            //Надпись выше окна контрола
            folderBrowserDialog1.Description = "Поиск папки";
            //folderBrowserDialog1.RootFolder = Environment.CurrentDirectory; //"D:\\МОЁ Ё-Ё\\REPO\Maze\\EffectEditor\\bin\\Release\\Data\\Images";

            string folderName = "";
            if (dialogresult == DialogResult.OK)
            {
                //Извлечение имени папки
                folderName = folderBrowserDialog1.SelectedPath;
            }
            txtMap.Text = folderName;

            string path = Maze.Classes.GlobalConstants.IMAGES_PATH + "Effects\\Map" + txtEffectID.Text + ".png";
            if (File.Exists(path) && path.EndsWith(folderName))
                ptbUploadedMap.Image = Image.FromFile(path);
        }

        private void btnSelectAura_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Environment.CurrentDirectory + "\\Data\\Images\\Effects";
            //openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            // Insert code to read the stream here.
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void tabControl1_Selected(object sender, EventArgs e)
        {
            if (this.tabIndex == 2)
                return;

            if (this.tabControl1.SelectedTab == tabPage2)
            {
                this.tabIndex = 2;

                CleanAll();
            }
        }

        private void CleanAll()
        {
            cmbTargets.Items.Clear();
            cmbType.Items.Clear();

            TargetsLoad();
            TypesLoad();

            txtEffectID.Text = Maze.Classes.DBStores.EffectStore.Count.ToString();
            txtAttributes.Text = "0x000";
            cmbTargets.SelectedItem = "None";
            cmbType.SelectedItem = "None";
            txtValue.Text = "0";

            if (chbOneTact.Checked == true)
            {
                txtDuration.Text = "-1";
                txtDuration.Enabled = false;
            }
            else
            {
                txtDuration.Text = "0";
                txtDuration.Enabled = true;
            }

            txtRange.Text = "0";
            txtND1.Text = "0";
            txtND2.Text = "0";
            txtND3.Text = "0";
            txtND4.Text = "0";
        }

        private void btnAddAttributes_Click(object sender, EventArgs e)
        {
            if (attrForm.IsDisposed)
                attrForm = new Attributes(this);
            attrForm.Show();

            attrForm.CheckAttributes(GetBinaryArray(AttributeValues));
        }

        public void SetAttributeValue(int value)
        {
            txtAttributes.Text = GetHexValue(value);
            effectInfo.Attributes = (ushort)value;
        }

        private void btnCreateString_Click(object sender, EventArgs e)
        {
            effectInfo.ID = System.Convert.ToUInt16(txtEffectID.Text);
            effectInfo.EffectName = txtEffectName.Text;
            effectInfo.Targets = (Maze.Classes.EffectTargets)Enum.Parse(typeof(Maze.Classes.EffectTargets), cmbTargets.SelectedItem.ToString());
            effectInfo.Range = System.Convert.ToInt16(txtRange.Text);
            effectInfo.EffectType = (Maze.Classes.EffectTypes)Enum.Parse(typeof(Maze.Classes.EffectTypes), cmbType.SelectedItem.ToString());
            effectInfo.Value = System.Convert.ToInt32(txtValue.Text);
            effectInfo.Duration = System.Convert.ToInt16(txtDuration.Text);
            effectInfo.ND1 = System.Convert.ToInt16(txtND1.Text);
            effectInfo.ND2 = System.Convert.ToInt16(txtND2.Text);
            effectInfo.ND3 = System.Convert.ToInt16(txtND3.Text);
            effectInfo.ND4 = System.Convert.ToInt16(txtND4.Text);
            effectInfo.Description = txtDescription.Text;

            txtSummary.Text =
                System.Convert.ToString(effectInfo.ID) + "|"
                + effectInfo.EffectName + "|"
                + System.Convert.ToString(effectInfo.Attributes) + "|"
                + System.Convert.ToString((int)(Maze.Classes.EffectTargets)Enum.Parse(typeof(Maze.Classes.EffectTargets), effectInfo.Targets.ToString())) + "|"
                + System.Convert.ToString(effectInfo.Range) + "|"
                + System.Convert.ToString((int)(Maze.Classes.EffectTypes)Enum.Parse(typeof(Maze.Classes.EffectTypes), effectInfo.EffectType.ToString())) + "|"
                + System.Convert.ToString(effectInfo.Value) + "|"
                + System.Convert.ToString(effectInfo.Duration) + "|"
                + System.Convert.ToString(effectInfo.ND1) + "|"
                + System.Convert.ToString(effectInfo.ND2) + "|"
                + System.Convert.ToString(effectInfo.ND3) + "|"
                + System.Convert.ToString(effectInfo.ND4) + "|"
                + effectInfo.Description;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (txtSummary.SelectedText != "")
                Clipboard.SetDataObject(txtSummary.SelectedText);
            else
                MessageBox.Show("No text selected in textBox");
        }

        private void chbOneTact_CheckedChanged(object sender, EventArgs e)
        {
            if (txtDuration.Enabled == true)
            {
                txtDuration.Enabled = false;
                txtDuration.Text = "-1";
            }
            else
            {
                txtDuration.Enabled = true;
                txtDuration.Text = "0";
            }
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            CleanAll();
            AttributeValues = 0;
            attrForm.attributesValue = 0;
            attrForm.UncheckAllAttributes();
            txtEffectName.Text = "";
            txtDescription.Text = "";
            txtSummary.Text = "";
        }

        void txtValue_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            CheckIsDigit(e);
        }

        private void txtDuration_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            CheckIsDigit(e);
        }

        void txtRange_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            CheckIsDigit(e);
        }

        private void CheckIsDigit(System.Windows.Forms.KeyPressEventArgs e)
        {
            if (System.Char.IsLetter(e.KeyChar))
                e.Handled = true;
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            // Reinitialize EffectStore
            Maze.Classes.DBStores.Load();
            object currentEffectId = cmbInfoID.SelectedItem;
            cmbInfoID.Items.Clear();
            EffectIDsLoad();
            cmbInfoID.SelectedItem = currentEffectId;
        }
    }
}
