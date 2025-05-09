using System;
using System.Collections.Generic;
using LibraryApp;

class Program
{
    static void Main(string[] args)
    {
        ILibrary repo = new LibraryRepository();

        while (true)
        {
            Console.WriteLine("\n--- MENU ---");
            Console.WriteLine("1. Add a new book");
            Console.WriteLine("2. Show number of registered visitors");
            Console.WriteLine("3. List of visitors with debts");
            Console.WriteLine("4. Show authors of a book");
            Console.WriteLine("5. Show available books");
            Console.WriteLine("6. Show books borrowed by a visitor");
            Console.WriteLine("7. Clear all debts");
            Console.WriteLine("0. Exit");
            Console.Write("Choose option: ");
            string input = Console.ReadLine();

            try
            {
                switch (input)
                {
                    case "1":
                        Console.Write("Book title: ");
                        string title = Console.ReadLine();
                        List<string> authors = new List<string>();
                        while (true)
                        {
                            Console.Write("Author (leave empty to finish): ");
                            string author = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(author)) break;
                            authors.Add(author);
                        }
                        repo.AddBook(title, authors);
                        Console.WriteLine("Book added.");
                        break;

                    case "2":
                        Console.WriteLine($"Total visitors: {repo.GetVisitorCount()}");
                        break;

                    case "3":
                        Console.WriteLine("Visitors with debts:");
                        repo.GetDebtors().ForEach(Console.WriteLine);
                        break;

                    case "4":
                        Console.Write("Book title: ");
                        repo.GetAuthorsByBook(Console.ReadLine()).ForEach(Console.WriteLine);
                        break;

                    case "5":
                        Console.WriteLine("Available books:");
                        repo.GetAvailableBooks().ForEach(Console.WriteLine);
                        break;

                    case "6":
                        Console.Write("Visitor name: ");
                        repo.GetBooksByVisitor(Console.ReadLine()).ForEach(Console.WriteLine);
                        break;

                    case "7":
                        repo.ClearAllDebts();
                        Console.WriteLine("All debts cleared.");
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
