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
        public static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowOnLoaded(object sender, RoutedEventArgs e) => Run();

        private void Run()
        {
            Logs.Document.Blocks.Clear();

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
            socket.Connect(ipPoint);

            Task.Run(() =>
            {
                while (true)
                {
                    // получаем ответ
                    byte[] data = new byte[256]; // буфер для ответа
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байт

                    do
                    {
                        //bytes = socket.Receive(data, data.Length, 0);
                        //builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        byte[] buffer = new byte[10000];
                        int byteRead = socket.Accept().Receive(buffer);

                    }
                    while (socket.Available > 0);

                    AddMessageInLogs("ответ сервера: " + builder.ToString());
                }
            });
        }

        private void AddMessageInLogs(string Message) { Logs.Document.Blocks.Add(new Paragraph(new Run(Message))); }

        private void DisconnectFromServerButtonClick(object sender, RoutedEventArgs e) 
        {
            try
            {
                AddMessageInLogs("Вы были отключены от сервера");
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); Close(); }
        }

        private void SendQueryOnServerButtonClick(object sender, RoutedEventArgs e) 
        {
            try
            {
                string query = QueryBox.Text;
                byte[] data = Encoding.Unicode.GetBytes(QueryBox.Text);

                socket.Send(data);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение"); }
        }
    }
}
