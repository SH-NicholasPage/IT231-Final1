/*
* Name: [YOUR NAME HERE]
* South Hills Username: [YOUR SOUTH HILLS USERNAME HERE]
*/

using Final1.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Final1.Server.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class CarController : ControllerBase
    {
        private static List<Car> Cars { get; set; } //Lazy load this

        [HttpGet]
        public IEnumerable<Car>? Get(String? ids)
        {
            List<Guid> IDs = null;
            if (String.IsNullOrWhiteSpace(ids) == false)
            {
                try
                {
                    IDs = JsonSerializer.Deserialize<List<Guid>>(ids)!;
                }
                catch { }
            }

            //TODO: Your code here

            throw new NotImplementedException();
        }

		[HttpPost]
		public Car? Post([FromBody] Tuple<String, String, int, String?, Int32?> carStuff)
		{
            (String? make, String? model, Int32? year, String? color, Int32? numOfWheels) carStuffTupleType = (carStuff.Item1, carStuff.Item2, carStuff.Item3, carStuff.Item4, carStuff.Item5);

            //TODO: Your code here

            throw new NotImplementedException();
        }

        //Only modify properties for not null items
		[HttpPut]
		public Car? Put([FromBody] Tuple<Guid, String?, String?, Int32?, String?, Int32?> carStuff)
		{
            (Guid id, String? make, String? model, Int32? year, String? color, Int32? numOfWheels) carStuffTupleType = (carStuff.Item1, carStuff.Item2, carStuff.Item3, carStuff.Item4, carStuff.Item5, carStuff.Item6);

            //TODO: Your code here

            throw new NotImplementedException();
        }

		[HttpDelete]
		public void Delete (Guid id)
		{
            //TODO: Your code here

            throw new NotImplementedException();
        }

		private Car? GetCarWithGUID(Guid guid) => Cars.Where(x => x.ID == guid).FirstOrDefault();


        //YOU SHOULD NOT USE PATCH THIS WAY IN A REAL ENVIRONMENT
        //Do not touch this method!
        [HttpPatch]
        public void Clear(String? _)
        {
            Cars = null!;
        }
	}
}