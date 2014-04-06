using System;
using System.Text;

using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ClientSocketTest
{
    class Program
    {
        private static byte[] result = new byte[1024];
        private static Socket clientSocket;

        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Bind(new IPEndPoint(ip,85));
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, 88));//配置服务器IP与端口   
                Console.WriteLine("连接服务器成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine("连接服务器失败，请按回车退出！ 错误：" + ex.ToString());
                Console.ReadLine();
                return;
            }

            //通过clientSocket发送数据
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    Thread.Sleep(1000);//等待1秒
                    string sendMessage = "client send message help" + DateTime.Now.ToString();
                    clientSocket.Send(Encoding.ASCII.GetBytes(sendMessage));
                    Console.WriteLine("向服务器发送信息：{0}", sendMessage);
                }
                catch (Exception ex)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    Console.WriteLine(ex.ToString());
                    clientSocket.Close();
                    break;
                }
            }

            //clientSocket.Listen(20);
            Console.WriteLine("发送完毕，按回车退出！");
            
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();
            Console.ReadLine();
        }

        /// <summary>   
        /// 监听客户端连接   
        /// </summary> 
        private static void ListenClientConnect()
        {
            while (true)
            {
                Socket mySocket = clientSocket.Accept();
                mySocket.Listen(10);
                //mySocket.Send(Encoding.ASCII.GetBytes("Server say hello"));
                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(mySocket);
            }
        }

        /// <summary>   
        /// 接收消息   
        /// </summary>   
        /// <param name="clientSocket"></param>  
        private static void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    //通过clientSocket接收数据
                    int receiveNumber = myClientSocket.Receive(result);
                    Console.WriteLine("接收服务端{0}消息{1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.ASCII.GetString(result, 0, receiveNumber));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }

        }
    }
}
