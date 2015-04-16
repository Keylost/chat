using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace srv
{
    struct data
    {
        public string message;
        public bool first;
    }
    class Program
    {
        const string ips = "127.0.0.1";//ip
        const int port = 11000; //порт
        static void Main(string[] args)
        {
            int max_cl = 10; //макс число клиентов в очереди
            data dt; dt.first = true; dt.message = null;
            // Устанавливаем для сокета локальную конечную точку
            IPAddress ipAddr = IPAddress.Parse(ips);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);//ip адрес и порт для прослушивания

            // Создаем сокет Tcp/Ip
            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(max_cl);

                // Начинаем слушать соединения
                while (true)
                {
                    Console.WriteLine("Ожидаем соединение через порт {0}", ipEndPoint);

                    // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();
                    
                    // Мы дождались клиента, пытающегося с нами соединиться
                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    if(dt.first==true)//если клиент был первым
                    {
                        dt.message += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                        // Показываем данные на консоли
                        Console.Write("Полученный текст: " + dt.message + "\n\n");
                        
                        // Отправляем ответ клиенту\
                        string reply = "сообщение получено";
                        byte[] msg = Encoding.UTF8.GetBytes(reply);
                        handler.Send(msg);
                        dt.first = false;
                    }
                    else //если клиент не первый
                    {
                        string dn = null;
                        dn += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                        // Показываем данные на консоли
                        Console.Write("Полученный текст: " + dt.message + "\n\n");
                        // Отправляем клиенту сообщение
                        byte[] msg = Encoding.UTF8.GetBytes(dt.message);
                        handler.Send(msg);
                    }

                    if (dt.message.IndexOf("<TheEnd>") > -1)
                    {
                        Console.WriteLine("Сервер завершил соединение с клиентом.");
                        break;
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
