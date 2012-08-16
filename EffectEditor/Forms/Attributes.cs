using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EffectEditor
{
    public partial class Attributes : Form
    {
        private EffectEditor owner;
        private int attrValue;
        public int attributesValue
        {
            get
            {
                return attrValue;
            }

            set
            {
                attrValue = value;
            }
        }

        public Attributes(EffectEditor mainForm)
        {
            owner = mainForm;

            this.AutoSize = true;

            InitializeComponent();
            AttributesLoad();

            clbAttributes.CheckOnClick = true;
            attrValue = owner.AttributeValues;

        }

        private void AttributesLoad()
        {
            for (int i = 0; i < Enum.GetNames(typeof(Maze.Classes.EffectAttributes)).Length - 1; ++i)
            {
                clbAttributes.Items.Add(Enum.GetName(typeof(Maze.Classes.EffectAttributes), System.Convert.ToInt32(Math.Pow(2,i))));
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            attrValue = 0;
            for (int i = 0; i < clbAttributes.CheckedItems.Count; ++i)
            {
                if (FindAttribute((string)clbAttributes.CheckedItems[i]) == -1)
                    continue;

                attrValue += FindAttribute((string)clbAttributes.CheckedItems[i]);
            }

            owner.AttributeValues = attrValue;

            owner.SetAttributeValue(attrValue);

            this.Hide();
        }

        public int FindAttribute(string attrName)
        {
            return (int)(Maze.Classes.EffectAttributes)Enum.Parse(typeof(Maze.Classes.EffectAttributes), attrName);
        }

        public void CheckAttributes(byte[] attributes)
        {
            for (int i = 0; i < attributes.Length; ++i)
            {
                if (attributes[i] == 1)
                    clbAttributes.SetItemChecked(i, true);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
