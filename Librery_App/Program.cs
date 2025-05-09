using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librery_App
{
    public interface ILibraryRepository
    {
        void AddBook(string title, List<string> authors);
        int GetRegisteredUsersCount();
        List<string> GetDebtors();
        List<string> GetAuthorsByBook(string title);
        List<string> GetAvailableBooks();
        List<string> GetBooksByVisitor(string visitorName);
        void ClearAllDebts();
    }

    internal class Program
    {
        static void Main()
        {
            ILibraryRepository repo = new LibraryRepository();

            while (true)
            {
                Console.WriteLine("\n1. Додати книгу\n2. К-сть користувачів\n3. Список боржників\n4. Автори книги\n5. Доступні книги\n6. Книги користувача\n7. Очистити борги\n0. Вихід");
                Console.Write("Оберіть опцію: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Назва книги: ");
                        string title = Console.ReadLine();
                        Console.Write("Автори (через кому): ");
                        var authors = new List<string>(Console.ReadLine().Split(','));
                        repo.AddBook(title, authors);
                        Console.WriteLine("Книгу додано.");
                        break;
                    case "2":
                        Console.WriteLine($"Кількість користувачів: {repo.GetRegisteredUsersCount()}");
                        break;
                    case "3":
                        var debtors = repo.GetDebtors();
                        Console.WriteLine("Боржники:");
                        debtors.ForEach(Console.WriteLine);
                        break;
                    case "4":
                        Console.Write("Назва книги: ");
                        var authorsByBook = repo.GetAuthorsByBook(Console.ReadLine());
                        Console.WriteLine("Автори:");
                        authorsByBook.ForEach(Console.WriteLine);
                        break;
                    case "5":
                        var books = repo.GetAvailableBooks();
                        Console.WriteLine("Доступні книги:");
                        books.ForEach(Console.WriteLine);
                        break;
                    case "6":
                        Console.Write("Ім'я користувача: ");
                        var booksByUser = repo.GetBooksByVisitor(Console.ReadLine());
                        Console.WriteLine("Книги на руках:");
                        booksByUser.ForEach(Console.WriteLine);
                        break;
                    case "7":
                        repo.ClearAllDebts();
                        Console.WriteLine("Борги очищено.");
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Невідома опція.");
                        break;
                }
            }
        }
    }

}
