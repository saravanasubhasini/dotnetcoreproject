using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonService
    {
       Task< PersonResponse> AddPerson(PersonAddRequest personAddRequest);

       Task< List<PersonResponse>> GetAllPersons();

        Task<PersonResponse>? GetPersonByPersonID(Guid? clientID);

        Task<List<PersonResponse>> GetFilteredPerson(string SearchBy, string? SearchName);

        Task<List<PersonResponse>>GetSortedPerson
            (List<PersonResponse> GetAllPerson, string SortBy, SortingOptions options);


       Task< PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

        Task<bool> DeletePerson(Guid? PersonID);
    }
}
