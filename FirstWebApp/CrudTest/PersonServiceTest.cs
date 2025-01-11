﻿using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using AutoFixture.Kernel;
using System.Linq.Expressions;

namespace CrudTest
{
    public class PersonServiceTest
    {
        public readonly IPersonService _personService;

        public readonly ICountriesService _countriesService;

        public readonly ITestOutputHelper _outputHelper;

        private readonly IFixture _fixer;


        private readonly IPersonsRepository _personRepository;

        private readonly Mock<IPersonsRepository> _personsRepositoryMock;

        public PersonServiceTest(ITestOutputHelper outputHelper)
        {
            _fixer = new Fixture();

            _personsRepositoryMock = new Mock<IPersonsRepository>();

            _personRepository = _personsRepositoryMock.Object;
            
            var InitialCountriesList = new List<Country>() { };

            var InitialPersonsList = new List<Person>() { };

            DbContextMock<FirstAppDbContext> dbContextMock = new DbContextMock<FirstAppDbContext>(
                new DbContextOptionsBuilder<FirstAppDbContext>().Options
                );


            FirstAppDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, InitialCountriesList);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, InitialPersonsList);


            _countriesService = new CountriesService(null);
            _personService = new PersonService(_personRepository);

           

            _outputHelper = outputHelper;
        }

        #region AddPerson

        [Fact]
        public async Task AddPerson_IsNull()
        {
            PersonAddRequest request = null;

            Func<Task> action = async () =>
            {
                await _personService.AddPerson(request);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
            //await Assert.ThrowsAsync<ArgumentNullException>( async () =>
            //{
            //    await _personService.AddPerson(request);
            //});

        }
        [Fact]
        public async Task AddPerson_PersonNameNull_ToBeArgumrntException()
        {
           
            PersonAddRequest request = _fixer.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Email, "sample@gmail.com").Create();

            Person person = request.ToPerson();

            _outputHelper.WriteLine("Request :" + request.ToString());

            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            Func<Task> action = async () =>
            {
                await _personService.AddPerson(request);
            };

           await action.Should().ThrowAsync<ArgumentException>();

        }

        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            PersonAddRequest? request = _fixer.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample@gmail.com").Create();


            Person person = request.ToPerson();
            PersonResponse personResposne = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            

            PersonResponse ResponseFromAddPerson = await _personService.AddPerson(request);


            personResposne.PersonID = ResponseFromAddPerson.PersonID;
            //Assert.True(ResponseFromAddPerson.PersonID != Guid.Empty);

            ResponseFromAddPerson.PersonID.Should().NotBe(Guid.Empty);

            //Assert.Contains(ResponseFromAddPerson, ResponseFromGetAllPerson);
            ResponseFromAddPerson.Should().Be(personResposne);

        }


        #endregion

        #region GetPersonByPersonID
        [Fact]
        public async Task GetPersonByPersonID_IsNull()
        {
            Guid? PersonID = null;

            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    await _personService.GetPersonByPersonID(PersonID);
            //});

            Func<Task> action = async () =>
            {
                await _personService.GetPersonByPersonID(PersonID);

            };
            await action.Should().ThrowAsync<ArgumentNullException>();



        }


        [Fact]

        public async Task  GetPersonByPersonID_WithPersonID_ToBeSuccess()
        {


            Person person = _fixer.Build<Person>()
               .With(temp => temp.Country,null as Country)
                .With(temp => temp.Email, "sample@gmail.com").Create();

            //PersonResponse ResponseFromAddPerson =
            //   await _personService.AddPerson(request);  
            PersonResponse personResponse_Excepted = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            
            PersonResponse ResPonseFromGetPerson =
               await _personService.GetPersonByPersonID(person.PersonID);

            //Assert.Equal(ResponseFromAddPerson, ResPonseFromGetPerson);

            ResPonseFromGetPerson.Should().Be(personResponse_Excepted);
        }
        #endregion

        #region GetAllPerson
        [Fact]
        public async Task GetAllPerson_EmptyList()
        {
            var persons = new List<Person>();

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            List<PersonResponse> personListFromGet = await _personService.GetAllPersons();

            //Assert.Empty(responses);
            personListFromGet.Should().BeEmpty();

        }
        [Fact]
        public async Task GetAllPerson_GetFewPerson()
        {


            List<Person> persons = new List<Person>()
            {
            _fixer.Build<Person>()
            .With(temp=>temp.Email,"sample1@gmail.com")
            .With(temp=>temp.Country,null as Country)
            .Create(),

            _fixer.Build<Person>()
            .With(temp=>temp.Email,"sample2@gmail.com")
            .With(temp=>temp.Country,null as Country)
            .Create(),

            _fixer.Build<Person>()
            .With(temp=>temp.Email,"sample3@gmail.com")
            .With(temp=>temp.Country,null as Country)
            .Create()
            };

            List<PersonResponse> PersonResponseExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            List<PersonResponse> PersonResponseGet =await
                _personService.GetAllPersons();

            //foreach (PersonResponse personFromAdd in PersonToAdd)
            //{
            //    Assert.Contains(personFromAdd, PersonFromResponse);
            //}

            PersonResponseGet.Should().BeEquivalentTo(PersonResponseExpected);


        }
        #endregion

        #region Filter

        [Fact]

        public async Task GetFilterName_EmptyPerson()
        {
            

            List<Person> persons =
                new List<Person>()
                {
                _fixer.Build<Person>().With(temp=>temp.Email,"sample1@gmail.com")
                .With(temp=>temp.Country,null as Country).Create(),

                _fixer.Build<Person>().With(temp=>temp.Email,"sample2@gmail.com")
                .With(temp=>temp.Country,null as Country ).Create(),

                _fixer.Build<Person>().With(temp=>temp.Email,"sample3@gmail.com")
                .With(temp=>temp.Country,null as Country ).Create(),
                };

            List<PersonResponse> PersonListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.FilterPerson(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);


            List<PersonResponse> GetAllPersonResponse =await
                _personService.GetFilteredPerson(nameof(Person.PersonName), "");


           

            GetAllPersonResponse.Should().BeEquivalentTo(PersonListExpected);

        }


        [Fact]
        public async Task GetFilterName_Person()
        {
            List<Person> persons =
                new List<Person>()
                {
                _fixer.Build<Person>().With(temp=>temp.Email,"sample1@gmail.com")
                .With(temp=>temp.Country,null as Country).Create(),

                _fixer.Build<Person>().With(temp=>temp.Email,"sample2@gmail.com")
                .With(temp=>temp.Country,null as Country ).Create(),

                _fixer.Build<Person>().With(temp=>temp.Email,"sample3@gmail.com")
                .With(temp=>temp.Country,null as Country ).Create(),
                };

            List<PersonResponse> PersonListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.FilterPerson(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);


            List<PersonResponse> GetAllPersonResponse = await
                _personService.GetFilteredPerson(nameof(Person.PersonName), "sa");




            GetAllPersonResponse.Should().BeEquivalentTo(PersonListExpected);


        }

        #endregion


        #region Sorting


        [Fact]
        public async Task GetSortingPerson()
        {
            CountryAddRequest AddCountryRequest1 = _fixer.Create<CountryAddRequest>();

            CountryAddRequest AddCountryRequest2 = _fixer.Create<CountryAddRequest>();


            CountryResponse reponse1 = await
                _countriesService.AddCountry(AddCountryRequest1);

            CountryResponse reponse2 = await
               _countriesService.AddCountry(AddCountryRequest2);

            PersonAddRequest? AddPersonRequest1 = _fixer.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "saravana")
                .With(temp => temp.CountryID, reponse1.CountryID)
                .With(temp => temp.Email, "sample1@gmail.com").Create();

            PersonAddRequest? AddPersonRequest2 = _fixer.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "sona")
                .With(temp => temp.CountryID, reponse2.CountryID)
                .With(temp => temp.Email, "sample2@gmail.com").Create();
            PersonAddRequest? AddPersonRequest3 = _fixer.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "liyaan")
                 .With(temp => temp.CountryID, reponse2.CountryID)
                 .With(temp => temp.Email, "sample3@gmail.com").Create();

            List<PersonAddRequest> personAddRequestsList =
                new List<PersonAddRequest>()
                {
                AddPersonRequest1, AddPersonRequest2,
                AddPersonRequest3
                };

            List<PersonResponse> AddedPersonResponse = new List<PersonResponse>();

            foreach (PersonAddRequest AddedPerson in personAddRequestsList)
            {
                AddedPersonResponse.Add(await _personService.AddPerson(AddedPerson));
            }

            List<PersonResponse> AllPerson =await _personService.GetAllPersons();


            List<PersonResponse> GetAllPersonResponse = await
                _personService.GetSortedPerson
                (AllPerson, nameof(Person.PersonName), SortingOptions.DESC);

            _outputHelper.WriteLine("List of Person");
            foreach (PersonResponse personResponse in GetAllPersonResponse)
            {
                if (personResponse != null)
                {
                    _outputHelper.WriteLine(personResponse.ToString());
                }

            }
            GetAllPersonResponse.Should().BeInDescendingOrder(temp => temp.PersonName);
            //AddedPersonResponse = AddedPersonResponse.OrderByDescending
            //    (temp => temp.PersonName).ToList();

            //for (int i = 0; i < AddedPersonResponse.Count; i++)
            //{
            //    Assert.Equal(AddedPersonResponse[i], GetAllPersonResponse[i]);
            //}

            //GetAllPersonResponse.Should().BeEquivalentTo(AddedPersonResponse);
        }
        #endregion


        #region UpdatePerson
        [Fact]
        public async Task UpdatePerson_IsNull()
        {
            PersonUpdateRequest? personUpdateRequest = null;

           //await Assert.ThrowsAsync<ArgumentNullException>(async() =>
           // {
           //     await _personService.UpdatePerson(personUpdateRequest);
           // });

            var action = async () =>
            {
                await _personService.UpdatePerson(personUpdateRequest);

            };

            await action.Should().ThrowAsync<ArgumentNullException>();

        }

        [Fact]
        public async Task UpdatePerson_InValidPersonID()
        {
            PersonUpdateRequest? personUpdateRequest = _fixer.Build<PersonUpdateRequest>()
    .Create();



            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //    await _personService.UpdatePerson(personUpdateRequest);
            //});

            var action = async () =>
            {
                await _personService.UpdatePerson(personUpdateRequest);

            };

            await action.Should().ThrowAsync<ArgumentException>();

        }


        [Fact]

        public async Task UpdatePerson_NullPersonName()
        {

            CountryAddRequest NewCountryRequest1 = _fixer.Create<CountryAddRequest>();

            CountryResponse responseCountry = await _countriesService.AddCountry(NewCountryRequest1);
            PersonAddRequest? personAddRequest = _fixer.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Rahman")
                .With(temp=>temp.CountryID, responseCountry.CountryID)
                .With(temp => temp.Email, "sample@gmail.com").Create();
            PersonResponse responsePersonAdd = await _personService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest =
                responsePersonAdd.ToPersonUpdateRequest();

            personUpdateRequest.PersonName = null;

            //await Assert.ThrowsAsync<ArgumentException>(async() =>
            //{
            //   await _personService.UpdatePerson(personUpdateRequest);
            //});

            var action = async () =>
            {
                await _personService.UpdatePerson(personUpdateRequest);

            };

            await action.Should().ThrowAsync<ArgumentException>();


        }

        [Fact]
        public async Task UpdatePerson_UpdateAll()
        {
            CountryAddRequest NewCountryRequest1 = _fixer.Create<CountryAddRequest>();

            CountryResponse responseCountry = await _countriesService.AddCountry(NewCountryRequest1);
            PersonAddRequest? personAddRequest = _fixer.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Murugan")
                .With(temp => temp.CountryID, responseCountry.CountryID)
                .With(temp => temp.Email, "sample@gmail.com").Create();
            PersonResponse responsePersonAdd =await _personService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest =
                responsePersonAdd.ToPersonUpdateRequest();

            personUpdateRequest.PersonName = "velPandi";

            personUpdateRequest.Email = "pheoajay@gmail.com";

            PersonResponse UpdateResponse = await _personService.UpdatePerson(personUpdateRequest);

            PersonResponse? PersonIDresponse = await
                 _personService.GetPersonByPersonID(personUpdateRequest.PersonID);

            //Assert.Equal(UpdateResponse, PersonIDresponse);

            PersonIDresponse.Should().BeEquivalentTo(UpdateResponse);
        }

        #endregion

        #region Delete
        [Fact]
        public async Task  DeletePerson_ValidPerson()
        {
            CountryAddRequest NewCountryRequest1 = _fixer.Create<CountryAddRequest>();

            CountryResponse responseCountry = await _countriesService.AddCountry(NewCountryRequest1);
            PersonAddRequest? personAddRequest = _fixer.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Murugan")
                .With(temp => temp.CountryID, responseCountry.CountryID)
                .With(temp => temp.Email, "sample@gmail.com").Create();

            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);

            bool value =await _personService.DeletePerson(personResponse.PersonID);

            //Assert.True(value);

            value.Should().BeTrue();
        }


        [Fact]
        public async Task DeletePerson_InValidPerson()
        {
            CountryAddRequest NewCountryRequest1 = _fixer.Create<CountryAddRequest>();

            CountryResponse responseCountry = await _countriesService.AddCountry(NewCountryRequest1);
            PersonAddRequest? personAddRequest = _fixer.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Murugan")
                .With(temp => temp.CountryID, responseCountry.CountryID)
                .With(temp => temp.Email, "sample@gmail.com").Create();

            PersonResponse personResponse = await  _personService.AddPerson(personAddRequest);

            bool value = await _personService.DeletePerson(Guid.NewGuid());

            // Assert.False(value);

            value.Should().BeFalse();
        }
        #endregion
    }
}