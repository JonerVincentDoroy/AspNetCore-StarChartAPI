using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        { 
            _context= context;
        }

        /*
        In the CelestialObjectController create a new method GetById

        This method should have a return type of IActionResult
        This method should accept a parameter of type int named id.
        This method should have an HttpGet attribute with an value of "{id:int}" and the Name property set to "GetById".
        This method should return NotFound() when there is no CelestialObject with an Id property that matches the parameter.
        This method should also set the Satellites property to any CelestialObjects who's OrbitedObjectId is the current CelestialObject's Id.
        This method should return an Ok with a value of the CelestialObject whose Id property matches the id parameter.
        */
        [HttpGet ("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject == null) 
            {
                return NotFound();
            }
            celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == id).ToList();

            return Ok(celestialObject);
        }

        /*
        Create GetByName Action

        Create the GetByName method

        This method should have a return type of IActionResult.
        This method should accept a parameter of type string named name.
        This method should have an HttpGet attribute with a value of "{name}".
        This method should return NotFound() when there is no CelestialObject with a Name property that matches the name parameter.
        This method should also set the Satellites property for each returned CelestialObject to any CelestialObjects who's OrbitedObjectId is the current CelestialObject's Id.
        This method should return an Ok with a value of all CelestialObjects whose Name property matches the name parameter.
        */
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObject = _context.CelestialObjects.Where(e => e.Name == name);

            if(!celestialObject.Any())
                return NotFound();

            foreach(var celestial in celestialObject) 
            {
                celestial.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestial.Id).ToList();
            }
            return Ok(celestialObject);
        }

        /*
        Create GetAll Action

        Create the GetAll method

        This method should have a return type of IActionResult.
        This method should also set the Satellites property for each of the CelestialObjects returned.
        This method should have an HttpGet attribute.
        This method should return Ok with a value of all CelestialObjects.
        */
        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(celestialObjects);
        }

        /*
        Create the Create Method

        In the CelestialObjectController class create the Create method

        This method should have a return type of IActionResult.
        This method should accept a parameter of type [FromBody]CelestialObject.
        This method should have an HttpPost attribute.
        This method should add the provided CelestialObject to the CelestialObjects DbSet then call SaveChanges.
        This method should return a CreatedAtRoute with the arguments
        "GetById"
        A new object with an id of the CelestialObject's Id. (Note: use the new {  } format.)
        The provided CelestialObject. (Note: You will need to add a using directive for StarChart.Models)
        */
        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestial)
        {
            _context.CelestialObjects.Add(celestial);
            
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestial.Id }, celestial);
        }

        /*
        Create Update Action

        Create the Update method

        This method should have a return type of IActionResult.
        This method should accept a parameter of type int named id and a parameter of type CelestialObject.
        This method should have the HttpPut attribute with a value of "{id}".
        This method should locate the CelestialObject with an Id that matches the provided int parameter.
        If no match is found return NotFound().
        If a match is found set it's Name, OrbitalPeriod, and OrbitedObjectId properties based on the provided CelestialObject parameter. Call Update on the CelestialObjects DbSet with an argument of the updated CelestialObject, and then call SaveChanges.
        This method should return NoContent().
        */
        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestial)
        {
            var locateCelestial = _context.CelestialObjects.Find(id);
            if (locateCelestial == null)
            {
                return NotFound();
            }
            locateCelestial.Name= celestial.Name;
            locateCelestial.OrbitalPeriod = celestial.OrbitalPeriod;
            locateCelestial.OrbitedObjectId = celestial.Id;
            _context.CelestialObjects.Update(locateCelestial);
            _context.SaveChanges();
            return NoContent();
        }

        /*
        Create RenameObject Action

        Create the RenameObject method

        This method should have a return type of IActionResult.
        This method should accept a parameter of type int named id and a parameter of type string named name.
        This method should have the HttpPatch attribute with an argument of "{id}/{name}".
        This method should locate the CelestialObject with an Id that matches the provided int parameter.
        If no match is found return NotFound().
        If a match is found set it's Name property to the provided string parameter. Then call Update on the CelestialObjects DbSet with an argument of the updated CelestialObject, and then call SaveChanges.
        This method should return NoContent().
        */
        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var locateCelestial = _context.CelestialObjects.Find(id);
            if (locateCelestial == null)
            {
                return NotFound();
            }
            locateCelestial.Name = name;
            _context.CelestialObjects.Update(locateCelestial);
            _context.SaveChanges();
            return NoContent();
        }

        /*
        Create Delete Action

        Create the Delete method

        This method should have a return type of IActionResult.
        This method should accept a parameter of type int named id.
        This method should have the HttpDelete attribute with an argument of "{id}".
        This method should get a List of all CelestialObjects who either have an Id or OrbitedObject with an Id that matches the provided parameter.
        If there are no matches it should return NotFound().
        If there are matching CelestialObjects call RemoveRange on the CelestialObjects DbSet with an argument of the list of matching CelestialObjects. Then call SaveChanges.
        This method should return NoContent().
        */
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestial = _context.CelestialObjects.Where(star => (star.Id == id || star.OrbitedObjectId == id)).ToList();
            if (!celestial.Any()) 
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(celestial);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
