using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.Issue.Example.Repository;
using Swashbuckle.Issue.Example.Service;
using ZNetCS.AspNetCore.Authentication.Basic;

namespace Swashbuckle.Issue.Example.Web.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ValuesController : ControllerBase
    {
        private readonly IValueService _valueService;

        public ValuesController(IValueService valueService)
        {
            _valueService = valueService;
        }

        /// <summary>
        ///     Get Values.
        /// </summary>
        /// <returns>A collection of <see cref="ValueModel" />.</returns>
        [HttpGet]
        public Task<IEnumerable<ValueModel>> Get()
        {
            return _valueService.GetAllValues();
        }

        /// <summary>
        ///     Get a Value.
        /// </summary>
        /// <param name="id">The Id of the value.</param>
        /// <returns>A <see cref="ValueModel" />.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ValueModel>> Get(int id)
        {
            return OkOrNotFound(await _valueService.Get(id));
        }

        /// <summary>
        ///     Create a Value.
        /// </summary>
        /// <param name="value">The Value to be created.</param>
        /// <returns>The Value with an Id assigned.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme)]
        public Task<ValueModel?> Post(ValueModel value)
        {
            TryValidateModel(value);
            return _valueService.Create(value);
        }

        /// <summary>
        ///     Update a Value.
        /// </summary>
        /// <param name="id">The id of the value to be updated.</param>
        /// <param name="value">The new Values.</param>
        /// <returns>The updated Value.</returns>
        /// <response code="409">A value with Id already exists.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ValueModel?>> Put(int id, [FromBody] ValueModel value)
        {
            if (id != value.Id)
            {
                return KeyMismatchBadRequest();
            }

            TryValidateModel(value);
            return OkOrNotFound(await _valueService.Update(value));
        }

        /// <summary>
        ///     Delete a Value.
        /// </summary>
        /// <param name="id">The Id of the Value to be updated.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task Delete(int id)
        {
            await _valueService.Delete(id);
        }
    }
}
