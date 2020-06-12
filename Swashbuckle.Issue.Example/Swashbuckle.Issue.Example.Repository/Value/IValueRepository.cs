using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swashbuckle.Issue.Example.Repository
{
    public interface IValueRepository
    {
        Task<IEnumerable<ValueModel>> GetAllValues();
        Task<ValueModel> Get(int id);
        Task<ValueModel?> Create(ValueModel value);
        Task<ValueModel?> Update(ValueModel value);
        Task Delete(int id);
    }
}
