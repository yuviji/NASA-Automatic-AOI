using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation.Results;
using LinqToDB;
using aoiRouting.Backend.Hubs;
using aoiRouting.Backend.Models;
using aoiRouting.Shared;
using aoiRouting.Shared.Models;
using aoiRouting.Shared.Models.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace aoiRouting.Backend.Controllers
{
    public class PinController : Controller
    {
        public const double TILE_SIZE = 0.01;
        private readonly AppDataConnection _connection;
        private readonly PinValidator _pinValidator;
        private readonly AOIValidator _aoiValidator;
        private readonly IHubContext<PinHub, IPinClient> _pinHub;
        public PinController([FromServices] AppDataConnection connection, [FromServices] IHubContext<PinHub, IPinClient> pinHubCtx)
        {
            _connection = connection;
            _pinHub = pinHubCtx;
            _pinValidator = new();
            _aoiValidator = new();
        }
        [Route("Pin/CreatePins")]
        [Authorize(Policy = "UserOnly")]
        [HttpPost]
        public async Task<IActionResult> CreatePin([FromBody] DbPin pin)
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.Sid), out Guid userID);
            if (userID == Guid.Empty)
            {
                return BadRequest();
            }
            pin.ID = Guid.NewGuid();
            pin.AOIID = Guid.NewGuid();
            pin.UserID = userID;

            // create AOI
            DbAOI aoi = new DbAOI
            {
                ID = pin.AOIID,
                UserID = pin.UserID,
                CentroidID = pin.ID,
                Created = pin.Created,
                Notes = ""
            };

            ValidationResult validPin = await _pinValidator.ValidateAsync(pin);
            ValidationResult validAOI = await _aoiValidator.ValidateAsync(aoi);
            await _connection.InsertAsync(aoi);

            // save centroid pin
            await _connection.InsertAsync(pin);
            string groupName = PinHub.GetGroupName(pin.Position.FloorRound());
            await _pinHub.Clients.Group(groupName).ReceivePin(pin);

            // create 36 other pins
            const double increment = 600.0 / 111000.0; // 600m * 1°/111000m
            Position incrementHorizontal = new Position(increment, 0);
            Position incrementVertical = new Position(0, increment);
            Position origin = pin.Position - 2.5 * incrementHorizontal - 2.5 * incrementVertical;
            int currPoint = 1;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    DbPin p = new DbPin
                    {
                        ID = Guid.NewGuid(),
                        AOIID = pin.AOIID,
                        UserID = pin.UserID,
                        PointID = currPoint,
                        Position = origin + i * incrementHorizontal + j * incrementVertical,
                        Created = pin.Created,
                        Collected = pin.Collected,
                        Notes = ""
                    };
                    ValidationResult valid = await _pinValidator.ValidateAsync(p);
                    await _connection.InsertAsync(p);
                    string gName = PinHub.GetGroupName(p.Position.FloorRound());
                    await _pinHub.Clients.Group(gName).ReceivePin(p);
                    currPoint++;
                }
            }

            return Ok();
        }
		[Route("Pin/UpdateLocation")]
		[Authorize(Policy = "UserOnly")]
		[HttpPost]
		public async Task<IActionResult> UpdateLocation([FromBody] object[] data)
		{
            Guid id = (Guid)data[0];
            double lat = (double)data[1];
            double lon = (double)data[2];
			Console.WriteLine("I'M IN: " + id);
			await _connection.Pins.Where(p => p.ID == id).Set(p => p.Lat, lat).Set(p => p.Lon, lon).UpdateAsync();
			Console.WriteLine("I'M OUT: " + id);
			return Ok();
		}
		[Route("Pin/UpdateCollection")]
        [Authorize(Policy = "UserOnly")]
        [HttpPost]
        public async Task<IActionResult> UpdateCollection([FromBody] Pin pin)
        {
            await _connection.Pins.Where(p => p.ID == pin.ID).Set(p => p.Collected, pin.Collected).UpdateAsync();
            return Ok();
        }
        [Route("Pin/GetPinCollected/{ss}")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet]
        public async Task<IActionResult> GetPinCollected(string ss)
        {
            Guid[] ids = JsonConvert.DeserializeObject<Guid[]>(ss);
            List<Pin> pins = await _connection.Pins.Where(pin => ids.Contains(pin.ID) && pin.Collected != null).OrderBy(x => x.PointID).DefaultIfEmpty().ToListAsync();
            return Json(pins);
        }
        [Route("Pin/GetPinPending/{ss}")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet]
        public async Task<IActionResult> GetPinPending(string ss)
        {
            Guid[] ids = JsonConvert.DeserializeObject<Guid[]>(ss);
            List<Pin> pins = await _connection.Pins.Where(pin => ids.Contains(pin.ID) && pin.Collected == null).OrderBy(x => x.PointID).DefaultIfEmpty().ToListAsync();
            return Json(pins);
        }
        [Route("Pin/GetPins")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet]
        public async Task<IActionResult> GetPins()
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.Sid), out Guid userID);
            List<Pin> pins = await _connection.Pins.Where(pin => pin.UserID == userID).DefaultIfEmpty().ToListAsync();
            Guid[] pinIds = pins.Select(p => p.ID).ToArray();
            return Json(pinIds);
        }
        [Route("Pin/ConvertPins/{ss}")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet]
        public async Task<IActionResult> ConvertPins(string ss)
        {
            Guid[] ids = JsonConvert.DeserializeObject<Guid[]>(ss);
            List<Pin> pins = await _connection.Pins.Where(pin => ids.Contains(pin.ID)).OrderBy(x => x.PointID).DefaultIfEmpty().ToListAsync();
            return Json(pins);
        }
    }
}