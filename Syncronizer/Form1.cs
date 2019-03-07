// Copy files from user location..............................DONE
// Prompt the location........................................DONE
// Log of copied files........................................DONE
// Copy files if node exists..................................DONE
// Copy files if node is the same.............................DONE
// "Data" file................................................DONE
// Create nodes...............................................DONE
// Copy from-only and Copy to-only nodes......................DONE
// Upgrade hashes
// Password protection
// Encryption
// List of files to be copied
// Progress bar...............................................DONE
// Multi threading............................................DONE
// Set up multi-client connection
// Set remote server connection...............................DONE
// Use tcp for file transfer over network
// Pass for network nodes
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
using NATUPNPLib;

namespace Syncronizer
{
    public partial class Form1 : Form
    {


        private Dictionary<String, List<NodeClass>> Nodes = new Dictionary<String, List<NodeClass>>();
        private List<Task> tasks = new List<Task>();
        private int diffCount;
        private int copied = 0;
        private String CopyNode;
        private ChooseNode nodeQuery;
        TcpListener server = null;
        UPnPNAT upnpnat = new UPnPNAT();
        IStaticPortMappingCollection mappings;

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

            /*mappings = upnpnat.StaticPortMappingCollection;
            mappings.Add(80, "TCP", 80, "192.168.1.4", true, "Local Web Server");
            */
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
            StreamWriter stream = new StreamWriter("log.txt");
            int n = Log.Items.Count;
            for(int i = 0; i < n; i++)
            {
                stream.WriteLine(Log.Items[i]);
            }
            stream.Close();
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
                    String str = sr.ReadLine();


                    // Skip potential empty lines on the start
                    if (String.IsNullOrEmpty(str)) continue;

                    String[] info = str.Split(' ');
                    String nodeName = info[0];
                    String net = info[1];
                    bool network;

                    if (net.Equals("Network=1"))
                    {
                        network = true;
                    }
                    else
                    {
                        network = false;
                    }

                    List<NodeClass> NodePaths = new List<NodeClass>();

                    
                    String path;

                    // Read node paths and store them in the list
                    while( !String.IsNullOrEmpty((path = sr.ReadLine())))
                    {
                        NodeClass node = new NodeClass();
                        node.Path = path;
                        node.NodeID = nodeName;
                        node.IsNetwork = network;

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

            while (true)
            {
                try
                {
                    // Set port on 13000
                    int port = 13000;

                    //Get the ip adress for the server
                    String localIp;

                    using (var client = new WebClient())
                    {
                        // Try connecting to Google public DNS and get the local endpoint as IP
                        // If failed to connect set IP as local IP
                        if (CheckForInternetConnection())
                        {
                            try
                            {
                                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                                {
                                    socket.Connect("8.8.8.8", 65530);
                                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                                    localIp = endPoint.Address.ToString();
                                }
                            }
                            catch (Exception e)
                            {
                                localIp = "127.0.0.1";
                            }
                        }
                        else
                        {
                            localIp = "127.0.0.1";
                        }
                    }


                    IPAddress IP = IPAddress.Parse(localIp);

                    // Create listener and start listening 
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

                        // Read length of file name
                        byte[] nameLength = new byte[4];
                        stream.Read(nameLength, 0, 4);
                        int nameSize = BitConverter.ToInt32(nameLength, 0);

                        // Read the name of file
                        byte[] name = new byte[nameSize];
                        stream.Read(name, 0, nameSize);
                        String fileName = Encoding.UTF8.GetString(name);

                        // Read size of file
                        byte[] fileSizeB = new byte[4];
                        stream.Read(fileSizeB, 0, 4);
                        int fileSize = BitConverter.ToInt32(fileSizeB, 0);

                        // Read start signal
                        byte[] startB = new byte[9 + 1];
                        stream.Read(startB, 0, 9);
                        String start = Encoding.UTF8.GetString(startB);

                        this.Invoke((MethodInvoker)(() => Log.Items.Add("Size of name: " + nameSize.ToString())));
                        this.Invoke((MethodInvoker)(() => Log.Items.Add("Name of file: " + fileName)));
                        this.Invoke((MethodInvoker)(() => Log.Items.Add("File size: " + fileSize.ToString())));
                        this.Invoke((MethodInvoker)(() => Log.Items.Add("Start signal: " + start)));

                        int bytesReceived = 0;
                        byte[] file = new byte[fileSize];

                        while (bytesReceived < fileSize)
                        {
                            byte[] PacketSizeB = new byte[4];
                            stream.Read(PacketSizeB, 0, 4);
                            int PacketSize = BitConverter.ToInt32(PacketSizeB, 0);
                            
                            stream.Read(file, bytesReceived, PacketSize);

                            bytesReceived += PacketSize;
                        }

                        File.WriteAllBytes(fileName, file);
                        

                        // Response to client
                        byte[] message = Encoding.UTF8.GetBytes("Testmessage");
                        stream.Write(message, 0, message.Length);
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

        private void AddNetworkNode_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            Task.Factory.StartNew(() =>
            {
                IPAddress iP = IPAddress.Parse(ConnectIp.Text);
                int port = 13000;
                int buffersize = 1024;

                TcpClient client = new TcpClient();
                NetworkStream netStream;

                // Try to connect to server
                try
                {
                    client.Connect(new IPEndPoint(iP, port));
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)(() => Log.Items.Add(ex.Message)));
                    return;
                }

                netStream = client.GetStream();

                String path = @"C:\Users\USER\Documents\ppt\raspored1.png";

                String Filename = Path.GetFileName(path);

                // We wish to send some data in advance:
                // File name, file size, number of packets, send start and send end

                byte[] data = File.ReadAllBytes(path);

                // First packet contains: name size, file name, file size and "sendStart" signal
                byte[] nameSize = BitConverter.GetBytes(Encoding.UTF8.GetByteCount(Filename)); // Int
                byte[] nameB = Encoding.UTF8.GetBytes(Filename);
                byte[] fileSize = BitConverter.GetBytes(data.Length);
                byte[] start = Encoding.UTF8.GetBytes("sendStart");

                // Last packet constains: "sendEnd" signal to stop reading netStream
                byte[] end = Encoding.UTF8.GetBytes("sendEnd");

                // Creating the first package: nameSize, fileName, fileSize and start signal
                byte[] FirstPackage = new byte[4 + nameB.Length + 4 + 9];
                nameSize.CopyTo(FirstPackage, 0);
                nameB.CopyTo(FirstPackage, 4);
                fileSize.CopyTo(FirstPackage, 4 + nameB.Length);
                start.CopyTo(FirstPackage, 4 + nameB.Length + 4);

                // Send the first pckage
                netStream.Write(FirstPackage, 0, FirstPackage.Length);

                // Send the file
                int bytesSent = 0;
                int bytesLeft = data.Length;
                int sendSize;
                byte[] filePackage = new byte[buffersize];

                while (bytesLeft > 0)
                {
                    sendSize = (data.Length - bytesSent < 1024) ? bytesLeft : buffersize;

                    netStream.Write(BitConverter.GetBytes(sendSize), 0, 4);

                    Array.Copy(data, bytesSent, filePackage, 0, sendSize);
                    netStream.Write(filePackage, 0, buffersize);
                    bytesSent += sendSize;
                    bytesLeft -= sendSize;

                }









                byte[] answer = new byte[30];

                // Read the response
                netStream.Read(answer, 0, 11);

                netStream.Close();

                this.Invoke((MethodInvoker)(() => Log.Items.Add(Encoding.UTF8.GetString(answer))));

                client.Close();
            });
        }
        
        public static String GetPublicIPAddress()
        {
            using(var client = new WebClient())
            {
                String site = client.DownloadString("http://checkip.dyn.com");

                String publicIP = site.Substring(site.IndexOf(": ") + 2, site.IndexOf("</body>") - site.IndexOf(": ") - 2);

                return publicIP;
            }
        }
        
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

    }

    public class NodeClass
    {
        private String nodeID;
        private bool canSend, canReceive;
        private String path;
        private bool isNetwork;

        public string NodeID { get => nodeID; set => nodeID = value; }
        public bool CanSend { get => canSend; set => canSend = value; }
        public bool CanReceive { get => canReceive; set => canReceive = value; }
        public string Path { get => path; set => path = value; }
        public bool IsNetwork { get => isNetwork; set => isNetwork = value; }
    }
    
}