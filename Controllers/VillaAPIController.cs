using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RestFullApi.Data;
using RestFullApi.Models.Dto;
using RestFullApi.Models;
using RestFullApi.Logging;
using Microsoft.EntityFrameworkCore;
namespace RestFullApi.Controllers
{
    //[Route("api/[controller]")] //that is auto take name of contoller as here:VillaAPI
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController: ControllerBase
    {
        //private readonly ILogger<VillaAPIController> _logger; //for serilog

        //custom logging
        private readonly ILogging _logger;

        
        private readonly ApplicationDbContext _db;
        
        //public VillaAPIController(ILogger<VillaAPIController> logger)
        //{
        //    _logger = logger;
        //}

        //Custom logging
        public VillaAPIController(ILogging logger,ApplicationDbContext Db)
        {
            _logger = logger;
            _db = Db;
        }



        [HttpGet]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            _logger.Log("Getting all villas","");
            //return Ok(VillaStore.villaList);
            return Ok(_db.Villas.ToList());
        }

        [HttpGet("{id:int}", Name="GetVilla")]
        //That is for multiple responses as statuscode
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(200, Type=typeof(VillaDTO))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id <= 0)
            {
                _logger.Log("Get villa Error with Id " + id,"error");
                return BadRequest();
            }
           
            //var villa= VillaStore.villaList.FirstOrDefault(u => u.Id == id);
           
            var villa=_db.Villas.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound(id);
            }
            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            //if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower())!=null)
            //{
            //    ModelState.AddModelError("CustomError", "Villa already Exists!");
            //    return BadRequest(ModelState);
            //}


            if(_db.Villas.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already Exists!");
                return BadRequest(ModelState);
            }


            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }
            if (villaDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            //villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            //VillaStore.villaList.Add(villaDTO);
            //return Ok(villaDTO);

            Villa model= new()
            {
                Name = villaDTO.Name,
                Sqft = villaDTO.Sqft,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                ImageUrl = villaDTO.ImageUrl
            };
            _db.Villas.Add(model);
            _db.SaveChanges();

            return CreatedAtRoute("GetVilla",new { id=villaDTO.Id},villaDTO);
        }



        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id:int}",Name ="DeleteVilla")]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            //var villa= VillaStore.villaList.FirstOrDefault(u=>u.Id == id);
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            if (villa==null)
            {
                return NotFound();
            }
            //VillaStore.villaList.Remove(villa);
            _db.Villas.Remove(villa);
            _db.SaveChanges();
            return NoContent();
        }


        [HttpPut("{id:int}",Name ="UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {
            if (villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();
            }
            //var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            //villa.Name=villaDTO.Name;
            //villa.Sqft=villaDTO.Sqft;
            //villa.Occupancy = villaDTO.Occupancy;

            Villa model= new()
            {
                Id = villaDTO.Id,
                Name = villaDTO.Name,
                Sqft = villaDTO.Sqft,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                ImageUrl = villaDTO.ImageUrl
            };
            _db.Villas.Update(model);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id:int}",Name ="UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            if(patchDTO==null || id == 0)
            {
                return BadRequest();
            }
            //var villa=VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = _db.Villas.AsNoTracking().FirstOrDefault(v => v.Id == id);
            
            VillaDTO villaDTO= new()
            {
                Id = villa.Id,
                Name = villa.Name,
                Sqft = villa.Sqft,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Amenity = villa.Amenity,
                Details = villa.Details,
                ImageUrl = villa.ImageUrl
            };
            if (villa == null)
            {
                return BadRequest();
            }
            //patchDTO.ApplyTo(villa, ModelState);
            patchDTO.ApplyTo(villaDTO, ModelState);

            Villa model = new()
            {
                Id = villaDTO.Id,
                Name = villaDTO.Name,
                Sqft = villaDTO.Sqft,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                ImageUrl = villaDTO.ImageUrl
            };
            _db.Villas.Update(model);
            _db.SaveChanges();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
