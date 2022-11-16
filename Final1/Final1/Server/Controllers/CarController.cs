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

        /// <summary>
        /// Returns the car(s) requested by the user.
        /// If the user does not pass in a parameter, returns all cars.
        /// </summary>
        /// <param name="ids"></param>
        [HttpGet]
        public IEnumerable<Car>? Get(String? ids)
        {
            List<Guid>? IDs = null;
            //If possible, will parse parameter to list of Guids
            if(TryParseStringToGuidList(ids, out IDs) == true)
            {
                //If the parameter was parsed to a list of Guids...
            }

            //TODO: Your code here

            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs expected REST POST and returns car created.
        /// </summary>
        /// <param name="carStuff"></param>
		[HttpPost]
		public Car? Post([FromBody] Tuple<String, String, int, String?, Int32?> carStuff)
		{
            (String make, String model, Int32 year, String? color, Int32? numOfWheels) carStuffTupleType = (carStuff.Item1, carStuff.Item2, carStuff.Item3, carStuff.Item4, carStuff.Item5);

            //TODO: Your code here

            throw new NotImplementedException();
        }

        /// <summary>
        /// Modifies a car matching a GUID.
        /// Does not modify properties that are null.
        /// </summary>
        /// <param name="carStuff"></param>
		[HttpPut]
		public Car? Put([FromBody] Tuple<Guid, String?, String?, Int32?, String?, Int32?> carStuff)
		{
            (Guid id, String? make, String? model, Int32? year, String? color, Int32? numOfWheels) carStuffTupleType = (carStuff.Item1, carStuff.Item2, carStuff.Item3, carStuff.Item4, carStuff.Item5, carStuff.Item6);

            //TODO: Your code here

            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs expected REST DELETE.
        /// </summary>
        /// <param name="id"></param>
		[HttpDelete]
		public void Delete (Guid id)
		{
            //TODO: Your code here

            throw new NotImplementedException();
        }

        public bool TryParseStringToGuidList(String? ids, out List<Guid>? guids)
        {
            guids = null;
            if (String.IsNullOrWhiteSpace(ids) == false)
            {
                try
                {
                    guids = JsonSerializer.Deserialize<List<Guid>>(ids)!;
                    return true;
                }
                catch 
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //YOU SHOULD NOT USE PATCH THIS WAY IN A REAL ENVIRONMENT
        //Do not modify this method!!
        [HttpPatch]
        public void Clear(String? _)
        {
            Cars = null!;
        }
	}
}