using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ClientObject
    {
        public static TcpClient client = null;
        public static NetworkStream stream = null; 

        public ClientObject(TcpClient tcpClient) => client = tcpClient;

        public void Process() 
        {
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[64]; // буфер для получаемых данных
                while (true)
                {
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        //new MainWindow().AddMessageInLogs("[" + DateTime.Now.ToShortTimeString() + "]" + "Запрос клиента: " + builder.ToString());
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();

                    // отправляем обратно сообщение
                    message = Encoding.Unicode.GetString(data);
                    data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
