using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helper;
using RepositoryContracts;

namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonsRepository _personRepository;

        

        public PersonService(IPersonsRepository personRepository)
        {
            _personRepository = personRepository;
            
            

        }

        
        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
                throw new ArgumentNullException();

            //validation

            ValidationHelper.ModelValidation(personAddRequest);

            //Converting Person Add Request to Person

            Person person = personAddRequest.ToPerson();

            person.PersonID = Guid.NewGuid();

            await _personRepository.AddPerson(person);

            //_db.Persons.Add(person);
            //await _db.SaveChangesAsync();

            //_db.sp_InsertPerson(person);

            return person .ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            // return _db.sp_GetAllPersons().ToList().Select(temp => ConvertingPersonToPersonResponse(temp)).ToList();
            //return await _db.Persons.Include("Country").Select(temp => (temp).ToPersonResponse()).ToListAsync();
            var person = await _personRepository.GetAllPersons();
            return person.Select(temp => (temp).ToPersonResponse()).ToList();
        }

        public async Task<PersonResponse>? GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
                throw new ArgumentNullException();
            Person? person = await _personRepository.GetPersonByPersonID(personID.Value);

            if (person == null)
                throw new InvalidOperationException();

            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPerson(string SearchBy, string? SearchName)
        {
            List<Person> persons = SearchBy switch
            {
                nameof(PersonResponse.PersonName) =>
                await _personRepository.FilterPerson(temp => temp.PersonName != null && temp.PersonName.Contains(SearchName ?? string.Empty)),


                nameof(PersonResponse.Email) =>
                  await _personRepository.FilterPerson(temp => temp.Email != null && temp.Email.Contains(SearchName ?? string.Empty)),

                nameof(PersonResponse.DateOfBirth) =>
               await _personRepository.FilterPerson(temp => temp.DateOfBirth.HasValue && temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(SearchName ?? string.Empty)),

                nameof(PersonResponse.Gender) =>
                await _personRepository.FilterPerson(temp => temp.Gender != null && temp.Gender.Contains(SearchName ?? string.Empty)),

                nameof(PersonResponse.CountryID) =>
                await _personRepository.FilterPerson(temp => temp.Country != null && temp.Country.CountryName != null && temp.Country.CountryName.Contains(SearchName ?? string.Empty)),

                nameof(PersonResponse.Address) =>
                await _personRepository.FilterPerson(temp => temp.Address != null && temp.Address.Contains(SearchName ?? string.Empty)),

                _ => await _personRepository.GetAllPersons()
            };
            return persons.Select(temp => temp.ToPersonResponse()).ToList();
        }

        public async Task<List<PersonResponse>> GetSortedPerson(List<PersonResponse> GetAllPerson, string SortBy, SortingOptions options)
        {
            if (string.IsNullOrEmpty(SortBy))
                return   GetAllPerson;

            List<PersonResponse> SortedPerson = (SortBy, options)
            switch
            {
                (nameof(PersonResponse.PersonName), SortingOptions.ASC) =>

                GetAllPerson.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortingOptions.DESC) =>

                GetAllPerson.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortingOptions.ASC) =>

                GetAllPerson.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortingOptions.DESC) =>

                GetAllPerson.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortingOptions.ASC) =>

                GetAllPerson.OrderBy(temp => temp.DateOfBirth).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortingOptions.DESC) =>

                GetAllPerson.OrderByDescending(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortingOptions.ASC) =>

               GetAllPerson.OrderBy(temp => temp.Age).ToList(),
                (nameof(PersonResponse.Age), SortingOptions.DESC) =>

                GetAllPerson.OrderByDescending(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Gender), SortingOptions.ASC) =>

                GetAllPerson.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Gender), SortingOptions.DESC) =>

                GetAllPerson.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.CountryName), SortingOptions.ASC) =>

               GetAllPerson.OrderBy(temp => temp.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.CountryName), SortingOptions.DESC) =>

                GetAllPerson.OrderByDescending(temp => temp.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),

                _ => GetAllPerson
            };
            await Task.Delay(50);
            return SortedPerson;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));
            }

            ValidationHelper.ModelValidation(personUpdateRequest);

            Person? MatchingPerson =await _personRepository.GetPersonByPersonID(personUpdateRequest.PersonID);

            if (MatchingPerson == null)
            {
                throw new ArgumentException("given Id is Invalid");

            }

            MatchingPerson.PersonName = personUpdateRequest.PersonName;
            MatchingPerson.Email = personUpdateRequest.Email;
            MatchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            MatchingPerson.Gender = personUpdateRequest.Gender.ToString();
            MatchingPerson.CountryID = personUpdateRequest.CountryID;
            MatchingPerson.Address = personUpdateRequest.Address;
            MatchingPerson.RecieveNewsLetter = personUpdateRequest.RecieveNewsLetter;

            await _personRepository.UpdatePerson(MatchingPerson);

            return MatchingPerson.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? PersonID)
        {
            if (PersonID == null)
                return  false;

            Person? person = await _personRepository.GetPersonByPersonID(PersonID.Value);

            if (person == null)
                return false;

           await _personRepository.DeletePerson( PersonID.Value);
             
            return true;
        }
    }
}
