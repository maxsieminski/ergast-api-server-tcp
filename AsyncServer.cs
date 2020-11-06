using System;
using System.Net;
using System.Linq;
using System.Net.Sockets;

namespace TCP_Server_Asynchronous
{
    class AsyncServer : Server
    {

        #region Fields

        private int counter = 0;

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
        /// Calculates ammount of each letter in a message
        /// </summary>
        /// <param name="message">Message sent by client</param>
        /// <returns>Count of each letter in meesage</returns>
        private static string CalculateLetterCount(string message)
        {
            message = message.Substring(0, message.IndexOf('\0'));

            var query = message.GroupBy(c => c).Select(c => new { Char = c.Key, Count = c.Count() });

            string response = "";

            foreach (var result in query)
            {
                if (result.Count > 0 && result.Count < 100 && result.Char != ' ') response += ("Char " + result.Char + " appears " + result.Count + " times\n");
            }

            return response;
        }

        /// <summary>
        /// Takes incoming messages and returns answer
        /// </summary>
        /// <param name="stream">Client stream</param>
        protected override void BeginDataTransmission(NetworkStream stream)
        {
            byte[] buffer = new byte[Buffer_size];
            byte[] serverResponseBuffer = new byte[Buffer_size];

            string firstMessage = "Enter string to count letters : ";

            stream.Write(System.Text.Encoding.ASCII.GetBytes(firstMessage), 0, firstMessage.Length);

            while (true)
            {
                try
                {
                    stream.Read(buffer, 0, Buffer_size);

                    string message = System.Text.Encoding.ASCII.GetString(buffer);
                    ReceivedMessages[counter] = message;

                    string response = CalculateLetterCount(message);
                    serverResponseBuffer = System.Text.Encoding.ASCII.GetBytes(response);
                    SentMessages[counter] = serverResponseBuffer.ToString();

                    counter++;

                    stream.Write(serverResponseBuffer, 0, serverResponseBuffer.Length);

                    Array.Clear(buffer, 0, buffer.Length);
                    Array.Clear(serverResponseBuffer, 0, serverResponseBuffer.Length);
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