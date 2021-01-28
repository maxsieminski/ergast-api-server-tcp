using System;
using System.Net;
using System.Linq;
using System.Net.Sockets;

namespace TCP_Server_Asynchronous
{
    class AsyncServer : Server
    {

        #region Fields

        const string firstMessage = "Enter command : ";
        const string loginFirstMessage = "Enter one of the following\nlogin [login] [password]\nregister [login] [password]:";
        const string adminMasterPassword = "kiskes";

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
        /// Takes incoming messages while user isn't authorized and returns if his credentials are correct
        /// </summary>
        /// <param name="message">Client message</param>
        private bool CheckCredentials(string[] message) {
            if(message[0] == "login") return (Authentication.AuthenticateUser(message[1], message[2]) == 'y');
            else if (message[0] == "register") return (Authentication.CreateUser(message[1], message[2], (message.Length == 4) ? message[3] == adminMasterPassword : false));

            return false;
        }

        /// <summary>
        /// Takes incoming messages and returns answer
        /// </summary>
        /// <param name="stream">Client stream</param>
        protected override async void BeginDataTransmission(NetworkStream stream)
        {
            string currentUser = null;
            bool isAuthenticated = false;
            bool lastMessageWasEmpty = false;

            byte[] buffer = new byte[Buffer_size];
            byte[] serverResponseBuffer = new byte[Buffer_size];

            while (true)
            {
                try
                {
                    string serverMessage = (isAuthenticated) ? firstMessage : loginFirstMessage;

                    if (!lastMessageWasEmpty)
                    {
                        stream.Write(System.Text.Encoding.ASCII.GetBytes(serverMessage), 0, serverMessage.Length);
                    }
                    stream.Read(buffer, 0, Buffer_size);

                    /*
                    If message starts with ASCII control character, it is ignored since
                    it does not represent a user message, but rather how client application
                    (e.g. PuTTY) works.
                    */
                    if (Int32.Parse(buffer[0].ToString()) < 33) 
                    {
                        buffer = new byte[Buffer_size];
                        lastMessageWasEmpty = true;
                        continue;
                    }

                    lastMessageWasEmpty = false;

                    string[] message = System.Text.Encoding.ASCII.GetString(buffer).Split(' ');
                    string[] args = null;

                    /*
                    Incoming byte buffer fills remaining space with Null characters.
                    We get rid of them on this line.
                    */
                    message[message.Length - 1] = message[message.Length - 1].Replace("\0", String.Empty);

                    if (!isAuthenticated) {
                        if (CheckCredentials(message)) {
                            currentUser = message[1];
                            isAuthenticated = true;
                        }
                    }
                    if (isAuthenticated && message[0] == "logout") {
                        currentUser = null;
                        isAuthenticated = false;
                    }
                        
                    if (message.Length > 1) args = message.Skip(1).Take(message.Length - 1).ToArray();

                    serverResponseBuffer = System.Text.Encoding.ASCII.GetBytes(await ConnectionHandler.GetRequestResponse(message[0], (args == null) ? null : args, currentUser));
                    stream.Write(serverResponseBuffer, 0, serverResponseBuffer.Length);
                }
                catch (Exception) {}
            }
        }

        public delegate void TransmissionDataDelegate(NetworkStream stream);

        #endregion
    }
}