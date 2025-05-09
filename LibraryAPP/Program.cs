using LibraryApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPP
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var libraryRepo = new LibraryRepository();

            Console.WriteLine("1. Додати нову книгу в бібліотеку");
            Console.WriteLine("2. Вивести кількість зареєстрованих користувачів");
            Console.WriteLine("3. Вивести список боржників");
            Console.WriteLine("4. Вивести список авторів певної книги");
            Console.WriteLine("5. Вивести список книг, які доступні в даний момент");
            Console.WriteLine("6. Вивести список книг, які на руках у певного користувача");
            Console.WriteLine("7. Очистити заборгованості всіх боржників");
            Console.Write("Виберіть опцію: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    libraryRepo.AddBook(new Book { Title = "Нова Книга", Available = 5 });
                    Console.WriteLine("Книга додана.");
                    break;
                case "2":
                    Console.WriteLine($"Кількість зареєстрованих користувачів: {libraryRepo.GetRegisteredUserCount()}");
                    break;
                case "3":
                    var debtors = libraryRepo.GetDebtors();
                    foreach (var debtor in debtors)
                    {
                        Console.WriteLine(debtor.UserName);
                    }
                    break;
                case "4":
                    Console.Write("Введіть назву книги: ");
                    var bookTitle = Console.ReadLine();
                    var authors = libraryRepo.GetAuthorsOfBook(bookTitle);
                    foreach (var author in authors)
                    {
                        Console.WriteLine(author.Name);
                    }
                    break;
                case "5":
                    var availableBooks = libraryRepo.GetAvailableBooks();
                    foreach (var book in availableBooks)
                    {
                        Console.WriteLine(book.Title);
                    }
                    break;
                case "6":
                    Console.Write("Введіть ім'я користувача: ");
                    var userName = Console.ReadLine();
                    var booksOnHand = libraryRepo.GetBooksOnHandOfUser(userName);
                    foreach (var book in booksOnHand)
                    {
                        Console.WriteLine(book.Title);
                    }
                    break;
                case "7":
                    libraryRepo.ClearDebts();
                    Console.WriteLine("Заборгованості очищені.");
                    break;
                default:
                    Console.WriteLine("Невірний вибір.");
                    break;
            }
        }
    }
}
