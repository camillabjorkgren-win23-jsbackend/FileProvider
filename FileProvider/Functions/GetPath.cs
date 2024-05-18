using Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FileProvider.Functions;

public class GetPath(ILogger<GetPath> logger, DataContext context)
{
    private readonly ILogger<GetPath> _logger = logger;
    private readonly DataContext _context = context;

    [Function("GetPath")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        if(!string.IsNullOrEmpty(body)){
            var fileName = JsonConvert.DeserializeObject<string>(body);
            if(fileName != null)
            {
                var file = await _context.Files
                    .Where(f => f.FileName.Contains(fileName))
                    .FirstOrDefaultAsync();

                if (file != null)
                {
                    return new OkObjectResult(file.FilePath);
                }
                return new NotFoundResult();
            }
           
        }
        return new BadRequestResult();
    }
}
