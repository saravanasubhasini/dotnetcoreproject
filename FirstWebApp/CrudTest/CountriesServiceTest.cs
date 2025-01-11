using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Entities;

using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;

namespace CrudTest
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;



        public CountriesServiceTest()
        {
            var InitialCountryList = new List<Country>() { };


            DbContextMock<FirstAppDbContext> dbContextMock = new DbContextMock<FirstAppDbContext>(
                new DbContextOptionsBuilder<FirstAppDbContext>().Options
                );

            FirstAppDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, InitialCountryList);

            _countriesService = new CountriesService(null);
        }
        #region AddCountry
        // when CountryAddRequest is null throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
           await  Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                //Act
                await _countriesService.AddCountry(request);
            });
        }

        //when  CountryName is null throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameISNull()
        {
            //Arrange 
            CountryAddRequest? request = new CountryAddRequest()
            { CountryName = null };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                //Act
               await _countriesService.AddCountry(request);
            });
        }

        //when CountryName is Duplicate throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            CountryAddRequest request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest request2 = new CountryAddRequest() { CountryName = "USA" };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(request1);
                await _countriesService.AddCountry(request2);
            });

        }


        //when Giving Proper Country object (Insert CountryID and CountryName)
        [Fact]
        public async Task AddCountry_ProperDetails()
        {
            //Arrange 
            CountryAddRequest? request = new CountryAddRequest()
            { CountryName = "Australia" };

            //Assert

            //Act
            CountryResponse response = await _countriesService.AddCountry(request);

            List<CountryResponse> countriesFromGetAllCountries =await  _countriesService.GetAllCountries();
            Assert.True(response.CountryID != Guid.Empty);

            Assert.Contains(response, countriesFromGetAllCountries);


        }
        #endregion

        #region GetAllCountries
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            List<CountryResponse> actual_country_response_list =await  _countriesService.GetAllCountries();

            Assert.Empty(actual_country_response_list);
        }
        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            List<CountryAddRequest> countryListAdded = new List<CountryAddRequest>() {
        new CountryAddRequest(){ CountryName="Australa"},
        new CountryAddRequest(){ CountryName="NewZeland"}
        };
            List<CountryResponse> country_list_from_addcountry = new List<CountryResponse>();
            foreach (CountryAddRequest countries in countryListAdded)
            {
                 country_list_from_addcountry.Add(await _countriesService.AddCountry(countries));
            }

            List<CountryResponse> ActualCountryList = await _countriesService.GetAllCountries();

            foreach (CountryResponse actualCountry in ActualCountryList)
            {
                Assert.Contains(actualCountry, ActualCountryList);
            }
        }
        #endregion

        #region GetCountryByID
        [Fact]

        public async Task GetCountryByCountryID_NullCountryID()
        {

            Guid? CountryID = null;

            CountryResponse CountryResponseIsNull = await _countriesService.GetCountryByCountryID(CountryID);

            Assert.Null(CountryResponseIsNull);
        }

        [Fact]

        public async Task GetCountryByCountryID_ValidCountryID()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Netherlands" };
            CountryResponse countryResponnseFromAdd = await _countriesService.AddCountry(countryAddRequest);

            CountryResponse? countryResponseFromGetCountry =await _countriesService.GetCountryByCountryID(countryResponnseFromAdd.CountryID);

            Assert.Equal(countryResponnseFromAdd, countryResponseFromGetCountry);
        }
        #endregion
    }
}
