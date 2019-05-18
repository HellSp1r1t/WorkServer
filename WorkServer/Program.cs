using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            const string ip = "127.0.0.1";
            const int port = 8080;

            var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            tcpSocket.Bind(tcpEndPoint);
            tcpSocket.Listen(15);

            while (true)
            {
                var listener = tcpSocket.Accept();
                var buffer = new byte[256];
                var size = 0;
                var data = new StringBuilder();

                do
                {
                    size = listener.Receive(buffer);
                    data.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (listener.Available > 0);

                Console.WriteLine(data);

                string[] fileContent = File.ReadAllLines("WhiteList.txt");

                string[] connectDate = data.ToString().Split('/');

                string answear = "";

                switch (connectDate[0])
                {
                    case "Connect":
                        bool flag = true;
                        foreach (string content in fileContent)
                        {
                            string[] dataFromFile = content.Split(' ');
                            if (connectDate[1] == dataFromFile[0] && connectDate[2] == dataFromFile[1])
                            {
                                answear += "authentication:true/";
                                answear += "files:" + getFilesOnServer();
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            answear += "authentication:false/";
                        }
                        break;
                    case "readFiles":
                        string nameFile = connectDate[1];
                        string[] index = File.ReadAllLines("texts/" + nameFile);
                        answear +="bodyFile/" + string.Join("/n", index);
                        break;
                    case "editFiles":
                        string nameEditFile = connectDate[1];
                        string[] bodyEditFile = File.ReadAllLines("texts/" + nameEditFile);
                        answear += "bodyEditFile/" + string.Join("/n", bodyEditFile) +"/" + nameEditFile;
                        break;
                    case "finalEditFiles":
                        string[] finalEditFile = connectDate[1].Split('|');
                        StreamWriter swe = File.CreateText("texts/" + finalEditFile[1]);
                        swe.WriteLine(finalEditFile[0]);
                        swe.Close();
                        break;
                    case "loadFiles":

                        answear += "filesList/files:" + getFilesOnServer();

                        break;
                    case "deleteFile":
                        File.Delete("texts/" + connectDate[1]);
                        break;
                    case "addFile":

                        string[] dataFile = connectDate[1].Split(':')[1].Split('|');
                        StreamWriter sw = File.CreateText("texts/" + dataFile[0] + ".txt");
                        sw.WriteLine(dataFile[1]);
                        sw.Close();

                        break;
                    case "addUser":
                        string[] userInfo = connectDate[1].Split('|');
                        StreamWriter swa = File.AppendText("whitelist.txt");
                        swa.WriteLine(userInfo[0] + " " + userInfo[1]);
                        swa.Close();
                        answear += "userAdded/";
                        break;
                    case "checkUsers":
                        answear +="usersList/" + getUsersListOnServer();
                        break;
                    default:
                        answear += "authentication:false/";
                        break;
                }

                listener.Send(Encoding.UTF8.GetBytes(answear));

                listener.Shutdown(SocketShutdown.Both);
                listener.Close();

            }

        }
        static string getFilesOnServer() {
            string[] dirs = Directory.GetFiles(Environment.CurrentDirectory + "/texts");
            for (int i = 0; i < dirs.Length; i++)
            {
                string[] t = dirs[i].Split('\\');
                dirs[i] = t[t.Length - 1];
            }
            return string.Join(",", dirs);
        }
        static string getUsersListOnServer()
        {
            string[] inf = File.ReadAllLines("whitelist.txt");
            string[] NamesUser = new string[inf.Length];
            for (int i = 0; i < inf.Length; i++) {
                NamesUser[i] = inf[i].Split(' ')[0];
            }
            return string.Join(",", NamesUser);
        }
        
    }
}