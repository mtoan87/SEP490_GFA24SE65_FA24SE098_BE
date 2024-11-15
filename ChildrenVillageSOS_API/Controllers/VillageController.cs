﻿using ChildrenVillageSOS_DAL.DTO.VillageDTO;
using ChildrenVillageSOS_DAL.Models;
using ChildrenVillageSOS_SERVICE.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ChildrenVillageSOS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillageController : ControllerBase
    {
        private readonly IVillageService _villageService;
        public VillageController(IVillageService villageService)
        {
            _villageService = villageService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllVillage()
        {
            var vil = await _villageService.GetAllVillage();
            return Ok(vil);
        }
        [HttpGet("GetVillageById/{Id}")]
        public async Task<IActionResult> GetVillageById(string villageId)
        {
            var vil = await _villageService.GetVillageById(villageId);
            return Ok(vil);
        }
        [HttpGet("GetVillagesDonatedByUser")]
        public IActionResult GetVillagesDonatedByUser(string userId)
        {
            var vil =  _villageService.GetVillagesDonatedByUser(userId);
            return Ok(vil);
        }
        [HttpGet("GetVillagesByUser/{userAccountId}")]
        public IActionResult GetVillagesByUser(string userAccountId)
        {
            var villages = _villageService.GetVillagesDonatedByUser(userAccountId);

            if (!villages.Any())
            {
                return NotFound(new { Message = "No villages found for this user." });
            }

            return Ok(villages.Select(v => new
            {
                v.Id,
                v.VillageName,
                v.Location,
                v.Description
            }));
        }
        [HttpPost]
        [Route("CreateVillage")]
        public async Task<ActionResult<Village>> CreateVillage([FromForm] CreateVillageDTO crevilDTO)
        {
            var createVil = await _villageService.CreateVillage(crevilDTO);
            return Ok(createVil);
        }
        [HttpPut]
        [Route("UpdateVillage")]
        public async Task<IActionResult> UpdateVillage(string villageId, [FromForm] UpdateVillageDTO updavilDTO)
        {
            var vil = await _villageService.UpdateVillage(villageId, updavilDTO);
            return Ok(vil);
        }
        [HttpDelete]
        [Route("DeleteVillage")]
        public async Task<IActionResult> DeleteVillage(string villageId)
        {
            var vil = await _villageService.DeleteVillage(villageId);
            return Ok(vil);
        }
    }
}
