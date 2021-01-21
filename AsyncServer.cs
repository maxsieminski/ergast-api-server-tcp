using System;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            for (int i = 0; i < message.Length; i++)
            {
                message[i] = rgx.Replace(message[i], "");
            }
            if ((string)message[0] == "login") return (Authentication.AuthenticateUser((string)message[1], (string)message[2]) == 'y');
            else if (message[0] == "register") return (Authentication.CreateUser(message[1], message[2], message[4] == adminMasterPassword));

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

            byte[] buffer = new byte[Buffer_size];
            byte[] serverResponseBuffer = new byte[Buffer_size];

            while (true)
            {
                try
                {
                    string serverMessage = (isAuthenticated) ? firstMessage : loginFirstMessage;

                    stream.Write(System.Text.Encoding.ASCII.GetBytes(serverMessage), 0, serverMessage.Length);
                    stream.Read(buffer, 0, Buffer_size);

                    /*
                    If message starts with ASCII control character, it is ignored since
                    it does not represent a user message, but rather how client application
                    (e.g. PuTTY) works.
                    */
                    if (Int32.Parse(buffer[0].ToString()) < 33) 
                    {
                        buffer = new byte[Buffer_size];
                        continue;
                    }

                    string[] message = System.Text.Encoding.ASCII.GetString(buffer).Split(' ');
                    string[] args = null;


                    if (!isAuthenticated) {
                        if (CheckCredentials(message)) {
                            currentUser = message[1];
                            isAuthenticated = true;
                        }
                    }

                    // if (message[0].Substring(0, 10) == "printusers")
                    // {
                    //     string response = Authentication.PrintUsers(currentUser);
                    //     stream.Write(System.Text.Encoding.ASCII.GetBytes(response), 0, response.Length);
                    // }
                    // else if (message[0].Substring(0, 10) == "deleteuser")
                    // {
                    //     string somemess = "Enter username of who you want to delete big man.";
                    //     stream.Write(System.Text.Encoding.ASCII.GetBytes(somemess), 0, somemess.Length);
                    //     stream.Read(buffer, 0, Buffer_size); // sorry ja wiem ze to jest obrzydliwy kod, troche idc musze spac a to działa wyzej i nie pluje tych losowych charów jak message
                    //     string[] someuser = System.Text.Encoding.ASCII.GetString(buffer).Split(separators, StringSplitOptions.None);
                    //     string delresponse = Authentication.DeleteUser(someuser[0], currentUser);
                    //     stream.Write(System.Text.Encoding.ASCII.GetBytes(delresponse), 0, delresponse.Length);

                    // }
                        
                    if (message.Length > 1) {
                        args = message.Skip(1).Take(message.Length - 1).ToArray();

                        /*
                        Incoming byte buffer fills remaining space with Null characters.
                        We get rid of them on this line.
                        */
                        args[args.Length - 1] = args[args.Length - 1].Replace("\0", String.Empty);
                    }

                    serverResponseBuffer = System.Text.Encoding.ASCII.GetBytes(await ConnectionHandler.GetRequest(message[0], (args == null) ? null : args, currentUser));
                    stream.Write(serverResponseBuffer, 0, serverResponseBuffer.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    System.Environment.Exit(1);
                }
            }
        }

        public delegate void TransmissionDataDelegate(NetworkStream stream);

        #endregion
    }
}