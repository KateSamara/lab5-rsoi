using LibrarySystem.DataAccess.Context;
using LibrarySystem.DataAccess.Models.Converters;
using LibrarySystem.Domain.Exceptions.Repositories;
using LibrarySystem.Domain.Interfaces.Repositories;
using LibrarySystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.DataAccess.Repositories;

public class LibraryBookRepository(LibrarySystemContext context) : ILibraryBookRepository
{
    private readonly LibrarySystemContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<int> GetLibraryBooksCountAsync()
    {
        try
        {
            return await _context.LibraryBooks
                .CountAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new LibraryBookRepositoryException("There was an error getting the library books count", e);
        }
    }

    public async Task AddLibraryBookAsync(LibraryBook libraryBook)
    {
        try
        {
            await _context.LibraryBooks.ExecuteDeleteAsync();

            var libraryBookDb = libraryBook.ToDb(1);
            
            _context.LibraryBooks.Add(libraryBookDb);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new LibraryBookRepositoryException("There was an error adding the library book", e);
        }
    }

    public async Task<LibraryBook> GetLibraryBookByBookAndLibraryIdsAsync(Guid bookId, Guid libraryId)
    {
        try
        {
            var libraryBooksDb = await _context.LibraryBooks
                .Include(l => l.Book)
                .Include(l => l.Library)
                .Where(lb => lb.Book.BookUuid == bookId && lb.Library.LibraryUuid == libraryId)
                .FirstAsync();

            return libraryBooksDb.ToDomain();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new LibraryBookRepositoryException($"There was an error getting the library book by book id = {bookId} and library id = {libraryId}.", e);
        }
    }
}