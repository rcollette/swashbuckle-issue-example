using System.Collections.Generic;
using System.Threading.Tasks;
using Swashbuckle.Issue.Example.Repository;

namespace Swashbuckle.Issue.Example.Service
{
    //TODO - Remove me.   This is only an example.
    public interface IValueService
    {
        Task<IEnumerable<ValueModel>> GetAllValues();
        Task<ValueModel> Get(int id);
        Task<ValueModel?> Create(ValueModel valueModel);
        Task<ValueModel?> Update(ValueModel model);
        Task Delete(int id);
    }
}
