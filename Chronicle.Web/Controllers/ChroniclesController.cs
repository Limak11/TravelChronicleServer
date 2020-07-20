using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Chronicle.Web.Data;
using Chronicle.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chronicle.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChroniclesController : ControllerBase
    {
        private readonly ChronicleContext chronicleContext;

        public ChroniclesController(ChronicleContext chronicleContext)
        {
            this.chronicleContext = chronicleContext;
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<string>> CreateChronicle(Models.Chronicle chronicle)
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";

            if (headers.ContainsKey("apikey"))
            {
                token = headers["apikey"];
                var user = chronicleContext.Users.Where(k => k.Key == token).FirstOrDefault();

                if (user != null)
                {
                    var chro = new Models.Chronicle
                    {
                        Name = chronicle.Name,
                        CreatedTime = DateTime.Now,
                        UserId = user.Id,
                        IsPrivate = chronicle.IsPrivate
                    };

                    chronicleContext.Chronicles.Add(chro);
                    await chronicleContext.SaveChangesAsync();
                    return "Added";
                }
            }
            return "Something went wrong";
        }


        [HttpPost]
        [Route("delete")]
        public async Task<ActionResult<Models.Chronicle>> DeleteChronicle(Models.Chronicle chronicle)
        {
            //TODO move this header code to separate service - AuthenticationService
            var re = Request;
            var headers = re.Headers;
            string token = "";

            if (headers.ContainsKey("apikey"))
            {
                token = headers["apikey"];
                var user = chronicleContext.Users.Where(k => k.Key == token).FirstOrDefault();

                if (user != null)
                {
                    if (user.Id != chronicle.UserId) return BadRequest("User is not the author");

                    chronicle.TimeDeleted = DateTime.Now;
                    chronicleContext.Chronicles.Update(chronicle);
                    await chronicleContext.SaveChangesAsync();

                    var posts = chronicleContext.Posts.Where(p => p.ChronicleId == chronicle.Id).ToList();
                    foreach (var post in posts)
                    {
                        post.TimeDeleted = DateTime.Now;
                        chronicleContext.Update(post);
                        await chronicleContext.SaveChangesAsync();
                    }
                    return Ok();
                }
            }

            return BadRequest();
        }

        //1. Update chronicle
        [HttpPost]
        [Route("update")]
        public async Task<ActionResult<Models.Chronicle>> UpdateChronicle(Models.Chronicle chronicle)
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
                    return NotFound("No user with such key");
                }
                //check if chronicle belongs to the user with such key
                if (chronicle.UserId != user.Id)
                {
                    return BadRequest("Chronicle doesn't belong to user with given key");
                }

                chronicleContext.Update(chronicle);
                await chronicleContext.SaveChangesAsync();

                return Ok();
            }

            return BadRequest();
        }

        //2. Get my chronicles
        [HttpGet]
        public List<Models.Chronicle> GetMyChronicles()
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";

            if (headers.ContainsKey("apikey"))
            {
                token = headers["apikey"];
                var user = chronicleContext.Users.Where(k => k.Key == token).FirstOrDefault();

                if (user == null)
                {
                    return new List<Models.Chronicle>();
                }

                var chronicleList = chronicleContext.Chronicles.Where(c => c.UserId == user.Id && c.TimeDeleted == null).ToList();
                var families = chronicleContext.FamilyMembers.Where(f => f.UserId == user.Id).ToList();

                foreach(var family in families)
                {
                    var members = chronicleContext.FamilyMembers.Where(m => m.FamilyId == family.FamilyId && m.UserId!=user.Id).ToList();
                    foreach(var member in members)
                    {
                        var memberChronicles = chronicleContext.Chronicles.Where(c => c.UserId == member.UserId && c.IsPrivate == false && c.TimeDeleted == null).ToList();
                        if (memberChronicles != null) chronicleList = chronicleList.Concat(memberChronicles).ToList();
                    }
                }

                if(chronicleList == null)
                {
                    return new List<Models.Chronicle>();
                }
                List<Models.Chronicle> chronicles = new List<Models.Chronicle>();
                foreach (var chro in chronicleList)
                {
                    chronicles.Add(new Models.Chronicle
                    {
                        Id = chro.Id,
                        Name = chro.Name,
                        CreatedTime = chro.CreatedTime,
                        UserId = chro.UserId
                    });

                }
                return chronicles;
            }
            return new List<Models.Chronicle>();
        }

        [HttpGet]
        [Route("get-chronicle/{id}")]
        public async Task<ActionResult<Models.Chronicle>> GetMyChronicle(long id)
        {
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

                var chro = chronicleContext.Chronicles.Where(c => c.Id == id && c.UserId == user.Id && c.TimeDeleted == null).FirstOrDefault();
                if (chro != null)
                {
                    Models.Chronicle chronicle = new Models.Chronicle
                    {
                        Id = chro.Id,
                        Name = chro.Name,
                        CreatedTime = chro.CreatedTime,
                        UserId = chro.UserId
                    };

                    return chronicle;
                }
                if (isFamilies(id, user.Id))
                {
                    var chronicleFromFamily = chronicleContext.Chronicles.Where(c => c.Id == id && c.TimeDeleted == null).FirstOrDefault();

                    {
                        Models.Chronicle chronicle = new Models.Chronicle
                        {
                            Id = chronicleFromFamily.Id,
                            Name = chronicleFromFamily.Name,
                            CreatedTime = chronicleFromFamily.CreatedTime,
                            UserId = chronicleFromFamily.UserId
                        };
                        return chronicle;
                    }
                }             
            }
            return NotFound();
        }

        bool isFamilies(long chronicleId, long userId)
        {

            var families = chronicleContext.FamilyMembers.Where(f => f.UserId == userId).ToList();

            foreach (var family in families)
            {
                var members = chronicleContext.FamilyMembers.Where(m => m.FamilyId == family.FamilyId && m.UserId != userId).ToList();
                foreach (var member in members)
                {
                    var memberChronicle = chronicleContext.Chronicles.Where(c => c.UserId == member.UserId && c.Id == chronicleId).FirstOrDefault();
                    if (memberChronicle != null) return true;
                }
            }
            return false;
        }

        //Inside App: 1. Add Location, 2. Get Locations Id from Request, 3. Add Post
        [HttpPost]
        [Route("add-post")]
        public async Task<ActionResult<Post>> AddPost([FromBody] Post post)
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";

            if (headers.ContainsKey("apikey"))
            {
                token = headers["apikey"];
                var user = chronicleContext.Users.Where(k => k.Key == token).FirstOrDefault();
                if (user == null)
                {
                    return NotFound("Such key doesn't exist");
                }

                var authorship = chronicleContext.Chronicles.Where(a => a.UserId == user.Id && a.Id == post.ChronicleId).FirstOrDefault();

                if (authorship == null)
                {
                    return BadRequest("User isn't author of this chronicle");
                }

                var newPost = new Post
                {
                    ChronicleId = post.ChronicleId,
                    CreatedTime = DateTime.Now,
                    Description = post.Description,
                    Latitude = post.Latitude,
                    Longitude = post.Longitude,
                    Name = post.Name
                };

                chronicleContext.Posts.Add(newPost);
                await chronicleContext.SaveChangesAsync();

                var returnPost = new Post
                {
                    Id = newPost.Id,
                    ChronicleId = post.ChronicleId,
                    CreatedTime = DateTime.Now,
                    Description = post.Description,
                    Latitude = post.Latitude,
                    Longitude = post.Longitude,
                    Name = post.Name
                };

                return returnPost;
            }
            return BadRequest();
        }

        //Inside App: 1. Add Location, 2. Get Locations Id from Request, 3. Add Post
        [HttpPost]
        [Route("edit-post")]
        public async Task<ActionResult<string>> EditPost([FromBody] Post post)
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";

            if (headers.ContainsKey("apikey"))
            {
                token = headers["apikey"];
                var user = chronicleContext.Users.Where(k => k.Key == token).FirstOrDefault();
                if (user == null)
                {
                    return NotFound("Such key doesn't exist");
                }

                var authorship = chronicleContext.Chronicles.Where(a => a.UserId == user.Id && a.Id == post.ChronicleId).FirstOrDefault();

                if (authorship == null)
                {
                    return BadRequest("User isn't author of this chronicle");
                }

                chronicleContext.Posts.Update(post);
                await chronicleContext.SaveChangesAsync();

                return "edited";
            }
            return BadRequest();
        }

        //Inside App: 1. Add Location, 2. Get Locations Id from Request, 3. Add Post
        [HttpPost]
        [Route("delete-post")]
        public async Task<ActionResult<string>> DeletePost([FromBody] Post post)
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";

            if (headers.ContainsKey("apikey"))
            {
                token = headers["apikey"];
                var user = chronicleContext.Users.Where(k => k.Key == token).FirstOrDefault();
                if (user == null)
                {
                    return NotFound("Such key doesn't exist");
                }

                var authorship = chronicleContext.Chronicles.Where(a => a.UserId == user.Id && a.Id == post.ChronicleId).FirstOrDefault();

                if (authorship == null)
                {
                    return BadRequest("User isn't author of this chronicle");
                }
                post.TimeDeleted = DateTime.Now;
                chronicleContext.Posts.Update(post);
                await chronicleContext.SaveChangesAsync();

                return "deleted";
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("add-photo")]
        public async Task<ActionResult<long>> AddPhoto([FromBody] HttpPhoto sentPhoto)
        {
            byte[] newBytes = Convert.FromBase64String(sentPhoto.ImageString);

            var photo = new Photo
            {
                ImageData = newBytes,
                PostId = sentPhoto.PostId
            };
            chronicleContext.Photos.Add(photo);
            await chronicleContext.SaveChangesAsync();

            return photo.Id;

        }

        //get all posts from chronicle
        [HttpGet]
        [Route("get-posts/{id}")]
        public ActionResult<List<Post>> GetPosts(long id)
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";

            if (headers.ContainsKey("apikey"))
            {
                token = headers["apikey"];
                var user = chronicleContext.Users.Where(k => k.Key == token).FirstOrDefault();

                if (user == null)
                {
                    return NotFound("Such key doesn't exist");
                }
                var authorship = chronicleContext.Chronicles.Where(a => a.UserId == user.Id && a.Id == id).FirstOrDefault();

                List<Post> posts;
                if (authorship == null)
                {
                    if (isFamilies(id, user.Id))
                    {
                        posts = chronicleContext.Posts.Where(c => c.ChronicleId == id && c.TimeDeleted == null).ToList();
                    }
                    else
                    {
                        return NotFound("Doesnt belong to user or his family");
                    }

                }
                else
                {
                    posts = chronicleContext.Posts.Where(p => p.ChronicleId == id && p.TimeDeleted == null).ToList();
                }


                var list = new List<Post>();

                foreach (var post in posts)
                {
                    list.Add(
                        new Post
                        {
                            Id = post.Id,
                            Description = post.Description,
                            CreatedTime = post.CreatedTime,
                            Latitude = post.Latitude,
                            Longitude = post.Longitude,
                            Name = post.Name,
                            ChronicleId = post.ChronicleId
                        });
                }
                list.Sort((x, y) => DateTime.Compare(x.CreatedTime ?? DateTime.MaxValue, y.CreatedTime ?? DateTime.MaxValue));
                return list;
            }
            return BadRequest("No token in request");
        }


        //get all locations from chronicle
        [HttpGet]
        [Route("get-photos/{id}")]
        public ActionResult<List<HttpPhoto>> GetPhotos(long id)
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";

            if (headers.ContainsKey("apikey"))
            {
                token = headers["apikey"];
                var user = chronicleContext.Users.Where(k => k.Key == token).FirstOrDefault();

                if (user == null)
                {
                    return NotFound("Such key doesn't exist");
                }
                var authorship = chronicleContext.Chronicles.Where(a => a.UserId == user.Id && a.Id == id).FirstOrDefault();

                List<Post> posts;
                if (authorship == null)
                {
                    if (isFamilies(id, user.Id))
                    {
                        posts = chronicleContext.Posts.Where(c => c.ChronicleId == id && c.TimeDeleted == null).ToList();
                    }
                    else
                    {
                        return NotFound("Doesnt belong to user or his family");
                    }
                }
                else
                {
                    posts = chronicleContext.Posts.Where(p => p.ChronicleId == id && p.TimeDeleted == null).ToList();
                }

                var list = new List<HttpPhoto>();

                foreach (var post in posts)
                {
                    var photo = chronicleContext.Photos.Where(ph => ph.PostId == post.Id).FirstOrDefault();
                    if (photo != null)
                    {
                        list.Add(new HttpPhoto
                        {
                            Id = photo.Id,
                            PostId = photo.PostId,
                            ImageString = Convert.ToBase64String(photo.ImageData)
                        });
                    }
                }
                return list;
            }
            return BadRequest("No token in request");
        }

        [HttpPost]
        [Route("check-changes")]
        public ActionResult<bool> CheckChanges([FromBody] Post post)
        {
            var re = Request;
            var headers = re.Headers;
            string token = "";
            long id = post.ChronicleId;
            DateTime? dateTime = post.CreatedTime;

            if (headers.ContainsKey("apikey"))
            {
                token = headers["apikey"];
                var user = chronicleContext.Users.Where(k => k.Key == token).FirstOrDefault();

                if (user == null)
                {
                    return NotFound("Such key doesn't exist");
                }
                var authorship = chronicleContext.Chronicles.Where(a => a.UserId == user.Id && a.Id == id).FirstOrDefault();

                if (authorship == null)
                {
                    return NotFound("User doesn't have such chronicle");
                }

                var posts = chronicleContext.Posts.Where(p => p.ChronicleId == id && p.CreatedTime > dateTime && p.TimeDeleted == null).FirstOrDefault();
                if (posts != null) return true;              
            }
            return false;
        }
    }
}