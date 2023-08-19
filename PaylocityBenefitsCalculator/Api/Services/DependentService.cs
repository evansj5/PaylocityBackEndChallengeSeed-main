using Api.Data;
using Api.Dtos.Dependent;

namespace Api.Services
{
    public interface IDependentService
    {
        List<GetDependentDto> GetAll();
    }

    public class DependentService : IDependentService
    {
        private readonly BenefitsDbContext _context;

        public DependentService(BenefitsDbContext context)
        {
            _context = context;
        }

        public List<GetDependentDto> GetAll()
        {
            var dependents = _context.Dependents
                .Select(d => new GetDependentDto
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Relationship = d.Relationship,
                    DateOfBirth = d.DateOfBirth,
                }).ToList();

            return dependents;
        }
    }
}