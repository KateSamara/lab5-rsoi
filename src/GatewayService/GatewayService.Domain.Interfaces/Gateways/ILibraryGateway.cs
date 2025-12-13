using GatewayService.Domain.Models.Books;
using GatewayService.Domain.Models.Libraries;

namespace GatewayService.Domain.Interfaces.Gateways;

public interface ILibraryGateway
{
    public Task<LibraryPaged> GetLibrariesByCityPagedAsync(int page, int size, string city, string accessToken);

    public Task<BookPaged> GetBooksPagedByLibraryUuid(Guid libraryUid, int page, int size, bool showAll, string accessToken);

    public Task<LibraryBook> UpdateAvailableBooksCount(Guid bookUuid, Guid libraryUuid, bool isIncrease, string accessToken);

    public Task<List<Book>> GetBooksByIdsAsync(List<Guid> bookUuids, string accessToken);
    
    public Task<List<Library>> GetLibrariesByIdsAsync(List<Guid> libraryUuids, string accessToken);
}