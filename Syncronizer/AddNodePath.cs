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
    public partial class AddNodePath : Form
    {
        private Dictionary<string, List<String>> NodeList;
        private String Path;
        public bool receive, send;
        public string Path1 { get => Path; set => Path = value; }

        public AddNodePath(Dictionary<string, List<String>> Nodes)
        {
            NodeList = Nodes;
            
            InitializeComponent();
            NodeToAdd.Items.AddRange(NodeList.Keys.ToArray<String>());
        }

        

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Confirm_Click(object sender, EventArgs e)
        {
            
            if(NodeToAdd.Text == "")
            {
                MessageBox.Show("Please choose a node!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if(!Directory.Exists(PathToAdd.Text.Trim()))
            {
                MessageBox.Show("Directory doesn't exist or path is invalid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                receive = canReceive.Checked;
                send = canSend.Checked;
                Path = PathToAdd.Text.Trim();
                String nd = Path + "\\" + NodeToAdd.Text + ".node";
                if (File.Exists(nd))
                {
                    DialogResult result = MessageBox.Show("Node already exists at target location.\nDo you want to overwrite it?",
                                                   "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                    if (result == DialogResult.OK)
                    {
                        File.Create(nd).Close();
                        Path = nd;


                        StreamWriter sw = new StreamWriter("Node_data.data");

                        foreach (var t in NodeList)
                        {
                            sw.WriteLine(t.Key);
                            foreach (var s in t.Value)
                            {
                                sw.WriteLine(s);
                            }
                            sw.WriteLine("");
                        }
                        sw.Close();
                        Close();
                    }
                }
                else
                {
                    File.Create(nd).Close();
                    NodeList[NodeToAdd.Text].Add(Path);
                    Path = nd;


                    StreamWriter sw = new StreamWriter("Node_data.data");

                    foreach (var t in NodeList)
                    {
                        sw.WriteLine(t.Key);
                        foreach (var s in t.Value)
                        {
                            sw.WriteLine(s);
                        }
                        sw.WriteLine("");
                    }
                    sw.Close();
                    Close();
                }
            }
            
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            if(browseFolder.ShowDialog(this) == DialogResult.OK)
            {
                PathToAdd.Text = browseFolder.SelectedPath;
            }
            
        }
    }
}
