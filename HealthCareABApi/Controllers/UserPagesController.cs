using Microsoft.AspNetCore.Mvc;
using HealthCareABApi.DTO;
using HealthCareABApi.Repositories;

namespace HealthCareABApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPagesController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserPagesController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]

    }
