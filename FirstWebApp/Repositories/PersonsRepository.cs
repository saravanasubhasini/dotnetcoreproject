using System.Linq.Expressions;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly FirstAppDbContext _db;

        public PersonsRepository(FirstAppDbContext db)
        {
           _db = db;
            
        }

        public async Task<Person> AddPerson(Person person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();

            return(person);
        }

        public async Task<bool> DeletePerson(Guid personID)
        {
            _db.Persons.RemoveRange(_db.Persons.Where(temp => temp.PersonID == personID));
             int rowCount = await _db.SaveChangesAsync();

            return rowCount > 0;

        }

        public async Task<List<Person>> FilterPerson(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons.Include("Country").Where(predicate).ToListAsync();
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonID(Guid personID)
        {
            return await _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonID == personID);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? MatchingData = await  _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == person.PersonID);

            if (MatchingData == null)
                return person;

            MatchingData.PersonName = person.PersonName;
            MatchingData.Email = person.Email;
            MatchingData.DateOfBirth = person.DateOfBirth;
            MatchingData.CountryID = person.CountryID;
            MatchingData.Gender = person.Gender;
            MatchingData.Address = person.Address;
            MatchingData.RecieveNewsLetter = person.RecieveNewsLetter;

            return MatchingData;
        }
    }
}
