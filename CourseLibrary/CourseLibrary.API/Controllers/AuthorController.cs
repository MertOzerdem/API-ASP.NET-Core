using AutoMapper;
using CourseLibrary.API.Helper;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    [ApiController] // returns auto respons contexts like 404 not found
    [Route("api/authors")]
    public class AuthorController : ControllerBase
    {
        private readonly ICourseLibraryRepository courseLibraryRepository;
        private readonly IMapper mapper;

        public AuthorController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            this.courseLibraryRepository = courseLibraryRepository ?? 
                throw new ArgumentNullException(nameof(courseLibraryRepository));
            this.mapper = mapper;
        }

        [HttpGet("")]
        [HttpHead]
        // Returns array of AuthorDto 
        //FromQuery to query by URL (exp: http://localhost:51044/api/authors?mainCategory=Rum)
        /* Old Query Type call
         *  public ActionResult<IEnumerable<AuthorDto>> GetAuthors( 
            [FromQuery] string mainCategory,
            [FromQuery] string searchQuery) */
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors( 
            [FromQuery] AuthorsResourceParameters authorsResourceParameters) 
        {
            var authorsFromRepo = this.courseLibraryRepository.GetAuthors(authorsResourceParameters);
            //var authors = new List<AuthorDto>();

            // Default by hand mapping
            //foreach(var author in authorsFromRepo)
            //{
            //    authors.Add(new AuthorDto()
            //    {
            //        Id = author.Id,
            //        Name = $"{author.FirstName} {author.LastName}",
            //        MainCategory = author.MainCategory,
            //        Age = author.DateOfBirth.GetCurrentAge()
            //    });
            //}

            // Object Mapper version
            return Ok(this.mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
        }

        [HttpGet("{authorId}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid authorId)
        {
            // Alternative method to check if author exist
            //if (!this.courseLibraryRepository.AuthorExists(authorId))
            //{
            //    return NotFound();
            //}
            // A better  way to do is
            var authorFromRepo = this.courseLibraryRepository.GetAuthor(authorId);

            if (authorFromRepo == null)
            {
                return NotFound();
            }

            return Ok(this.mapper.Map<AuthorDto>(authorFromRepo));
            
        }
        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(AuthorForCreationDto author)
        {
            var authorEntity = this.mapper.Map<Entities.Author>(author);
            this.courseLibraryRepository.AddAuthor(authorEntity);
            this.courseLibraryRepository.Save();

            var authorToReturn = this.mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor",
                new { authorId = authorToReturn.Id },
                authorToReturn);
        }
    }
}
