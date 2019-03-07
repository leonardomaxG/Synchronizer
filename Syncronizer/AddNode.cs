using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Syncronizer
{
    public partial class AddNode : Form
    {

        private String InputName;
        List<String> Names;
        public AddNode(List<string> IDs)
        {
            Names = IDs;
            InitializeComponent();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Confirm_Click(object sender, EventArgs e)
        {
            if((Input.Text.Trim() == String.Empty))
            {
                MessageBox.Show("Please enter a name for the node!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (Names.Contains(Input.Text))
            {
                MessageBox.Show("A node with that name already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                InputName = Input.Text.Trim();

                bool network = isNetwork.Checked ? true : false;

                StreamWriter sw = new StreamWriter("Node_data.data", append: true);

                sw.WriteLine("");
                sw.WriteLine(InputName + " " + (network ? "Network=1" : "Network=0"));
                sw.Close();
                Close();
            }
            
        }
    }
}
