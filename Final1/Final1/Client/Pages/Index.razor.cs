using Final1.Shared;
using Microsoft.AspNetCore.Components;
using System.Drawing;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Final1.Client.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        private HttpClient? Http { get; set; }

        private const byte MAX_POINTS = 75;
        private const byte NUM_OF_TESTS = 6;
        private List<Boolean?> TestStatuses = Enumerable.Repeat<Boolean?>(null, NUM_OF_TESTS).ToList();
        private String[] TestMessages = new String[NUM_OF_TESTS];
        private readonly String[] TestNames = new String[NUM_OF_TESTS]{ 
            "Checking API calls before data is inserted", "POST test", "GET test", "PUT test", "DELETE test", "Final integrity check"
        };
        private bool fatalError = false;

        private List<Car> cCars = new List<Car>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await Http!.PatchAsync("API/Car", null);//Clear server list

            (bool passed, String msg) r = await PerformFunctionalityTests();
            TestStatuses[0] = r.passed;
            TestMessages[0] = r.msg;
            StateHasChanged();

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

            r = await PerformSeveralGets();
            TestStatuses[2] = r.passed;
            TestMessages[2] = r.msg;
            StateHasChanged();

            r = await PerformSeveralPuts();
            TestStatuses[3] = r.passed;
            TestMessages[3] = r.msg;
            StateHasChanged();

            r = await PerformSeveralDeletes();
            TestStatuses[4] = r.passed;
            TestMessages[4] = r.msg;
            StateHasChanged();

            r = await PerformValidation();
            TestStatuses[5] = r.passed;
            TestMessages[5] = r.msg;
            StateHasChanged();
        }

        private async Task<(bool passed, String msg)> PerformFunctionalityTests()
        {
            (Car[]? car, bool exceptionOccurred) r = await PerformGetRequest();
            if (r.exceptionOccurred == true) return (false, "GET API threw exception. Check console/terminal for more details.");

            if (await PerformDeleteRequest(Guid.Empty) == true) return (false, "DELETE API threw exception. Check console/terminal for more details.");

            (Car? car, bool exceptionOccurred)  r2 = await PerformPutRequest(GenerateCarStuffForPut());
            if (r2.exceptionOccurred == true) return (false, "PUT API threw exception. Check console/terminal for more details.");

            r2 = await PerformPostRequest(GenerateCarStuff());
            if (r2.exceptionOccurred == true) return (false, "POST API threw exception. Check console/terminal for more details.");
            cCars.Add(r2.car!.Value);

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
                    return (false, r.exceptionOccurred ? "POST API threw exception. Check console/terminal for more details." : "Car added in POST was not correct.");
                }

                cCars[cCars.Count - 1] = r.car!.Value;
            }

            return (true, String.Empty);
        }

        private async Task<(bool passed, String msg)> PerformSeveralGets()
        {
            List<Car> carsToTest = cCars.Where(x => Random.Shared.Next(0, 10) == 0).ToList();
            
            while(carsToTest.Count == 0)
            {
                carsToTest = cCars.Where(x => Random.Shared.Next(0, 10) == 0).ToList();
            }

            (Car[]? cars, bool exceptionOccurred) r = await PerformGetRequest(carsToTest.Select(x => x.ID).ToList());

            if(r.exceptionOccurred == true)
            {
                return (false, "GET with parameters threw an exception. Check console/terminal for more details.");
            }
            else if(r.cars == null)
            {
                return (false, "GET with parameters returned null.");
            }
            else if(r.cars.ToList().OrderBy(x => x.ID).SequenceEqual(carsToTest.OrderBy(x => x.ID)) == false)
            {
                return (false, "GET with parameters returned incorrect results.");
            }

            r = await PerformGetRequest("goijusngonsn vbiuo4t4 gs \\getg ", true);

            if (r.exceptionOccurred == true)
            {
                return (false, "GET with junk value threw an exception. Check console/terminal for more details.");
            }
            else if (r.cars != null)
            {
                return (false, "GET with junk value returned something (should have returned null).");
            }

            r = await PerformGetRequest();

            if (r.exceptionOccurred == true)
            {
                return (false, "GET with no parameters threw an exception. Check console/terminal for more details.");
            }
            else if (r.cars == null)
            {
                return (false, "GET with no parameters returned null.");
            }
            else if (r.cars.ToList().OrderBy(x => x.ID).SequenceEqual(cCars.OrderBy(x => x.ID)) == false)
            {
                return (false, "GET with no parameters did not return all cars or did not return them properly.");
            }

            carsToTest = new List<Car> { cCars[Random.Shared.Next(0, cCars.Count)] };
            r = await PerformGetRequest(carsToTest.Select(x => x.ID).ToList());

            if (r.exceptionOccurred == true)
            {
                return (false, "GET with one parameters threw exception. Check console/terminal for more details.");
            }
            else if (r.cars == null)
            {
                return (false, "GET with one parameters returned null.");
            }
            else if (r.cars.Count() != 1)
            {
                return (false, "GET with one parameters returned the incorrect number of items.");
            }
            else if (carsToTest[0].Equals(r.cars[0]) == false)
            {
                return (false, "GET with no parameters did not return the correct car.");
            }

            return (true, "");
        }

        private async Task<(bool passed, String msg)> PerformSeveralPuts()
        {
            (Car? car, bool exceptionOccurred) r = await PerformPutRequest(GenerateCarStuffForPut(), true);

            if (r.exceptionOccurred == true)
            {
                return (false, "PUT with empty GUID threw exception. Check console/terminal for more details.");
            }
            else if (r.car != null)
            {
                return (false, "PUT with empty GUID returned something (should have returned null).");
            }

            int index = Random.Shared.Next(0, cCars.Count);
            Car car = cCars[Random.Shared.Next(0, cCars.Count)];
            (Guid id, String? make, String? model, Int32? year, String? color, Int32? numOfWheels) newCarAttr = GenerateCarStuffForPut(car.ID);

            r = await PerformPutRequest(newCarAttr);

            cCars.Remove(car);

            if (newCarAttr.make != null)
            {
                car = car with { Make = newCarAttr.make };
            }

            if (newCarAttr.model != null)
            {
                car = car with { Model = newCarAttr.model };
            }

            if (newCarAttr.year != null)
            {
                car = car with { Year = newCarAttr.year.Value };
            }

            if(newCarAttr.color != null)
            {
                car = car with { Color = newCarAttr.color };
            }

            if (newCarAttr.numOfWheels != null)
            {
                car = car with { NumberOfWheels = newCarAttr.numOfWheels };
            }

            cCars.Insert(index, car);

            if (r.exceptionOccurred == true)
            {
                return (false, "PUT threw an exception. Check console/terminal for more details.");
            }
            else if (r.car == null)
            {
                return (false, "PUT returned null.");
            }
            else if (car.Equals(r.car) == false)
            {
                return (false, "PUT did not return the correct car.");
            }

            return (true, "");
        }

        private async Task<(bool passed, String msg)> PerformSeveralDeletes()
        {
            bool erroredOut = await PerformDeleteRequest(Guid.Empty, true);

            if (erroredOut == true)
            {
                return (false, "DELETE with empty GUID threw an exception. Check console/terminal for more details.");
            }

            Car carToDelete = cCars[Random.Shared.Next(0, cCars.Count)];
            cCars.Remove(carToDelete);

            erroredOut = await PerformDeleteRequest(carToDelete.ID);

            if (erroredOut == true)
            {
                return (false, "DELETE threw an exception. Check console/terminal for more details.");
            }

            carToDelete = cCars[Random.Shared.Next(0, cCars.Count)];
            cCars.Remove(carToDelete);

            erroredOut = await PerformDeleteRequest(carToDelete.ID);

            if (erroredOut == true)
            {
                return (false, "DELETE threw an exception. Check console/terminal for more details.");
            }

            erroredOut = await PerformDeleteRequest(carToDelete.ID);

            if (erroredOut == true)
            {
                return (false, "DELETE with deleted GUID threw an exception. Check console/terminal for more details.");
            }

            return (true, "");
        }

        private async Task<(bool passed, String msg)> PerformValidation()
        {
            (Car[]? cars, bool exceptionOccurred) r = await PerformGetRequest();

            if (r.exceptionOccurred == true)
            {
                return (false, "GET with no parameters threw an exception. Check console/terminal for more details.");
            }
            else if (r.cars == null)
            {
                return (false, "GET with no parameters returned null.");
            }
            else if (r.cars.Length != cCars.Count)
            {
                return (false, "Validation failed. Server returned an incorrect number of cars.");
            }
            else if (r.cars.ToList().OrderBy(x => x.ID).SequenceEqual(cCars.OrderBy(x => x.ID)) == false)
            {
                cCars.OrderBy(x => x.ID);
                r.cars.OrderBy(x => x.ID);
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < cCars.Count; i++)
                {
                    if (cCars[i].Equals(r.cars[i]) == false)
                    {
                        sb.Append("\nERROR FOUND.\nClient: \t " + cCars[i].ToString() + "\nServer: \t" + r.cars[i].ToString());
                    }
                }

                return (false, "Validation failed. Server returned incorrect cars." + sb.ToString());
            }

            return (true, "");
        }

        private async Task<(Car[]? cars, bool exceptionOccurred)> PerformGetRequest(List<Guid>? id = null, bool junk = false)
        {
            (Car[]? cars, bool exceptionOccurred) r;
            r.exceptionOccurred = false;
            try
            {
                r.cars = await Http!.GetFromJsonAsync<Car[]?>("API/Car?ids=" + ((id != null) ? JsonSerializer.Serialize(id) : ""));
                return r;
            }
            catch (HttpRequestException)
            {
                r.cars = null;
                r.exceptionOccurred = true;
                return r;
            }
            catch (Exception)
            {
                if(cCars.Count > 0 && junk == false)
                {
                    r.exceptionOccurred = true;
                }

                r.cars = null;
                return r;
            }
        }

        private async Task<(Car[]? cars, bool exceptionOccurred)> PerformGetRequest(String id, bool junk = false)
        {
            (Car[]? cars, bool exceptionOccurred) r;
            r.exceptionOccurred = false;
            try
            {
                r.cars = await Http!.GetFromJsonAsync<Car[]?>("API/Car?ids=" + id);
                return r;
            }
            catch (HttpRequestException)
            {
                r.cars = null;
                r.exceptionOccurred = true;
                return r;
            }
            catch (Exception)
            {
                if (cCars.Count > 0 && junk == false)
                {
                    r.exceptionOccurred = true;
                }

                r.cars = null;
                return r;
            }
        }

        private async Task<(Car? car, bool exceptionOccurred)> PerformPostRequest((String make, String model, int year, String? color, Int32? numOfWheels) carStuff)
        {
            (Car? car, bool exceptionOccurred) r;
            r.exceptionOccurred = false;
            try
            {
                HttpResponseMessage msg = await Http!.PostAsJsonAsync("API/Car", new Tuple<String, String, int, String?, Int32?>(carStuff.make, carStuff.model, carStuff.year, carStuff.color, carStuff.numOfWheels));
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

        private async Task<(Car? car, bool exceptionOccurred)> PerformPutRequest((Guid id, String? make, String? model, int? year, String? color, Int32? numOfWheels) carStuff, bool junk = false)
        {
            (Car? car, bool exceptionOccurred) r;
            r.exceptionOccurred = false;
            try
            {
                HttpResponseMessage msg = await Http!.PutAsJsonAsync("API/Car", new Tuple<Guid, String?, String?, Int32?, String?, Int32?>(carStuff.id, carStuff.make, carStuff.model, carStuff.year, carStuff.color, carStuff.numOfWheels));
                r.car = await msg.Content.ReadFromJsonAsync<Car?>();
                return r;
            }
            catch (HttpRequestException)
            {
                r.car = null;
                r.exceptionOccurred = true;
                return r;
            }
            catch
            {
                if (cCars.Count > 0 && junk == false)
                {
                    r.exceptionOccurred = true;
                }

                r.car = null;
                return r;
            }
        }

        private async Task<bool> PerformDeleteRequest(Guid id, bool junk = false)
        {
            try
            {
                await Http!.DeleteAsync("API/Car?id=" + id);
                return false;
            }
            catch (HttpRequestException)
            {
                return true;
            }
            catch
            {
                if (cCars.Count > 0 && junk == false)
                {
                    return false;
                }

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
            return GenerateCarStuffForPut(Guid.Empty);
        }

        private (Guid id, String? make, String? model, Int32? year, String? color, Int32? numOfWheels) GenerateCarStuffForPut(Guid id)
        {
            int cat = Random.Shared.Next(Cars.Length);
            String? make = Random.Shared.Next(0, 8) == 0 ? null : Cars[cat][0];
            String? model = Random.Shared.Next(0, 8) == 0 ? null : Cars[cat][Random.Shared.Next(1, Cars[cat].Length)];
            Int32? year = Random.Shared.Next(0, 6) == 0 ? null : Random.Shared.Next(1988, DateTime.Now.Year + 1);
            String? color = Random.Shared.Next(0, 5) == 0 ? null : Enum.GetValues(typeof(KnownColor)).GetValue(Random.Shared.Next(Enum.GetValues(typeof(KnownColor)).Length))!.ToString();
            Int32? numOfWheels = Random.Shared.Next(0, 3) == 0 ? null : 4;
            return (id, make, model, year, color, numOfWheels);
        }
    }
}
