//Copy files from user location..............................DONE
//Prompt the location........................................DONE
//Log of copied files........................................DONE
//Copy files if node exists..................................DONE
//Copy files if node is the same.............................DONE
//"Data" file................................................DONE
//Create nodes...............................................DONE
//Copy from-only and Copy to-only nodes
//Password protection
//Encryption
//Multi threading
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
            nodeQuery.Show(this);


            if (nodeQuery.getNode() == null) return; //Don't continue if no input was given

            // Copy all files on a separate thread both ways
            CopyNode = nodeQuery.getNode();
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < Nodes[CopyNode].Count; i++)
                {
                    for (int j = i + 1; j < Nodes[CopyNode].Count; j++)
                    {

                        Copy_All(Nodes[CopyNode].ElementAt(i), Nodes[CopyNode].ElementAt(j));
                        Copy_All(Nodes[CopyNode].ElementAt(j), Nodes[CopyNode].ElementAt(i));


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
                if (s == CopyNode + ".node") continue;
                
                // Skip the folder if nodes are not the same (will be changed)
                if(s != CopyNode)
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

        private void WriteData()
        {

        }

        private void LoadData()
        {
            
            try
            {
                // If the file doesn't exist create it
                if (!File.Exists("Node_data.data"))
                {
                    Log.Items.Add("Creating data file");
                    File.Create("Node_data.data");
                    return;
                }

                // Open file and read it
                StreamReader sr = new StreamReader("Node_data.data");
                while (!sr.EndOfStream)
                {
                    // Read the node name
                    String nodeName = sr.ReadLine();
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
        
    }
    
}
