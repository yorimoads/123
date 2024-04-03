using System; // подключаем пространство имен System (стандартная библиотека C#)
using System.IO; // подключаем пространство имен System.IO (работа с файлами и потоками данных)
using System.Net; // подключаем пространство имен System.Net (работа с сетью)
using System.Net.Sockets; // подключаем пространство имен System.Net.Sockets (работа с сетевыми сокетами)
using System.Text; // подключаем пространство имен System.Text (работа с кодировками и строками)
using System.Windows.Forms; //подключение пространства имен System.Windows.Forms

namespace _123123Client //создаем пространство имен
{
    public partial class Form1 : Form // объявляем класс формы 
    {
        private TcpClient client; //Объявление приватного поля client типа TcpClient для установления TCP-соединения с удаленным сервером
        private NetworkStream stream; //Объявление приватного поля stream типа NetworkStream для обмена данными по сети
        private string localIp; //Объявление приватного поля localIp для хранения локального IP-адреса
        private string remoteIp; //Объявление приватного поля remoteIp для хранения IP-адреса удаленного сервера

        public Form1() //Определение конструктора класса Form1
        {
            InitializeComponent(); //Вызов метода InitializeComponent() для инициализации компонентов формы
            localIp = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString(); // Получение локального IP-адреса хоста и сохранение его в переменной localIp
        }

        private void ConnectToServer()//Объявление метода ConnectToServer
        {
            client = new TcpClient(remoteIp, 12345);//Создание нового объекта TcpClient для подключения к серверу с IP-адресом remoteIp и портом 12345
            stream = client.GetStream();// Получение сетевого потока для обмена данными с сервером

            var receiveThread = new Thread(ReceiveMessages); //Создание нового потока для асинхронного приема сообщений от сервера.
            receiveThread.Start();//Запуск созданного потока для выполнения метода ReceiveMessages
        }

        private void ReceiveMessages()//Объявление метода ReceiveMessages для приема сообщений от сервера
        {
            while (true)
            {
                byte[] data = new byte[1024]; //Создание буфера для данных размером 1024 байта
                int bytesRead = stream.Read(data, 0, data.Length);//Чтение данных из сетевого потока в буфер data и сохранение количества считанных байтов в переменную bytesRead
                string message = Encoding.UTF8.GetString(data, 0, bytesRead);//Декодирование считанных байтов в строку с использованием кодировки UTF-8

                Invoke((MethodInvoker)delegate//Вызов делегата для обновления графического интерфейса из потока приема сообщений
                {
                    textBox2.Text += $"{remoteIp}: " + message + Environment.NewLine;//Добавление полученного сообщения в текстовое поле textBox2 с указанием IP-адреса отправителя
                });
            }
        }

        private void SendMessage() //Объявление метода SendMessage для отправки сообщений
        {
            string message = textBox1.Text;//Получение текста сообщения из текстового поля textBox1

            byte[] data = Encoding.UTF8.GetBytes(message);//Кодирование текста сообщения в байты с использованием кодировки UTF-8
            stream.Write(data, 0, data.Length); //Отправка закодированных данных в сетевой поток

            textBox2.Text += $"{localIp}: {message}{Environment.NewLine}";//Добавление отправленного сообщения в текстовое поле textBox2 с указанием локального IP-адреса
        }

        private void button1_Click(object sender, EventArgs e) //Обработчик нажатия кнопки 1
        {
            remoteIp = textBox3.Text; //Получение IP-адреса сервера из текстового поля textBox3
            ConnectToServer(); //Вызов метода ConnectToServer для подключения к серверу
        }

        private void button2_Click(object sender, EventArgs e) //Обработчик нажатия кнопки 2
        {
            SendMessage(); //Вызов метода SendMessage для отправки сообщения
        }
    }
}