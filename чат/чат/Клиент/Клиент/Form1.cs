using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Клиент;

namespace Клиент
{
	public partial class Form1 : Form
	{
		private delegate void ChatEvent(string content, string clr);
		private Socket _serverSocket; // сокеты
		private Thread listenThread; // поток 
		private string _host = "192.168.43.224"; // IP
		private int _port = 8001; // порт
		private ChatEvent _addMessage; // добавление сообщений 
		public Form1()//вот это запускается первым делом. Тут в основном добавляю только методы и текст
		{
			InitializeComponent();

			_addMessage = new ChatEvent(AddMessage); //для добавление сообщения 

			textBox1.Text = "Истории сообщений еще нет...";
			textBox1.ForeColor = Color.Gray;

			textBox2.Text = "Нажмите на кнопку \"Отправить\", чтобы отправить сообщение...";
			textBox2.ForeColor = Color.Gray;
			textBox2.Enter += textBox2_Enter;//такие штуки, чтобы убирать серый текст
			textBox1.Leave += textBox2_Leave;//а такие, чтоб возвращать

			textBox3.Text = "Напишите ваше имя...";
			textBox3.ForeColor = Color.Gray;
			textBox3.Enter += textBox3_Enter;
			textBox1.Leave += textBox3_Leave;
		}
		private void AddMessage(string Content, string Color = "Black") //передача сообщений (и наших, и других пользователей - все проходит сначала через сервер) в чат
		{
			if (InvokeRequired)
			{
				Invoke(_addMessage, Content, Color); //инвок это штука для изменения штучек во время работы потоков
				return;
			}
			textBox1.SelectionStart = textBox1.TextLength;
			textBox1.SelectionLength = Content.Length;
			textBox1.AppendText(Content + Environment.NewLine);//добавляем сообщение в чат
		}

		/*private Color getColor(string text)
        {
           
            if (Color.Red.Name.Contains(text))
                return Color.Red;
            return Color.Black;

        }*/

		private void Form_Load(object sender, EventArgs e) //подключение юзера с сервером с помощью потоков - это тоже сразу запускается 
		{
			IPAddress temp = IPAddress.Parse(_host);//записываем айпишник
			_serverSocket = new Socket(temp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);//создаем сокета
			_serverSocket.Connect(new IPEndPoint(temp, _port));//подключаем сокета по этому порту и хосту
			if (_serverSocket.Connected)//если все ок
			{
				textBox3.Enabled = true;
				AddMessage("Связь с сервером установлена.");//ну это тоже конечно пишется там сереньким текстом, но пусть будет
				listenThread = new Thread(listner);//поток для получения сообщений типа
				listenThread.IsBackground = true;
				listenThread.Start();
			}
			else
				AddMessage("Связь с сервером не установлена.");

		}

		public void Send(string Buffer)//отправочка сообщений
		{
			try
			{
				_serverSocket.Send(Encoding.Unicode.GetBytes(Buffer));//передаем в сокет
			}
			catch { }
		}

		public void handleCommand(string cmd)//это для обычныз сообщений и системных, типа добро пожаловать и тд
		{
			string[] commands = cmd.Split('#');
			for (int i = 0; i < commands.Length; i++)
			{
				try
				{
					string currentCommand = commands[i];
					if (string.IsNullOrEmpty(currentCommand))
						continue;
					if (currentCommand.Contains("setnamesuccess"))
					{
						//из-за того что программа пыталась получить доступ к контролам из другого потока вылетал эксепщен и поля не разблокировались

						Invoke((MethodInvoker)delegate // переход из одного потока в другой
						{
							AddMessage($"Добро пожаловать, {textBox3.Text}");
							textBox1.Enabled = true;
							textBox2.Enabled = true;
							textBox3.Enabled = false;
						});
						continue;
					}
					if (currentCommand.Contains("setnamefailed")) // проверка на неверный ник
					{
						AddMessage("Неверный ник!");
						continue;
					}
					if (currentCommand.Contains("msg")) // сообщения
					{
						string[] Arguments = currentCommand.Split('|');
						AddMessage(Arguments[1], Arguments[2]);
						continue;
					}
				}
				catch (Exception exp) { Console.WriteLine("Error with handleCommand: " + exp.Message); } // если какой-то из пунктов не сработол, выведет сообщение об ошибке
			}
		}
		public void listner() // если отключим сервер, то все юзеры сразу отключатся
		{
			try
			{
				while (_serverSocket.Connected)//пока пользователь подключен
				{
					byte[] buffer = new byte[2048];
					int bytesReceive = _serverSocket.Receive(buffer);
					handleCommand(Encoding.Unicode.GetString(buffer, 0, bytesReceive));
				}
			}
			catch
			{
				MessageBox.Show("Связь с сервером прервана");
				Application.Exit();
			}
		}

		private void Form_FormClosing(object sender, FormClosingEventArgs e) // окончание работы 
		{
			if (_serverSocket.Connected)
				Send("#endsession");
		}

		////это для вывода текста в пустые формачки и чтоб пропадал, когда тыкаешь туда
		private void textBox2_Enter(object sender, EventArgs e)
		{
			if (textBox2.Text == "Нажмите на кнопку \"Отправить\", чтобы отправить сообщение...")
			{
				textBox2.Clear();
				textBox2.ForeColor = Color.Black;
			}
		}
		private void textBox3_Enter(object sender, EventArgs e)
		{
			if (textBox3.Text == "Напишите ваше имя...")
			{
				textBox3.Clear();
				textBox3.ForeColor = Color.Black;
			}
		}
		private void textBox2_Leave(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(textBox2.Text))
			{
				textBox2.Text = "Нажмите на кнопку \"Отправить\", чтобы отправить сообщение...";
				textBox2.ForeColor = Color.Gray;
			}
		}
		private void textBox3_Leave(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(textBox3.Text))
			{
				textBox3.Text = "Напишите ваше имя...";
				textBox3.ForeColor = Color.Gray;
			}
		}
		////это для изменения размеров формы и всего остального
		bool размер_огромный_обычный = false;//не огромный
		private void label1_Click(object sender, EventArgs e)//меняем размер
		{
			this.WindowState = FormWindowState.Minimized;
		}
		private void label2_Click(object sender, EventArgs e)//отвечает за размер окна
		{
			if (размер_огромный_обычный == false)
			{
				this.WindowState = FormWindowState.Maximized;
				textBox1.Location = new Point(60, 115);
				textBox2.Location = new Point(60, 1030);
				textBox3.Location = new Point(60, 70);
				textBox1.Size = new Size(1800, 900);
				textBox2.Size = new Size(1800, 30);
				textBox3.Size = new Size(1800, 30);
				button1.Location = new Point(825, 1090);
				button1.Size = new Size(272, 58);
				размер_огромный_обычный = true;
			}
			else
			{
				this.WindowState = FormWindowState.Normal;
				textBox1.Location = new Point(30, 80);
				textBox2.Location = new Point(30, 508);
				textBox3.Location = new Point(30, 47);
				textBox1.Size = new Size(830, 422);
				textBox2.Size = new Size(830, 30);
				textBox3.Size = new Size(830, 30);
				button1.Location = new Point(392, 553);
				button1.Size = new Size(136, 29);
				размер_огромный_обычный = false;
			}
		}
		private void label3_Click(object sender, EventArgs e)//закрываем все
		{
			this.Close();
		}
		////это для красивого цвета кнопочек, когда наводим мышку
		private void label1_MouseEnter(object sender, EventArgs e)
		{
			label1.ForeColor = Color.DeepPink;
		}
		private void label1_MouseLeave(object sender, EventArgs e)
		{
			label1.ForeColor = Color.White;
		}
		private void label2_MouseEnter(object sender, EventArgs e)
		{
			label2.ForeColor = Color.DeepPink;
		}
		private void label2_MouseLeave(object sender, EventArgs e)
		{
			label2.ForeColor = Color.White;
		}
		private void label3_MouseEnter(object sender, EventArgs e)
		{
			label3.ForeColor = Color.DeepPink;
		}
		private void label3_MouseLeave(object sender, EventArgs e)
		{
			label3.ForeColor = Color.White;
		}
		////это чтобы заставить форму двигаться по лкм
		Point lastPoint;
		private void Form1_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				this.Left += e.X - lastPoint.X;
				this.Top += e.Y - lastPoint.Y;
			}
		}
		private void Form1_MouseDown(object sender, MouseEventArgs e)
		{
			lastPoint = new Point(e.X, e.Y);
		}
		////собственно отправляем сообщение
		bool Подключился = false;
		private void button1_Click(object sender, EventArgs e)
		{

			if (textBox2.Text == "Нажмите на кнопку \"Отправить\", чтобы отправить сообщение..." || textBox2.Text.Trim() == "" || textBox3.Text == "Напишите ваше имя...")
			{
				MessageBox.Show("Не заполнены поля имени или сообщения");
			}
			else
			{
				if (!Подключился)
				{
					string nickName = textBox3.Text;
					if (string.IsNullOrEmpty(nickName))
						return;
					Send($"#setname|{nickName}");

					textBox3.ReadOnly = true;//меняем, чтобы нельзя было поменять свое имя
											 //Chat.user = textBox3.Text;
					textBox1.Text = "";
					textBox1.ForeColor = Color.Black;
					Подключился = true;
				}

				string msgData = textBox2.Text;
				Send($"#message|{msgData}"); // всем юзерам 
				textBox2.Text = "";
			}
		}


	}
}