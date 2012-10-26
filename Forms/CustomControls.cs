using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maze.Classes;

namespace Maze.Forms
{
    public class SpellBarPictureBox : PictureBox
    {
        protected EffectEntry pr_RelatedEffect;
        protected int pr_SpellNumber;
        protected bool pr_IsPermanentSpell;

        /// <summary>
        /// Gets or sets the Entry of the Spell
        /// </summary>
        public EffectEntry RelatedEffect
        {
            get
            {
                return pr_RelatedEffect;
            }
            set
            {
                pr_RelatedEffect = value;
            }
        }

        /// <summary>
        /// Gets a serial number of the Spell on a Bar
        /// </summary>
        public int SpellNumber
        {
            get
            {
                return pr_SpellNumber;
            }
            protected set
            {
                pr_SpellNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the spell disappers after using or doesn't
        /// </summary>
        public bool IsPermanentSpell
        {
            get
            {
                return pr_IsPermanentSpell;
            }
            set
            {
                pr_IsPermanentSpell = value;
            }
        }

        public SpellBarPictureBox(int spellNumber)
            : base()
        {
            RelatedEffect = new EffectEntry();
            SpellNumber = spellNumber;
            IsPermanentSpell = false;
        }
    }
}
