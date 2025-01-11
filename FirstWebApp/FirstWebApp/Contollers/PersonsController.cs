using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace FirstWebApp.Contollers
{
    [Route("[Controller]")]
    public class PersonsController : Controller
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countryService;

        public PersonsController(IPersonService personService, ICountriesService countryService)
        {
            _personService = personService;
            _countryService = countryService;
        }

        [Route("[action]")]
        //[Route("Persons/Index")]
        [Route("/")]
        public async Task<IActionResult> Index(string SearchBy, string? SearchString,
            string SortBy = nameof(PersonResponse.PersonName),SortingOptions SortingOptions =SortingOptions.ASC )
        {
            //Search
            ViewBag.SearchFields = new Dictionary<string , string>()
                {
                { nameof(PersonResponse.PersonName),"Person Name"},
                 { nameof(PersonResponse.Email),"Email"},
                 { nameof(PersonResponse.DateOfBirth),"DOB"},
                 { nameof(PersonResponse.Gender),"Gender"},
                 { nameof(PersonResponse.CountryName),"Country"},
                 { nameof(PersonResponse.Address),"Address"},
                };
            List<PersonResponse> GetAllPerson = await _personService.GetFilteredPerson(SearchBy, SearchString);
            
            ViewBag.CurrentSearchBy = SearchBy;
            ViewBag.CurrentSearchString = SearchString;

            //sort

            List<PersonResponse> sortedPerson = await _personService.GetSortedPerson(GetAllPerson, SortBy, SortingOptions);

            ViewBag.CurrentSortBy = SortBy;
            ViewBag.CurrentSortingOptions =Convert.ToString(SortingOptions);
            return View(sortedPerson);
        }
        [Route("[action]")]
        //[Route("Persons/Create")]
        [HttpGet]
        public async  Task<IActionResult> Create()
        {
            List<CountryResponse> countryResponse = await _countryService.GetAllCountries();
            ViewBag.Countries = countryResponse.Select(temp => new SelectListItem() { Value = temp.CountryID.ToString(), Text = temp.CountryName}) ;
            return View();
        }

        [Route("[action]")]
        //[Route("Persons/Create")]
        [HttpPost]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countryResponse = await _countryService.GetAllCountries();
                ViewBag.Countries = countryResponse.Select(temp => new SelectListItem() { Value = temp.CountryID.ToString(), Text = temp.CountryName });
                ViewBag.Errors = ModelState.Values.SelectMany(t => t.Errors).SelectMany(a => a.ErrorMessage).ToList();
                return View();
            }

            PersonResponse AddResponse = await _personService.AddPerson(personAddRequest);

            return RedirectToAction("Create", "Persons");
        }

        [HttpGet]
        [Route("[action]/{PersonID}")]
        public async Task<IActionResult> Edit(Guid PersonID)
        {
            
            PersonResponse? responseEdit = await _personService.GetPersonByPersonID(PersonID);

            if (responseEdit == null)
                return RedirectToAction("Index", "Persons");

            PersonUpdateRequest UpdateResposne =  responseEdit.ToPersonUpdateRequest();

            List<CountryResponse> AllCountry = await _countryService.GetAllCountries();

            ViewBag.Countries = AllCountry.Select(temp => new SelectListItem()
            {
                Value = temp.CountryID.ToString(),
                Text = temp.CountryName
            });
            

            return View(UpdateResposne);
        }

        public async Task GetCountryInView()
        {
            List<CountryResponse> AllCountry = await _countryService.GetAllCountries();

            ViewBag.Countries = AllCountry.Select(temp => new SelectListItem()
            {
                Value = temp.CountryID.ToString(),
                Text = temp.CountryName
            });
        }

        [HttpPost]
        [Route("[action]/{PersonID}")]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest, Guid PersonID)
        {
            PersonResponse? personAvailable = await _personService.GetPersonByPersonID(PersonID);

            if (personAvailable == null)
            {
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(err => err.Errors).Select(errMsg => errMsg.ErrorMessage).ToList();
                await GetCountryInView();
                return RedirectToAction("Edit");
            }
            else
            {
                 PersonResponse responseUpdate = await _personService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        [Route("[action]/{PersonID}")]

        public async Task<IActionResult> Delete(Guid PersonID)
        {
            PersonResponse? getResponse = await _personService.GetPersonByPersonID(PersonID);

            if (getResponse == null) 
                return RedirectToAction("Index");
            return View(getResponse);
        }

        [HttpPost]
        [Route("[action]/{PersonID}")]
        public IActionResult Delete(PersonResponse personResponse)
        { 
        
            if(personResponse == null)
                return RedirectToAction("Index");
            _personService.DeletePerson(personResponse.PersonID);

            return RedirectToAction("Index");
        }
    }

   
}
