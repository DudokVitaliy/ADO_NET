using System;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var manager = new LibraryManager();
        while (true)
        {
            Console.WriteLine("\n1. Додати книгу\n2. К-ть відвідувачів\n3. Список боржників\n4. Автори книги\n5. Доступні книги\n6. Книги відвідувача\n7. Очистити борги\n0. Вийти");
            Console.Write("Ваш вибір: ");
            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("Назва книги: ");
                    string title = Console.ReadLine();
                    Console.Write("Автори (через кому): ");
                    string[] authors = Console.ReadLine().Split(',');
                    manager.AddBook(title.Trim(), Array.ConvertAll(authors, s => s.Trim()));
                    break;
                case "2": manager.CountVisitors(); break;
                case "3": manager.ListDebtors(); break;
                case "4":
                    Console.Write("Назва книги: ");
                    manager.ListAuthorsByBook(Console.ReadLine().Trim());
                    break;
                case "5": manager.ListAvailableBooks(); break;
                case "6":
                    Console.Write("Імʼя відвідувача: ");
                    manager.ListBooksOfUser(Console.ReadLine().Trim());
                    break;
                case "7": manager.ClearAllDebts(); break;
                case "0": return;
                default: Console.WriteLine("Невірний вибір."); break;
            }
        }
    }
}
