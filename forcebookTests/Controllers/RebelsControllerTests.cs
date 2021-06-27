using Forcebook.Controllers;
using Forcebook.DTO;
using Forcebook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace forcebookTests.Controllers
{
    class RebelsControllerTests
    {
        private RebelsDbContext _context;

        private RebelsController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<RebelsDbContext>()
                .UseInMemoryDatabase(databaseName: "Rebels")
                .Options;

            _context = new RebelsDbContext(options);

            _context.Database.EnsureCreated();

            _context.Rebels.Include(rebel => rebel.Inventory).ToList();
            _context.Rebels.Include(rebel => rebel.TreasonReports).ToList();
            _context.Items.Include(item => item.Template).ToList();

            _controller = new RebelsController(_context);
        }

        [Test]
        public void TestCreateRebelSuccess()
        {
            var request = new CreateRebelRequest()
            {
                Name = "Ben Kenobi",
                Age = int.MaxValue,
                Gender = Gender.Male,
                Inventory = new ItemDTO[]
                {
                    new ItemDTO()
                    {
                        Name = "Weapon",
                        Quantity = 1
                    }
                },
                Location = new Location()
                {
                    Name = "Tatooine",
                    Latitude = int.MaxValue,
                    Longitude = int.MaxValue
                }
            };

            var response = _controller.CreateRebel(request).Result;

            Assert.IsTrue(response is ActionResult<CreateRebelResponse>);

            Rebel benKenoby = _context.Rebels.First(x => x.Name == request.Name);

            Assert.IsTrue(benKenoby != null);
            Assert.IsTrue(benKenoby.Age == request.Age);
            Assert.IsTrue(benKenoby.Gender == request.Gender);
            Assert.IsTrue(benKenoby.Location.Name == request.Location.Value.Name);
            Assert.IsTrue(benKenoby.Location.Latitude == request.Location.Value.Latitude);
            Assert.IsTrue(benKenoby.Location.Longitude == request.Location.Value.Longitude);
            
            foreach (var item in benKenoby.Inventory)
            {
                Assert.IsTrue(request.Inventory.Where(x => x.Name == item.Template.Name && x.Quantity == item.Quantity).Count() == 1);
            }
        }

        [Test]
        public async Task CreateExistingRebel()
        {
            var request = new CreateRebelRequest()
            {
                Name = "Luke Skywalker",
                Age = int.MaxValue,
                Gender = Gender.Male,
                Inventory = new ItemDTO[]
                {
                    new ItemDTO()
                    {
                        Name = "Weapon",
                        Quantity = 1
                    }
                },
                Location = new Location()
                {
                    Name = "Tatooine",
                    Latitude = int.MaxValue,
                    Longitude = int.MaxValue
                }
            };

            var response = await _controller.CreateRebel(request);

            Assert.IsInstanceOf<BadRequestObjectResult>(response.Result);
        }

        [Test]
        public async Task CreateExistingRebelWithUnknownItem()
        {
            var request = new CreateRebelRequest()
            {
                Name = "Obi Wan",
                Age = int.MaxValue,
                Gender = Gender.Male,
                Inventory = new ItemDTO[]
                {
                    new ItemDTO()
                    {
                        Name = "Lightsaber",
                        Quantity = 1
                    }
                },
                Location = new Location()
                {
                    Name = "Tatooine",
                    Latitude = int.MaxValue,
                    Longitude = int.MaxValue
                }
            };

            var response = await _controller.CreateRebel(request);

            Assert.IsInstanceOf<NotFoundObjectResult>(response.Result);
        }

        [Test]
        public async Task UpdateLocationSuccess()
        {
            var request = new UpdateLocationRequest()
            {
                Name = "Luke Skywalker",
                Location = new Location()
                {
                    Name = "Dagoba",
                    Latitude = 100,
                    Longitude = 100
                }
            };

            var response = await _controller.UpdateLocation(request);

            Assert.IsTrue(response is ActionResult<UpdateLocationResponse>);

            Rebel luke = _context.Rebels.First(x => x.Name == request.Name);

            Assert.IsTrue(luke.Location.Name == request.Location.Name);
            Assert.IsTrue(luke.Location.Latitude == request.Location.Latitude);
            Assert.IsTrue(luke.Location.Longitude == request.Location.Longitude);
        }

        [Test]
        public async Task UpdateLocationUnknownRebel()
        {
            var request = new UpdateLocationRequest()
            {
                Name = "Anakin Skywalker",
                Location = new Location()
                {
                    Name = "Mustafar",
                    Latitude = 666,
                    Longitude = 666
                }
            };

            var response = await _controller.UpdateLocation(request);

            Assert.IsInstanceOf<NotFoundObjectResult>(response.Result);
        }

        [Test]
        public async Task UpdateReportUnknownRebel()
        {
            var request = new ReportTreasonRequest()
            {
                Accused = "Anakin Skywalker",
                Accuser = "Luke Skywalker"
            };

            var response = await _controller.ReportTreason(request);

            Assert.IsInstanceOf<NotFoundObjectResult>(response.Result);
        }

        [Test]
        public async Task UpdateReporterUnknownRebel()
        {
            var request = new ReportTreasonRequest()
            {
                Accuser = "Anakin Skywalker",
                Accused = "Luke Skywalker"
            };

            var response = await _controller.ReportTreason(request);

            Assert.IsInstanceOf<NotFoundObjectResult>(response.Result);
        }

        [Test]
        public async Task ReportThreeDifferent()
        {
            var request = new ReportTreasonRequest()
            {
                Accuser = "Luke Skywalker",
                Accused = "Chewbacca"
            };

            var response = await _controller.ReportTreason(request);

            request = new ReportTreasonRequest()
            {
                Accuser = "Luke Skywalker",
                Accused = "Chewbacca"
            };

            response = await _controller.ReportTreason(request);

            request = new ReportTreasonRequest()
            {
                Accuser = "Luke Skywalker",
                Accused = "Chewbacca"
            };

            response = await _controller.ReportTreason(request);

            Assert.IsFalse(_context.Rebels.First(x => x.Name == "Chewbacca").IsTraitor);

            request = new ReportTreasonRequest()
            {
                Accuser = "Leia Organa",
                Accused = "Chewbacca"
            };

            response = await _controller.ReportTreason(request);

            request = new ReportTreasonRequest()
            {
                Accuser = "Han Solo",
                Accused = "Chewbacca"
            };

            response = await _controller.ReportTreason(request);

            Assert.IsTrue(_context.Rebels.First(x => x.Name == "Chewbacca").IsTraitor);
        }

        [Test]
        public async Task NegotiateWithUnknownRebel()
        {
            var request = new NegotiateItemsRequest()
            {
                LegA = new NegotiationLeg()
                {
                    Name = "Anakin Skywalker",
                    Items = new ItemDTO[]
                    {

                    }
                },
                LegB = new NegotiationLeg()
                {
                    Name = "Luke Skywalker",
                    Items = new ItemDTO[]
                    {

                    }
                }
            };

            var response = await _controller.NegotiateItems(request);

            Assert.IsInstanceOf<NotFoundObjectResult>(response.Result);

            request = new NegotiateItemsRequest()
            {
                LegB = new NegotiationLeg()
                {
                    Name = "Anakin Skywalker",
                    Items = new ItemDTO[]
                    {

                    }
                },
                LegA = new NegotiationLeg()
                {
                    Name = "Luke Skywalker",
                    Items = new ItemDTO[]
                    {

                    }
                }
            };

            response = await _controller.NegotiateItems(request);

            Assert.IsInstanceOf<NotFoundObjectResult>(response.Result);
        }

        [Test]
        public async Task NegotiateUnknownItem()
        {
            var request = new NegotiateItemsRequest()
            {
                LegA = new NegotiationLeg()
                {
                    Name = "Leia Organa",
                    Items = new ItemDTO[]
                    {
                        new ItemDTO()
                        {
                            Name = "Lightsaber"
                        }
                    }
                },
                LegB = new NegotiationLeg()
                {
                    Name = "Luke Skywalker",
                    Items = new ItemDTO[]
                    {
                        new ItemDTO()
                        {
                            Name = "Weapon"
                        }
                    }
                }
            };

            var response = await _controller.NegotiateItems(request);

            Assert.IsInstanceOf<BadRequestObjectResult>(response.Result);

            request = new NegotiateItemsRequest()
            {
                LegB = new NegotiationLeg()
                {
                    Name = "Leia Organa",
                    Items = new ItemDTO[]
                    {
                        new ItemDTO()
                        {
                            Name = "Weapon"
                        }
                    }
                },
                LegA = new NegotiationLeg()
                {
                    Name = "Luke Skywalker",
                    Items = new ItemDTO[]
                    {
                        new ItemDTO()
                        {
                            Name = "Lightsaber"
                        }
                    }
                }
            };

            response = await _controller.NegotiateItems(request);

            Assert.IsInstanceOf<BadRequestObjectResult>(response.Result);
        }

        [Test]
        public async Task NegotiateWithTraitor()
        {
            var request = new NegotiateItemsRequest()
            {
                LegA = new NegotiationLeg()
                {
                    Name = "Boba Fett",
                    Items = new ItemDTO[]
                    {

                    }
                },
                LegB = new NegotiationLeg()
                {
                    Name = "Luke Skywalker",
                    Items = new ItemDTO[]
                    {

                    }
                }
            };

            var response = await _controller.NegotiateItems(request);

            Assert.IsInstanceOf<BadRequestObjectResult>(response.Result);

            request = new NegotiateItemsRequest()
            {
                LegB = new NegotiationLeg()
                {
                    Name = "Boba Fett",
                    Items = new ItemDTO[]
                    {

                    }
                },
                LegA = new NegotiationLeg()
                {
                    Name = "Luke Skywalker",
                    Items = new ItemDTO[]
                    {

                    }
                }
            };

            response = await _controller.NegotiateItems(request);

            Assert.IsInstanceOf<BadRequestObjectResult>(response.Result);
        }

        [Test]
        public async Task NegotiatePointMismatch()
        {
            var request = new NegotiateItemsRequest()
            {
                LegA = new NegotiationLeg()
                {
                    Name = "Leia Organa",
                    Items = new ItemDTO[]
                    {
                        new ItemDTO()
                        {
                            Name = "Weapon",
                            Quantity = 1
                        }
                    }
                },
                LegB = new NegotiationLeg()
                {
                    Name = "Luke Skywalker",
                    Items = new ItemDTO[]
                    {
                        new ItemDTO()
                        {
                            Name = "Food",
                            Quantity = 1
                        }
                    }
                }
            };

            var response = await _controller.NegotiateItems(request);

            Assert.IsInstanceOf<BadRequestObjectResult>(response.Result);

            request = new NegotiateItemsRequest()
            {
                LegB = new NegotiationLeg()
                {
                    Name = "Leia Organa",
                    Items = new ItemDTO[]
                    {
                        new ItemDTO()
                        {
                            Name = "Weapon",
                            Quantity = 1
                        }
                    }
                },
                LegA = new NegotiationLeg()
                {
                    Name = "Luke Skywalker",
                    Items = new ItemDTO[]
                    {
                        new ItemDTO()
                        {
                            Name = "Food",
                            Quantity = 1
                        }
                    }
                }
            };

            response = await _controller.NegotiateItems(request);

            Assert.IsInstanceOf<BadRequestObjectResult>(response.Result);
        }

        [Test]
        public async Task NegotiateMoreThanAvailable()
        {
            var request = new NegotiateItemsRequest()
            {
                LegA = new NegotiationLeg()
                {
                    Name = "Leia Organa",
                    Items = new ItemDTO[]
                    {
                        new ItemDTO()
                        {
                            Name = "Weapon",
                            Quantity = 100
                        }
                    }
                },
                LegB = new NegotiationLeg()
                {
                    Name = "Luke Skywalker",
                    Items = new ItemDTO[]
                    {
                        new ItemDTO()
                        {
                            Name = "Weapon",
                            Quantity = 100
                        }
                    }
                }
            };

            var response = await _controller.NegotiateItems(request);

            Assert.IsInstanceOf<BadRequestObjectResult>(response.Result);

            request = new NegotiateItemsRequest()
            {
                LegB = new NegotiationLeg()
                {
                    Name = "Leia Organa",
                    Items = new ItemDTO[]
                    {
                        new ItemDTO()
                        {
                            Name = "Weapon",
                            Quantity = 100
                        }
                    }
                },
                LegA = new NegotiationLeg()
                {
                    Name = "Luke Skywalker",
                    Items = new ItemDTO[]
                    {
                        new ItemDTO()
                        {
                            Name = "Weapon",
                            Quantity = 100
                        }
                    }
                }
            };

            response = await _controller.NegotiateItems(request);

            Assert.IsInstanceOf<BadRequestObjectResult>(response.Result);
        }

        [Test]
        public async Task NegotiateSuccess()
        {
            int leiaWeapon = _context.Rebels.First(x => x.Name == "Leia Organa").Inventory.First(x => x.TemplateId == "Weapon").Quantity;
            int leiaFood = _context.Rebels.First(x => x.Name == "Leia Organa").Inventory.First(x => x.TemplateId == "Food").Quantity;

            int lukeWeapon = _context.Rebels.First(x => x.Name == "Luke Skywalker").Inventory.First(x => x.TemplateId == "Weapon").Quantity;
            int lukeFood = _context.Rebels.First(x => x.Name == "Luke Skywalker").Inventory.First(x => x.TemplateId == "Food").Quantity;

            var request = new NegotiateItemsRequest()
            {
                LegA = new NegotiationLeg()
                {
                    Name = "Leia Organa",
                    Items = new ItemDTO[]
                    {
                        new ItemDTO()
                        {
                            Name = "Weapon",
                            Quantity = 1
                        }
                    }
                },
                LegB = new NegotiationLeg()
                {
                    Name = "Luke Skywalker",
                    Items = new ItemDTO[]
                    {
                        new ItemDTO()
                        {
                            Name = "Water",
                            Quantity = 1
                        },
                        new ItemDTO()
                        {
                            Name = "Food",
                            Quantity = 2
                        }
                    }
                }
            };

            var response = await _controller.NegotiateItems(request);

            Assert.IsInstanceOf<OkObjectResult>(response.Result);

            int leiaFinalWeapon = _context.Rebels.First(x => x.Name == "Leia Organa").Inventory.First(x => x.TemplateId == "Weapon").Quantity;
            int leiaFinalFood = _context.Rebels.First(x => x.Name == "Leia Organa").Inventory.First(x => x.TemplateId == "Food").Quantity;

            int lukeFinalWeapon = _context.Rebels.First(x => x.Name == "Luke Skywalker").Inventory.First(x => x.TemplateId == "Weapon").Quantity;
            int lukeFinalFood = _context.Rebels.First(x => x.Name == "Luke Skywalker").Inventory.First(x => x.TemplateId == "Food").Quantity;

            Assert.IsTrue(leiaFinalWeapon == leiaWeapon - 1);
            Assert.IsTrue(lukeFinalWeapon == lukeWeapon + 1);

            Assert.IsTrue(leiaFinalFood == leiaFood + 2);
            Assert.IsTrue(lukeFinalFood == lukeFood - 2);
        }
    }
}
