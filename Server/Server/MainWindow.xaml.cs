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
        static IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        static EndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowOnLoaded(object sender, RoutedEventArgs e) => Run();

        private void Run()
        {
            ClientQuery.Clear();

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

                        //do
                        //{
                            bytes = handler.ReceiveFrom(data, ref remotePoint);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        //}
                        //while (handler.Available > 0);

                        AddMessageInLogsFromClient(builder.ToString());

                        // отправляем ответ
                        string message = "Ваше сообщение доставлено";
                        data = Encoding.Unicode.GetBytes(message);
                        handler.SendTo(data, ipPoint);
                    }
                });
            }
            catch (Exception ex) { AddMessageInLogs(ex.Message); }
        }

        private void AddMessageInLogsFromClient(string Message) => Application.Current.Dispatcher.Invoke(() => { ClientQuery.Text += "[" + DateTime.Now.ToShortTimeString() + "]" + "Клиент: " + Message + "\n"; });
        private void AddMessageInLogs(string Message) => Application.Current.Dispatcher.Invoke(() => { ClientQuery.Text += Message + "\n"; });

        private void OffServerButtonClick(object sender, RoutedEventArgs e) 
        {
            try
            {
                AddMessageInLogs("Сервер остановлен");
                //listenSocket.Shutdown(SocketShutdown.Both);
                listenSocket.Close();
                Close();
            }
            catch (Exception ex) { }
        }
    }
}
