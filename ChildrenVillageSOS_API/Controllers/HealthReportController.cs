﻿using ChildrenVillageSOS_DAL.DTO.HealthReportDTO;
using ChildrenVillageSOS_DAL.Models;
using ChildrenVillageSOS_SERVICE.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ChildrenVillageSOS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthReportController : ControllerBase
    {
        private readonly IHealthReportService _healthReportService;

        public HealthReportController(IHealthReportService healthReportService)
        {
            _healthReportService = healthReportService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHealthReports()
        {
            var reports = await _healthReportService.GetAllHealthReports();
            return Ok(reports);
        }

        [HttpGet("GetHealthReportById/{id}")]
        public async Task<IActionResult> GetHealthReportById(int id)
        {
            var report = await _healthReportService.GetHealthReportById(id);
            if (report == null)
            {
                return NotFound($"Health report with ID {id} not found.");
            }
            return Ok(report);
        }

        [HttpPost]
        [Route("CreateHealthReport")]
        public async Task<ActionResult<HealthReport>> CreateHealthReport([FromForm] CreateHealthReportDTO createReport)
        {
            var newReport = await _healthReportService.CreateHealthReport(createReport);
            return Ok(newReport);
        }

        [HttpPut]
        [Route("UpdateHealthReport/{id}")]
        public async Task<IActionResult> UpdateHealthReport(int id, [FromForm] UpdateHealthReportDTO updateReport)
        {
            try
            {
                var updatedReport = await _healthReportService.UpdateHealthReport(id, updateReport);
                return Ok(updatedReport);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteHealthReport/{id}")]
        public async Task<IActionResult> DeleteHealthReport(int id)
        {
            try
            {
                var deletedReport = await _healthReportService.DeleteHealthReport(id);
                return Ok(deletedReport);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
