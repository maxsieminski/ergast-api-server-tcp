using System;
using System.Net;
using System.Net.Sockets;

namespace TCP_Server_Asynchronous
{
    class AsyncServer : Server
    {

        #region Fields

        

        #endregion

        #region Constructors

        public AsyncServer(IPAddress address, int port) : base(address, port) { }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes asynchronous TCP Server
        /// </summary>
        public override void Start()
        {
            StartListening();
            AcceptClient();
        }

        public string[] ReceivedMessages { get; } = new string[100];
        public string[] SentMessages { get; } = new string[100];

        /// <summary>
        /// Listens for incoming clients
        /// </summary>
        protected override void AcceptClient()
        {
            while (true)
            {
                TcpClient tcpClient = TcpListener.AcceptTcpClient();

                Stream = tcpClient.GetStream();

                TransmissionDataDelegate transmissionDelegate = new TransmissionDataDelegate(BeginDataTransmission);

                transmissionDelegate.BeginInvoke(Stream, TransmissionCallback, tcpClient);
            }
        }

        private void TransmissionCallback(IAsyncResult asyncResult)
        {
            TcpClient = (TcpClient)asyncResult.AsyncState;
            TcpClient.Close();
        }


        /// <summary>
        /// Takes incoming messages and returns answer
        /// </summary>
        /// <param name="stream">Client stream</param>
        protected override void BeginDataTransmission(NetworkStream stream)
        {
            byte[] buffer = new byte[Buffer_size];
            byte[] serverResponseBuffer = new byte[Buffer_size];

            string firstMessage = "Enter command : ";

            while (true)
            {
                try
                {
                    stream.Write(System.Text.Encoding.ASCII.GetBytes(firstMessage), 0, firstMessage.Length);
                    
                    stream.Read(buffer, 0, Buffer_size);
                    string message = System.Text.Encoding.ASCII.GetString(buffer);

                    Console.WriteLine("Message from client : " + message);

                    serverResponseBuffer = System.Text.Encoding.ASCII.GetBytes(ApiConnector.GetRequestAsync("standings", null).Result);
                    stream.Write(serverResponseBuffer, 0, Buffer_size);

                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        public delegate void TransmissionDataDelegate(NetworkStream stream);

        #endregion
    }
}