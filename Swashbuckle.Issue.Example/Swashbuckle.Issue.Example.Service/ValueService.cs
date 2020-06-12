using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Swashbuckle.Issue.Example.Repository;

namespace Swashbuckle.Issue.Example.Service
{
    //TODO - Remove me.   This is only an example.
    public class ValueService : IValueService
    {
        private readonly IValueRepository _repository;
        private readonly ILogger<ValueService> _logger;

        public ValueService(IValueRepository valueRepository, ILogger<ValueService> logger)
        {
            _repository = valueRepository;
            _logger = logger;
        }

        public Task<IEnumerable<ValueModel>> GetAllValues()
        {
            return _repository.GetAllValues();
        }

        public async Task<ValueModel> Get(int id)
        {
            var value = await _repository.Get(id);
            _logger.LogDebug("Got value {@value}", value);
            return value;
        }

        public async Task<ValueModel?> Create(ValueModel valueModel)
        {
            var value = await _repository.Create(valueModel);
            _logger.LogDebug("Created value {@value}", value);
            return value;
        }

        public async Task<ValueModel?> Update(ValueModel model)
        {
            var value = await _repository.Update(model);
            _logger.LogDebug("Updated value {@value}", value);
            return value;
        }

        public async Task Delete(int id)
        {
            await _repository.Delete(id);
            _logger.LogDebug("Deleted value with id {id}", id);
        }
    }
}
