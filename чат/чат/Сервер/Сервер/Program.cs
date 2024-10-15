using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public static class Server
    {
        //инфа о файлах и юзерах    
        public static int CountUsers = 0; //сколько юзеров
        public delegate void UserEvent(string Name);
        //инфа о сервере
        public static List<User> UserList = new List<User>(); // список юзеров
        public static Socket ServerSocket; // сервер сокетов
        public const string Host = "192.168.43.224"; //прописываем ip-шник
        public const int Port = 8001;//прописываем порт
        public static bool Work = true; // логическая функция отчечающая за работу сервера которая идет в ваил 

        public static event UserEvent UserConnected = (Username) =>//подключене юзера
        {
            Console.WriteLine($"User {Username} connected."); // оповещение о подключении юзера на сервер
            CountUsers++; // после добовления юзира в счетчик добовляем +1
            SendGlobalMessage($"Пользователь {Username} подключился к чату.", "Black"); // отправляет всем юзерам находящимся в чате, что подключился новый юзер
            SendUserList(); //выдение списка появившихся юзров

        };

        public static event UserEvent UserDisconnected = (Username) =>//отключение юзера из чата, работает точно также
        {
            Console.WriteLine($"User {Username} disconnected."); // отключился юзе 
            CountUsers--; // уменьшился счетчик
            SendGlobalMessage($"Пользователь {Username} отключился от чата.", "Black"); // прошло оповещение
            SendUserList(); // вывелся список оставщихся юзеров
        };


        public static void NewUser(User usr)//конектим юзера
        {
            if (UserList.Contains(usr))//если появился новый юзер
                return;
            UserList.Add(usr);//добавляем в список юзеров
            UserConnected(usr.Username);//добавляем к законекченым юзирам
        }
        public static void EndUser(User usr)//дисконектим юзера
        {
            if (!UserList.Contains(usr))//если исчез юзер
                return;
            UserList.Remove(usr);//убираем его из списка бзеров
            usr.End(); // заканчиваем работу исчезнувшего юзира
            UserDisconnected(usr.Username); // и отключаем его от сервера

        }


        public static void SendUserList()//юзера в список добавим
        {
            string userList = "#userlist|";

            for (int i = 0; i < CountUsers; i++)
            {
                userList += UserList[i].Username + ",";
            }

            SendAllUsers(userList);//покажем его всем юзерам
        }
        public static void SendGlobalMessage(string content, string clr)//сохранение в список текста сообщения пользователя (с языковыми параметрами)
        {
            for (int i = 0; i < CountUsers; i++)
            {
                UserList[i].SendMessage(content, clr);
            }
        }

        public static void SendAllUsers(string data)//сохранение в список параметр пользователя
        {
            for (int i = 0; i < CountUsers; i++)
            {
                UserList[i].Send(data);
            }
        }


    }
    public class User
    {
        private Thread _userThread; // подключение потоков
        private string _userName; // имя юзера
        private bool AuthSuccess = false; // аунтефикация для проверки
        public string Username//преобразования имени
        {
            get { return _userName; }
        }
        private Socket _userHandle;
        public User(Socket handle)//потоки
        {
            _userHandle = handle;
            _userThread = new Thread(listner);
            _userThread.IsBackground = true; // работает фоновый поток
            _userThread.Start();
        }
        private void listner()//подключение пользователя
        {
            try//защита от вылета в случае какой-либо проблемы
            {
                while (_userHandle.Connected)//попытка подключения
                {
                    byte[] buffer = new byte[2048];//выделение памяти под пользователя
                    int bytesReceive = _userHandle.Receive(buffer);//выделение места в параметре под код пользователя
                    handleCommand(Encoding.Unicode.GetString(buffer, 0, bytesReceive));//перевод в строковый тип
                }
            }
            catch { Server.EndUser(this); }//выход с cлучае неудачного подключения 
        }
        private bool setName(string Name)//имя 
        {

            _userName = Name; // прописываем имя 
            Server.NewUser(this);//сохранение имени
            AuthSuccess = true; // аунтифицируем
            return true;
        }
        private void handleCommand(string cmd)
        {
            try//защита от вылета в случае какой-либо проблемы
            {

                string[] commands = cmd.Split('#'); // команда разделитель              
                for (int i = 0; i < commands.Length; i++) // проход по списку команд
                {
                    string currentCommand = commands[i];
                    if (string.IsNullOrEmpty(currentCommand))//проверка на пустоту
                        continue;
                    /*Вход (или ввод имени) пользователя*/
                    if (!AuthSuccess)
                    {
                        if (currentCommand.Contains("setname"))//проверка на соответствие (выполнение команды по введнному в чат тексту (или нажатую кнопку с соотвествующим текстом))
                        {
                            if (setName(currentCommand.Split('|')[1]))//вывод удачного/неудачного ввода
                                Send("#setnamesuccess");
                            else
                                Send("#setnamefailed");
                        }
                        continue;
                    }

                    /*Отправка сообщения*/
                    if (currentCommand.Contains("message"))//проверка на соответствие (выполнение команды по введнному в чат тексту (или нажатую кнопку с соотвествующим текстом))
                    {
                        string[] Arguments = currentCommand.Split('|');
                        Server.SendGlobalMessage($"[{_userName}]: {Arguments[1]}", "Black");//вывод параметров пользователя при отправке

                        continue;
                    }

                    /*Выход из программы*/
                    if (currentCommand.Contains("endsession"))//проверка на соответствие (выполнение команды по введнному в чат тексту (или нажатую кнопку с соотвествующим текстом))
                    {
                        Server.EndUser(this);//выключение программы в случае соответствия 
                        return;
                    }

                }

            }
            catch (Exception exp) { Console.WriteLine("Error with handleCommand: " + exp.Message); }//в случае проблемы вывод ошибки и ее текста
        }
        public void SendMessage(string content, string clr)//отправка сообщения
        {
            Send($"#msg|{content}|{clr}");//вывод сообщения (с языковыми параметрами)
        }
        public void Send(string Buffer)//проеборазование кода сообщения в текст
        {
            try//защита от вылета в случае какой-либо проблемы
            {
                _userHandle.Send(Encoding.Unicode.GetBytes(Buffer));
            }
            catch { }
        }
        public void End()//закрытие программы
        {
            try//защита от вылета в случае какой-либо проблемы
            {
                _userHandle.Close();
            }
            catch { }

        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress address = IPAddress.Parse(Server.Host);//преобразуем строку ip-адрес в экземляр класса
            Server.ServerSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);// создание объекта сокета 
            Server.ServerSocket.Bind(new IPEndPoint(address, Server.Port));//связываем сокет с локальной конечной точкой
            Server.ServerSocket.Listen(100);//прослушиваем скоеты для принятие юзеров
            Console.WriteLine($"Server has been started on {Server.Host}:{Server.Port}");// вывод о работе сервера по его Ip и порту
            Console.WriteLine("Waiting connections...");//вывод о подключениях
            while (Server.Work)//пока сервер работает выполняем действия по булевой функции
            {
                Socket handle = Server.ServerSocket.Accept();//ожидание подключение клиета
                Console.WriteLine($"New connection: {handle.RemoteEndPoint.ToString()}"); //законектился юзер
                new User(handle); // добовляем в список юзеров

            }
            Console.WriteLine("Server off...");//отключение сервера

        }
    }
}
