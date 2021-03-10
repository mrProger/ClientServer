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

namespace Server
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static int port = 8005;
        static Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowOnLoaded(object sender, RoutedEventArgs e) => Run();

        private void Run()
        {
            ClientQuery.Document.Blocks.Clear();

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                AddMessageInLogs("Сервер запущен. Ожидание подключений...");

                Task.Run(() =>
                {
                    while (true)
                    {
                        Socket handler = listenSocket.Accept();
                        // получаем сообщение
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0; // количество полученных байтов
                        byte[] data = new byte[256]; // буфер для получаемых данных

                        do
                        {
                            //bytes = handler.Receive(data);
                            //builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                            byte[] buffer = new byte[10000];
                            int byteRead = listenSocket.Accept().Receive(buffer);

                        }
                        while (handler.Available > 0);

                        AddMessageInLogsFromClient(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());

                        // отправляем ответ
                        string message = "Ваше сообщение доставлено";
                        data = Encoding.Unicode.GetBytes(message);
                        handler.Send(data);
                    }
                });
            }
            catch (Exception ex) { AddMessageInLogs(ex.Message); }
        }

        private void AddMessageInLogsFromClient(string Message) { ClientQuery.Document.Blocks.Add(new Paragraph(new Run("Клиент: " + Message))); }
        private void AddMessageInLogs(string Message) { ClientQuery.Document.Blocks.Add(new Paragraph(new Run(Message))); }

        private void OffServerButtonClick(object sender, RoutedEventArgs e) 
        {
            try
            {
                AddMessageInLogs("Сервер остановлен");
                listenSocket.Shutdown(SocketShutdown.Both);
                listenSocket.Close();
            }
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message, "Исключение"); 
                Close(); 
            }
        }
    }
}
