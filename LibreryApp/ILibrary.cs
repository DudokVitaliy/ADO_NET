using System.Collections.Generic;

namespace LibraryApp
{
    public interface ILibrary
    {
        void AddBook(string title, List<string> authors);
        int GetVisitorCount();
        List<string> GetDebtors();
        List<string> GetAuthorsByBook(string bookTitle);
        List<string> GetAvailableBooks();
        List<string> GetBooksByVisitor(string visitorName);
        void ClearAllDebts();
    }
}
