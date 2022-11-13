using Final1.Shared;
using Microsoft.AspNetCore.Components;
using System.Drawing;
using System.Net.Http.Json;

namespace Final1.Client.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        private HttpClient? Http { get; set; }

        private const byte NUM_OF_TESTS = 10;
        private List<Boolean> TestStatuses = Enumerable.Repeat<Boolean>(true, NUM_OF_TESTS).ToList();
        private String[] TestMessages = new String[NUM_OF_TESTS];
        private readonly String[] TestNames = { 
            "Checking API calls before data is inserted", "Adding 100 posts" 
        };
        private bool fatalError = false;

        private Car[]? cars;
        private List<Car> cCars = new List<Car>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            (bool passed, String msg) r = await PerformFunctionalityTests();
            TestStatuses[0] = r.passed;
            TestMessages[0] = r.msg;

            r = await PerformSeveralPosts();
            TestStatuses[1] = r.passed;
            TestMessages[1] = r.msg;

            if(r.passed == false)
            {
                fatalError = true;
                StateHasChanged();
                return;
            }

            StateHasChanged();
        }

        private async Task<(bool passed, String msg)> PerformFunctionalityTests()
        {
            (Car[]? car, bool exceptionOccurred) r = await PerformGetRequest();
            if (r.exceptionOccurred == true) return (false, "GET API threw exception. Check console for more details.");

            (Car? car, bool exceptionOccurred) r2 = await PerformPostRequest(GenerateCarStuff());
            if (r2.exceptionOccurred == true) return (false, "POST API threw exception. Check console for more details.");

            r2 = await PerformPutRequest(GenerateCarStuffForPut());
            if (r2.exceptionOccurred == true) return (false, "PUT API threw exception. Check console for more details.");

            if (await PerformDeleteRequest(Guid.Empty) == true) return (false, "DELETE API threw exception. Check console for more details.");

            return (true, String.Empty);
        }

        private async Task<(bool passed, String msg)> PerformSeveralPosts(int numToPerform = 100)
        {
            while(numToPerform-- > 0)
            {
                (String make, String model, int year, String? color, Int32? numOfWheels) c = GenerateCarStuff();
                cCars.Add(new Car() {
                    Make = c.make,
                    Model = c.model,
                    Year = c.year,
                    Color = c.color,
                    NumberOfWheels = c.numOfWheels
                });

                (Car? car, bool exceptionOccurred) r = await PerformPostRequest(c);

                if (cCars.Last().EqualsWithoutGuid(r.car) == false)
                {
                    return (false, r.exceptionOccurred ? "POST API threw exception. Check console for more details." : "Car added in POST was not correct.");
                }

                cCars[cCars.Count - 1] = r.car!.Value;
            }

            return (true, String.Empty);
        }

        private async Task<(Car[]? cars, bool exceptionOccurred)> PerformGetRequest()
        {
            (Car[]? cars, bool exceptionOccurred) r;
            r.exceptionOccurred = false;
            try
            {
                r.cars = await Http!.GetFromJsonAsync<Car[]>("API/Car");
                return r;
            }
            catch
            {
                r.cars = null;
                r.exceptionOccurred = true;
                return r;
            }
        }

        private async Task<(Car? car, bool exceptionOccurred)> PerformPostRequest((String make, String model, int year, String? color, Int32? numOfWheels) carStuff)
        {
            (Car? car, bool exceptionOccurred) r;
            r.exceptionOccurred = false;
            try
            {
                HttpResponseMessage msg = await Http!.PostAsJsonAsync("API/Car", carStuff);
                r.car = await msg.Content.ReadFromJsonAsync<Car?>();
                return r;
            }
            catch
            {
                r.car = null;
                r.exceptionOccurred = true;
                return r;
            }
        }

        private async Task<(Car? car, bool exceptionOccurred)> PerformPutRequest((Guid id, String? make, String? model, int? year, String? color, Int32? numOfWheels) carStuff)
        {
            (Car? car, bool exceptionOccurred) r;
            r.exceptionOccurred = false;
            try
            {
                HttpResponseMessage msg = await Http!.PutAsJsonAsync("API/Car", carStuff);
                r.car = await msg.Content.ReadFromJsonAsync<Car?>();
                return r;
            }
            catch
            {
                r.car = null;
                r.exceptionOccurred = true;
                return r;
            }
        }

        private async Task<bool> PerformDeleteRequest(Guid id)
        {
            try
            {
                await Http!.DeleteAsync("API/Car?id='" + id + "'");
                return false;
            }
            catch
            {
                return true;
            }
        }

        private String[][] Cars { get; } = { 
            new String[] { "Ford", "F-550 SUPER DUTY", "ESCAPE", "FOCUS", "FUSION" },
            new String[] { "Honda", "ELEMENT", "SABRE", "CBR00RR", "RUBICON", "RINCON", "CBR250R", "ACCORD", "AQUATRAX", "FSC600A" },
            new String[] { "Mercedes-Benz", "S500", "SLK55", "CLS500", "SL55", "ML500" },
            new String[] { "Chevrolet", "TAHOE", "SUBURBAN 2500", "CHEVY PICKUP" },
            new String[] { "Dodge", "Status", "RAM 1500", "RAM 3500" },
            new String[] { "Audi", "A6", "ALLROAD", "S8", "A4", "S5" },
            new String[] { "Lexus", "RX330", "LS460", "LS430" },
            new String[] { "Nissan", "TSURU", "ALTIMA", "TIIDA" },
            new String[] { "Toyota", "TUNDRA", "RAV4", "AVALON", "MATRIX", "SIENNA", "LAND CRUISER" }
        };

        private (String make, String model, int year, String? color, Int32? numOfWheels) GenerateCarStuff()
        {
            int cat = Random.Shared.Next(Cars.Length);
            String make = Cars[cat][0];
            String model = Cars[cat][Random.Shared.Next(1, Cars[cat].Length)];
            int year = Random.Shared.Next(1988, DateTime.Now.Year + 1);
            String? color = Random.Shared.Next(0, 5) == 0 ? null : Enum.GetValues(typeof(KnownColor)).GetValue(Random.Shared.Next(Enum.GetValues(typeof(KnownColor)).Length))!.ToString();
            Int32? numOfWheels = Random.Shared.Next(0, 3) == 0 ? null : 4;
            return (make, model, year, color, numOfWheels);
        }

        private (Guid id, String? make, String? model, Int32? year, String? color, Int32? numOfWheels) GenerateCarStuffForPut()
        {
            int cat = Random.Shared.Next(Cars.Length);
            String? make = Random.Shared.Next(0, 8) == 0 ? null : Cars[cat][0];
            String? model = Random.Shared.Next(0, 8) == 0 ? null : Cars[cat][Random.Shared.Next(1, Cars[cat].Length)];
            Int32? year = Random.Shared.Next(0, 6) == 0 ? null : Random.Shared.Next(1988, DateTime.Now.Year + 1);
            String? color = Random.Shared.Next(0, 5) == 0 ? null : Enum.GetValues(typeof(KnownColor)).GetValue(Random.Shared.Next(Enum.GetValues(typeof(KnownColor)).Length))!.ToString();
            Int32? numOfWheels = Random.Shared.Next(0, 3) == 0 ? null : 4;
            return (Guid.Empty, make, model, year, color, numOfWheels);
        }
    }
}
