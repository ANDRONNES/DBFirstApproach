using DBFirst.Services;
using Microsoft.AspNetCore.Mvc;

namespace DBFirst.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    public IClientsService _service;

    public ClientsController(IClientsService service)
    {
        _service = service;
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id,CancellationToken ct)
    {
        await _service.DeleteClientByIdAsync(id,ct);
        return Ok(new { message = $"Client with id = {id} successfully deleted." });
    }
}