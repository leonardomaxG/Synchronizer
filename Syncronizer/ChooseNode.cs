using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Syncronizer
{
    public partial class ChooseNode : Form
    {
        private string copyNode;

        public string CopyNode { get => copyNode; set => copyNode = value; }

        public ChooseNode(List<String> IDs)
        {

            InitializeComponent();
            _Init(IDs);
        }

        private void _Init(List<string> IDs)
        {
            NodeToCopy.Items.AddRange(IDs.ToArray<String>());
        }

        ~ChooseNode()
        {
            CopyNode = null;
        }
      
        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Submit_Click(object sender, EventArgs e)
        {

            if (NodeToCopy.Text == "")
            {
                MessageBox.Show("Please choose a node.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                CopyNode = NodeToCopy.Text;

                Close();
            }
        }
    }
}
