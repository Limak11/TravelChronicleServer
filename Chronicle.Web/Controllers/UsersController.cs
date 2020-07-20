using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Chronicle.Web.Data;
using Chronicle.Web.Models;
using Chronicle.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chronicle.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ChronicleContext chronicleContext;
        private readonly KeyGeneratorService keyGenerator;

        public UsersController(ChronicleContext chronicleContext, KeyGeneratorService keyGenerator)
        {
            this.chronicleContext = chronicleContext;
            this.keyGenerator = keyGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<User>> GetUser()
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";

            if (headers.ContainsKey("apikey"))
            {
                token = headers["apikey"];
                var user = chronicleContext.Users.Where(k => k.Key == token).FirstOrDefault();

                if(user == null)
                {
                    var newUser = new User
                    {
                        Key = token
                        };
                    chronicleContext.Users.Add(newUser);
                    await chronicleContext.SaveChangesAsync();
                    return newUser;
                }
                else
                {
                    return user;
                }
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateUser([FromBody]User user)
        {
            //TODO move this header code to separate service - AuthenticationService
            var re = Request;
            var headers = re.Headers;
            string token = "";

            if (headers.ContainsKey("apikey"))
            {
                token = headers["apikey"];
                var key = chronicleContext.Users.Where(k => k.Key == token).FirstOrDefault();

                if(key == null)
                {
                    return NotFound();
                }
                else
                {
                    key.Description = user.Description;
                    key.BirthDate = user.BirthDate;
                    key.Name = user.Name;
                    chronicleContext.Users.Update(key);
                    await chronicleContext.SaveChangesAsync();
                    return Ok();
                }
            }
            return BadRequest();
        }       

        [HttpGet]
        [Route("get-key")]
        public string getGeneratedKey()
        {
            string key = keyGenerator.generateKey();
            return key;
        }

        [HttpPost]
        [Route("create-family")]
        public async Task<ActionResult> createFamily([FromBody] Family family)
        {
            //TODO move this header code to separate service - AuthenticationService
            var re = Request;
            var headers = re.Headers;
            string token = "";

            if (headers.ContainsKey("apikey"))
            {
                token = headers["apikey"];
                var user = chronicleContext.Users.Where(k => k.Key == token).FirstOrDefault();

                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    Family f = new Family
                    {
                        Name = family.Name,
                        JoinKey = keyGenerator.generateFamilyKey()
                    };
                    chronicleContext.Add(f);
                    await chronicleContext.SaveChangesAsync();

                    FamilyMember fm = new FamilyMember
                    {
                        UserId = user.Id,
                        FamilyId = f.Id
                    };
                    chronicleContext.Add(fm);
                    await chronicleContext.SaveChangesAsync();

                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("join-family")]
        public async Task<ActionResult<string>> joinFamily([FromBody] Family familyKey)
        {
            //TODO move this header code to separate service - AuthenticationService
            var re = Request;
            var headers = re.Headers;
            string token = "";

            if (headers.ContainsKey("apikey"))
            {
                token = headers["apikey"];
                var user = chronicleContext.Users.Where(k => k.Key == token).FirstOrDefault();

                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    var family = chronicleContext.Families.Where(f => f.JoinKey == familyKey.JoinKey).FirstOrDefault();
                    if (family == null)
                    {
                        return "No family with key like that";
                    }
                    else
                    {
                        var checkIfAlreadyJoined = chronicleContext.FamilyMembers.Where(f => f.UserId == user.Id && f.FamilyId == family.Id).FirstOrDefault();
                        if(checkIfAlreadyJoined != null)
                        {
                            return "Already member";
                        }
                        FamilyMember fm = new FamilyMember
                        {
                            UserId = user.Id,
                            FamilyId = family.Id
                        };

                        chronicleContext.Add(fm);
                        await chronicleContext.SaveChangesAsync();
                        return "Joined";
                    }
                }
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("get-families")]
        public async Task<ActionResult<List<Family>>> getFamilies()
        {
            //TODO move this header code to separate service - AuthenticationService
            var re = Request;
            var headers = re.Headers;
            string token = "";

            if (headers.ContainsKey("apikey"))
            {
                token = headers["apikey"];
                var user = chronicleContext.Users.Where(k => k.Key == token).FirstOrDefault();

                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    var memberships = chronicleContext.FamilyMembers.Where(f => f.UserId == user.Id).ToList();
                    if(memberships == null)
                    {
                        return new List<Family>();
                    }
                    List<Family> families = new List<Family>();

                    foreach(var m in memberships)
                    {
                        var f = chronicleContext.Families.Where(fam => fam.Id == m.FamilyId).FirstOrDefault();

                        families.Add(
                            new Family
                            {
                                Id = f.Id,
                                Name = f.Name,
                                JoinKey = f.JoinKey
                            });
                    }
                    return families;
                }
            }
            return BadRequest();
        }
    }
}