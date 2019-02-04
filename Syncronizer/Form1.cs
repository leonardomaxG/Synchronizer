//Copy files from user location..............................DONE
//Prompt the location........................................DONE
//Log of copied files........................................DONE
//Copy files if node exists..................................DONE
//Copy files if node is the same.............................
//"Data" file
//Create nodes
//Copy from-only and Copy to-only nodes
//Password protection
//Multi threaded
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
            LoadData();
            this.FormClosed += MyClosedHandler;
            InitializeComponent();
            _Init();

        }

        private void MyClosedHandler(object sender, FormClosedEventArgs e)
        {
            server.Stop();
        }

        private void _Init()
        {
            serverWorker.DoWork += (_, args) =>
            {
                tasks.Add(Task.Factory.StartNew(() => Server_Start()));
            };

            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            serverWorker.RunWorkerAsync();
            
        }

        private void AddNode_Click(object sender, EventArgs e)
        {

            List<String> IDs = new List<String>();
            foreach (var node in Nodes)
            {
                IDs.Add(node.Key);
            }

            AddNode NewNode = new AddNode(IDs);
            NewNode.ShowDialog(this);
            LoadData();
        }

        private void AddPath_Click(object sender, EventArgs e)
        {
            AddNodePath NewPath = new AddNodePath(Nodes);
            NewPath.ShowDialog(this);
            if (NewPath.Path1 != null)
            {
                Log.Items.Add("Created node: " + NewPath.Path1);
                LoadData();
            }
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            List<String> IDs = new List<String>();
            foreach (var node in Nodes)
            {
                IDs.Add(node.Key);
            }

            nodeQuery = new ChooseNode(IDs);
            nodeQuery.Show(this);
            List<Task> work = new List<Task>();

            if (nodeQuery.getNode() == null) return; //Don't continue if no input was given

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
            string fileName;
            string destFile;
            
            string[] files = Directory.GetFiles(from);
            string[] directories = Directory.GetDirectories(from);

            foreach (string s in files)
            {
                //skip the node file
                if (s == CopyNode + ".node") continue;

                fileName = Path.GetFileName(s);
                destFile = Path.Combine(to, fileName);

                if (File.Exists(destFile)) continue;  //Don't copy if it exists
                this.Invoke((MethodInvoker)(() => Log.Items.Add("Now copying: " + fileName + " to " + to)));
                File.Copy(s, destFile, true);
                this.Invoke((MethodInvoker)(() => Log.Items.Add("Done: " + fileName)));
               
            }
            foreach (string s in directories)
            {
                fileName = Path.GetFileName(s);
                destFile = Path.Combine(to, fileName);
                if (!Directory.Exists(destFile)) {
                    Directory.CreateDirectory(destFile);
                    this.Invoke((MethodInvoker)(() => Log.Items.Add("Created directory " + destFile)));
                }
                Copy_All(s, destFile);
            }
        }

        private void ClearLog_Click(object sender, EventArgs e)
        {
            Log.Items.Clear();
        }

        private void Filewrite_Click(object sender, EventArgs e)
        {

        }

        private void WriteData()
        {

        }

        private void LoadData()
        {
            
            try
            {
                if (!File.Exists("C:\\Users\\Korisnik\\Documents\\Node_data.data"))
                {
                    Log.Items.Add("Creating data file");
                    File.Create("C:\\Users\\Korisnik\\Documents\\Node_data.data");
                    return;
                }
                StreamReader sr = new StreamReader("C:\\Users\\Korisnik\\Documents\\Node_data.data");
                while (!sr.EndOfStream)
                {
                    String nodeName = sr.ReadLine();
                    List<String> NodePaths = new List<String>();
                    String Path;
                    while( !String.IsNullOrEmpty((Path = sr.ReadLine())))
                    {
                        NodePaths.Add(Path);
                    }
                    Nodes.Add(nodeName, NodePaths);
                    sr.ReadLine();
                }
                sr.Close();
            }
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
