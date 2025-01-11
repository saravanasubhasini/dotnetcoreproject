using System.Linq.Expressions;
using Entities;
namespace RepositoryContracts
{
    public interface IPersonsRepository
    {

        Task<Person> AddPerson(Person person);

        Task<List<Person>> GetAllPersons();

        Task<Person?> GetPersonByPersonID(Guid personID);

        Task<List<Person>> FilterPerson(Expression<Func<Person, bool>> predicate);

        Task<Person> UpdatePerson(Person person);

        Task<bool> DeletePerson(Guid personID);

    }
}
