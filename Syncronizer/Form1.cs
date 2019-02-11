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
//Progress bar
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


        private Dictionary<string, List<String>> Nodes = new Dictionary<string, List<string>>();
        private List<Task> tasks = new List<Task>();

        private string CopyNode;
        private ChooseNode nodeQuery;
        TcpListener server = null;
        BackgroundWorker serverWorker = new BackgroundWorker();

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

            // If path was entered clear node lit and load data
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
            if ((CopyNode = nodeQuery.getNode()) == "") return; 

            // Copy all files on a separate thread both ways
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < Nodes[CopyNode].Count; i++)
                {
                    // Path to first node
                    String node1 = Nodes[CopyNode].ElementAt(i);

                    for (int j = i + 1; j < Nodes[CopyNode].Count; j++)
                    {
                        // Path to second node
                        String node2 = Nodes[CopyNode].ElementAt(j);

                        StreamReader sr1 = new StreamReader(node1 + "\\" + CopyNode + ".node");
                        StreamReader sr2 = new StreamReader(node2 + "\\" + CopyNode + ".node");

                        String i1, i2, i3, i4;
                        i1 = GetHash("canSend=1");
                        i2 = GetHash("canSend=0");
                        i3 = GetHash("canReceive=1");
                        i4 = GetHash("canReceive=0");

                        
                        var rec1 = sr1.ReadLine();
                        var send1 = sr1.ReadLine();
                        sr1.Close();

                        var rec2 = sr2.ReadLine();
                        var send2 = sr2.ReadLine();
                        sr2.Close();

                        // If the read line is empty, the file has been modified, skip if so
                        if (String.IsNullOrEmpty(send1) || String.IsNullOrEmpty(rec1))
                        {
                            this.Invoke((MethodInvoker)(() => Log.Items.Add("Node file " + node1 + " is unreadable")));
                            break;
                        }

                        if (String.IsNullOrEmpty(send2) || String.IsNullOrEmpty(rec2))
                        {
                            this.Invoke((MethodInvoker)(() => Log.Items.Add("Node file " + node2 + " is unreadable")));
                            continue;
                        }


                        // Check if node 1 can send and if node 2 can receive data
                        // Break if the file has been modified or if node 1 cannot send data
                        if (VerifyHash("canSend=1", send1) && VerifyHash("canReceive=1", rec2))
                        {
                            Copy_All(node1, node2);
                        }

                        else if(VerifyHash("canSend=0", send1))
                        {
                            break; 
                        }
                        else
                        {
                            this.Invoke((MethodInvoker)(() => Log.Items.Add("Node file:" + node1 + " has been modified outside of the application and will not work")));
                            break;
                        }

                        // Check the other direction (node 2 -> node 1)
                        if (send2.Equals(GetHash("canSend=1")) && rec2.Equals(GetHash("canReceive=1")))
                        {
                            Copy_All(node1, node2);
                        }

                        else if (send2.Equals("canSend=0"))
                        {
                            continue;
                        }
                        else
                        {
                            this.Invoke((MethodInvoker)(() => Log.Items.Add("Node file:" + node2 + " has been modified outside of the application and will not work")));
                            continue;
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

                    List<String> NodePaths = new List<String>();
                    String Path;

                    // Read node paths and store them in the list
                    while( !String.IsNullOrEmpty((Path = sr.ReadLine())))
                    {
                        NodePaths.Add(Path);
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
    }
    
}
