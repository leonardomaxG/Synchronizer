//Copy files from user location..............................DONE
//Prompt the location........................................DONE
//Log of copied files........................................DONE
//Copy files if node exists..................................DONE
//Copy files if node is the same.............................DONE
//"Data" file................................................DONE
//Create nodes...............................................DONE
//Copy from-only and Copy to-only nodes......................DONE
//Upgrade hashes
//Password protection
//Encryption
//List of files to be copied
//Progress bar...............................................DONE
//Multi threading............................................DONE
//Set up multi-client connection
//Set remote server connection
//Use ftp for file transfer over network
//Pass for network nodes
//...


using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Syncronizer
{
    public partial class Form1 : Form
    {


        private Dictionary<string, List<NodeClass>> Nodes = new Dictionary<string, List<NodeClass>>();
        private List<Task> tasks = new List<Task>();
        private int diffCount;
        private int copied = 0;
        private string CopyNode;
        private ChooseNode nodeQuery;
        TcpListener server = null;

        public Form1()
        {
            
            this.FormClosed += MyClosedHandler;
            InitializeComponent();
            LoadData();
            _Init();

        }

        private void MyClosedHandler(object sender, FormClosedEventArgs e)
        {
            server.Stop();
        }

        private void _Init()
        {

            Log.Items.Add(GetHash("canReceive=1"));

            // Start server on a separate thread on init
            Task.Factory.StartNew(() =>
            {
                Server_Start();
            });
            
        }

        private void AddNode_Click(object sender, EventArgs e)
        {

            // Make a list of node IDs
            List<String> IDs = new List<String>();
            foreach (var node in Nodes)
            {
                IDs.Add(node.Key);
            }

            // Open dialog window to enter the new node;
            AddNode NewNode = new AddNode(IDs);
            NewNode.ShowDialog(this);

            // Clear the existing node list and load data
            Nodes.Clear();
            LoadData();
        }

        private void AddPath_Click(object sender, EventArgs e)
        {
            // Open dialog window to add new path
            AddNodePath NewPath = new AddNodePath(Nodes);
            NewPath.ShowDialog(this);

            // If path was entered, write data to the new node, clear node dictionary and load data
            if (NewPath.Path1 != null)
            {
                WriteNode(NewPath.Path1, NewPath.receive, NewPath.send);
                Log.Items.Add("Created node: " + NewPath.Path1);
                
                Nodes.Clear();
                LoadData();
            }
        }

        private void Copy_Click(object sender, EventArgs e)
        {

            // Create a list of node IDs
            List<String> IDs = new List<String>();
            foreach (var node in Nodes)
            {
                IDs.Add(node.Key);
            }

            // Open prompt for selecting the node
            nodeQuery = new ChooseNode(IDs);
            nodeQuery.ShowDialog(this);

            // Don't continue if no input was given
            if ((CopyNode = nodeQuery.CopyNode) == null) return;

            File_Difference(CopyNode);
            
            if(diffCount == 0)
            {
                Log.Items.Add("There is nothing to copy.");
            }

            progressBar1.Step = 1;
            progressBar1.Maximum = diffCount;
            progressBar1.Value = 0;
            diffCount = 0;

            // Copy all files on a separate thread both ways
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < Nodes[CopyNode].Count; i++)
                {
                    // Path to first node
                    NodeClass node1 = Nodes[CopyNode].ElementAt(i);

                    for (int j = i + 1; j < Nodes[CopyNode].Count; j++)
                    {
                        // Path to second node
                        NodeClass node2 = Nodes[CopyNode].ElementAt(j);

                        if(node1.CanSend && node2.CanReceive)
                        {
                            Copy_All(node1.Path, node2.Path);
                        }
                        
                        if(node2.CanSend && node1.CanReceive)
                        {
                            Copy_All(node1.Path, node2.Path);
                        }
                       
                    }
                }
            });
            
            nodeQuery = null;

            
        }

        private void Copy_All(string from, string to)
        {
            String fileName;
            String destFile;
            
            // Make an array of files and directories in starting folder
            String[] files = Directory.GetFiles(from);
            String[] directories = Directory.GetDirectories(from);

            
            foreach (String s in files)
            {
                // Skip the node file
               
                
                // Skip the folder if nodes are not the same (will be changed)
                if(Path.GetExtension(s) == "node" && Path.GetFileName(s) != CopyNode)
                {
                    this.Invoke((MethodInvoker)(() => Log.Items.Add("Node at " + from + " and " + to + " are not the same!.")));
                    this.Invoke((MethodInvoker)(() => Log.Items.Add("This folder will be skipped!")));
                    return;
                }
                // Determine destination file path
                fileName = Path.GetFileName(s);
                destFile = Path.Combine(to, fileName);

                // Don't copy if it exists
                if (File.Exists(destFile)) continue;  
                this.Invoke((MethodInvoker)(() => Log.Items.Add("Now copying: " + fileName + " to " + to)));

                //Copy the files
                File.Copy(s, destFile, true);
                copied++;
                this.Invoke((MethodInvoker)(() => progressBar1.Value = copied));
                this.Invoke((MethodInvoker)(() => Count.Text = copied + "/" + diffCount));
                this.Invoke((MethodInvoker)(() => Log.Items.Add("Done: " + fileName)));
               
            }
            foreach (string s in directories)
            {
                // Determine destination directory and create it if it doesn't exist
                fileName = Path.GetFileName(s);
                destFile = Path.Combine(to, fileName);

                if (!Directory.Exists(destFile)) {
                    Directory.CreateDirectory(destFile);
                    this.Invoke((MethodInvoker)(() => Log.Items.Add("Created directory " + destFile)));
                }

                // Recursively copy all files inside the directory
                Copy_All(s, destFile);
            }
        }

        private void ClearLog_Click(object sender, EventArgs e)
        {
            Log.Items.Clear();
        }

        private void WriteNode(String path, bool rec, bool send)
        {
            StreamWriter sw = new StreamWriter(path);

            // Write hashed string in the node file if it can or cannot receive files
            if (rec == true)
            {
                sw.WriteLine(GetHash("canReceive=1"));
            }
            else
            {
                sw.WriteLine(GetHash("canReceive=0"));
            }

            // Write hashed string in the node file if it can or cannot send files
            if(send == true)
            {
                sw.WriteLine(GetHash("canSend=1"));
            }
            else
            {
                sw.WriteLine(GetHash("canSend=0"));
            }
            sw.Close();
        }

        private void LoadData()
        {
            
            try
            {
                // If the file doesn't exist create it
                if (!File.Exists("Node_data.data"))
                {
                    Log.Items.Add("Creating data file");
                    File.Create("Node_data.data").Close();
                    return;
                }

                // Open file and read it
                StreamReader sr = new StreamReader("Node_data.data");
                while (!sr.EndOfStream)
                {
                    // Read the node name
                    String nodeName = sr.ReadLine();

                    // Skip potential empty lines on the start
                    if (String.IsNullOrEmpty(nodeName)) continue;

                    List<NodeClass> NodePaths = new List<NodeClass>();

                    
                    String path;

                    // Read node paths and store them in the list
                    while( !String.IsNullOrEmpty((path = sr.ReadLine())))
                    {
                        NodeClass node = new NodeClass();
                        node.Path = path;
                        node.NodeID = nodeName;

                        StreamReader srNode = new StreamReader(path + "\\" + nodeName + ".node");
                        String line = srNode.ReadLine();
                        if (String.IsNullOrEmpty(line))
                        {
                            Log.Items.Add("Node at: " + path + " could not be read and it will be skipped");
                            srNode.Close();
                            continue;
                        }


                        if (line.Equals(GetHash("canReceive=1")))
                        {
                            node.CanReceive = true;
                        }
                        else if (line.Equals(GetHash("canReceive=0")))
                        {
                            node.CanReceive = false;
                        }
                        else
                        {
                            Log.Items.Add("Node at: " + path + " could not be read and it will be skipped");
                            srNode.Close();
                            continue;
                        }


                        line = srNode.ReadLine();

                        if (String.IsNullOrEmpty(line))
                        {
                            Log.Items.Add("Node at: " + path + " could not be read and it will be skipped");
                            srNode.Close();
                            continue;
                        }


                        if (line.Equals(GetHash("canSend=1")))
                        {
                            node.CanSend = true;
                        }
                        else if (line.Equals(GetHash("canReceive=0")))
                        {
                            node.CanSend = false;
                        }
                        else
                        {
                            Log.Items.Add("Node at: " + path + " could not be read and it will be skipped");
                            srNode.Close();
                            continue;
                        }
                        
                        NodePaths.Add(node);
                    }

                    // Add node with paths to dictionary
                    Nodes.Add(nodeName, NodePaths);
                    sr.ReadLine();
                }
                sr.Close();
            }

            // Catch any exceptions
            catch(Exception e)
            {
                Log.Items.Add(e);
            }
            
        }

        private void Server_Start()
        {
            try
            {
                // Set port on 13000 and ip adress to local adress
                Int32 port = 13000;
                IPAddress IP = IPAddress.Parse("127.0.0.1");

                // Create server and start listening 
                server = new TcpListener(IP, port);
                server.Start();

                // Buffer for data
                Byte[] bytes = new byte[256];
                String data = string.Empty;
                this.Invoke((MethodInvoker)(() => Log.Items.Add("Server started on ip: " + IP.ToString())));
                while (true)
                {

                    // Accepting requests
                    TcpClient client = server.AcceptTcpClient();

                    // Get the stream object
                    NetworkStream stream = client.GetStream();

                    int i;

                    while((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = Encoding.ASCII.GetString(bytes, 0, i);

                        this.Invoke((MethodInvoker)(() => Log.Items.Add(data)));
                    }

                }
                server.Stop();
                Log.Items.Add("Server started on ip: " + IP.ToString());
            }
            catch (Exception e)
            {
                this.Invoke((MethodInvoker)(() => Log.Items.Add(e)));
            }
           
        }
        
        private String GetHash(String str)
        {
            SHA256 hash = SHA256.Create();

            // Convert string to byte array and comput hash
            byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(str));

            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data and format each one as a hexadecimal string
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string
            return sBuilder.ToString();
        }

        private bool VerifyHash(string input, string hash)
        {
            var inputHash = GetHash(input);

            StringComparer comp = StringComparer.OrdinalIgnoreCase;

            return (comp.Compare(inputHash, hash) == 0);
        }

        private void File_Difference(String nodeID)
        {

            int count = Nodes[nodeID].Count();

            for(int i = 0; i < count; i++)
            {

                NodeClass node1 = Nodes[nodeID].ElementAt(i);

                for (int j = i+1; j < count; j++)
                {
                    NodeClass node2 = Nodes[nodeID].ElementAt(j);

                    if (node1.CanSend && node2.CanReceive)
                    {
                        Count(node1.Path, node2.Path);
                    }

                    if(node1.CanReceive && node2.CanSend)
                    {
                        Count(node2.Path, node1.Path);
                    }


                    void Count(String from, String to) {

                        String[] files = Directory.GetFiles(from);
                        String[] directories = Directory.GetDirectories(from);

                        
                        foreach (var file in files)
                        {
                            String fileName = Path.GetFileName(file);
                            String destFile = Path.Combine(to, fileName);

                            if (String.Compare(fileName, nodeID+ ".node") == 0)
                            {
                                continue;
                            }

                            if (!File.Exists(destFile))
                            {
                                diffCount++;
                            }
                        }

                        foreach(var dir in directories)
                        {
                            String dirName = Path.GetFileName(dir);
                            String destDir = Path.Combine(to, dirName);

                            Count(dir, destDir);
                        }
                        
                    }
                }
            }
        }

       
    }

    public class NodeClass
    {
        private String nodeID;
        private bool canSend, canReceive;
        private String path;

        public string NodeID { get => nodeID; set => nodeID = value; }
        public bool CanSend { get => canSend; set => canSend = value; }
        public bool CanReceive { get => canReceive; set => canReceive = value; }
        public string Path { get => path; set => path = value; }
    }
    
}
