    using System; // подключаем пространство имен System (стандартная библиотека C#)
    using System.IO; // подключаем пространство имен System.IO (работа с файлами и потоками данных)
    using System.Net; // подключаем пространство имен System.Net (работа с сетью)
    using System.Net.Sockets; // подключаем пространство имен System.Net.Sockets (работа с сетевыми сокетами)
    using System.Text; // подключаем пространство имен System.Text (работа с кодировками и строками)
    using System.Threading.Tasks; // подключаем пространство имен System.Threading.Tasks (работа с асинхронными задачами)

    class Program // объявляем класс Program
    {
    static TcpListener server; // объявляем статическое поле server типа TcpListener
    static StreamWriter fileWriter; // объявляем статическое поле fileWriter типа StreamWriter


    static async Task Main(string[] args) // объявляем асинхронный метод Main
    {
        server = new TcpListener(IPAddress.Any, 12345); // инициализируем объект server как TcpListener, прослушивающий все IP-адреса на порту 12345
        server.Start(); // запускаем сервер
        fileWriter = new StreamWriter("chat_history.txt"); // создаем объект fileWriter для записи в файл "chat_history.txt"

        Console.WriteLine("Сервер запущен. Ожидание подключений..."); // выводим сообщение в консоль

        while (true) // бесконечный цикл
        {
            TcpClient client = await server.AcceptTcpClientAsync(); // принимаем новое подключение от клиента и сохраняем его в переменной client
            ProcessClient(client); // вызываем метод ProcessClient для обработки подключенного клиента
        }
    }

    static async Task ProcessClient(TcpClient client) // объявляем асинхронный метод ProcessClient, принимающий объект TcpClient
    {
        NetworkStream stream = client.GetStream(); // получаем сетевой поток для обмена данными с клиентом
        Console.WriteLine("Новый клиент подключен."); // выводим сообщение о подключении клиента в консоль

        while (true) // бесконечный цикл для работы с клиентом
        {
            try // обработка исключений
            {
                byte[] data = new byte[1024]; // создаем буфер для приема данных от клиента
                int bytesRead = await stream.ReadAsync(data, 0, data.Length); // асинхронно считываем данные из сетевого потока
                string message = Encoding.UTF8.GetString(data, 0, bytesRead); // декодируем полученные данные в строку

                Console.WriteLine("Новое сообщение от клиента: " + message); // выводим сообщение от клиента в консоль

                byte[] responseData = Encoding.UTF8.GetBytes(message); // кодируем ответное сообщение в байты
                await stream.WriteAsync(responseData, 0, responseData.Length); // асинхронно отправляем ответное сообщение клиенту

                string history = $"IP: {((IPEndPoint)client.Client.RemoteEndPoint).Address}, Message: {message}"; // формируем строку для записи в историю чата
                Console.WriteLine(history); // выводим историю чата в консоль

                fileWriter.WriteLine(history); // записываем историю чата в файл
                fileWriter.Flush(); // сбрасываем буфер записи в файл
            }
            catch (Exception ex) // обработка исключений
            {
                Console.WriteLine("Ошибка при чтении/отправке сообщения: " + ex.Message); // выводим сообщение об ошибке в консоль
                break; // выходим из цикла
            }
        }
    }
    }





