using Api.Data;
using Api.Dtos.Dependent;
using Api.Models;

namespace Api.Services
{
    public interface IDependentService
    {
        List<GetDependentDto> GetAll();

        GetDependentDto Get(int id);
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
                .Select(MapToDto).ToList();

            return dependents;
        }

        public GetDependentDto Get(int id) 
        {
            var dependent = _context.Dependents
                .Select(MapToDto).FirstOrDefault(d => d.Id == id);

            // could be null, but lets handle in the controller
            return dependent;
        }

        private GetDependentDto MapToDto(Dependent dependent)
        {
            return new GetDependentDto
            {
                Id = dependent.Id,
                FirstName = dependent.FirstName,
                LastName = dependent.LastName,
                Relationship = dependent.Relationship,
                DateOfBirth = dependent.DateOfBirth
            };
        }
    }
}