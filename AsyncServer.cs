using System;
using System.Linq;
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
                            string[] message = System.Text.Encoding.ASCII.GetString(buffer).Split(' ');
                            
                            if (message[0] == "Help")
                            {
                        Console.WriteLine("[Komenda] [Rok] [Runda]");
                            }

                            if (message.Length == 1)
                            {
                                serverResponseBuffer = System.Text.Encoding.ASCII.GetBytes(ApiConnector.GetRequest(message[0], null));
                            }
                            else
                            {

                                string[] test = message.Skip(1).Take(message.Length).ToArray();

                                for (int i = 0; i < test.Length; i++)
                                {
                                    test[i] = test[i].Replace("\0", string.Empty);
                                }

                                serverResponseBuffer = System.Text.Encoding.ASCII.GetBytes(ApiConnector.GetRequest(message[0], test));
                            }

                            stream.Write(serverResponseBuffer, 0, serverResponseBuffer.Length);

                        }
                   

                    /*
                    stream.Write(System.Text.Encoding.ASCII.GetBytes(firstMessage), 0, firstMessage.Length);

                    stream.Read(buffer, 0, Buffer_size);
                    string message = System.Text.Encoding.ASCII.GetString(buffer);

                    char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                    string[] words = message.Split(delimiterChars); //Split(' ')
                    ArraySegment<String> myArrSegAll = new ArraySegment<String>(words);
                  //  string komenda1 = myArrSegAll.Array[0];
                    string root= "help";
                    string root2 = myArrSegAll.Array[0];
                    //string komenda2 = myArrSegAll.Array[1];
                    bool comparison = String.Equals(root, root2);
                    if (comparison == true)
                    {
                        Console.WriteLine("[komenda], [rok], [liczba]"); //nie dziala :((((
                    }

                    //string[] reszta = { myArrSegAll.Array[1], myArrSegAll.Array[2] };




                    Console.WriteLine("Message from client : " + message);

                    //Console.WriteLine(ApiConnector.GetRequest("standings", null));


                    serverResponseBuffer = System.Text.Encoding.ASCII.GetBytes(ApiConnector.GetRequest(myArrSegAll.Array[0], null));

                    stream.Write(serverResponseBuffer, 0, serverResponseBuffer.Length);

                }*/
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





/*if (System.Text.Encoding.ASCII.GetString(buffer) == "Help")
{
    Console.WriteLine("[komenda], [ew. rok], [ew. numer]");
}*/