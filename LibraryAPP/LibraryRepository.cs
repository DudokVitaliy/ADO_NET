using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using System.Configuration;

namespace LibraryApp
{
    public class Book
    {
        public string Title { get; set; }
        public int Available { get; set; }
    }

    public class Author
    {
        public string Name { get; set; }
    }

    public class User
    {
        public string UserName { get; set; }
    }

    public interface ILibraryRepository
    {
        void AddBook(Book book);
        int GetRegisteredUserCount();
        List<User> GetDebtors();
        List<Author> GetAuthorsOfBook(string bookTitle);
        List<Book> GetAvailableBooks();
        List<Book> GetBooksOnHandOfUser(string userName);
        void ClearDebts();
    }

    public class LibraryRepository : ILibraryRepository
    {
        private readonly string connectionString;

        public LibraryRepository()
        {
            connectionString = ConfigurationManager.ConnectionStrings["LibraryDB"].ConnectionString;
        }

        public void AddBook(Book book)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string cmdText = @"INSERT INTO Books (Title, Available) VALUES (@title, @available)";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.Parameters.Add("@title", System.Data.SqlDbType.NVarChar).Value = book.Title;
                command.Parameters.Add("@available", System.Data.SqlDbType.Int).Value = book.Available;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public int GetRegisteredUserCount()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string cmdText = "SELECT COUNT(*) FROM Users";
                SqlCommand command = new SqlCommand(cmdText, connection);

                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }

        public List<User> GetDebtors()
        {
            List<User> debtors = new List<User>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string cmdText = @"SELECT u.UserName FROM Users u
                                    INNER JOIN Debtors d ON u.UserId = d.UserId
                                    WHERE d.IsDebtor = 1";
                SqlCommand command = new SqlCommand(cmdText, connection);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    debtors.Add(new User { UserName = reader["UserName"].ToString() });
                }
            }
            return debtors;
        }

        public List<Author> GetAuthorsOfBook(string bookTitle)
        {
            List<Author> authors = new List<Author>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string cmdText = @"SELECT a.Name FROM Authors a
                                    INNER JOIN BookAuthors ba ON a.AuthorId = ba.AuthorId
                                    INNER JOIN Books b ON ba.BookId = b.BookId
                                    WHERE b.Title = @bookTitle";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.Parameters.Add("@bookTitle", System.Data.SqlDbType.NVarChar).Value = bookTitle;

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    authors.Add(new Author { Name = reader["Name"].ToString() });
                }
            }
            return authors;
        }

        public List<Book> GetAvailableBooks()
        {
            List<Book> books = new List<Book>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string cmdText = "SELECT Title FROM Books WHERE Available > 0";
                SqlCommand command = new SqlCommand(cmdText, connection);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    books.Add(new Book { Title = reader["Title"].ToString() });
                }
            }
            return books;
        }

        public List<Book> GetBooksOnHandOfUser(string userName)
        {
            List<Book> books = new List<Book>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string cmdText = @"SELECT b.Title FROM Books b
                                    INNER JOIN Borrows br ON b.BookId = br.BookId
                                    INNER JOIN Users u ON br.UserId = u.UserId
                                    WHERE u.UserName = @userName AND br.ReturnDate IS NULL";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.Parameters.Add("@userName", System.Data.SqlDbType.NVarChar).Value = userName;

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    books.Add(new Book { Title = reader["Title"].ToString() });
                }
            }
            return books;
        }

        public void ClearDebts()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string cmdText = @"UPDATE Debtors SET IsDebtor = 0";
                SqlCommand command = new SqlCommand(cmdText, connection);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}