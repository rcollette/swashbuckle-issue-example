using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swashbuckle.Issue.Example.Repository
{
    //TODO - Remove me.   This is only an example.
    public class ValueRepository : IValueRepository
    {
        private static readonly ConcurrentDictionary<int, ValueModel> _values =
            new ConcurrentDictionary<int, ValueModel>
            {
                [1] = new ValueModel { Id = 1, Value = "x" }, [2] = new ValueModel { Id = 2, Value = "y" }
            };

        public async Task<IEnumerable<ValueModel>> GetAllValues()
        {
            return await Task.FromResult(_values.Values);
        }

        public async Task<ValueModel> Get(int id)
        {
            return !_values.TryGetValue(id, out var value)
                ? throw new NotFoundException()
                : await Task.FromResult(value);
        }

        public Task<ValueModel?> Create(ValueModel value)
        {
            int nextId = (from v in _values select v.Key).Max() + 1;
            value.Id = nextId;
            return _values.TryAdd(value.Id, value) ? Task.FromResult<ValueModel?>(value) : Task.FromResult<ValueModel?>(null);
        }

        public Task<ValueModel?> Update(ValueModel value)
        {
            if (!_values.TryGetValue(value.Id, out var comparisonValue))
            {
                throw new NotFoundException();
            }

            return _values.TryUpdate(value.Id, value, comparisonValue)
                ? Task.FromResult<ValueModel?>(value)
                : Task.FromResult<ValueModel?>(null);
        }

        public Task Delete(int id)
        {
            // ReSharper disable once UnusedVariable
            if (!_values.TryRemove(id, out var value))
            {
                throw new NotFoundException();
            }

            return Task.CompletedTask;
        }
    }
}
