using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static int port = 8005;
        static string address = "127.0.0.1";
        static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
        static TcpClient client = null;
        static NetworkStream stream = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowOnLoaded(object sender, RoutedEventArgs e) => Run();

        private void Run()
        {
            Logs.Clear();

            /*try
            {
                socket.Connect(ipPoint);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение"); Close(); }

            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        // получаем ответ
                        byte[] data = new byte[256]; // буфер для ответа
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0; // количество полученных байт

                        //do
                        //{
                            bytes = socket.Receive(data, data.Length, 0);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        //}
                        //while (socket.Available > 0);

                        AddMessageInLogs("[" + DateTime.Now.ToShortTimeString() + "]" + "Ответ сервера: " + builder.ToString());
                    }
                    catch (Exception ex) { }
                }
            });*/

            try
            {
                client = new TcpClient(address, port);
                stream = client.GetStream();
                Task.Run(() => 
                {
                    while (true)
                    {
                        byte[] data = new byte[64];
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;

                        try
                        {
                            do
                            {
                                bytes = stream.Read(data, 0, data.Length);
                                builder.Append(Encoding.Unicode.GetString(data));
                                AddMessageInLogs("[" + DateTime.Now.ToShortTimeString() + "]" + "Ответ сервера: " + builder.ToString());
                            }
                            while (stream.DataAvailable);
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение"); Close(); }
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение"); Close(); }
        }

        private void AddMessageInLogs(string Message) => Application.Current.Dispatcher.Invoke(() => { Logs.Text += Message + "\n"; });

        private void DisconnectFromServerButtonClick(object sender, RoutedEventArgs e) 
        {
            try
            {
                AddMessageInLogs("Вы были отключены от сервера");
                //socket.Shutdown(SocketShutdown.Both);
                //socket.Close();
                client.Close();
                Close();
            }
            catch (Exception ex) { }
        }

        private void SendQueryOnServerButtonClick(object sender, RoutedEventArgs e) 
        {
            if (!QueryBox.Text.Replace(" ", "").Equals(""))
            {
                try
                {
                    string query = QueryBox.Text;
                    byte[] data = Encoding.Unicode.GetBytes(QueryBox.Text);

                    stream.Write(data, 0, data.Length);
                    //socket.SendTo(data, ipPoint);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение"); }
            }
            else MessageBox.Show("Вы пытаетесь отправить пустой запрос", "Исключение");

            QueryBox.Text = "";
        }
    }
}
