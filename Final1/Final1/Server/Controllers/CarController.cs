using Final1.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Final1.Server.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class CarController : ControllerBase
    {
        List<Car> Cars { get; } //Lazy load this

        [HttpGet]
        public IEnumerable<Car> Get()
        {
            return null;
        }

		[HttpPost]
		public void Post((String make, String model, int year, String? color, Int32? numOfWheels) carStuff)
		{
			
		}

		[HttpPut]
		public void Put((Guid id, String? make, String? model, Int32? year, String? color, Int32? numOfWheels) carStuff)
		{
			
		}

		[HttpDelete]
		public void Delete (Guid id)
		{
			
		}

		private Car? GetCarWithGUID(Guid guid) => Cars.Where(x => x.ID == guid).FirstOrDefault();
	}
}