using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EditMessageController : ControllerBase
{
    private readonly ILogger<EditMessageController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public EditMessageController(ILogger<EditMessageController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> EditMessageAsync(Guid id, EditMessageCommand command)
    {
        try
        {
            command.Id = id;
            await _commandDispatcher.SendAsync(command);
            return Ok(new BaseResponse
            {
                Message = "Edit message request completed successfully!"
            });
        }
        catch (InvalidOperationException e)
        {
            _logger.Log(LogLevel.Warning, "Client made a bad request!");
            return BadRequest(new BaseResponse
            {
                Message = e.Message
            });
        }
        catch (AggregateNotFoundException e)
        {
            _logger.Log(LogLevel.Warning,
                "Could not retrieve aggregate,. client passed an incorrect post ID targeting the aggregate!");
            return BadRequest(new BaseResponse
            {
                Message = e.Message
            });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to edit the message of a post!";
            _logger.Log(LogLevel.Error, ex.Message, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new BaseResponse { Message = SAFE_ERROR_MESSAGE });
        }
    }
}