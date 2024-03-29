using Listem.API.Exceptions;
using Listem.Shared.Contracts;

namespace Listem.API.Domain.Lists;

internal class ListService(IListRepository listRepository) : IListService
{
    public async Task<List<ListResponse>> GetAllAsync()
    {
        var result = await listRepository.GetAllAsync();
        return result.Select(l => l.ToResponse()).ToList();
    }

    public async Task<ListResponse?> GetByIdAsync(string listId)
    {
        var result = await listRepository.GetByIdAsync(listId);
        return result is not null
            ? result.ToResponse()
            : throw new NotFoundException($"List {listId} not found");
    }

    public Task<bool> ExistsAsync(string listId)
    {
        return listRepository.ExistsAsync(listId);
    }

    public async Task<ListResponse?> CreateAsync(string userId, ListRequest listRequest)
    {
        var toCreate = List.From(listRequest, userId);
        var result = await listRepository.CreateAsync(toCreate);

        if (result is null)
        {
            throw new ConflictException("List cannot be created, it already exists");
        }

        return result.ToResponse();
    }

    public async Task<ListResponse?> UpdateAsync(string listId, ListRequest requested)
    {
        var existing = await listRepository.GetByIdAsync(listId);

        if (existing is null)
            throw new NotFoundException(
                $"Failed to update list {listId} because it does not exist"
            );

        existing.Update(requested);
        var result = await listRepository.UpdateAsync(existing);

        if (result is not null)
            return result.ToResponse();

        throw new SystemException($"Failed to update list {listId} even though it was found");
    }

    public async Task DeleteByIdAsync(string listId)
    {
        var hasBeenDeleted = await listRepository.DeleteByIdAsync(listId);
        if (!hasBeenDeleted)
        {
            throw new NotFoundException(
                $"Failed to delete list {listId} because it doesn't exist or user has no access to it"
            );
        }
    }
}
