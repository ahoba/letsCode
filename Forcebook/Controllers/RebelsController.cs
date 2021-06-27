using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Forcebook.Models;
using Forcebook.DTO;

namespace Forcebook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RebelsController : ControllerBase
    {
        private readonly RebelsDbContext _context;

        public RebelsController(RebelsDbContext context)
        {
            _context = context;

            _context.Rebels.Include(rebel => rebel.Inventory).ToList();
            _context.Rebels.Include(rebel => rebel.TreasonReports).ToList();
            _context.Items.Include(item => item.Template).ToList();
        }

        /// <summary>
        /// Returns a collection of all rebels
        /// </summary>
        /// <returns>Returns a collection of all rebels</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RebelDTO>>> GetRebels()
        {
            return await _context.Rebels.Select(x => new RebelDTO(x)).ToListAsync();
        }

        /// <summary>
        /// Returns the percentage of traitors among the rebels
        /// </summary>
        /// <returns>Returns the percentage of traitors among the rebels</returns>
        [HttpGet("traitorsRate")]
        public async Task<ActionResult<double>> GetTraitorsRate()
        {
            double traitors = (double) await _context.Rebels.Where(x => x.TreasonReports.Count() >= 3).CountAsync();
            double total = (double) await _context.Rebels.CountAsync();

            return traitors / total * 100;
        }

        /// <summary>
        /// Returns the percentage of non-traitors among the rebels
        /// </summary>
        /// <returns>Returns the percentage of non-traitors among the rebels</returns>
        [HttpGet("rebelsRate")]
        public async Task<ActionResult<double>> GetRebelsRate()
        {
            double traitors = (double)await _context.Rebels.Where(x => x.TreasonReports.Count() >= 3).CountAsync();
            double total = (double)await _context.Rebels.CountAsync();

            return (total - traitors) / total * 100;
        }

        /// <summary>
        /// Returns the a collection of each existing item in the database and its average occurrence between the rebels
        /// </summary>
        /// <returns>Returns the a collection of each existing item in the database and its average occurrence between the rebels</returns>
        [HttpGet("avgItem")]
        public async Task<ActionResult<IEnumerable<AvgItemResponse>>> GetAvgItem()
        {
            Dictionary<string, int> itemCount = new Dictionary<string, int>();

            await _context.ItemTemplates.ForEachAsync(template => {
                itemCount[template.Name] = _context.Items.Where(x => x.Template.Name == template.Name && x.Owner.TreasonReports.Count() < 3).Sum(x => x.Quantity);
            });

            double total = (double) _context.Rebels.Count();

            return itemCount.Select(x => new AvgItemResponse { Name = x.Key, Avg = ((double)x.Value / total) }).ToList();
        }

        /// <summary>
        /// Returns the total amount of points belonging to traitors
        /// </summary>
        /// <returns>Returns the total amount of points belonging to traitors</returns>
        [HttpGet("traitorsPoints")]
        public async Task<ActionResult<int>> GetTraitorsPoints()
        {
            return await _context.Rebels.Where(rebel => rebel.TreasonReports.Count() >= 3).SumAsync(rebel => rebel.Inventory.Sum(item => item.Quantity * item.Template.Points));
        }

        /// <summary>
        /// Creates a new rebel
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns the created rebel or BadResponse/NotFound in case of error</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>   
        [HttpPost("createRebel")]
        public async Task<ActionResult<CreateRebelResponse>> CreateRebel(CreateRebelRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (HasRebel(request.Name))
            {
                return BadRequest($"There already is a rebel with name {request.Name}. Consider providing a nickname!");
            }

            Rebel rebel = CreateRebelRequest.FromRequest(request);

            foreach (ItemDTO itemDTO in request.Inventory)
            {
                if (!TryGetItemTemplate(itemDTO.Name, out ItemTemplate template))
                {
                    return ItemTemplateNotFound(itemDTO.Name);
                }

                Item item = rebel.Inventory.FirstOrDefault(x => x.Template.Name == itemDTO.Name);

                if (item == null)
                {
                    item = new Item()
                    {
                        Quantity = itemDTO.Quantity,
                        Template = template,
                        TemplateId = template.Name
                    };

                    rebel.Inventory.Add(item);
                }
                else
                {
                    item.Quantity += itemDTO.Quantity;
                }
            }

            _context.Rebels.Add(rebel);

            await _context.SaveChangesAsync();

            CreateRebelResponse response = new CreateRebelResponse(rebel);

            return CreatedAtAction(nameof(CreateRebel), new { name = response.Name }, response);
        }

        /// <summary>
        /// Updates a rebel's location
        /// </summary>
        /// <param name="request">
        /// </param>
        /// <response code="200">Returns the rebel's name and new location</response>
        /// <response code="400">If request is not valid</response>   
        /// <response code="404">If rebel with requested name is not found</response>   
        [HttpPut("updateLocation")]
        public async Task<ActionResult<UpdateLocationResponse>> UpdateLocation(UpdateLocationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!TryGetRebel(request.Name, out Rebel rebel))
            {
                return UnableToFindRebel(request.Name);
            }

            rebel.Location = request.Location;

            _context.Entry(rebel).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            UpdateLocationResponse response = new UpdateLocationResponse(rebel);

            return Ok(response);
        }

        /// <summary>
        /// Reports a rebel as traitor with a given accuser and accused
        /// </summary>
        /// <param name="request">Accuser's name and Accused Rebel's name</param>
        /// <response code="200">Returns the accused rebel's name and a list with his distinct accusers</response>
        /// <response code="400">If request is not valid</response>   
        /// <response code="404">If rebel with requested name is not found</response>
        [HttpPut("reportTreason")]
        public async Task<ActionResult<ReportTreasonResponse>> ReportTreason(ReportTreasonRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!HasRebel(request.Accuser))
            {
                return UnableToFindRebel(request.Accuser);
            }

            if (!HasRebel(request.Accused))
            {
                return UnableToFindRebel(request.Accused);
            }

            TreasonReport report = _context.TreasonReports.FirstOrDefault(x => x.AccusedId == request.Accused && x.AccuserId == request.Accuser);

            if (report == null)
            {
                report = new TreasonReport()
                {
                    AccusedId = request.Accused,
                    AccuserId = request.Accuser,
                    ReportCount = 1
                };

                _context.TreasonReports.Add(report);
            }
            else
            {
                report.ReportCount++;

                _context.Entry(report).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            ReportTreasonResponse response = new ReportTreasonResponse(report.Accused);

            return Ok(response);
        }

        /// <summary>
        /// Negotiates items between two rebels.
        /// </summary>
        /// <param name="request">Two negotiation legs, the first containing the rebel's name and offer, the second containing the counterparty's name and requested items</param>
        /// <response code="200">Returns the name and resulting inventory of each rebel</response>
        /// <response code="400">If request is not valid</response>
        /// <response code="404">If rebel or item with requested name is not found</response>
        [HttpPut("negotiateItems")]
        public async Task<ActionResult<NegotiateItemsResponse>> NegotiateItems(NegotiateItemsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!TryGetRebel(request.LegA.Name, out Rebel rebelA))
            {
                return UnableToFindRebel(request.LegA.Name);
            }
            else if (rebelA.IsTraitor)
            {
                return BannedRebel(request.LegA.Name);
            }

            if (!TryGetRebel(request.LegB.Name, out Rebel rebelB))
            {
                return UnableToFindRebel(request.LegB.Name);
            }
            else if (rebelB.IsTraitor)
            {
                return BannedRebel(request.LegB.Name);
            }

            if (!TryValidateAndGroup(request.LegA.Items, rebelA, out IEnumerable<ItemDTO> itemsA, out int pointsA, out ObjectResult result))
            {
                return result;
            }

            if (!TryValidateAndGroup(request.LegB.Items, rebelB, out IEnumerable<ItemDTO> itemsB, out int pointsB, out result))
            {
                return result;
            }

            if (pointsA != pointsB)
            {
                return BadRequest($"Mismatching points: [{rebelA.Name}:{pointsA}, {rebelB.Name}:{pointsB}]");
            }

            ProcessTransaction(itemsA, itemsB, rebelA);
            ProcessTransaction(itemsB, itemsA, rebelB);

            NegotiateItemsResponse response = new NegotiateItemsResponse()
            {
                LegA = new NegotiationLeg()
                {
                    Name = rebelA.Name,
                    Items = rebelA.Inventory.Select(x => new ItemDTO(x))
                },
                LegB = new NegotiationLeg()
                {
                    Name = rebelB.Name,
                    Items = rebelB.Inventory.Select(x => new ItemDTO(x))
                }
            };

            await _context.SaveChangesAsync();
            
            return Ok(response);
        }

        private BadRequestObjectResult BannedRebel(string traitor)
        {
            return BadRequest($"{traitor} is a traitor, therefore banned from negotiating.");
        }

        private bool TryValidateAndGroup(IEnumerable<ItemDTO> itemsDTO, Rebel owner, out IEnumerable<ItemDTO> groupedItems, out int points, out ObjectResult result)
        {
            Dictionary<string, ItemDTO> itemsDict = new Dictionary<string, ItemDTO>();

            points = 0;

            result = null;

            foreach (ItemDTO itemDTO in itemsDTO)
            {
                if (!TryGetItemTemplate(itemDTO.Name, out ItemTemplate template))
                {
                    groupedItems = null;

                    result = ItemTemplateNotFound(itemDTO.Name);

                    return false;
                }

                if (itemsDict.ContainsKey(itemDTO.Name))
                {
                    itemsDict[itemDTO.Name].Quantity += itemDTO.Quantity;
                }
                else
                {
                    itemsDict[itemDTO.Name] = new ItemDTO()
                    {
                        Name = itemDTO.Name,
                        Quantity = itemDTO.Quantity
                    };
                }

                if (itemsDict[itemDTO.Name].Quantity > owner.Inventory.Where(x => x.Template.Name == itemDTO.Name).Sum(x => x.Quantity))
                {
                    groupedItems = null;

                    result = BadRequest($"{owner.Name} does not have enough {itemDTO.Name}.");

                    return false;
                }

                points += template.Points * itemDTO.Quantity;
            }

            groupedItems = itemsDict.Values;

            return true;
        }
    
        private void ProcessTransaction(IEnumerable<ItemDTO> toRemove, IEnumerable<ItemDTO> toAdd, Rebel owner)
        {
            foreach (ItemDTO item in toRemove)
            {
                Item inInventory = owner.Inventory.First(x => x.Template.Name == item.Name);

                if (inInventory == null)
                {
                    throw new ArgumentOutOfRangeException(item.Name);
                }

                inInventory.Quantity -= item.Quantity;

                if (inInventory.Quantity < 0)
                {
                    throw new ArgumentOutOfRangeException(item.Name);
                }
                else if (inInventory.Quantity == 0)
                {
                    _context.Items.Remove(inInventory);
                }
                else
                {
                    _context.Entry(inInventory).State = EntityState.Modified;
                }
            }

            foreach (ItemDTO item in toAdd)
            {
                Item inInventory = owner.Inventory.FirstOrDefault(x => x.Template.Name == item.Name);

                if (inInventory == null)
                {
                    inInventory = new Item()
                    {
                        Quantity = item.Quantity,
                        Template = _context.ItemTemplates.First(x => x.Name == item.Name),
                        TemplateId = item.Name
                    };

                    _context.Items.Add(inInventory);
                }
                else
                {
                    inInventory.Quantity += item.Quantity;

                    _context.Entry(inInventory).State = EntityState.Modified;
                }
            }
        }
    
        private bool TryGetRebel(string name, out Rebel rebel)
        {
            rebel = _context.Rebels.FirstOrDefault(x => x.Name == name);

            return rebel != null;
        }

        private bool HasRebel(string name)
        {
            return _context.Rebels.Any(x => x.Name == name);
        }

        private NotFoundObjectResult UnableToFindRebel(string name)
        {
            return NotFound($"Unable to find rebel named {name}.");
        }

        private bool TryGetItemTemplate(string name, out ItemTemplate template)
        {
            template = _context.ItemTemplates.FirstOrDefault(x => x.Name == name);

            return template != null;
        }

        private NotFoundObjectResult ItemTemplateNotFound(string name)
        {
            return NotFound($"Unable to find item named {name}.");
        }
    }
}
