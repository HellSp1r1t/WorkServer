using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using WorkServer;
using Newtonsoft.Json;

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

                ParametrsClass receivedData = JsonConvert.DeserializeObject<ParametrsClass>(data.ToString());

                Console.WriteLine(DateTime.Now.ToString() + " " + "User - " + receivedData.userName + 
                                ", did operation - " + receivedData.operation);

                ParametrsClass answerObject = new ParametrsClass();

                switch (receivedData.operation)
                {
                    case "Connect":
                        answerObject.authentication = Users.AuthUser(receivedData.userName, receivedData.userPass).ToString();
                        answerObject.operation = "authentication";
                        answerObject.userName = receivedData.userName;
                        break;
                    case "readFile":
                        answerObject.operation = "bodyFile";
                        answerObject.fileBody = Documents.GetContentFromFile("texts/" + receivedData.fileName);
                        break;
                    case "readFileForEdit":
                        answerObject.operation = "bodyEditFile";
                        answerObject.fileBody = Documents.GetContentFromFile("texts/" + receivedData.fileName);
                        answerObject.userName = receivedData.userName;
                        break;
                    case "loadFiles":

                        break;
                    case "deleteFile":

                        break;
                    case "addOrEditFile":
                        Documents.WriteContentToFile("texts/" + receivedData.fileName + ".txt", receivedData.fileBody);
                        break;
                    case "addUser":
                        string login = receivedData.answer.Split('/')[0].Split(':')[1];
                        string password = receivedData.answer.Split('/')[1].Split(':')[1];
                        Users.AddNewUser(login, password);
                        answerObject.answer = "True";
                        break;
                    case "checkUsers":

                        break;
                    default:

                        break;
                }

                listener.Send(Encoding.UTF8.GetBytes(JsonEncode(answerObject)));

                listener.Shutdown(SocketShutdown.Both);
                listener.Close();

            }

        }
        //Метод для создания запроса json
        private static string JsonEncode(ParametrsClass parametrsClass)
        {
            return JsonConvert.SerializeObject(parametrsClass);
        }
        class ParametrsClass
        {
            public string operation = null;
            public string userName = null;
            public string userPass = null;
            public string fileName = null;
            public string fileBody = null;
            public string answer = null;
            public string authentication = null;
        }

    }
}